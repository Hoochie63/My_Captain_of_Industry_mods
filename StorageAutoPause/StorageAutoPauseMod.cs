using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mafi;
using Mafi.Collections;
using Mafi.Collections.ImmutableCollections;
using Mafi.Collections.ReadonlyCollections;
using Mafi.Core;
using Mafi.Core.Buildings.Mine;
using Mafi.Core.Buildings.OreSorting;
using Mafi.Core.Buildings.Storages;
using Mafi.Core.Buildings.Towers;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Dynamic;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Factory.ElectricPower;
using Mafi.Core.Factory.Machines;
using Mafi.Core.Factory.Recipes;
using Mafi.Core.Game;
using Mafi.Core.GameLoop;
using Mafi.Core.Maintenance;
using Mafi.Core.Mods;
using Mafi.Core.Population;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using Mafi.Core.SaveGame;
using Mafi.Core.Simulation;
using Mafi.Localization;
using Mafi.Unity;
using Mafi.Unity.Entities;
using Mafi.Unity.Ui;
using Mafi.Unity.Ui.Library;
using Mafi.Unity.UiToolkit.Component;
using Mafi.Unity.UiToolkit.Library;
using UnityEngine;
using UnityEngine.Networking;

namespace StorageAutoPause
{
	// Token: 0x02000002 RID: 2
	public sealed class StorageAutoPauseMod : IMod, IDisposable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public ModManifest Manifest { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public bool IsUiOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205B File Offset: 0x0000025B
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002063 File Offset: 0x00000263
		[Obsolete("Use JsonConfig instead.")]
		public Option<IConfig> ModConfig { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000206C File Offset: 0x0000026C
		public ModJsonConfig JsonConfig { get; }

		// Token: 0x06000006 RID: 6 RVA: 0x00002074 File Offset: 0x00000274
		public StorageAutoPauseMod(ModManifest manifest)
		{
			this.Manifest = manifest;
			this.JsonConfig = new ModJsonConfig(this);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002131 File Offset: 0x00000331
		public void RegisterPrototypes(ProtoRegistrator registrator)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002133 File Offset: 0x00000333
		public void RegisterDependencies(DependencyResolverBuilder depBuilder, ProtosDb protosDb, bool gameWasLoaded)
		{
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002135 File Offset: 0x00000335
		public void EarlyInit(DependencyResolver resolver)
		{
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002138 File Offset: 0x00000338
		public void Initialize(DependencyResolver resolver, bool gameWasLoaded)
		{
			this._entities = resolver.Resolve<EntitiesManager>();
			this._gameLoop = resolver.Resolve<IGameLoopEvents>();
			this._simLoop = resolver.Resolve<ISimLoopEvents>();
			this._saveManager = resolver.Resolve<ISaveManager>();
			this._inspectors = resolver.Resolve<InspectorsManager>();
			this._assetsDb = resolver.Resolve<AssetsDb>();
			this._protoModelFactory = resolver.Resolve<ProtoModelFactory>();
			this._gameLoop.RegisterInitState(this, new Action(this.OnInitState));
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021B4 File Offset: 0x000003B4
		private void OnInitState()
		{
			try
			{
				this.InitPersistence();
				this._diagnosticSemaphoreEnabled = this.JsonConfig.GetBool("diagnostic_semaphore_enabled", true);
				this._breakdownFireworksEnabled = this.JsonConfig.GetBool("breakdown_fireworks_enabled", true);
				this._breakdownFireworksSoundEnabled = this.JsonConfig.GetBool("breakdown_fireworks_sound_enabled", true);
				this.LoadRules();
				this.PrimeBrokenEntityBaseline();
				this._fireworkSuppressUntilUtc = DateTime.UtcNow.AddSeconds(60.0);
				this.InitializeFireworkAudio();
				this._inputUpdate = new Action<GameTime>(this.OnInputUpdate);
				this._simUpdate = new Action(this.OnSimUpdate);
				this._gameLoop.InputUpdate.AddNonSaveable<StorageAutoPauseMod>(this, this._inputUpdate);
				this._simLoop.ReadGameStateFrequent.AddNonSaveable<StorageAutoPauseMod>(this, this._simUpdate);
				this._pauseChanged = new Action<IEntity, bool>(this.OnEntityPauseStateChanged);
				this._entities.EntityPauseStateChanged.AddNonSaveable<StorageAutoPauseMod>(this, this._pauseChanged);
				this._enabledChanged = new Action<IEntity, bool>(this.OnEntityEnabledChanged);
				this._entities.EntityEnabledChanged.AddNonSaveable<StorageAutoPauseMod>(this, this._enabledChanged);
				Log.Info("StorageAutoPause: factory-centric automation initialized.");
			}
			catch (Exception ex)
			{
				string text = "StorageAutoPause: initialization failed: ";
				Exception ex2 = ex;
				Log.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000229C File Offset: 0x0000049C
		private void BuildInspectorPanel()
		{
			this._labelType = StorageAutoPauseMod.FindType("Mafi.Unity.UiToolkit.Library.Label");
			this._buttonTextType = StorageAutoPauseMod.FindType("Mafi.Unity.UiToolkit.Library.ButtonText");
			this._uiComponentType = StorageAutoPauseMod.FindType("Mafi.Unity.UiToolkit.Component.UiComponent");
			if (this._labelType == null || this._buttonTextType == null || this._uiComponentType == null)
			{
				throw new InvalidOperationException("COI native UiToolkit components not found.");
			}
			MethodInfo methodInfo = this._machineInspector.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault<MethodInfo>((MethodInfo x) => x.Name == "AddPanel" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsArray);
			bool flag = false;
			if (methodInfo == null)
			{
				methodInfo = this._machineInspector.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault<MethodInfo>((MethodInfo x) => x.Name == "AddPanelWithHeader" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsArray);
				flag = true;
			}
			if (methodInfo == null)
			{
				throw new MissingMethodException("MachineInspector native panel factory not found.");
			}
			// Keep all mod-owned content inside a dedicated child container.  Clearing the
			// panel body returned by BaseInspector also clears/rebuilds native panel
			// infrastructure and can leave its input surface covering the game viewport.
			// This was the cause of structure clicks being swallowed after the first
			// inspector refresh on Update 4 (0.8.6c).
			Column column = new Column(new Px(6f));
			Array array = Array.CreateInstance(this._uiComponentType, 1);
			array.SetValue(column, 0);
			this._panel = methodInfo.Invoke(this._machineInspector, new object[] { array });
			StorageAutoPauseMod.MoveInjectedPanelToTop(this._machineInspector, this._panel);
			if (flag)
			{
				StorageAutoPauseMod.Invoke(this._panel, "Title", new object[]
				{
					new LocStrFormatted("")
				});
			}
			this._panelBody = column;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000242C File Offset: 0x0000062C
		private bool ActivateOrBuildOtherInspectorPanel(object inspector)
		{
			if (inspector == null)
			{
				return false;
			}
			StorageAutoPauseMod.InspectorPanelBinding inspectorPanelBinding;
			if (this._otherPanelBindings.TryGetValue(inspector, out inspectorPanelBinding) && inspectorPanelBinding != null && inspectorPanelBinding.Panel != null && inspectorPanelBinding.Body != null)
			{
				this._otherInspector = inspector;
				this._otherPanel = inspectorPanelBinding.Panel;
				this._otherPanelBody = inspectorPanelBinding.Body;
				return true;
			}
			this._labelType = StorageAutoPauseMod.FindType("Mafi.Unity.UiToolkit.Library.Label");
			this._buttonTextType = StorageAutoPauseMod.FindType("Mafi.Unity.UiToolkit.Library.ButtonText");
			this._uiComponentType = StorageAutoPauseMod.FindType("Mafi.Unity.UiToolkit.Component.UiComponent");
			if (this._labelType == null || this._buttonTextType == null || this._uiComponentType == null)
			{
				return false;
			}
			MethodInfo[] methods = inspector.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo methodInfo = methods.FirstOrDefault<MethodInfo>((MethodInfo x) => x.Name == "AddPanel" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsArray);
			bool flag = false;
			if (methodInfo == null)
			{
				methodInfo = methods.FirstOrDefault<MethodInfo>((MethodInfo x) => x.Name == "AddPanelWithHeader" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsArray);
				flag = true;
			}
			if (methodInfo == null)
			{
				return false;
			}
			Column column = new Column(new Px(6f));
			StorageAutoPauseMod.ApplyMinWidth(column, 570f);
			Array array = Array.CreateInstance(this._uiComponentType, 1);
			array.SetValue(column, 0);
			object obj = methodInfo.Invoke(inspector, new object[] { array });
			if (obj == null)
			{
				return false;
			}
			StorageAutoPauseMod.MoveInjectedPanelToTop(inspector, obj);
			if (flag)
			{
				StorageAutoPauseMod.Invoke(obj, "Title", new object[]
				{
					new LocStrFormatted("")
				});
			}
			object obj2 = column;
			if (obj2 == null)
			{
				return false;
			}
			StorageAutoPauseMod.InspectorPanelBinding inspectorPanelBinding2 = new StorageAutoPauseMod.InspectorPanelBinding(obj, obj2);
			this._otherPanelBindings[inspector] = inspectorPanelBinding2;
			this._otherInspector = inspector;
			this._otherPanel = obj;
			this._otherPanelBody = obj2;
			return true;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002600 File Offset: 0x00000800
		private static void MoveInjectedPanelToTop(object inspector, object panel)
		{
			try
			{
				UiComponent uiComponent = panel as UiComponent;
				if (inspector == null || uiComponent == null)
				{
					return;
				}
				FieldInfo field = null;
				for (Type type = inspector.GetType(); type != null && field == null; type = type.BaseType)
				{
					field = type.GetField("MainBody", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				}
				Column column = ((field == null) ? null : (field.GetValue(inspector) as Column));
				if (column == null)
				{
					Log.Warning("StorageAutoPause: inspector MainBody not found for " + inspector.GetType().FullName + ".");
					return;
				}
				uiComponent.RemoveFromHierarchy();
				column.InsertAt(0, uiComponent, false);
				Log.Info("StorageAutoPause: injected panel moved to top of " + inspector.GetType().FullName + ".");
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: failed to move injected panel to visible position: " + ex.Message);
			}
		}

		private bool TryGetActiveOtherLayoutEntity(out LayoutEntityBase entity)
		{
			entity = null;
			bool flag;
			try
			{
				if (this._otherInspector == null || this._otherPanelBody == null)
				{
					flag = false;
				}
				else
				{
					IEntityInspector firstActiveInspectorOrNull = this._inspectors.GetFirstActiveInspectorOrNull();
					if (firstActiveInspectorOrNull == null || firstActiveInspectorOrNull != this._otherInspector)
					{
						flag = false;
					}
					else
					{
						IEntity firstActiveEntityOrNull = this._inspectors.GetFirstActiveEntityOrNull();
						entity = firstActiveEntityOrNull as LayoutEntityBase;
						flag = entity != null;
					}
				}
			}
			catch
			{
				entity = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002678 File Offset: 0x00000878
		private bool IsSemaphoreCapable(IEntity entity)
		{
			return entity != null && entity is LayoutEntityBase && (entity is IEntityWithWorkers || entity is IElectricityConsumingEntity || entity is IMaintainedEntity || entity is IAreaManagingTower || entity is OreSortingPlant);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000026B4 File Offset: 0x000008B4
		private bool IsManualSemaphoreEnabled(int entityId)
		{
			return this._manualSemaphoreEntities.Contains(entityId);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000026C4 File Offset: 0x000008C4
		private bool HasAutomaticLogisticsMarker(int entityId)
		{
			if (this._rules.ContainsKey(entityId))
			{
				return true;
			}
			foreach (StorageAutoPauseMod.MachineRule machineRule in this._rules.Values)
			{
				if (machineRule.Conditions.Any((StorageAutoPauseMod.StorageCondition x) => x.StorageId == entityId))
				{
					return true;
				}
			}
			if (this._towerRules.ContainsKey(entityId))
			{
				return true;
			}
			foreach (StorageAutoPauseMod.MineTowerRule mineTowerRule in this._towerRules.Values)
			{
				if (mineTowerRule.Sorters.Any((StorageAutoPauseMod.SorterCondition x) => x.SorterId == entityId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000027F4 File Offset: 0x000009F4
		private void ToggleManualSemaphore(LayoutEntityBase entity)
		{
			if (entity == null)
			{
				return;
			}
			if (!this._manualSemaphoreEntities.Add(entity.Id.Value))
			{
				this._manualSemaphoreEntities.Remove(entity.Id.Value);
			}
			this.SaveRules();
			this.MarkMarkersDirty();
			this.RequestUiRefresh();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002848 File Offset: 0x00000A48
		private bool TryGetActiveMachine(out Machine machine)
		{
			machine = null;
			bool flag;
			try
			{
				if (this._machineInspector == null)
				{
					flag = false;
				}
				else
				{
					IEntityInspector firstActiveInspectorOrNull = this._inspectors.GetFirstActiveInspectorOrNull();
					if (firstActiveInspectorOrNull == null || firstActiveInspectorOrNull != this._machineInspector)
					{
						flag = false;
					}
					else if (firstActiveInspectorOrNull.GetType().FullName != "Mafi.Unity.Ui.Inspectors.MachineInspector")
					{
						flag = false;
					}
					else
					{
						IEntity firstActiveEntityOrNull = this._inspectors.GetFirstActiveEntityOrNull();
						machine = firstActiveEntityOrNull as Machine;
						if (machine == null)
						{
							flag = false;
						}
						else
						{
							PropertyInfo property = this._machineInspector.GetType().GetProperty("IsActive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							if (property != null && !(bool)property.GetValue(this._machineInspector, null))
							{
								flag = false;
							}
							else
							{
								flag = true;
							}
						}
					}
				}
			}
			catch
			{
				machine = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002910 File Offset: 0x00000B10
		private Machine CurrentMachine()
		{
			Machine machine;
			if (!this.TryGetActiveMachine(out machine))
			{
				return null;
			}
			return machine;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000292A File Offset: 0x00000B2A
		private void RequestUiRefresh()
		{
			this._uiRefreshRequested = true;
			if (this._uiRefreshDelayInputFrames < 1)
			{
				this._uiRefreshDelayInputFrames = 1;
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002948 File Offset: 0x00000B48
		private void RefreshPanel()
		{
			Machine machine;
			if (this._panelBody == null || !this.TryGetActiveMachine(out machine))
			{
				return;
			}
			StorageAutoPauseMod.Invoke(this._panelBody, "Clear", Array.Empty<object>());
			StorageAutoPauseMod.MachineRule machineRule;
			bool configured = this._rules.TryGetValue(machine.Id.Value, out machineRule) && machineRule.Conditions.Count > 0;
			bool flag = this._expandedMachineId == machine.Id.Value;
			bool editing = configured && this._editMachineId == machine.Id.Value;
			string text = (configured ? "<color=#46C96F>●</color>" : "<color=#D94B4B>●</color>");
			object obj = this.NewRow(8f);
			this.AddLabelTo(obj, "AUTOMATICKÉ ŘÍZENÍ   " + text, 1f);
			this.AddButtonTo(obj, flag ? "▲" : "▼", delegate
			{
				this.ToggleAutomationPanel(machine);
			}, 0f, false);
			this.AddToPanel(obj);
			if (!flag)
			{
				return;
			}
			if (configured)
			{
				this.RemoveMissingStorages(machineRule);
				if (machineRule.Conditions.Count == 0)
				{
					this.ReleaseRulePause(machineRule, machine, "poslední sklad byl odstraněn");
					this._rules.Remove(machine.Id.Value);
					this.SaveRules();
					configured = false;
					machineRule = null;
					this._editMachineId = 0;
					editing = false;
				}
			}
			int num = (configured ? machineRule.Conditions.Count : 0);
			string text2 = ((!configured) ? "NENASTAVENO" : (machine.IsPaused ? (machineRule.PausedByMod ? "<color=#E65B4A>PAUZA AUTOMATIKOU</color>" : "<color=#4E9BE8>PAUZA HRÁČEM</color>") : "<color=#46C96F>BĚŽÍ</color>"));
			object obj2 = this.NewRow(10f);
			StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj2, "Sklady   " + num.ToString() + "   ☷", 0f), 140f);
			StorageAutoPauseMod.ApplyMinWidth(this.AddLabelTo(obj2, "Stav   " + text2, 1f), 220f);
			StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj2, "✎  " + (editing ? "Hotovo" : "Upravit"), delegate
			{
				if (!configured)
				{
					this.StartSelectingStorage(machine);
					return;
				}
				this._editMachineId = (editing ? 0 : machine.Id.Value);
				this.RequestUiRefresh();
			}, 0f, false), 120f);
			this.AddToPanel(obj2);
			if (this._diagnosticSemaphoreEnabled)
			{
				object obj3 = this.NewRow(8f);
				this.AddLabelTo(obj3, "Semafor", 1f);
				if (configured)
				{
					this.AddLabelTo(obj3, "<color=#46C96F>Aktivní s logistikou</color>", 0f);
				}
				else
				{
					this.AddButtonTo(obj3, this.IsManualSemaphoreEnabled(machine.Id.Value) ? "Vypnout" : "Zapnout", delegate
					{
						this.ToggleManualSemaphore(machine);
					}, 0f, false);
				}
				this.AddToPanel(obj3);
			}
			if (!string.IsNullOrWhiteSpace(this._selectionNotice))
			{
				object obj4 = this.NewRow(6f);
				this.AddLabelTo(obj4, this._selectionNotice, 1f);
				this.AddToPanel(obj4);
			}
			object obj5 = this.NewColumn(8f);
			StorageAutoPauseMod.SetPanelLike(obj5);
			if (!configured)
			{
				this.AddLabelTo(obj5, "Přidejte první sklad pro nastavení automatického řízení.", 1f);
			}
			else if (!editing)
			{
				for (int i = 0; i < machineRule.Conditions.Count; i++)
				{
					StorageAutoPauseMod.StorageCondition storageCondition = machineRule.Conditions[i];
					Storage storage2;
					if (this._entities.TryGetEntity<Storage>(new global::Mafi.Core.EntityId(storageCondition.StorageId), out storage2))
					{
						string text3 = (storageCondition.Active ? "<color=#D95A4A>●</color>" : (StorageAutoPauseMod.IsApproachingThreshold(storageCondition) ? "<color=#E79B36>●</color>" : "<color=#4B9B38>✓</color>"));
						string text4 = (storageCondition.Active ? "<color=#D95A4A>▰</color>" : (StorageAutoPauseMod.IsApproachingThreshold(storageCondition) ? "<color=#E79B36>▰</color>" : "<color=#5BA34A>▰</color>"));
						object obj6 = this.NewRow(8f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj6, "▣", 0f), 28f);
						StorageAutoPauseMod.ApplyMinWidth(this.AddButtonTo(obj6, this.GetEntityTitle(storage2), delegate
						{
							this._inspectors.TryActivateFor(storage2);
						}, 1f, false), 140f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj6, text4, 0f), 28f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj6, storageCondition.PauseAtPercent.ToString() + "%", 0f), 52f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj6, "▶", 0f), 24f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj6, storageCondition.ResumeAtPercent.ToString() + "%", 0f), 52f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj6, text3, 0f), 28f);
						this.AddToContainer(obj5, obj6);
					}
				}
			}
			else
			{
				for (int j = 0; j < machineRule.Conditions.Count; j++)
				{
					StorageAutoPauseMod.StorageCondition storageCondition2 = machineRule.Conditions[j];
					Storage storage;
					if (this._entities.TryGetEntity<Storage>(new global::Mafi.Core.EntityId(storageCondition2.StorageId), out storage))
					{
						int conditionIndex = j;
						int storageId = storageCondition2.StorageId;
						if (conditionIndex > 0)
						{
							object obj7 = this.NewRow(8f);
							this.AddLabelTo(obj7, "Podmínka mezi sklady", 1f);
							StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj7, storageCondition2.Join.ToString() + "  ▼", delegate
							{
								this.CycleJoin(machine, conditionIndex);
							}, 0f, false), 96f);
							this.AddToContainer(obj5, obj7);
						}
						string text5 = (storageCondition2.Active ? "<color=#D95A4A>●</color>" : (StorageAutoPauseMod.IsApproachingThreshold(storageCondition2) ? "<color=#E79B36>●</color>" : "<color=#4B9B38>●</color>"));
						object obj8 = this.NewColumn(8f);
						StorageAutoPauseMod.SetPanelLike(obj8);
						object obj9 = this.NewRow(8f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj9, "▣", 0f), 28f);
						StorageAutoPauseMod.ApplyMinWidth(this.AddButtonTo(obj9, this.GetEntityTitle(storage), delegate
						{
							this._inspectors.TryActivateFor(storage);
						}, 1f, false), 150f);
						StorageAutoPauseMod.ApplyWidth(this.AddLabelTo(obj9, text5, 0f), 28f);
						StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj9, "✕", delegate
						{
							this.RemoveStorage(machine, storageId);
						}, 0f, true), 44f);
						this.AddToContainer(obj8, obj9);
						object obj10 = this.NewRow(8f);
						this.AddLabelTo(obj10, "PAUSE při naplnění", 1f);
						object pauseValue = this.AddLabelTo(obj10, storageCondition2.PauseAtPercent.ToString() + "%", 0f);
						StorageAutoPauseMod.ApplyWidth(pauseValue, 56f);
						this.AddToContainer(obj8, obj10);
						this.AddFivePercentSlider(obj8, storageCondition2.PauseAtPercent, delegate(int value)
						{
							int num2 = this.SetPauseThreshold(machine, conditionIndex, value);
							StorageAutoPauseMod.SetLabelText(pauseValue, num2.ToString() + "%");
							this.ApplyRuleNow(machine, "změna PAUSE");
							this.RequestUiRefresh();
						});
						object obj11 = this.NewRow(8f);
						this.AddLabelTo(obj11, "RESUME při poklesu", 1f);
						object resumeValue = this.AddLabelTo(obj11, storageCondition2.ResumeAtPercent.ToString() + "%", 0f);
						StorageAutoPauseMod.ApplyWidth(resumeValue, 56f);
						this.AddToContainer(obj8, obj11);
						this.AddFivePercentSlider(obj8, storageCondition2.ResumeAtPercent, delegate(int value)
						{
							int num3 = this.SetResumeThreshold(machine, conditionIndex, value);
							StorageAutoPauseMod.SetLabelText(resumeValue, num3.ToString() + "%");
							this.ApplyRuleNow(machine, "změna RESUME");
							this.RequestUiRefresh();
						});
						this.AddLabelTo(obj8, "ⓘ  Továrna se pozastaví při dosažení PAUSE a automatika ji znovu uvolní po poklesu na RESUME. Ruční pauzu hráče mod neruší.", 1f);
						this.AddToContainer(obj5, obj8);
					}
				}
			}
			ScrollColumn scrollColumn = new ScrollColumn();
			scrollColumn.ScrollerAuto();
			StorageAutoPauseMod.ApplyMaxHeight(scrollColumn, editing ? 520f : 360f);
			this.AddToContainer(scrollColumn, obj5);
			this.AddToPanel(scrollColumn);
			object obj12 = this.NewRow(10f);
			StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj12, "+  Přidat sklad", delegate
			{
				this.StartSelectingStorage(machine);
			}, 0f, false), 150f);
			this.AddLabelTo(obj12, "", 1f);
			StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj12, "▣  Odstranit automatiku", delegate
			{
				if (configured)
				{
					this.ClearRule(machine);
				}
			}, 0f, true), 210f);
			this.AddToPanel(obj12);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00003244 File Offset: 0x00001444
		private void RefreshOtherPanel()
		{
			LayoutEntityBase entity;
			if (this._otherPanelBody == null || !this.TryGetActiveOtherLayoutEntity(out entity))
			{
				return;
			}
			StorageAutoPauseMod.Invoke(this._otherPanelBody, "Clear", Array.Empty<object>());
			if (this._diagnosticSemaphoreEnabled && this.IsSemaphoreCapable(entity))
			{
				object obj = this.NewRow(8f);
				this.AddLabelTo(obj, "STAVOVÝ SEMAFOR", 1f);
				if (this.HasAutomaticLogisticsMarker(entity.Id.Value))
				{
					this.AddLabelTo(obj, "<color=#46C96F>Aktivní s logistikou</color>", 0f);
				}
				else
				{
					this.AddButtonTo(obj, this.IsManualSemaphoreEnabled(entity.Id.Value) ? "Vypnout" : "Zapnout", delegate
					{
						this.ToggleManualSemaphore(entity);
					}, 0f, false);
				}
				this.AddToOtherPanel(obj);
			}
			MineTower tower = entity as MineTower;
			if (tower == null)
			{
				return;
			}
			StorageAutoPauseMod.MineTowerRule mineTowerRule;
			bool flag = this._towerRules.TryGetValue(tower.Id.Value, out mineTowerRule) && mineTowerRule.Sorters.Count > 0;
			object obj2 = this.NewRow(8f);
			this.AddLabelTo(obj2, "LOGISTIKA TĚŽEBNÍ VĚŽE   " + (flag ? "<color=#46C96F>●</color>" : "<color=#D94B4B>●</color>"), 1f);
			this.AddToOtherPanel(obj2);
			object obj3 = this.NewRow(8f);
			this.AddLabelTo(obj3, "Třídírny   " + (flag ? mineTowerRule.Sorters.Count : 0).ToString(), 1f);
			StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj3, "+  Přidat třídírnu", delegate
			{
				this.StartSelectingSorter(tower);
			}, 0f, false), 160f);
			this.AddToOtherPanel(obj3);
			if (!string.IsNullOrWhiteSpace(this._otherSelectionNotice))
			{
				object obj4 = this.NewRow(6f);
				this.AddLabelTo(obj4, this._otherSelectionNotice, 1f);
				this.AddToOtherPanel(obj4);
			}
			if (!flag)
			{
				return;
			}
			object obj5 = this.NewColumn(8f);
			for (int i = 0; i < mineTowerRule.Sorters.Count; i++)
			{
				StorageAutoPauseMod.SorterCondition sorterCondition = mineTowerRule.Sorters[i];
				OreSortingPlant sorter;
				if (this._entities.TryGetEntity<OreSortingPlant>(new global::Mafi.Core.EntityId(sorterCondition.SorterId), out sorter))
				{
					int conditionIndex = i;
					int sorterId = sorterCondition.SorterId;
					object obj6 = this.NewColumn(6f);
					StorageAutoPauseMod.SetPanelLike(obj6);
					object obj7 = this.NewRow(8f);
					this.AddButtonTo(obj7, this.GetEntityTitle(sorter), delegate
					{
						this._inspectors.TryActivateFor(sorter);
					}, 1f, false);
					this.AddLabelTo(obj7, sorterCondition.PausedByMod ? "<color=#D95A4A>●</color>" : (sorter.IsPaused ? "<color=#4E9BE8>●</color>" : (StorageAutoPauseMod.IsApproachingSorterThreshold(sorterCondition) ? "<color=#E79B36>●</color>" : "<color=#4B9B38>●</color>")), 0f);
					StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj7, "✕", delegate
					{
						this.RemoveSorter(tower, sorterId);
					}, 0f, true), 44f);
					this.AddToContainer(obj6, obj7);
					object obj8 = this.NewRow(8f);
					this.AddLabelTo(obj8, "PAUSE při naplnění třídírny", 1f);
					object pauseValue = this.AddLabelTo(obj8, sorterCondition.PauseAtPercent.ToString() + "%", 0f);
					StorageAutoPauseMod.ApplyWidth(pauseValue, 56f);
					this.AddToContainer(obj6, obj8);
					this.AddFivePercentSlider(obj6, sorterCondition.PauseAtPercent, delegate(int value)
					{
						int num = this.SetSorterPauseThreshold(tower, conditionIndex, value);
						StorageAutoPauseMod.SetLabelText(pauseValue, num.ToString() + "%");
						this.RequestUiRefresh();
					});
					object obj9 = this.NewRow(8f);
					this.AddLabelTo(obj9, "RESUME při poklesu", 1f);
					object resumeValue = this.AddLabelTo(obj9, sorterCondition.ResumeAtPercent.ToString() + "%", 0f);
					StorageAutoPauseMod.ApplyWidth(resumeValue, 56f);
					this.AddToContainer(obj6, obj9);
					this.AddFivePercentSlider(obj6, sorterCondition.ResumeAtPercent, delegate(int value)
					{
						int num2 = this.SetSorterResumeThreshold(tower, conditionIndex, value);
						StorageAutoPauseMod.SetLabelText(resumeValue, num2.ToString() + "%");
						this.RequestUiRefresh();
					});
					this.AddToContainer(obj5, obj6);
				}
			}
			ScrollColumn scrollColumn = new ScrollColumn();
			scrollColumn.ScrollerAuto();
			StorageAutoPauseMod.ApplyMaxHeight(scrollColumn, 460f);
			this.AddToContainer(scrollColumn, obj5);
			this.AddToOtherPanel(scrollColumn);
			object obj10 = this.NewRow(8f);
			StorageAutoPauseMod.ApplyWidth(this.AddButtonTo(obj10, "▣  Odstranit logistiku věže", delegate
			{
				this.ClearTowerRule(tower);
			}, 0f, true), 220f);
			this.AddToOtherPanel(obj10);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000372B File Offset: 0x0000192B
		private void AddToOtherPanel(object component)
		{
			this.AddToContainer(this._otherPanelBody, component);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003754 File Offset: 0x00001954
		private void StartSelectingSorter(MineTower tower)
		{
			if (tower == null)
			{
				return;
			}
			this._otherSelectionNotice = null;
			this._waitingMineTowerId = tower.Id.Value;
			this._lastActiveEntityId = tower.Id.Value;
			Log.Info("StorageAutoPause: waiting for Ore Sorting Plant selection for Mine Tower " + tower.Id.Value.ToString());
		}

		private bool TryGetTowerOwningSorter(int sorterId, out int towerId)
		{
			foreach (StorageAutoPauseMod.MineTowerRule rule in this._towerRules.Values)
			{
				if (rule.Sorters.Any((StorageAutoPauseMod.SorterCondition x) => x.SorterId == sorterId))
				{
					towerId = rule.TowerId;
					return true;
				}
			}
			towerId = 0;
			return false;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000037B0 File Offset: 0x000019B0
		private void RemoveSorter(MineTower tower, int sorterId)
		{
			if (tower == null)
			{
				return;
			}
			StorageAutoPauseMod.MineTowerRule mineTowerRule;
			if (!this._towerRules.TryGetValue(tower.Id.Value, out mineTowerRule))
			{
				return;
			}
			for (int i = mineTowerRule.Sorters.Count - 1; i >= 0; i--)
			{
				if (mineTowerRule.Sorters[i].SorterId == sorterId)
				{
					this.ReleaseSorterPause(mineTowerRule.Sorters[i]);
					mineTowerRule.Sorters.RemoveAt(i);
				}
			}
			if (mineTowerRule.Sorters.Count == 0)
			{
				this._towerRules.Remove(tower.Id.Value);
			}
			this.SaveRules();
			this.MarkMarkersDirty();
			this.RequestUiRefresh();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000385C File Offset: 0x00001A5C
		private void ClearTowerRule(MineTower tower)
		{
			if (tower == null)
			{
				return;
			}
			StorageAutoPauseMod.MineTowerRule mineTowerRule;
			if (this._towerRules.TryGetValue(tower.Id.Value, out mineTowerRule))
			{
				foreach (StorageAutoPauseMod.SorterCondition sorterCondition in mineTowerRule.Sorters)
				{
					this.ReleaseSorterPause(sorterCondition);
				}
				this._towerRules.Remove(tower.Id.Value);
				this.SaveRules();
				this.MarkMarkersDirty();
			}
			this.RequestUiRefresh();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000038F8 File Offset: 0x00001AF8
		private int SetSorterPauseThreshold(MineTower tower, int index, int value)
		{
			StorageAutoPauseMod.MineTowerRule mineTowerRule;
			if (tower == null || !this._towerRules.TryGetValue(tower.Id.Value, out mineTowerRule) || index < 0 || index >= mineTowerRule.Sorters.Count)
			{
				return StorageAutoPauseMod.SnapFive(value);
			}
			StorageAutoPauseMod.SorterCondition sorterCondition = mineTowerRule.Sorters[index];
			value = StorageAutoPauseMod.SnapFive(value);
			int num = Math.Min(100, sorterCondition.ResumeAtPercent + 5);
			if (value < num)
			{
				value = num;
			}
			sorterCondition.PauseAtPercent = value;
			this.SaveRules();
			this.MarkMarkersDirty();
			return value;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000397C File Offset: 0x00001B7C
		private int SetSorterResumeThreshold(MineTower tower, int index, int value)
		{
			StorageAutoPauseMod.MineTowerRule mineTowerRule;
			if (tower == null || !this._towerRules.TryGetValue(tower.Id.Value, out mineTowerRule) || index < 0 || index >= mineTowerRule.Sorters.Count)
			{
				return StorageAutoPauseMod.SnapFive(value);
			}
			StorageAutoPauseMod.SorterCondition sorterCondition = mineTowerRule.Sorters[index];
			value = StorageAutoPauseMod.SnapFive(value);
			int num = Math.Max(0, sorterCondition.PauseAtPercent - 5);
			if (value > num)
			{
				value = num;
			}
			sorterCondition.ResumeAtPercent = value;
			this.SaveRules();
			this.MarkMarkersDirty();
			return value;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003A00 File Offset: 0x00001C00
		private static bool IsApproachingSorterThreshold(StorageAutoPauseMod.SorterCondition c)
		{
			if (c == null || c.Active)
			{
				return false;
			}
			int num = Math.Max(0, c.PauseAtPercent - 5);
			return c.LastFillPercent >= num && c.LastFillPercent < c.PauseAtPercent;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003A44 File Offset: 0x00001C44
		private void ReleaseSorterPause(StorageAutoPauseMod.SorterCondition cond)
		{
			if (cond == null || !cond.PausedByMod)
			{
				return;
			}
			OreSortingPlant oreSortingPlant;
			if (this._entities.TryGetEntity<OreSortingPlant>(new global::Mafi.Core.EntityId(cond.SorterId), out oreSortingPlant) && oreSortingPlant.IsPaused && oreSortingPlant.CanBePaused)
			{
				this.SetPausedEntityByAutomation(oreSortingPlant, false);
			}
			cond.PausedByMod = false;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003A98 File Offset: 0x00001C98
		private static void ApplyWidth(object component, float px)
		{
			if (component == null || px <= 0f)
			{
				return;
			}
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.UiToolkit.Component.UiComponentLayoutExtensions", component, "MinWidth", new object[]
			{
				new Px(px)
			});
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.UiToolkit.Component.UiComponentLayoutExtensions", component, "MaxWidth", new object[]
			{
				new Px(px)
			});
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003AFB File Offset: 0x00001CFB
		private static void ApplyMinWidth(object component, float px)
		{
			if (component == null || px <= 0f)
			{
				return;
			}
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.UiToolkit.Component.UiComponentLayoutExtensions", component, "MinWidth", new object[]
			{
				new Px(px)
			});
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003B2E File Offset: 0x00001D2E
		private static void ApplyMaxHeight(object component, float px)
		{
			if (component == null || px <= 0f)
			{
				return;
			}
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.UiToolkit.Component.UiComponentLayoutExtensions", component, "MaxHeight", new object[]
			{
				new Px(px)
			});
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003B64 File Offset: 0x00001D64
		private static void ApplyGrow(object component, float grow)
		{
			if (component == null || grow <= 0f)
			{
				return;
			}
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.UiToolkit.Component.UiComponentLayoutExtensions", component, "FlexGrow", new object[] { grow });
			StorageAutoPauseMod.Invoke(component, "Grow", new object[] { grow });
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003BB8 File Offset: 0x00001DB8
		private static void ApplyMinHeight(object component, float px)
		{
			if (component == null || px <= 0f)
			{
				return;
			}
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.UiToolkit.Component.UiComponentLayoutExtensions", component, "MinHeight", new object[]
			{
				new Px(px)
			});
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003BEC File Offset: 0x00001DEC
		private void AddFivePercentSlider(object container, int value, Action<int> changed)
		{
			int num = StorageAutoPauseMod.SnapFive(value) / 5;
			SliderWithIncrements sliderWithIncrements = new SliderWithIncrements().Range(0, 20, true).Value(num, false).OnValueChanged(delegate(int step)
			{
				if (step < 0)
				{
					step = 0;
				}
				if (step > 20)
				{
					step = 20;
				}
				lock (this._stateLock)
				{
					changed(step * 5);
				}
			});
			try
			{
				FieldInfo field = typeof(SliderWithIncrements).GetField("m_fill", BindingFlags.Instance | BindingFlags.NonPublic);
				UiComponent uiComponent = ((field == null) ? null : (field.GetValue(sliderWithIncrements) as UiComponent));
				if (uiComponent != null)
				{
					uiComponent.BackgroundTint(new ColorRgba?(ColorRgba.Green));
				}
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: failed to tint threshold slider: " + ex.Message);
			}
			StorageAutoPauseMod.ApplyGrow(sliderWithIncrements, 1f);
			this.AddToContainer(container, sliderWithIncrements);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003CBC File Offset: 0x00001EBC
		private static int SnapFive(int value)
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 100)
			{
				value = 100;
			}
			return (int)Math.Round((double)value / 5.0, MidpointRounding.AwayFromZero) * 5;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003CE3 File Offset: 0x00001EE3
		private static void SetPanelLike(object component)
		{
			StorageAutoPauseMod.InvokeUiExtension("Mafi.Unity.Ui.Library.UiComponentExtensions", component, "BackgroundPanelLike", Array.Empty<object>());
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003CFC File Offset: 0x00001EFC
		private static object InvokeUiExtension(string extensionTypeName, object component, string methodName, params object[] extraArgs)
		{
			if (component == null)
			{
				return null;
			}
			Type type = StorageAutoPauseMod.FindType(extensionTypeName);
			if (type == null)
			{
				return null;
			}
			IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods.Where((MethodInfo x) => x.Name == methodName && x.GetParameters().Length == extraArgs.Length + 1))
			{
				try
				{
					MethodInfo methodInfo2 = methodInfo;
					if (methodInfo.IsGenericMethodDefinition)
					{
						methodInfo2 = methodInfo.MakeGenericMethod(new Type[] { component.GetType() });
					}
					object[] array = new object[extraArgs.Length + 1];
					array[0] = component;
					for (int i = 0; i < extraArgs.Length; i++)
					{
						array[i + 1] = extraArgs[i];
					}
					return methodInfo2.Invoke(null, array);
				}
				catch
				{
				}
			}
			return null;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003E1C File Offset: 0x0000201C
		private object NewRow(float gap = 6f)
		{
			return new Row(new Px(gap));
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003E29 File Offset: 0x00002029
		private object NewColumn(float gap = 6f)
		{
			return new Column(new Px(gap));
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003E38 File Offset: 0x00002038
		private object AddLabelTo(object container, string text, float grow = 0f)
		{
			object obj = Activator.CreateInstance(this._labelType, new object[]
			{
				new LocStrFormatted(text)
			});
			if (grow > 0f)
			{
				StorageAutoPauseMod.Invoke(obj, "TextGrow", new object[] { grow });
				StorageAutoPauseMod.ApplyGrow(obj, grow);
			}
			this.AddToContainer(container, obj);
			return obj;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003E98 File Offset: 0x00002098
		private object AddButtonTo(object container, string text, Action action, float grow = 1f, bool danger = false)
		{
			Action synchronizedAction = delegate
			{
				lock (this._stateLock)
				{
					action();
				}
			};
			object obj = Activator.CreateInstance(this._buttonTextType, new object[]
			{
				new LocStrFormatted(text),
				synchronizedAction
			});
			StorageAutoPauseMod.Invoke(obj, "TextGrow", new object[] { grow });
			StorageAutoPauseMod.ApplyGrow(obj, grow);
			if (danger)
			{
				StorageAutoPauseMod.Invoke(obj, "Danger", Array.Empty<object>());
				StorageAutoPauseMod.Invoke(obj, "MarkDanger", Array.Empty<object>());
			}
			this.AddToContainer(container, obj);
			return obj;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003F1C File Offset: 0x0000211C
		private void AddPercentSliderTo(object container, int value, Action<int> changed)
		{
			SliderWithIncrements sliderWithIncrements = new SliderWithIncrements().Range(0, 100, true).Value(value, false).OnValueChanged(delegate(int v)
			{
				if (v < 0)
				{
					v = 0;
				}
				if (v > 100)
				{
					v = 100;
				}
				lock (this._stateLock)
				{
					changed(v);
				}
			});
			this.AddToContainer(container, sliderWithIncrements);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003F68 File Offset: 0x00002168
		private void AddToContainer(object container, object component)
		{
			if (container == null || component == null)
			{
				return;
			}
			MethodInfo methodInfo = container.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault<MethodInfo>((MethodInfo x) => x.Name == "Add" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsInstanceOfType(component));
			if (methodInfo == null)
			{
				methodInfo = container.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault<MethodInfo>((MethodInfo x) => x.Name == "Add" && x.GetParameters().Length == 1);
			}
			if (methodInfo == null)
			{
				throw new MissingMethodException(container.GetType().FullName + ".Add(UiComponent) not found.");
			}
			methodInfo.Invoke(container, new object[] { component });
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004023 File Offset: 0x00002223
		private void ToggleAutomationPanel(Machine machine)
		{
			if (machine == null)
			{
				return;
			}
			if (this._expandedMachineId == machine.Id.Value)
			{
				this._expandedMachineId = 0;
				this._editMachineId = 0;
			}
			else
			{
				this._expandedMachineId = machine.Id.Value;
			}
			this.RequestUiRefresh();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004064 File Offset: 0x00002264
		private void RemoveMissingStorages(StorageAutoPauseMod.MachineRule rule)
		{
			bool flag = false;
			for (int i = rule.Conditions.Count - 1; i >= 0; i--)
			{
				Storage storage;
				if (!this._entities.TryGetEntity<Storage>(new global::Mafi.Core.EntityId(rule.Conditions[i].StorageId), out storage))
				{
					rule.Conditions.RemoveAt(i);
					flag = true;
				}
			}
			if (rule.Conditions.Count > 0)
			{
				rule.Conditions[0].Join = StorageAutoPauseMod.LogicJoin.IF;
			}
			if (flag)
			{
				this.SaveRules();
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000040E8 File Offset: 0x000022E8
		private static void SetLabelText(object label, string text)
		{
			if (label == null)
			{
				return;
			}
			try
			{
				Type type = label.GetType().GetInterfaces().FirstOrDefault<Type>((Type x) => x.FullName == "Mafi.Unity.UiToolkit.Component.IComponentWithText");
				MethodInfo methodInfo = ((type == null) ? null : type.GetMethod("SetValue"));
				if (methodInfo != null)
				{
					methodInfo.Invoke(label, new object[]
					{
						new LocStrFormatted(text)
					});
				}
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: failed to refresh label: " + ex.Message);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00004194 File Offset: 0x00002394
		private void AddToPanel(object component)
		{
			this.AddToContainer(this._panelBody, component);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000041A4 File Offset: 0x000023A4
		private static Type FindType(string fullName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type type = assemblies[i].GetType(fullName, false);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000041E4 File Offset: 0x000023E4
		private static object Invoke(object target, string name, params object[] args)
		{
			if (target == null)
			{
				return null;
			}
			IEnumerable<MethodInfo> methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods.Where((MethodInfo x) => x.Name == name && x.GetParameters().Length == args.Length))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				bool flag = true;
				for (int i = 0; i < parameters.Length; i++)
				{
					if (args[i] != null && !parameters[i].ParameterType.IsInstanceOfType(args[i]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return methodInfo.Invoke(target, args);
				}
			}
			return null;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000042D0 File Offset: 0x000024D0
		private void StartSelectingStorage(Machine machine)
		{
			this._waitingMachineId = machine.Id.Value;
			this._lastActiveEntityId = machine.Id.Value;
			Log.Info("StorageAutoPause: waiting for storage selection for machine " + machine.Id.Value.ToString());
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004324 File Offset: 0x00002524
		private bool IsStorageCommodityCompatible(Machine machine, Storage storage, out string reason)
		{
			reason = null;
			return machine != null && storage != null;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004478 File Offset: 0x00002678
		private void MarkMarkersDirty()
		{
			this._markersDirty = true;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004484 File Offset: 0x00002684
		private void OnInputUpdate(GameTime time)
		{
			lock (this._stateLock)
			{
				this.OnInputUpdateSynchronized(time);
			}
		}

		private void OnInputUpdateSynchronized(GameTime time)
		{
			try
			{
				this.ProcessPendingFireworks();
				bool flag = this._uiRefreshDelayInputFrames <= 0;
				if (this._uiRefreshDelayInputFrames > 0)
				{
					this._uiRefreshDelayInputFrames--;
				}
				IEntity firstActiveEntityOrNull = this._inspectors.GetFirstActiveEntityOrNull();
				IEntityInspector firstActiveInspectorOrNull = this._inspectors.GetFirstActiveInspectorOrNull();
				int num = ((firstActiveEntityOrNull == null) ? 0 : firstActiveEntityOrNull.Id.Value);
				if (this._waitingMineTowerId != 0 && num != 0 && num != this._lastActiveEntityId && firstActiveEntityOrNull is OreSortingPlant)
				{
					OreSortingPlant sorter = (OreSortingPlant)firstActiveEntityOrNull;
					MineTower mineTower;
					if (this._entities.TryGetEntity<MineTower>(new global::Mafi.Core.EntityId(this._waitingMineTowerId), out mineTower))
					{
						StorageAutoPauseMod.MineTowerRule mineTowerRule;
						if (!this._towerRules.TryGetValue(mineTower.Id.Value, out mineTowerRule))
						{
							mineTowerRule = new StorageAutoPauseMod.MineTowerRule
							{
								TowerId = mineTower.Id.Value
							};
							this._towerRules[mineTower.Id.Value] = mineTowerRule;
						}
						int owningTowerId;
						if (this.TryGetTowerOwningSorter(sorter.Id.Value, out owningTowerId) && owningTowerId != mineTower.Id.Value)
						{
							this._otherSelectionNotice = "<color=#E79B36>Třídírna je již přiřazena jiné řídicí věži.</color>";
						}
						else if (!mineTowerRule.Sorters.Any<StorageAutoPauseMod.SorterCondition>((StorageAutoPauseMod.SorterCondition x) => x.SorterId == sorter.Id.Value))
						{
							mineTowerRule.Sorters.Add(new StorageAutoPauseMod.SorterCondition
							{
								SorterId = sorter.Id.Value,
								PauseAtPercent = 100,
								ResumeAtPercent = 90,
								LastFillPercent = sorter.PercentFull.ToIntPercentRounded(),
								Active = false,
								PausedByMod = false
							});
							this.SaveRules();
							this.MarkMarkersDirty();
						}
						this._waitingMineTowerId = 0;
						this._pendingOtherRefreshId = mineTower.Id.Value;
						if (owningTowerId == 0 || owningTowerId == mineTower.Id.Value)
						{
							this._otherSelectionNotice = null;
						}
						this._lastActiveEntityId = num;
						this._inspectors.TryActivateFor(mineTower);
						return;
					}
					this._waitingMineTowerId = 0;
				}
				if (this._waitingMachineId != 0 && num != 0 && num != this._lastActiveEntityId && firstActiveEntityOrNull is Storage)
				{
					Storage storage = (Storage)firstActiveEntityOrNull;
					Machine machine;
					if (this._entities.TryGetEntity<Machine>(new global::Mafi.Core.EntityId(this._waitingMachineId), out machine))
					{
						StorageAutoPauseMod.MachineRule machineRule;
						if (!this._rules.TryGetValue(machine.Id.Value, out machineRule))
						{
							machineRule = new StorageAutoPauseMod.MachineRule
							{
								MachineId = machine.Id.Value,
								ElseResume = true,
								PausedByMod = false
							};
							this._rules[machine.Id.Value] = machineRule;
						}
						string text;
						if (this.IsStorageCommodityCompatible(machine, storage, out text))
						{
							if (!machineRule.Conditions.Any<StorageAutoPauseMod.StorageCondition>((StorageAutoPauseMod.StorageCondition x) => x.StorageId == storage.Id.Value))
							{
								machineRule.Conditions.Add(new StorageAutoPauseMod.StorageCondition
								{
									StorageId = storage.Id.Value,
									Join = ((machineRule.Conditions.Count == 0) ? StorageAutoPauseMod.LogicJoin.IF : StorageAutoPauseMod.LogicJoin.AND),
									PauseAtPercent = 100,
									ResumeAtPercent = 90,
									Active = false
								});
								this.SaveRules();
								this.MarkMarkersDirty();
							}
							this._selectionNotice = null;
						}
						else
						{
							if (machineRule.Conditions.Count == 0)
							{
								this._rules.Remove(machine.Id.Value);
							}
							this._selectionNotice = "<color=#E79B36>Nelze přidat sklad:</color> " + text;
							string[] array = new string[6];
							array[0] = "StorageAutoPause: rejected storage ";
							int num2 = 1;
							global::Mafi.Core.EntityId entityId = storage.Id;
							array[num2] = entityId.Value.ToString();
							array[2] = " for machine ";
							int num3 = 3;
							entityId = machine.Id;
							array[num3] = entityId.Value.ToString();
							array[4] = ": ";
							array[5] = text;
							Log.Info(string.Concat(array));
						}
						this._waitingMachineId = 0;
						this._pendingMachineRefreshId = machine.Id.Value;
						this.RequestUiRefresh();
						this._lastActiveEntityId = num;
						this._inspectors.TryActivateFor(machine);
						return;
					}
					this._waitingMachineId = 0;
				}
				if (firstActiveEntityOrNull is LayoutEntityBase && !(firstActiveEntityOrNull is Machine))
				{
					LayoutEntityBase layoutEntityBase = (LayoutEntityBase)firstActiveEntityOrNull;
					if ((layoutEntityBase is MineTower || (this._diagnosticSemaphoreEnabled && this.IsSemaphoreCapable(layoutEntityBase))) && firstActiveInspectorOrNull != null)
					{
						bool flag2 = firstActiveInspectorOrNull != this._otherInspector;
						if ((flag2 || this._otherPanelBody == null) && !this.ActivateOrBuildOtherInspectorPanel(firstActiveInspectorOrNull))
						{
							this._otherInspector = null;
							this._otherPanel = null;
							this._otherPanelBody = null;
						}
						if (this._otherPanelBody != null && firstActiveInspectorOrNull == this._otherInspector)
						{
							bool flag3 = num != this._lastActiveEntityId;
							bool flag4 = this._pendingOtherRefreshId == num;
							bool flag5 = flag && this._uiRefreshRequested;
							if (flag4)
							{
								this._pendingOtherRefreshId = 0;
							}
							if (flag2 || flag3 || flag4 || flag5)
							{
								if (flag)
								{
									this._uiRefreshRequested = false;
								}
								this.RefreshOtherPanel();
							}
						}
					}
				}
				if (firstActiveEntityOrNull is Machine && firstActiveInspectorOrNull != null && firstActiveInspectorOrNull.GetType().FullName == "Mafi.Unity.Ui.Inspectors.MachineInspector")
				{
					bool flag6 = firstActiveInspectorOrNull != this._machineInspector;
					if (flag6)
					{
						this._machineInspector = firstActiveInspectorOrNull;
						this._panel = null;
						this._panelBody = null;
						this.BuildInspectorPanel();
					}
					else if (this._panelBody == null)
					{
						this.BuildInspectorPanel();
					}
					Machine machine2 = (Machine)firstActiveEntityOrNull;
					bool flag7 = num != this._lastActiveEntityId;
					bool flag8 = this._pendingMachineRefreshId == machine2.Id.Value;
					bool uiRefreshRequested = this._uiRefreshRequested;
					bool flag9 = flag6 || flag7;
					bool flag10 = flag && (flag8 || uiRefreshRequested);
					if (flag9 || flag10)
					{
						if (flag8)
						{
							this._pendingMachineRefreshId = 0;
						}
						if (flag)
						{
							this._uiRefreshRequested = false;
						}
						this.RefreshPanel();
					}
				}
				this._lastActiveEntityId = num;
				if (this._markersDirty)
				{
					this._markersDirty = false;
					this.UpdateAutomationMarkers();
				}
				if (this._orangePulseActive || this._brokenPulseActive)
				{
					int num4 = this._orangePulseDivider + 1;
					this._orangePulseDivider = num4;
					if (num4 >= 3)
					{
						this._orangePulseDivider = 0;
						this.UpdateOrangePulseMaterial();
					}
				}
				else
				{
					this._orangePulseDivider = 0;
				}
				if (this._persistenceDirty)
				{
					this._persistenceDirty = false;
					this.SaveRules();
				}
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: UI selection update failed: " + ex.Message);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004AC0 File Offset: 0x00002CC0
		private void SetPausedEntityByAutomation(LayoutEntity entity, bool paused)
		{
			if (entity == null || !entity.CanBePaused)
			{
				return;
			}
			int value = entity.Id.Value;
			this._modPauseMutations.Add(value);
			try
			{
				entity.SetPaused(paused);
			}
			finally
			{
				this._modPauseMutations.Remove(value);
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004B1C File Offset: 0x00002D1C
		private void SetPausedByAutomation(Machine machine, bool paused)
		{
			this.SetPausedEntityByAutomation(machine, paused);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004B28 File Offset: 0x00002D28
		private void OnEntityPauseStateChanged(IEntity entity, bool paused)
		{
			lock (this._stateLock)
			{
				this.OnEntityPauseStateChangedSynchronized(entity, paused);
			}
		}

		private void OnEntityPauseStateChangedSynchronized(IEntity entity, bool paused)
		{
			if (entity is IMaintainedEntity)
			{
				if (paused) this._pausedMaintainedEntities.Add(entity.Id.Value);
				else this._pausedMaintainedEntities.Remove(entity.Id.Value);
			}
			Machine machine = entity as Machine;
			OreSortingPlant oreSortingPlant = entity as OreSortingPlant;
			if (machine == null && oreSortingPlant == null)
			{
				return;
			}
			int id = entity.Id.Value;
			if (this._modPauseMutations.Contains(id))
			{
				return;
			}
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(id, out machineRule))
			{
				foreach (StorageAutoPauseMod.MineTowerRule mineTowerRule in this._towerRules.Values)
				{
					StorageAutoPauseMod.SorterCondition sorterCondition = mineTowerRule.Sorters.FirstOrDefault((StorageAutoPauseMod.SorterCondition x) => x.SorterId == id);
					if (sorterCondition != null)
					{
						if (sorterCondition.PausedByMod || !paused)
						{
							sorterCondition.PausedByMod = false;
							sorterCondition.PlayerOverrideUntilClear = !paused && sorterCondition.Active;
						}
						this.SaveRules();
						this.MarkMarkersDirty();
						this.RequestUiRefresh();
						return;
					}
				}
				return;
			}
			if (machineRule.PausedByMod || !paused)
			{
				machineRule.PausedByMod = false;
				// Manual unpause while the rule is already TRUE owns the current cycle.
				// Automation rearms only after the rule naturally becomes FALSE, which
				// prevents a tug-of-war without deleting the configured logistics.
				machineRule.PlayerOverrideUntilClear = !paused && StorageAutoPauseMod.Evaluate(machineRule.Conditions);
				this._rules[id] = machineRule;
				Log.Info("StorageAutoPause: player pause change observed for machine " + id.ToString() + "; overrideUntilClear=" + machineRule.PlayerOverrideUntilClear.ToString() + ".");
			}
			// Visual state depends on IsPaused even when no ownership flag changes.
			this.MarkMarkersDirty();
			this.RequestUiRefresh();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004CA8 File Offset: 0x00002EA8
		private bool ApplyRuleNow(Machine machine, string reason)
		{
			if (machine == null)
			{
				return false;
			}
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(machine.Id.Value, out machineRule) || machineRule.Conditions.Count == 0)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < machineRule.Conditions.Count; i++)
			{
				StorageAutoPauseMod.StorageCondition storageCondition = machineRule.Conditions[i];
				Storage storage;
				if (this._entities.TryGetEntity<Storage>(new global::Mafi.Core.EntityId(storageCondition.StorageId), out storage))
				{
					int num = storage.PercentFull.ToIntPercentRounded();
					storageCondition.LastFillPercent = num;
					bool active = storageCondition.Active;
					if (num >= storageCondition.PauseAtPercent)
					{
						storageCondition.Active = true;
					}
					else if (num <= storageCondition.ResumeAtPercent)
					{
						storageCondition.Active = false;
					}
					if (active != storageCondition.Active)
					{
						flag = true;
					}
					machineRule.Conditions[i] = storageCondition;
				}
			}
			machineRule.Conditions[0].Join = StorageAutoPauseMod.LogicJoin.IF;
			bool flag2 = StorageAutoPauseMod.Evaluate(machineRule.Conditions);
			bool flag3 = false;
			if (flag2)
			{
				if (!machineRule.PlayerOverrideUntilClear && !machine.IsPaused && machine.CanBePaused)
				{
					this.SetPausedByAutomation(machine, true);
					machineRule.PausedByMod = true;
					flag3 = true;
					Log.Info(string.Concat(new string[]
					{
						"StorageAutoPause: rule TRUE after ",
						reason,
						"; paused machine ",
						machineRule.MachineId.ToString(),
						"."
					}));
				}
			}
			else
			{
				machineRule.PlayerOverrideUntilClear = false;
				if (machineRule.PausedByMod && machine.IsPaused && machine.CanBePaused)
				{
					this.SetPausedByAutomation(machine, false);
				}
				if (machineRule.PausedByMod)
				{
					machineRule.PausedByMod = false;
					flag3 = true;
					Log.Info("StorageAutoPause: rule FALSE after " + reason + "; released machine " + machineRule.MachineId.ToString() + ".");
				}
			}
			this._rules[machine.Id.Value] = machineRule;
			if (flag || flag3)
			{
				this.MarkMarkersDirty();
			}
			return flag || flag3;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004E9C File Offset: 0x0000309C
		private void DetectDiagnosticStateChanges()
		{
			if (!this._diagnosticSemaphoreEnabled || this._markers.Count == 0)
			{
				return;
			}
			foreach (int num in this._markers.Keys)
			{
				IEntity entity;
				if (this._entities.TryGetEntity<IEntity>(new global::Mafi.Core.EntityId(num), out entity))
				{
					StorageAutoPauseMod.DiagnosticLightState diagnosticLightState;
					StorageAutoPauseMod.DiagnosticLightState diagnosticLightState2;
					if (StorageAutoPauseMod.IsEntityBroken(entity))
					{
						diagnosticLightState = StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseA;
						diagnosticLightState2 = StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseB;
					}
					else
					{
						diagnosticLightState = this.GetPowerLightState(entity);
						diagnosticLightState2 = this.GetWorkerLightState(entity);
					}
					int num2 = ((int)diagnosticLightState & 7) | (((int)diagnosticLightState2 & 7) << 3);
					int num3;
					if (!this._diagnosticStateCache.TryGetValue(num, out num3) || num3 != num2)
					{
						this._diagnosticStateCache[num] = num2;
						this.MarkMarkersDirty();
						int num4;
						if (!this._diagnosticLoggedState.TryGetValue(num, out num4) || num4 != num2)
						{
							this._diagnosticLoggedState[num] = num2;
							Log.Info(string.Concat(new string[]
							{
								"StorageAutoPause: semaphore state entity=",
								num.ToString(),
								" power=",
								diagnosticLightState.ToString(),
								" workers=",
								diagnosticLightState2.ToString(),
								" broken=",
								StorageAutoPauseMod.IsEntityBroken(entity).ToString(),
								"."
							}));
						}
					}
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00005020 File Offset: 0x00003220
		private void OnSimUpdate()
		{
			lock (this._stateLock)
			{
				this.OnSimUpdateSynchronized();
			}
		}

		private void PrimeBrokenEntityBaseline()
		{
			this._knownBrokenEntities.Clear();
			this._pausedMaintainedEntities.Clear();
			foreach (IMaintainedEntity maintained in this._entities.GetAllEntitiesOfType<IMaintainedEntity>())
			{
				IEntity entity = maintained as IEntity;
				if (entity == null) continue;
				if (StorageAutoPauseMod.IsEntityBroken(entity)) this._knownBrokenEntities.Add(entity.Id.Value);
				if (entity.IsPaused) this._pausedMaintainedEntities.Add(entity.Id.Value);
			}
		}

		private void OnEntityEnabledChanged(IEntity entity, bool enabled)
		{
			lock (this._stateLock)
			{
				if (!(entity is IMaintainedEntity)) return;
				this.HandleBreakdownState(entity);
			}
		}

		private void HandleBreakdownState(IEntity entity)
		{
			int id = entity.Id.Value;
			if (StorageAutoPauseMod.IsEntityBroken(entity))
			{
				if (this._knownBrokenEntities.Add(id) && this._breakdownFireworksEnabled && DateTime.UtcNow >= this._fireworkSuppressUntilUtc)
				{
					this._pendingFireworks.Enqueue(id);
					Log.Info("StorageAutoPause: new breakdown detected; queued firework for entity " + id.ToString() + ".");
				}
			}
			else this._knownBrokenEntities.Remove(id);
		}

		private void DetectBreakdownsOfPausedEntities()
		{
			foreach (int id in this._pausedMaintainedEntities.ToArray<int>())
			{
				IEntity entity;
				if (!this._entities.TryGetEntity<IEntity>(new global::Mafi.Core.EntityId(id), out entity) || !(entity is IMaintainedEntity) || !entity.IsPaused)
				{
					this._pausedMaintainedEntities.Remove(id);
					continue;
				}
				this.HandleBreakdownState(entity);
			}
		}

		private void InitializeFireworkAudio()
		{
			try
			{
				string path = Path.Combine(this.Manifest.RootDirectoryPath, "Assets", "firework_cc0.ogg");
				if (!File.Exists(path)) return;
				GameObject host = new GameObject("StorageAutoPause.FireworkAudioLoader");
				host.layer = 2;
				UnityEngine.Object.DontDestroyOnLoad(host);
				StorageAutoPauseMod.FireworkAudioLoader loader = host.AddComponent<StorageAutoPauseMod.FireworkAudioLoader>();
				loader.Begin(path);
				this._fireworkAudioHost = host;
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: firework audio initialization failed: " + ex.Message);
			}
		}

		private void ProcessPendingFireworks()
		{
			while (this._pendingFireworks.Count > 0)
			{
				int id = this._pendingFireworks.Dequeue();
				IEntity entity;
				if (!this._entities.TryGetEntity<IEntity>(new global::Mafi.Core.EntityId(id), out entity)) continue;
				Vector3 position;
				bool isVehicle = entity is Vehicle;
				StorageAutoPauseMod.AutomationMarker marker;
				if (this._markers.TryGetValue(id, out marker) && marker.Root != null) position = marker.Root.transform.position + Vector3.up * 7.1f;
				else if (this.TryFindCanonicalMarkerPosition(entity, out position)) position += Vector3.up * 7.1f;
				else if (isVehicle) position = ((Vehicle)entity).Position3f.ToVector3() + Vector3.up * 0.8f;
				else continue;
				if (!this.EnsureSharedMarkerResources()) continue;
				if (isVehicle) StorageAutoPauseMod.FireworkController.SpawnVehicleSmoke(position, this._sharedSmokeMaterial);
				StorageAutoPauseMod.FireworkController.Spawn(position, id, this._sharedTipRedMaterial, this._sharedTipOrangeMaterial, this._sharedTipGreenMaterial, this._sharedSmokeMaterial, this._breakdownFireworksSoundEnabled);
			}
		}

		private void OnSimUpdateSynchronized()
		{
			int i = this._tickDivider + 1;
			this._tickDivider = i;
			if (i < 20)
			{
				return;
			}
			this._tickDivider = 0;
			this.DetectBreakdownsOfPausedEntities();
			bool flag = false;
			bool flag2 = false;
			bool @bool = this.JsonConfig.GetBool("diagnostic_semaphore_enabled", true);
			this._breakdownFireworksEnabled = this.JsonConfig.GetBool("breakdown_fireworks_enabled", true);
			this._breakdownFireworksSoundEnabled = this.JsonConfig.GetBool("breakdown_fireworks_sound_enabled", true);
			if (@bool != this._diagnosticSemaphoreEnabled)
			{
				this._diagnosticSemaphoreEnabled = @bool;
				flag2 = true;
				this._diagnosticStateCache.Clear();
			}
			this.DetectDiagnosticStateChanges();
			this._ruleIterationBuffer.Clear();
			foreach (int num in this._rules.Keys)
			{
				this._ruleIterationBuffer.Add(num);
			}
			for (int j = 0; j < this._ruleIterationBuffer.Count; j++)
			{
				int num2 = this._ruleIterationBuffer[j];
				StorageAutoPauseMod.MachineRule machineRule;
				if (this._rules.TryGetValue(num2, out machineRule))
				{
					Machine machine;
					if (!this._entities.TryGetEntity<Machine>(new global::Mafi.Core.EntityId(machineRule.MachineId), out machine))
					{
						this._rules.Remove(num2);
						flag = true;
					}
					else
					{
						for (int k = machineRule.Conditions.Count - 1; k >= 0; k--)
						{
							StorageAutoPauseMod.StorageCondition storageCondition = machineRule.Conditions[k];
							Storage storage;
							if (!this._entities.TryGetEntity<Storage>(new global::Mafi.Core.EntityId(storageCondition.StorageId), out storage))
							{
								machineRule.Conditions.RemoveAt(k);
								flag = true;
							}
							else
							{
								bool flag3 = StorageAutoPauseMod.IsApproachingThreshold(storageCondition);
								int oldTrend = storageCondition.FillTrend;
								int num3 = storage.PercentFull.ToIntPercentRounded();
								StorageAutoPauseMod.UpdateFillTrend(storageCondition, num3);
								bool active = storageCondition.Active;
								if (num3 >= storageCondition.PauseAtPercent)
								{
									storageCondition.Active = true;
								}
								else if (num3 <= storageCondition.ResumeAtPercent)
								{
									storageCondition.Active = false;
								}
								bool flag4 = StorageAutoPauseMod.IsApproachingThreshold(storageCondition);
								if (active != storageCondition.Active)
								{
									flag = true;
									flag2 = true;
								}
								if (flag3 != flag4)
								{
									flag2 = true;
								}
								if (oldTrend != storageCondition.FillTrend)
								{
									flag2 = true;
								}
								machineRule.Conditions[k] = storageCondition;
							}
						}
						if (machineRule.Conditions.Count == 0)
						{
							this.ReleaseRulePause(machineRule, machine, "žádné aktivní sklady");
							this._rules.Remove(num2);
							flag = true;
						}
						else
						{
							machineRule.Conditions[0].Join = StorageAutoPauseMod.LogicJoin.IF;
							if (StorageAutoPauseMod.Evaluate(machineRule.Conditions))
							{
								if (!machineRule.PlayerOverrideUntilClear && !machine.IsPaused && machine.CanBePaused)
								{
									this.SetPausedByAutomation(machine, true);
									machineRule.PausedByMod = true;
									flag = true;
									flag2 = true;
									Log.Info("StorageAutoPause: rule TRUE; paused machine " + machineRule.MachineId.ToString() + ".");
								}
							}
							else
							{
								machineRule.PlayerOverrideUntilClear = false;
								if (machineRule.PausedByMod && machine.IsPaused && machine.CanBePaused)
								{
									this.SetPausedByAutomation(machine, false);
								}
								if (machineRule.PausedByMod)
								{
									machineRule.PausedByMod = false;
									flag = true;
									flag2 = true;
									Log.Info("StorageAutoPause: rule FALSE; released machine " + machineRule.MachineId.ToString() + ".");
								}
							}
							this._rules[num2] = machineRule;
						}
					}
				}
			}
			foreach (StorageAutoPauseMod.MineTowerRule mineTowerRule in this._towerRules.Values.ToArray<StorageAutoPauseMod.MineTowerRule>())
			{
				MineTower mineTower;
				if (!this._entities.TryGetEntity<MineTower>(new global::Mafi.Core.EntityId(mineTowerRule.TowerId), out mineTower))
				{
					this._towerRules.Remove(mineTowerRule.TowerId);
					flag2 = true;
				}
				else
				{
					for (int l = mineTowerRule.Sorters.Count - 1; l >= 0; l--)
					{
						StorageAutoPauseMod.SorterCondition sorterCondition = mineTowerRule.Sorters[l];
						OreSortingPlant oreSortingPlant;
						if (!this._entities.TryGetEntity<OreSortingPlant>(new global::Mafi.Core.EntityId(sorterCondition.SorterId), out oreSortingPlant))
						{
							mineTowerRule.Sorters.RemoveAt(l);
							flag2 = true;
						}
						else
						{
							bool flag5 = StorageAutoPauseMod.IsApproachingSorterThreshold(sorterCondition);
							int oldSorterTrend = sorterCondition.FillTrend;
							int num4 = oreSortingPlant.PercentFull.ToIntPercentRounded();
							StorageAutoPauseMod.UpdateSorterFillTrend(sorterCondition, num4);
							bool active2 = sorterCondition.Active;
							if (num4 >= sorterCondition.PauseAtPercent)
							{
								sorterCondition.Active = true;
							}
							else if (num4 <= sorterCondition.ResumeAtPercent)
							{
								sorterCondition.Active = false;
							}
							bool flag6 = StorageAutoPauseMod.IsApproachingSorterThreshold(sorterCondition);
							if (flag5 != flag6 || active2 != sorterCondition.Active || oldSorterTrend != sorterCondition.FillTrend)
							{
								flag2 = true;
								if (active2 != sorterCondition.Active)
								{
									this._persistenceDirty = true;
								}
							}
							if (sorterCondition.Active)
							{
								if (!sorterCondition.PlayerOverrideUntilClear && !oreSortingPlant.IsPaused && oreSortingPlant.CanBePaused)
								{
									this.SetPausedEntityByAutomation(oreSortingPlant, true);
									sorterCondition.PausedByMod = true;
									this._persistenceDirty = true;
									flag2 = true;
								}
							}
							else
							{
								sorterCondition.PlayerOverrideUntilClear = false;
								if (sorterCondition.PausedByMod && oreSortingPlant.IsPaused && oreSortingPlant.CanBePaused)
								{
									this.SetPausedEntityByAutomation(oreSortingPlant, false);
								}
								if (sorterCondition.PausedByMod)
								{
									sorterCondition.PausedByMod = false;
									this._persistenceDirty = true;
									flag2 = true;
								}
							}
						}
					}
					if (mineTowerRule.Sorters.Count == 0)
					{
						this._towerRules.Remove(mineTowerRule.TowerId);
					}
				}
			}
			if (flag2)
			{
				this.MarkMarkersDirty();
				this.RequestUiRefresh();
			}
			if (flag)
			{
				this._persistenceDirty = true;
				this.MarkMarkersDirty();
				this.RequestUiRefresh();
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00005504 File Offset: 0x00003704
		private static bool Evaluate(List<StorageAutoPauseMod.StorageCondition> conditions)
		{
			if (conditions.Count == 0)
			{
				return false;
			}
			bool flag = conditions[0].Active;
			for (int i = 1; i < conditions.Count; i++)
			{
				if (conditions[i].Join == StorageAutoPauseMod.LogicJoin.AND)
				{
					flag = flag && conditions[i].Active;
				}
				else
				{
					flag = flag || conditions[i].Active;
				}
			}
			return flag;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00005574 File Offset: 0x00003774
		private void CycleJoin(Machine machine, int index)
		{
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(machine.Id.Value, out machineRule) || index <= 0 || index >= machineRule.Conditions.Count)
			{
				return;
			}
			StorageAutoPauseMod.StorageCondition storageCondition = machineRule.Conditions[index];
			storageCondition.Join = ((storageCondition.Join == StorageAutoPauseMod.LogicJoin.AND) ? StorageAutoPauseMod.LogicJoin.OR : StorageAutoPauseMod.LogicJoin.AND);
			machineRule.Conditions[index] = storageCondition;
			this._rules[machine.Id.Value] = machineRule;
			this.SaveRules();
			this.ApplyRuleNow(machine, "změna AND/OR");
			this.MarkMarkersDirty();
			this.RequestUiRefresh();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00005614 File Offset: 0x00003814
		private int SetPauseThreshold(Machine machine, int index, int value)
		{
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(machine.Id.Value, out machineRule) || index < 0 || index >= machineRule.Conditions.Count)
			{
				return StorageAutoPauseMod.SnapFive(value);
			}
			StorageAutoPauseMod.StorageCondition storageCondition = machineRule.Conditions[index];
			value = StorageAutoPauseMod.SnapFive(value);
			int num = Math.Min(100, storageCondition.ResumeAtPercent + 5);
			if (value < num)
			{
				value = num;
			}
			storageCondition.PauseAtPercent = value;
			machineRule.Conditions[index] = storageCondition;
			this.SaveRules();
			return value;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000569C File Offset: 0x0000389C
		private int SetResumeThreshold(Machine machine, int index, int value)
		{
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(machine.Id.Value, out machineRule) || index < 0 || index >= machineRule.Conditions.Count)
			{
				return StorageAutoPauseMod.SnapFive(value);
			}
			StorageAutoPauseMod.StorageCondition storageCondition = machineRule.Conditions[index];
			value = StorageAutoPauseMod.SnapFive(value);
			int num = Math.Max(0, storageCondition.PauseAtPercent - 5);
			if (value > num)
			{
				value = num;
			}
			storageCondition.ResumeAtPercent = value;
			machineRule.Conditions[index] = storageCondition;
			this.SaveRules();
			return value;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00005724 File Offset: 0x00003924
		private void RemoveStorage(Machine machine, int storageId)
		{
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(machine.Id.Value, out machineRule))
			{
				return;
			}
			machineRule.Conditions.RemoveAll((StorageAutoPauseMod.StorageCondition x) => x.StorageId == storageId);
			if (machineRule.Conditions.Count > 0)
			{
				machineRule.Conditions[0].Join = StorageAutoPauseMod.LogicJoin.IF;
				if (!StorageAutoPauseMod.Evaluate(machineRule.Conditions) && machineRule.PausedByMod)
				{
					this.ReleaseRulePause(machineRule, machine, "odpojení skladu");
				}
				this._rules[machine.Id.Value] = machineRule;
			}
			else
			{
				this.ReleaseRulePause(machineRule, machine, "odpojení posledního skladu");
				this._rules.Remove(machine.Id.Value);
			}
			this.SaveRules();
			this.MarkMarkersDirty();
			this.RequestUiRefresh();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005804 File Offset: 0x00003A04
		private void ClearRule(Machine machine)
		{
			StorageAutoPauseMod.MachineRule machineRule;
			if (this._rules.TryGetValue(machine.Id.Value, out machineRule))
			{
				this.ReleaseRulePause(machineRule, machine, "odstranění automatiky");
				this._rules.Remove(machine.Id.Value);
				this._expandedMachineId = 0;
				this.SaveRules();
				this.MarkMarkersDirty();
			}
			this.RequestUiRefresh();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00005868 File Offset: 0x00003A68
		private void ReleaseRulePause(StorageAutoPauseMod.MachineRule rule, Machine machine, string reason)
		{
			if (!rule.PausedByMod)
			{
				return;
			}
			try
			{
				if (machine != null && machine.IsPaused && machine.CanBePaused)
				{
					this.SetPausedByAutomation(machine, false);
				}
				rule.PausedByMod = false;
				Log.Info(string.Concat(new string[]
				{
					"StorageAutoPause: released machine ",
					rule.MachineId.ToString(),
					" because ",
					reason,
					"."
				}));
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: failed to release pause: " + ex.Message);
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005908 File Offset: 0x00003B08
		private string GetEntityTitle(IEntity entity)
		{
			string text2;
			try
			{
				string text = entity.DefaultTitle.ToString();
				text2 = (string.IsNullOrWhiteSpace(text) ? entity.Prototype.Id.ToString() : text);
			}
			catch
			{
				text2 = "Entity #" + entity.Id.Value.ToString();
			}
			return text2;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005984 File Offset: 0x00003B84
		private void InitPersistence()
		{
			string text = Path.Combine(this.Manifest.RootDirectoryPath, "Bindings");
			this._dataPath = Path.Combine(text, StorageAutoPauseMod.Sanitize(this._saveManager.GameName) + ".bindings");
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000059D0 File Offset: 0x00003BD0
		private static string Sanitize(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return "game";
			}
			foreach (char c in Path.GetInvalidFileNameChars())
			{
				name = name.Replace(c, '_');
			}
			return name;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00005A10 File Offset: 0x00003C10
		private void LoadRules()
		{
			this._rules.Clear();
			this._towerRules.Clear();
			this._manualSemaphoreEntities.Clear();
			string @string = this.JsonConfig.GetString("automation_state", "");
			if (!string.IsNullOrWhiteSpace(@string))
			{
				this.ParseRules(@string.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
				this.NormalizeSorterOwnership();
				Log.Info("StorageAutoPause: loaded automation rules from save-integrated ModJsonConfig.");
				return;
			}
			if (!string.IsNullOrEmpty(this._dataPath) && File.Exists(this._dataPath))
			{
				this.ParseRules(File.ReadAllLines(this._dataPath));
				this.NormalizeSorterOwnership();
				this.SaveRules();
				Log.Info("StorageAutoPause: migrated legacy .bindings rules into save-integrated state.");
			}
		}

		private void NormalizeSorterOwnership()
		{
			HashSet<int> claimedSorters = new HashSet<int>();
			foreach (StorageAutoPauseMod.MineTowerRule rule in this._towerRules.Values.OrderBy((StorageAutoPauseMod.MineTowerRule x) => x.TowerId))
			{
				for (int i = rule.Sorters.Count - 1; i >= 0; i--)
				{
					if (!claimedSorters.Add(rule.Sorters[i].SorterId))
					{
						Log.Warning("StorageAutoPause: duplicate Ore Sorting Plant assignment removed from Mine Tower " + rule.TowerId.ToString() + ".");
						rule.Sorters.RemoveAt(i);
					}
				}
			}
			foreach (int emptyTower in this._towerRules.Where((KeyValuePair<int, StorageAutoPauseMod.MineTowerRule> x) => x.Value.Sorters.Count == 0).Select((KeyValuePair<int, StorageAutoPauseMod.MineTowerRule> x) => x.Key).ToArray())
			{
				this._towerRules.Remove(emptyTower);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00005ABC File Offset: 0x00003CBC
		private void ParseRules(IEnumerable<string> lines)
		{
			foreach (string text in lines)
			{
				if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("#"))
				{
					string[] array = text.Split(new char[] { '|' }, StringSplitOptions.None);
					int num10;
					if (array.Length >= 5 && array[0] == "M")
					{
						int num;
						bool flag;
						bool flag2;
						if (int.TryParse(array[1], out num) && bool.TryParse(array[2], out flag) && bool.TryParse(array[3], out flag2))
						{
							StorageAutoPauseMod.MachineRule machineRule = new StorageAutoPauseMod.MachineRule
							{
								MachineId = num,
								PausedByMod = flag,
								ElseResume = true,
								PlayerOverrideUntilClear = array.Length >= 6 && string.Equals(array[5], "True", StringComparison.OrdinalIgnoreCase)
							};
							string[] array2 = array[4].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < array2.Length; i++)
							{
									string[] array3 = array2[i].Split(new char[] { ':' }, StringSplitOptions.None);
								int num2;
								StorageAutoPauseMod.LogicJoin logicJoin;
								int num3;
								int num4;
								bool flag3;
								if (array3.Length >= 5 && int.TryParse(array3[0], out num2) && Enum.TryParse<StorageAutoPauseMod.LogicJoin>(array3[1], out logicJoin) && int.TryParse(array3[2], out num3) && int.TryParse(array3[3], out num4) && bool.TryParse(array3[4], out flag3))
								{
									StorageAutoPauseMod.NormalizeThresholds(ref num3, ref num4);
									machineRule.Conditions.Add(new StorageAutoPauseMod.StorageCondition
									{
										StorageId = num2,
										Join = ((machineRule.Conditions.Count == 0) ? StorageAutoPauseMod.LogicJoin.IF : ((logicJoin == StorageAutoPauseMod.LogicJoin.IF) ? StorageAutoPauseMod.LogicJoin.OR : logicJoin)),
										PauseAtPercent = num3,
										ResumeAtPercent = num4,
										Active = flag3
									});
								}
							}
							if (machineRule.Conditions.Count > 0)
							{
								this._rules[num] = machineRule;
							}
						}
					}
					else if (array.Length >= 3 && array[0] == "T")
					{
						int num5;
						if (int.TryParse(array[1], out num5))
						{
							StorageAutoPauseMod.MineTowerRule mineTowerRule = new StorageAutoPauseMod.MineTowerRule
							{
								TowerId = num5
							};
							string[] array2 = array[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < array2.Length; i++)
							{
									string[] array4 = array2[i].Split(new char[] { ':' }, StringSplitOptions.None);
								int num6;
								int num7;
								int num8;
								if (array4.Length >= 3 && int.TryParse(array4[0], out num6) && int.TryParse(array4[1], out num7) && int.TryParse(array4[2], out num8))
								{
									bool sorterActive = array4.Length >= 4 && string.Equals(array4[3], "True", StringComparison.OrdinalIgnoreCase);
									bool sorterPausedByMod = array4.Length >= 5 && string.Equals(array4[4], "True", StringComparison.OrdinalIgnoreCase);
									bool sorterOverride = array4.Length >= 6 && string.Equals(array4[5], "True", StringComparison.OrdinalIgnoreCase);
									StorageAutoPauseMod.NormalizeThresholds(ref num7, ref num8);
									mineTowerRule.Sorters.Add(new StorageAutoPauseMod.SorterCondition
									{
										SorterId = num6,
										PauseAtPercent = num7,
										ResumeAtPercent = num8,
										Active = sorterActive,
										PausedByMod = sorterPausedByMod,
										PlayerOverrideUntilClear = sorterOverride
									});
								}
							}
							if (mineTowerRule.Sorters.Count > 0)
							{
								this._towerRules[num5] = mineTowerRule;
							}
						}
					}
					else if (array.Length >= 2 && array[0] == "S")
					{
						string[] array2 = array[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < array2.Length; i++)
						{
							int num9;
							if (int.TryParse(array2[i], out num9))
							{
								this._manualSemaphoreEntities.Add(num9);
							}
						}
					}
					else if (int.TryParse(array[0], out num10))
					{
						if (array.Length >= 6)
						{
							int num11 = 100;
							int num12 = 90;
							int num13;
							bool flag4;
							bool flag5;
							if (int.TryParse(array[1], out num13) && bool.TryParse(array[2], out flag4) && bool.TryParse(array[3], out flag5))
							{
								int.TryParse(array[4], out num11);
								int.TryParse(array[5], out num12);
								StorageAutoPauseMod.NormalizeThresholds(ref num11, ref num12);
								this.AddMigratedCondition(num13, num10, flag5, flag4, num11, num12);
								continue;
							}
						}
						bool flag6;
						int num14;
						int num15;
						if (array.Length >= 5 && bool.TryParse(array[2], out flag6) && int.TryParse(array[3], out num14) && int.TryParse(array[4], out num15))
						{
							StorageAutoPauseMod.NormalizeThresholds(ref num14, ref num15);
							string[] array2 = array[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < array2.Length; i++)
							{
									string[] array5 = array2[i].Split(new char[] { ':' }, StringSplitOptions.None);
								bool flag7 = false;
								int num16;
								if (array5.Length >= 1 && int.TryParse(array5[0], out num16))
								{
									if (array5.Length >= 2)
									{
										bool.TryParse(array5[1], out flag7);
									}
									this.AddMigratedCondition(num16, num10, flag7, flag6, num14, num15);
								}
							}
						}
					}
				}
			}
			foreach (StorageAutoPauseMod.MachineRule machineRule2 in this._rules.Values)
			{
				if (machineRule2.Conditions.Count > 0)
				{
					machineRule2.Conditions[0].Join = StorageAutoPauseMod.LogicJoin.IF;
				}
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005FA8 File Offset: 0x000041A8
		private void AddMigratedCondition(int machineId, int storageId, bool pausedByMod, bool elseResume, int pause, int resume)
		{
			StorageAutoPauseMod.MachineRule machineRule;
			if (!this._rules.TryGetValue(machineId, out machineRule))
			{
				machineRule = new StorageAutoPauseMod.MachineRule
				{
					MachineId = machineId,
					PausedByMod = pausedByMod,
					ElseResume = elseResume
				};
				this._rules[machineId] = machineRule;
			}
			else
			{
				machineRule.PausedByMod = machineRule.PausedByMod || pausedByMod;
			}
			if (!machineRule.Conditions.Any<StorageAutoPauseMod.StorageCondition>((StorageAutoPauseMod.StorageCondition x) => x.StorageId == storageId))
			{
				machineRule.Conditions.Add(new StorageAutoPauseMod.StorageCondition
				{
					StorageId = storageId,
					Join = ((machineRule.Conditions.Count == 0) ? StorageAutoPauseMod.LogicJoin.IF : StorageAutoPauseMod.LogicJoin.AND),
					PauseAtPercent = pause,
					ResumeAtPercent = resume,
					Active = false
				});
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000606E File Offset: 0x0000426E
		private static void NormalizeThresholds(ref int pause, ref int resume)
		{
			if (pause < 1 || pause > 100)
			{
				pause = 100;
			}
			if (resume < 0 || resume >= pause)
			{
				resume = Math.Max(0, pause - 10);
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00006098 File Offset: 0x00004298
		private string SerializeRules()
		{
			List<string> list = new List<string>();
			foreach (StorageAutoPauseMod.MachineRule machineRule in this._rules.Values.OrderBy<StorageAutoPauseMod.MachineRule, int>((StorageAutoPauseMod.MachineRule x) => x.MachineId))
			{
				string text = string.Join(",", machineRule.Conditions.Select<StorageAutoPauseMod.StorageCondition, string>((StorageAutoPauseMod.StorageCondition c) => string.Concat(new string[]
				{
					c.StorageId.ToString(),
					":",
					c.Join.ToString(),
					":",
					c.PauseAtPercent.ToString(),
					":",
					c.ResumeAtPercent.ToString(),
					":" + c.Active.ToString()
				})));
				list.Add("M|" + machineRule.MachineId.ToString() + "|" + machineRule.PausedByMod.ToString() + "|True|" + text + "|" + machineRule.PlayerOverrideUntilClear.ToString());
			}
			foreach (StorageAutoPauseMod.MineTowerRule mineTowerRule in this._towerRules.Values.OrderBy<StorageAutoPauseMod.MineTowerRule, int>((StorageAutoPauseMod.MineTowerRule x) => x.TowerId))
			{
				string text2 = string.Join(",", mineTowerRule.Sorters.Select<StorageAutoPauseMod.SorterCondition, string>((StorageAutoPauseMod.SorterCondition c) => string.Concat(new string[]
				{
					c.SorterId.ToString(),
					":",
					c.PauseAtPercent.ToString(),
					":",
					c.ResumeAtPercent.ToString(),
					":",
					c.Active.ToString(),
					":",
					c.PausedByMod.ToString(),
					":",
					c.PlayerOverrideUntilClear.ToString()
				})));
				list.Add("T|" + mineTowerRule.TowerId.ToString() + "|" + text2);
			}
			if (this._manualSemaphoreEntities.Count > 0)
			{
				list.Add("S|" + string.Join<int>(",", this._manualSemaphoreEntities.OrderBy<int, int>((int x) => x)));
			}
			return string.Join("\n", list);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00006280 File Offset: 0x00004480
		private void SaveRules()
		{
			try
			{
				string text;
				if (!this.JsonConfig.TrySetValue("automation_state", this.SerializeRules(), out text))
				{
					Log.Error("StorageAutoPause: failed to persist automation state into save config: " + text);
				}
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: failed to persist rules into save: " + ex.Message);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000062E4 File Offset: 0x000044E4
		private void UpdateAutomationMarkers()
		{
			Dictionary<int, StorageAutoPauseMod.MarkerState> desiredMarkerStates = this._desiredMarkerStates;
			desiredMarkerStates.Clear();
			bool flag = false;
			bool flag2 = false;
			foreach (StorageAutoPauseMod.MachineRule machineRule in this._rules.Values)
			{
				Machine machine;
				if (this._entities.TryGetEntity<Machine>(new global::Mafi.Core.EntityId(machineRule.MachineId), out machine))
				{
					bool flag3 = machineRule.Conditions.Any<StorageAutoPauseMod.StorageCondition>(new Func<StorageAutoPauseMod.StorageCondition, bool>(StorageAutoPauseMod.IsApproachingThreshold));
					bool ruleActive = StorageAutoPauseMod.Evaluate(machineRule.Conditions);
					StorageAutoPauseMod.MarkerVisualState trendState = StorageAutoPauseMod.GetFillTrendState(machineRule.Conditions);
					desiredMarkerStates[machineRule.MachineId] = this.BuildMarkerState(machine, false, (machine.IsPaused && !machineRule.PausedByMod) ? StorageAutoPauseMod.MarkerVisualState.ManualPaused : (ruleActive ? StorageAutoPauseMod.MarkerVisualState.AutomationPaused : (flag3 ? StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold : trendState)), true);
				}
				foreach (StorageAutoPauseMod.StorageCondition storageCondition in machineRule.Conditions)
				{
					Storage storage;
					if (this._entities.TryGetEntity<Storage>(new global::Mafi.Core.EntityId(storageCondition.StorageId), out storage))
					{
						bool flag4 = storageCondition.Active;
						bool flag5 = StorageAutoPauseMod.IsApproachingThreshold(storageCondition);
						bool filling = storageCondition.FillTrend > 0;
						bool draining = storageCondition.FillTrend < 0;
						StorageAutoPauseMod.MarkerState markerState;
						if (desiredMarkerStates.TryGetValue(storageCondition.StorageId, out markerState))
						{
							flag4 = flag4 || markerState.LogisticsState == StorageAutoPauseMod.MarkerVisualState.AutomationPaused;
							flag5 = flag5 || markerState.LogisticsState == StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold;
							filling = filling || markerState.LogisticsState == StorageAutoPauseMod.MarkerVisualState.FillingUp;
							draining = draining || markerState.LogisticsState == StorageAutoPauseMod.MarkerVisualState.DrainingDown;
						}
						desiredMarkerStates[storageCondition.StorageId] = this.BuildMarkerState(storage, true, flag4 ? StorageAutoPauseMod.MarkerVisualState.AutomationPaused : (flag5 ? StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold : (filling ? StorageAutoPauseMod.MarkerVisualState.FillingUp : (draining ? StorageAutoPauseMod.MarkerVisualState.DrainingDown : StorageAutoPauseMod.MarkerVisualState.Normal))), true);
					}
				}
			}
			foreach (StorageAutoPauseMod.MineTowerRule mineTowerRule in this._towerRules.Values)
			{
				MineTower mineTower;
				if (this._entities.TryGetEntity<MineTower>(new global::Mafi.Core.EntityId(mineTowerRule.TowerId), out mineTower))
				{
					desiredMarkerStates[mineTowerRule.TowerId] = this.BuildMarkerState(mineTower, false, StorageAutoPauseMod.MarkerVisualState.Normal, true);
				}
				foreach (StorageAutoPauseMod.SorterCondition sorterCondition in mineTowerRule.Sorters)
				{
					OreSortingPlant oreSortingPlant;
					if (this._entities.TryGetEntity<OreSortingPlant>(new global::Mafi.Core.EntityId(sorterCondition.SorterId), out oreSortingPlant))
					{
						StorageAutoPauseMod.MarkerVisualState sorterTrendState = sorterCondition.FillTrend > 0 ? StorageAutoPauseMod.MarkerVisualState.FillingUp : (sorterCondition.FillTrend < 0 ? StorageAutoPauseMod.MarkerVisualState.DrainingDown : StorageAutoPauseMod.MarkerVisualState.Normal);
						StorageAutoPauseMod.MarkerVisualState markerVisualState = ((oreSortingPlant.IsPaused && !sorterCondition.PausedByMod) ? StorageAutoPauseMod.MarkerVisualState.ManualPaused : (sorterCondition.Active ? StorageAutoPauseMod.MarkerVisualState.AutomationPaused : (StorageAutoPauseMod.IsApproachingSorterThreshold(sorterCondition) ? StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold : sorterTrendState)));
						desiredMarkerStates[sorterCondition.SorterId] = this.BuildMarkerState(oreSortingPlant, false, markerVisualState, true);
					}
				}
			}
			if (this._diagnosticSemaphoreEnabled)
			{
				foreach (int num in this._manualSemaphoreEntities.ToArray<int>())
				{
					LayoutEntityBase layoutEntityBase;
					StorageAutoPauseMod.MarkerState markerState2;
					if (!this._entities.TryGetEntity<LayoutEntityBase>(new global::Mafi.Core.EntityId(num), out layoutEntityBase))
					{
						this._manualSemaphoreEntities.Remove(num);
					}
					else if (desiredMarkerStates.TryGetValue(num, out markerState2))
					{
						desiredMarkerStates[num] = this.BuildMarkerState(layoutEntityBase, markerState2.IsStorage, markerState2.LogisticsState, true);
					}
					else
					{
						desiredMarkerStates[num] = this.BuildMarkerState(layoutEntityBase, false, StorageAutoPauseMod.MarkerVisualState.Off, true);
					}
				}
			}
			foreach (StorageAutoPauseMod.MarkerState markerState3 in desiredMarkerStates.Values)
			{
				if (markerState3.LogisticsState == StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold || markerState3.LogisticsState == StorageAutoPauseMod.MarkerVisualState.FillingUp || markerState3.LogisticsState == StorageAutoPauseMod.MarkerVisualState.DrainingDown || markerState3.PowerState == StorageAutoPauseMod.DiagnosticLightState.Warning || markerState3.WorkerState == StorageAutoPauseMod.DiagnosticLightState.Warning)
				{
					flag = true;
				}
				if (markerState3.PowerState == StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseA || markerState3.PowerState == StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseB || markerState3.WorkerState == StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseA || markerState3.WorkerState == StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseB)
				{
					flag2 = true;
				}
			}
			this._orangePulseActive = flag;
			this._brokenPulseActive = flag2;
			this._markerRemovalBuffer.Clear();
			foreach (int num2 in this._markers.Keys)
			{
				if (!desiredMarkerStates.ContainsKey(num2))
				{
					this._markerRemovalBuffer.Add(num2);
				}
			}
			for (int j = 0; j < this._markerRemovalBuffer.Count; j++)
			{
				int num3 = this._markerRemovalBuffer[j];
				StorageAutoPauseMod.AutomationMarker automationMarker;
				if (this._markers.TryGetValue(num3, out automationMarker))
				{
					StorageAutoPauseMod.DestroyMarker(automationMarker);
				}
				this._markers.Remove(num3);
			}
			foreach (KeyValuePair<int, StorageAutoPauseMod.MarkerState> keyValuePair in desiredMarkerStates)
			{
				StorageAutoPauseMod.AutomationMarker automationMarker2;
				if (!this._markers.TryGetValue(keyValuePair.Key, out automationMarker2) || automationMarker2.Root == null)
				{
					automationMarker2 = this.CreateMarker(keyValuePair.Value.Entity, keyValuePair.Value.IsStorage);
					if (automationMarker2 == null)
					{
						continue;
					}
					this._markers[keyValuePair.Key] = automationMarker2;
				}
				this.ApplyMarkerState(automationMarker2, keyValuePair.Value);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00006874 File Offset: 0x00004A74
		private static string GetOperationalStateName(IEntity entity)
		{
			if (entity == null)
			{
				return "";
			}
			string text;
			try
			{
				PropertyInfo property = entity.GetType().GetProperty("CurrentState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				object obj = ((property == null) ? null : property.GetValue(entity, null));
				text = ((obj == null) ? "" : obj.ToString());
			}
			catch
			{
				text = "";
			}
			return text;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000068E0 File Offset: 0x00004AE0
		private static bool IsEntityBroken(IEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			if (string.Equals(StorageAutoPauseMod.GetOperationalStateName(entity), "Broken", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			IMaintainedEntity maintainedEntity = entity as IMaintainedEntity;
			return maintainedEntity != null && maintainedEntity.Maintenance != null && maintainedEntity.Maintenance.Status.IsBroken;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000692C File Offset: 0x00004B2C
		private StorageAutoPauseMod.DiagnosticLightState GetPowerLightState(IEntity entity)
		{
			if (entity != null && entity.IsPaused)
			{
				return StorageAutoPauseMod.DiagnosticLightState.Ok;
			}
			IElectricityConsumingEntity electricityConsumingEntity = entity as IElectricityConsumingEntity;
			if (electricityConsumingEntity == null)
			{
				return StorageAutoPauseMod.DiagnosticLightState.Ok;
			}
			Option<IElectricityConsumerReadonly> electricityConsumer = electricityConsumingEntity.ElectricityConsumer;
			StorageAutoPauseMod.DiagnosticLightState result = StorageAutoPauseMod.DiagnosticLightState.Ok;
			if (string.Equals(StorageAutoPauseMod.GetOperationalStateName(entity), "NotEnoughPower", StringComparison.OrdinalIgnoreCase))
			{
				result = StorageAutoPauseMod.DiagnosticLightState.Warning;
			}
			else if (electricityConsumer.HasValue && electricityConsumer.Value.NotEnoughPower)
			{
				result = StorageAutoPauseMod.DiagnosticLightState.Warning;
			}
			if (entity != null && !entity.IsPaused)
			{
				this._lastPowerStates[entity.Id.Value] = result;
			}
			return result;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00006998 File Offset: 0x00004B98
		private StorageAutoPauseMod.DiagnosticLightState GetWorkerLightState(IEntity entity)
		{
			if (entity != null && entity.IsPaused)
			{
				return StorageAutoPauseMod.DiagnosticLightState.Ok;
			}
			IEntityWithWorkers entityWithWorkers = entity as IEntityWithWorkers;
			if (entityWithWorkers == null || entityWithWorkers.WorkersNeeded <= 0)
			{
				return StorageAutoPauseMod.DiagnosticLightState.Ok;
			}
			StorageAutoPauseMod.DiagnosticLightState result = StorageAutoPauseMod.DiagnosticLightState.Ok;
			string operationalStateName = StorageAutoPauseMod.GetOperationalStateName(entity);
			if (string.Equals(operationalStateName, "NotEnoughWorkers", StringComparison.OrdinalIgnoreCase) || string.Equals(operationalStateName, "MissingWorkers", StringComparison.OrdinalIgnoreCase))
			{
				result = StorageAutoPauseMod.DiagnosticLightState.Warning;
			}
			if (entity != null && !entity.IsPaused)
			{
				this._lastWorkerStates[entity.Id.Value] = result;
			}
			return result;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000069EC File Offset: 0x00004BEC
		private StorageAutoPauseMod.MarkerState BuildMarkerState(IEntity entity, bool isStorage, StorageAutoPauseMod.MarkerVisualState logistics, bool semaphoreVisible)
		{
			StorageAutoPauseMod.DiagnosticLightState diagnosticLightState = StorageAutoPauseMod.DiagnosticLightState.Off;
			StorageAutoPauseMod.DiagnosticLightState diagnosticLightState2 = StorageAutoPauseMod.DiagnosticLightState.Off;
			if (this._diagnosticSemaphoreEnabled && semaphoreVisible)
			{
				if (StorageAutoPauseMod.IsEntityBroken(entity))
				{
					diagnosticLightState = StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseA;
					diagnosticLightState2 = StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseB;
				}
				else
				{
					diagnosticLightState = this.GetPowerLightState(entity);
					diagnosticLightState2 = this.GetWorkerLightState(entity);
				}
			}
			return new StorageAutoPauseMod.MarkerState(entity, isStorage, logistics, diagnosticLightState, diagnosticLightState2);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00006A30 File Offset: 0x00004C30
		private static bool IsApproachingThreshold(StorageAutoPauseMod.StorageCondition condition)
		{
			if (condition == null || condition.Active)
			{
				return false;
			}
			int num = Math.Max(0, condition.PauseAtPercent - 5);
			return condition.LastFillPercent >= num && condition.LastFillPercent < condition.PauseAtPercent;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00006A74 File Offset: 0x00004C74
		private StorageAutoPauseMod.AutomationMarker CreateMarker(IEntity entity, bool isStorage)
		{
			StorageAutoPauseMod.AutomationMarker automationMarker;
			try
			{
				Vector3 vector;
				if (!this.TryFindCanonicalMarkerPosition(entity, out vector))
				{
					automationMarker = null;
				}
				else if (!this.EnsureSharedMarkerResources())
				{
					automationMarker = null;
				}
				else
				{
					GameObject gameObject = new GameObject("StorageAutoPause.Marker." + entity.Id.Value.ToString());
					// Never let the purely visual antenna participate in world picking.
					gameObject.layer = 2; // Unity built-in Ignore Raycast layer.
					gameObject.transform.position = vector;
					MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
					meshFilter.sharedMesh = this._sharedMarkerMesh;
					MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterials = new Material[] { this._sharedMastMaterial, this._sharedDiagOffMaterial, this._sharedDiagOffMaterial, this._sharedTipGreenMaterial };
					automationMarker = new StorageAutoPauseMod.AutomationMarker
					{
						EntityId = entity.Id.Value,
						IsStorage = isStorage,
						Root = gameObject,
						Filter = meshFilter,
						Renderer = meshRenderer
					};
				}
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: failed to create automation antenna: " + ex.Message);
				automationMarker = null;
			}
			return automationMarker;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00006B78 File Offset: 0x00004D78
		private bool EnsureSharedMarkerResources()
		{
			if (this._sharedMarkerMesh != null && this._sharedDiagnosticMarkerMesh != null && this._sharedMastMaterial != null && this._sharedTipGreenMaterial != null)
			{
				return true;
			}
			Material nativeMastMaterial = this.GetNativeMastMaterial();
			if (nativeMastMaterial == null)
			{
				Log.Warning("StorageAutoPause: native mast material could not be loaded.");
				return false;
			}
			this._sharedMarkerMesh = StorageAutoPauseMod.BuildSharedMarkerMesh(true);
			this._sharedDiagnosticMarkerMesh = StorageAutoPauseMod.BuildSharedMarkerMesh(false);
			if (this._sharedMarkerMesh == null || this._sharedDiagnosticMarkerMesh == null)
			{
				return false;
			}
			this._sharedMastMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.12f, 0.13f, 0.14f, 1f), false, 0f);
			this._sharedTipGreenMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.08f, 1f, 0.22f, 1f), true, 4f);
			this._sharedTipRedMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(1f, 0.08f, 0.04f, 1f), true, 4f);
			this._sharedTipBlueMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.08f, 0.38f, 1f, 1f), true, 4f);
			this._sharedTipCyanMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.02f, 0.82f, 1f, 1f), true, 3.6f);
			this._sharedTipOrangeMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.82f, 0.28f, 0.02f, 1f), true, 2.8f);
			this._sharedDiagGreenMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.08f, 1f, 0.22f, 1f), true, 3.2f);
			this._sharedDiagRedPhaseAMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(1f, 0.06f, 0.03f, 1f), true, 3.5f);
			this._sharedDiagRedPhaseBMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(1f, 0.06f, 0.03f, 1f), true, 3.5f);
			this._sharedDiagOffMaterial = StorageAutoPauseMod.CreateSharedMarkerMaterial(nativeMastMaterial, new Color(0.16f, 0.17f, 0.18f, 1f), false, 0f);
			Shader smokeShader = Shader.Find("Particles/Standard Unlit");
			if (smokeShader == null) smokeShader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
			this._sharedSmokeMaterial = smokeShader == null ? new Material(nativeMastMaterial) : new Material(smokeShader);
			if (this._sharedMastMaterial == null || this._sharedTipGreenMaterial == null || this._sharedTipRedMaterial == null || this._sharedTipBlueMaterial == null || this._sharedTipCyanMaterial == null || this._sharedTipOrangeMaterial == null || this._sharedDiagGreenMaterial == null || this._sharedDiagRedPhaseAMaterial == null || this._sharedDiagRedPhaseBMaterial == null || this._sharedDiagOffMaterial == null)
			{
				return false;
			}
			this._sharedMaterialSets.Clear();
			return true;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006DF8 File Offset: 0x00004FF8
		private static Material CreateSharedMarkerMaterial(Material template, Color color, bool emission, float emissionStrength)
		{
			Material material2;
			try
			{
				Material material = new Material(template);
				material.enableInstancing = true;
				StorageAutoPauseMod.SetMaterialColor(material, color, emission, emissionStrength);
				material2 = material;
			}
			catch
			{
				material2 = null;
			}
			return material2;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00006E34 File Offset: 0x00005034
		private static void SetMaterialColor(Material material, Color color, bool emission, float emissionStrength)
		{
			if (material == null)
			{
				return;
			}
			if (material.HasProperty("_Color"))
			{
				material.color = color;
			}
			if (material.HasProperty("_BaseColor"))
			{
				material.SetColor("_BaseColor", color);
			}
			if (emission && material.HasProperty("_EmissionColor"))
			{
				material.EnableKeyword("_EMISSION");
				material.SetColor("_EmissionColor", color * emissionStrength);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006EA8 File Offset: 0x000050A8
		private static Mesh BuildSharedMarkerMesh(bool includeLogistics)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<List<int>> subMeshes = new List<List<int>>();
			int subMeshCount = includeLogistics ? 27 : 17;
			for (int s = 0; s < subMeshCount; s++) subMeshes.Add(new List<int>());
			List<int> mastTriangles = subMeshes[0];
			Action<Vector3, Vector3, float> action = delegate(Vector3 p0, Vector3 p1, float thickness)
			{
				Vector3 vector2 = p1 - p0;
				float magnitude = vector2.magnitude;
				if (magnitude < 0.0001f)
				{
					return;
				}
				Vector3 vector3 = (p0 + p1) * 0.5f;
				Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, vector2 / magnitude);
				float num7 = magnitude * 0.5f;
				float num8 = thickness * 0.5f;
				Vector3[] array = new Vector3[]
				{
					new Vector3(-num8, -num7, -num8),
					new Vector3(num8, -num7, -num8),
					new Vector3(num8, num7, -num8),
					new Vector3(-num8, num7, -num8),
					new Vector3(num8, -num7, num8),
					new Vector3(-num8, -num7, num8),
					new Vector3(-num8, num7, num8),
					new Vector3(num8, num7, num8)
				};
				int count = vertices.Count;
				foreach (Vector3 vector4 in array)
				{
					vertices.Add(vector3 + quaternion * vector4);
				}
				foreach (int num9 in new int[]
				{
					0, 1, 2, 0, 2, 3, 4, 5, 6, 4,
					6, 7, 5, 0, 3, 5, 3, 6, 1, 4,
					7, 1, 7, 2, 3, 2, 7, 3, 7, 6,
					5, 4, 1, 5, 1, 0
				})
				{
					mastTriangles.Add(count + num9);
				}
			};
			Action<float, float, float, float, float, List<int>> addArc = delegate(float bottomY, float height, float radius, float startAngle, float endAngle, List<int> tris)
			{
				int count2 = vertices.Count;
				float a0 = startAngle * 0.0174532924f;
				float a1 = endAngle * 0.0174532924f;
				vertices.Add(new Vector3(0f, bottomY, 0f));
				vertices.Add(new Vector3(Mathf.Cos(a0) * radius, bottomY, Mathf.Sin(a0) * radius));
				vertices.Add(new Vector3(Mathf.Cos(a1) * radius, bottomY, Mathf.Sin(a1) * radius));
				vertices.Add(new Vector3(0f, bottomY + height, 0f));
				vertices.Add(new Vector3(Mathf.Cos(a0) * radius, bottomY + height, Mathf.Sin(a0) * radius));
				vertices.Add(new Vector3(Mathf.Cos(a1) * radius, bottomY + height, Mathf.Sin(a1) * radius));
				tris.AddRange(new int[] { count2, count2 + 2, count2 + 1, count2 + 3, count2 + 4, count2 + 5,
					count2 + 1, count2 + 2, count2 + 5, count2 + 1, count2 + 5, count2 + 4,
					count2, count2 + 1, count2 + 4, count2, count2 + 4, count2 + 3,
					count2, count2 + 3, count2 + 5, count2, count2 + 5, count2 + 2 });
			};
			float logisticsBottom = 6.35f;
			float powerBottom = logisticsBottom - 0.25f;
			float workerBottom = powerBottom - 0.25f;
			float mastTop = includeLogistics ? logisticsBottom + 0.65f : powerBottom + 0.2f;
			for (int i = 0; i < 3; i++)
			{
				float num3 = 0.0174532924f * (90f + (float)i * 120f);
				// Extend the mast 200% farther downward.  On very tall prefabs the
				// canonical marker is placed at the roof/light level; the original
				// 16 m tail therefore ended visibly in mid-air.  The extra part is
				// harmlessly hidden below terrain on ordinary buildings.
				Vector3 vector = new Vector3(Mathf.Cos(num3) * 0.046188f, -48f, Mathf.Sin(num3) * 0.046188f);
				action(vector, vector + Vector3.up * (48f + mastTop), 0.018f);
			}
			for (int segment = 0; segment < 8; segment++)
			{
				float start = 90f + segment * 45f + 1.8f;
				float end = 90f + (segment + 1) * 45f - 1.8f;
				addArc(workerBottom, 0.2f, 0.064f, start, end, subMeshes[1 + segment]);
				addArc(powerBottom, 0.2f, 0.064f, start, end, subMeshes[9 + segment]);
			}
			if (includeLogistics)
			{
				for (int level = 0; level < 10; level++)
				{
					float bottom = logisticsBottom + level * 0.065f;
					addArc(bottom, 0.055f, 0.071f, 0f, 120f, subMeshes[17 + level]);
					addArc(bottom, 0.055f, 0.071f, 120f, 240f, subMeshes[17 + level]);
					addArc(bottom, 0.055f, 0.071f, 240f, 360f, subMeshes[17 + level]);
				}
			}
			Mesh mesh = new Mesh();
			mesh.name = includeLogistics ? "StorageAutoPause.SegmentedAutomationAntenna" : "StorageAutoPause.SegmentedDiagnosticAntenna";
			mesh.vertices = vertices.ToArray();
			mesh.subMeshCount = subMeshCount;
			for (int sm = 0; sm < subMeshCount; sm++) mesh.SetTriangles(subMeshes[sm].ToArray(), sm);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000704C File Offset: 0x0000524C
		private Material GetNativeMastMaterial()
		{
			GameObject gameObject = null;
			Material material;
			try
			{
				gameObject = this._assetsDb.GetClonedPrefabOrEmptyGo("Assets/Base/Transports/StackerTower/StackerTower-pillar.prefab");
				if (gameObject == null)
				{
					material = null;
				}
				else
				{
					Renderer renderer = gameObject.GetComponentsInChildren<Renderer>(true).FirstOrDefault<Renderer>((Renderer r) => r != null && r.sharedMaterial != null);
					material = ((renderer == null) ? null : renderer.sharedMaterial);
				}
			}
			catch
			{
				material = null;
			}
			finally
			{
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			return material;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000070EC File Offset: 0x000052EC
		private void ApplyMarkerState(StorageAutoPauseMod.AutomationMarker marker, StorageAutoPauseMod.MarkerState state)
		{
			if (marker == null || marker.Renderer == null || marker.Filter == null)
			{
				return;
			}
			bool showLogistics = state.LogisticsState != StorageAutoPauseMod.MarkerVisualState.Off;
			int ringPhase = (int)(Time.realtimeSinceStartup * 6f) % 8;
			int logisticsPhase = (int)(Time.realtimeSinceStartup * 6f) % 10;
			int num = (int)state.LogisticsState | ((int)state.WorkerState << 3) | ((int)state.PowerState << 6) | (ringPhase << 9) | (logisticsPhase << 12);
			if (marker.MaterialStateApplied && marker.MaterialKey == num)
			{
				return;
			}
			marker.MaterialKey = num;
			marker.MaterialStateApplied = true;
			Material[] array;
			if (!this._sharedMaterialSets.TryGetValue(num, out array))
			{
				array = new Material[showLogistics ? 27 : 17];
				array[0] = this._sharedMastMaterial;
				this.FillDiagnosticSegments(array, 1, state.WorkerState, ringPhase, 1);
				this.FillDiagnosticSegments(array, 9, state.PowerState, ringPhase, -1);
				if (showLogistics) this.FillLogisticsSegments(array, 17, state.LogisticsState, logisticsPhase);
				this._sharedMaterialSets[num] = array;
			}
			marker.Filter.sharedMesh = showLogistics ? this._sharedMarkerMesh : this._sharedDiagnosticMarkerMesh;
			marker.Renderer.sharedMaterials = array;
		}

		private static void UpdateFillTrend(StorageAutoPauseMod.StorageCondition condition, int newFillPercent)
		{
			if (!condition.HasFillSample)
			{
				condition.HasFillSample = true;
				condition.FillTrend = 0;
				condition.TrendHoldUpdates = 0;
			}
			else if (newFillPercent > condition.LastFillPercent)
			{
				condition.FillTrend = 1;
				condition.TrendHoldUpdates = 5;
			}
			else if (newFillPercent < condition.LastFillPercent)
			{
				condition.FillTrend = -1;
				condition.TrendHoldUpdates = 5;
			}
			else if (condition.TrendHoldUpdates > 0)
			{
				condition.TrendHoldUpdates--;
			}
			else
			{
				condition.FillTrend = 0;
			}
			condition.LastFillPercent = newFillPercent;
		}

		private static StorageAutoPauseMod.MarkerVisualState GetFillTrendState(List<StorageAutoPauseMod.StorageCondition> conditions)
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				if (conditions[i].FillTrend > 0) return StorageAutoPauseMod.MarkerVisualState.FillingUp;
				if (conditions[i].FillTrend < 0) return StorageAutoPauseMod.MarkerVisualState.DrainingDown;
			}
			return StorageAutoPauseMod.MarkerVisualState.Normal;
		}

		private static void UpdateSorterFillTrend(StorageAutoPauseMod.SorterCondition condition, int newFillPercent)
		{
			if (!condition.HasFillSample)
			{
				condition.HasFillSample = true;
				condition.FillTrend = 0;
				condition.TrendHoldUpdates = 0;
			}
			else if (newFillPercent > condition.LastFillPercent)
			{
				condition.FillTrend = 1;
				condition.TrendHoldUpdates = 5;
			}
			else if (newFillPercent < condition.LastFillPercent)
			{
				condition.FillTrend = -1;
				condition.TrendHoldUpdates = 5;
			}
			else if (condition.TrendHoldUpdates > 0) condition.TrendHoldUpdates--;
			else condition.FillTrend = 0;
			condition.LastFillPercent = newFillPercent;
		}

		private void FillDiagnosticSegments(Material[] materials, int offset, StorageAutoPauseMod.DiagnosticLightState state, int phase, int direction)
		{
			for (int i = 0; i < 8; i++)
			{
				Material material = this.GetDiagnosticMaterial(state);
				if (state == StorageAutoPauseMod.DiagnosticLightState.Warning || state == StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseA || state == StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseB)
				{
					int head = direction > 0 ? phase : (8 - phase) % 8;
					int distance = (head - i + 8) % 8;
					if (distance > 2) material = this._sharedDiagOffMaterial;
					else if (distance > 0 && state != StorageAutoPauseMod.DiagnosticLightState.Warning) material = this._sharedDiagRedPhaseBMaterial;
				}
				materials[offset + i] = material;
			}
		}

		private void FillLogisticsSegments(Material[] materials, int offset, StorageAutoPauseMod.MarkerVisualState state, int phase)
		{
			for (int i = 0; i < 10; i++)
			{
				Material material = this.GetLogisticsMaterial(state);
				if (state == StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold)
				{
					int head = phase % 10;
					material = i <= head ? this._sharedTipOrangeMaterial : this._sharedDiagOffMaterial;
				}
				else if (state == StorageAutoPauseMod.MarkerVisualState.FillingUp)
				{
					int head = phase % 10;
					material = i <= head ? this._sharedTipGreenMaterial : this._sharedDiagOffMaterial;
				}
				else if (state == StorageAutoPauseMod.MarkerVisualState.DrainingDown)
				{
					int head = 9 - phase % 10;
					material = i >= head ? this._sharedTipCyanMaterial : this._sharedDiagOffMaterial;
				}
				materials[offset + i] = material;
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000071B0 File Offset: 0x000053B0
		private Material GetLogisticsMaterial(StorageAutoPauseMod.MarkerVisualState state)
		{
			switch (state)
			{
			case StorageAutoPauseMod.MarkerVisualState.Off:
				return this._sharedDiagOffMaterial;
			case StorageAutoPauseMod.MarkerVisualState.ApproachingThreshold:
				return this._sharedTipOrangeMaterial;
			case StorageAutoPauseMod.MarkerVisualState.AutomationPaused:
				return this._sharedTipRedMaterial;
			case StorageAutoPauseMod.MarkerVisualState.ManualPaused:
				return this._sharedTipBlueMaterial;
			}
			return this._sharedTipGreenMaterial;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000071F0 File Offset: 0x000053F0
		private Material GetDiagnosticMaterial(StorageAutoPauseMod.DiagnosticLightState state)
		{
			switch (state)
			{
			case StorageAutoPauseMod.DiagnosticLightState.Ok:
				return this._sharedDiagGreenMaterial;
			case StorageAutoPauseMod.DiagnosticLightState.Warning:
				return this._sharedTipOrangeMaterial;
			case StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseA:
				return this._sharedDiagRedPhaseAMaterial;
			case StorageAutoPauseMod.DiagnosticLightState.BrokenPhaseB:
				return this._sharedDiagRedPhaseBMaterial;
			default:
				return this._sharedDiagOffMaterial;
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00007230 File Offset: 0x00005430
		private void UpdateOrangePulseMaterial()
		{
			// Animation is positional only. Mutating shared material brightness while
			// advancing the segment introduced visible out-of-order flashes.
			foreach (KeyValuePair<int, StorageAutoPauseMod.MarkerState> pair in this._desiredMarkerStates)
			{
				StorageAutoPauseMod.AutomationMarker marker;
				if (this._markers.TryGetValue(pair.Key, out marker)) this.ApplyMarkerState(marker, pair.Value);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000738C File Offset: 0x0000558C
		private static Bounds GetCombinedBounds(GameObject go)
		{
			Renderer[] array = (from r in go.GetComponentsInChildren<Renderer>(true)
				where r != null
				select r).ToArray<Renderer>();
			if (array.Length == 0)
			{
				return new Bounds(go.transform.position, Vector3.zero);
			}
			Bounds bounds = array[0].bounds;
			for (int i = 1; i < array.Length; i++)
			{
				bounds.Encapsulate(array[i].bounds);
			}
			return bounds;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000740C File Offset: 0x0000560C
		private bool TryFindCanonicalMarkerPosition(IEntity entity, out Vector3 result)
		{
			result = Vector3.zero;
			LayoutEntityBase layoutEntityBase = entity as LayoutEntityBase;
			if (layoutEntityBase == null || layoutEntityBase.Prototype == null || layoutEntityBase.Prototype.Layout == null)
			{
				return false;
			}
			EntityLayout layout = layoutEntityBase.Prototype.Layout;
			if (layout.LayoutTiles.IsEmpty)
			{
				return false;
			}
			LayoutTile layoutTile = layout.LayoutTiles[0];
			long num = long.MaxValue;
			foreach (LayoutTile layoutTile2 in layout.LayoutTiles)
			{
				long num2 = (long)layoutTile2.Coord.X * (long)layoutTile2.Coord.X + (long)layoutTile2.Coord.Y * (long)layoutTile2.Coord.Y;
				if (num2 < num || (num2 == num && (layoutTile2.Coord.X < layoutTile.Coord.X || (layoutTile2.Coord.X == layoutTile.Coord.X && layoutTile2.Coord.Y < layoutTile.Coord.Y))))
				{
					num = num2;
					layoutTile = layoutTile2;
				}
			}
			Tile2i tile2i = layout.Transform(layoutTile.Coord, layoutEntityBase.Transform);
			Vector3 vector = new Tile3i(layoutEntityBase.Transform.Position.X, layoutEntityBase.Transform.Position.Y, layoutEntityBase.Transform.Position.Z).ToCornerVector3();
			float num3;
			if (!this.TryGetRenderedSurfaceLocalY(layoutEntityBase, layoutTile, out num3))
			{
				num3 = 0f;
				if (!this.TryGetRenderedModelTopLocalY(layoutEntityBase, out num3))
				{
					num3 = (float)(layoutTile.OccupiedThickness.From.Value * 2);
				}
			}
			Vector3 vector2 = new Tile3i(tile2i.X, tile2i.Y, layoutEntityBase.Transform.Position.Z).ToCornerVector3() + new Vector3(1f, 0f, 1f);
			result = new Vector3(vector2.x, vector.y + num3 + 0.01f, vector2.z);
			return true;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00007628 File Offset: 0x00005828
		private bool TryGetRenderedSurfaceLocalY(LayoutEntityBase entity, LayoutTile selected, out float surfaceY)
		{
			surfaceY = 0f;
			GameObject gameObject = null;
			bool flag;
			try
			{
				gameObject = this.CreatePrototypeModel(entity.Prototype);
				if (gameObject == null)
				{
					flag = false;
				}
				else
				{
					int num = int.MaxValue;
					int num2 = int.MinValue;
					int num3 = int.MaxValue;
					int num4 = int.MinValue;
					foreach (LayoutTile layoutTile in entity.Prototype.Layout.LayoutTiles)
					{
						num = Math.Min(num, layoutTile.Coord.X);
						num2 = Math.Max(num2, layoutTile.Coord.X);
						num3 = Math.Min(num3, layoutTile.Coord.Y);
						num4 = Math.Max(num4, layoutTile.Coord.Y);
					}
					Bounds combinedBounds = StorageAutoPauseMod.GetCombinedBounds(gameObject);
					Vector3 vector = gameObject.transform.InverseTransformPoint(combinedBounds.min);
					Vector3 vector2 = gameObject.transform.InverseTransformPoint(combinedBounds.max);
					float num5 = ((float)(selected.Coord.X - num) + 0.5f) / Math.Max(1f, (float)(num2 - num) + 1f);
					float num6 = ((float)(selected.Coord.Y - num3) + 0.5f) / Math.Max(1f, (float)(num4 - num3) + 1f);
					float num7 = Mathf.Lerp(vector.x, vector2.x, num5);
					float num8 = Mathf.Lerp(vector.z, vector2.z, num6);
					bool flag2 = false;
					float num9 = float.NegativeInfinity;
					foreach (MeshFilter meshFilter in gameObject.GetComponentsInChildren<MeshFilter>(true))
					{
						if (!(meshFilter == null) && !(meshFilter.sharedMesh == null))
						{
							Mesh sharedMesh = meshFilter.sharedMesh;
							Vector3[] vertices;
							int[] triangles;
							try
							{
								vertices = sharedMesh.vertices;
								triangles = sharedMesh.triangles;
							}
							catch
							{
								goto IL_028D;
							}
							if (vertices != null && triangles != null)
							{
								int num10 = 0;
								while (num10 + 2 < triangles.Length)
								{
									Vector3 vector3 = gameObject.transform.InverseTransformPoint(meshFilter.transform.TransformPoint(vertices[triangles[num10]]));
									Vector3 vector4 = gameObject.transform.InverseTransformPoint(meshFilter.transform.TransformPoint(vertices[triangles[num10 + 1]]));
									Vector3 vector5 = gameObject.transform.InverseTransformPoint(meshFilter.transform.TransformPoint(vertices[triangles[num10 + 2]]));
									float num11;
									if (StorageAutoPauseMod.TryVerticalTriangleIntersection(num7, num8, vector3, vector4, vector5, out num11) && (!flag2 || num11 > num9))
									{
										num9 = num11;
										flag2 = true;
									}
									num10 += 3;
								}
							}
						}
						IL_028D:;
					}
					if (flag2)
					{
						surfaceY = num9;
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Warning("StorageAutoPause: prototype surface sampling failed: " + ex.Message);
				flag = false;
			}
			finally
			{
				this.ReturnPrototypeModel(entity.Prototype, gameObject);
			}
			return flag;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00007960 File Offset: 0x00005B60
		private bool TryGetRenderedModelTopLocalY(LayoutEntityBase entity, out float topY)
		{
			topY = 0f;
			GameObject gameObject = null;
			bool flag;
			try
			{
				gameObject = this.CreatePrototypeModel(entity.Prototype);
				if (gameObject == null)
				{
					flag = false;
				}
				else
				{
					Renderer[] array = (from r in gameObject.GetComponentsInChildren<Renderer>(true)
						where r != null
						select r).ToArray<Renderer>();
					if (array.Length == 0)
					{
						flag = false;
					}
					else
					{
						bool flag2 = false;
						float num = float.NegativeInfinity;
						foreach (Renderer renderer in array)
						{
							foreach (Vector3 vector in new Vector3[]
							{
								new Vector3(renderer.bounds.min.x, renderer.bounds.min.y, renderer.bounds.min.z),
								new Vector3(renderer.bounds.max.x, renderer.bounds.max.y, renderer.bounds.max.z)
							})
							{
								float y = gameObject.transform.InverseTransformPoint(vector).y;
								if (!flag2 || y > num)
								{
									num = y;
									flag2 = true;
								}
							}
						}
						if (!flag2)
						{
							flag = false;
						}
						else
						{
							topY = num;
							flag = true;
						}
					}
				}
			}
			catch
			{
				flag = false;
			}
			finally
			{
				this.ReturnPrototypeModel(entity.Prototype, gameObject);
			}
			return flag;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00007B34 File Offset: 0x00005D34
		private GameObject CreatePrototypeModel(object proto)
		{
			if (this._protoModelFactory == null || proto == null)
			{
				return null;
			}
			GameObject gameObject;
			try
			{
				MethodInfo methodInfo = typeof(ProtoModelFactory).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault<MethodInfo>((MethodInfo m) => m.Name == "CreateModelFor" && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);
				if (methodInfo == null)
				{
					gameObject = null;
				}
				else
				{
					gameObject = methodInfo.MakeGenericMethod(new Type[] { proto.GetType() }).Invoke(this._protoModelFactory, new object[] { proto }) as GameObject;
				}
			}
			catch
			{
				gameObject = null;
			}
			return gameObject;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00007BDC File Offset: 0x00005DDC
		private void ReturnPrototypeModel(object proto, GameObject model)
		{
			if (model == null)
			{
				return;
			}
			try
			{
				if (this._protoModelFactory != null && proto != null)
				{
					MethodInfo methodInfo = typeof(ProtoModelFactory).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault<MethodInfo>((MethodInfo m) => m.Name == "ReturnModelOf" && m.IsGenericMethodDefinition && m.GetParameters().Length == 2);
					if (methodInfo != null)
					{
						object[] array = new object[] { proto, model };
						methodInfo.MakeGenericMethod(new Type[] { proto.GetType() }).Invoke(this._protoModelFactory, array);
						return;
					}
				}
			}
			catch
			{
			}
			try
			{
				UnityEngine.Object.Destroy(model);
			}
			catch
			{
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00007CA0 File Offset: 0x00005EA0
		private static bool TryVerticalTriangleIntersection(float x, float z, Vector3 a, Vector3 b, Vector3 c, out float y)
		{
			y = 0f;
			float num = (b.z - c.z) * (a.x - c.x) + (c.x - b.x) * (a.z - c.z);
			if (Math.Abs(num) < 1E-06f)
			{
				return false;
			}
			float num2 = ((b.z - c.z) * (x - c.x) + (c.x - b.x) * (z - c.z)) / num;
			float num3 = ((c.z - a.z) * (x - c.x) + (a.x - c.x) * (z - c.z)) / num;
			float num4 = 1f - num2 - num3;
			if (num2 < -0.0005f || num3 < -0.0005f || num4 < -0.0005f)
			{
				return false;
			}
			y = num2 * a.y + num3 * b.y + num4 * c.y;
			return true;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00007DAC File Offset: 0x00005FAC
		private static void DestroyMarker(StorageAutoPauseMod.AutomationMarker marker)
		{
			try
			{
				if (marker != null)
				{
					if (marker.Root != null)
					{
						UnityEngine.Object.Destroy(marker.Root);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00007DEC File Offset: 0x00005FEC
		public void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues)
		{
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00007DF0 File Offset: 0x00005FF0
		public void Dispose()
		{
			try
			{
				if (this._entities != null && this._pauseChanged != null)
				{
					this._entities.EntityPauseStateChanged.RemoveNonSaveable<StorageAutoPauseMod>(this, this._pauseChanged);
				}
			}
			catch
			{
			}
			try
			{
				if (this._entities != null && this._enabledChanged != null)
				{
					this._entities.EntityEnabledChanged.RemoveNonSaveable<StorageAutoPauseMod>(this, this._enabledChanged);
				}
			}
			catch
			{
			}
			try
			{
				if (this._gameLoop != null && this._inputUpdate != null)
				{
					this._gameLoop.InputUpdate.RemoveNonSaveable<StorageAutoPauseMod>(this, this._inputUpdate);
				}
			}
			catch
			{
			}
			try
			{
				if (this._simLoop != null && this._simUpdate != null)
				{
					this._simLoop.ReadGameStateFrequent.RemoveNonSaveable<StorageAutoPauseMod>(this, this._simUpdate);
				}
			}
			catch
			{
			}
			StorageAutoPauseMod.AutomationMarker[] array = this._markers.Values.ToArray<StorageAutoPauseMod.AutomationMarker>();
			for (int i = 0; i < array.Length; i++)
			{
				StorageAutoPauseMod.DestroyMarker(array[i]);
			}
			this._markers.Clear();
			this._diagnosticStateCache.Clear();
			this._diagnosticLoggedState.Clear();
			this._lastPowerStates.Clear();
			this._lastWorkerStates.Clear();
			this._otherPanelBindings.Clear();
			this._otherInspector = null;
			this._otherPanel = null;
			this._otherPanelBody = null;
			if (this._fireworkAudioHost != null) UnityEngine.Object.Destroy(this._fireworkAudioHost);
			this._fireworkAudioHost = null;
			this.DestroySharedMarkerResources();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00007EE0 File Offset: 0x000060E0
		private void DestroySharedMarkerResources()
		{
			if (this._sharedMarkerMesh != null)
			{
				UnityEngine.Object.Destroy(this._sharedMarkerMesh);
			}
			if (this._sharedDiagnosticMarkerMesh != null)
			{
				UnityEngine.Object.Destroy(this._sharedDiagnosticMarkerMesh);
			}
			if (this._sharedMastMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedMastMaterial);
			}
			if (this._sharedTipGreenMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedTipGreenMaterial);
			}
			if (this._sharedTipRedMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedTipRedMaterial);
			}
			if (this._sharedTipBlueMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedTipBlueMaterial);
			}
			if (this._sharedTipCyanMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedTipCyanMaterial);
			}
			if (this._sharedTipOrangeMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedTipOrangeMaterial);
			}
			if (this._sharedDiagGreenMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedDiagGreenMaterial);
			}
			if (this._sharedDiagRedPhaseAMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedDiagRedPhaseAMaterial);
			}
			if (this._sharedDiagRedPhaseBMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedDiagRedPhaseBMaterial);
			}
			if (this._sharedDiagOffMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedDiagOffMaterial);
			}
			if (this._sharedSmokeMaterial != null)
			{
				UnityEngine.Object.Destroy(this._sharedSmokeMaterial);
			}
			this._sharedMarkerMesh = null;
			this._sharedDiagnosticMarkerMesh = null;
			this._sharedMastMaterial = (this._sharedTipGreenMaterial = (this._sharedTipRedMaterial = (this._sharedTipBlueMaterial = (this._sharedTipCyanMaterial = (this._sharedTipOrangeMaterial = null)))));
			this._sharedDiagGreenMaterial = (this._sharedDiagRedPhaseAMaterial = (this._sharedDiagRedPhaseBMaterial = (this._sharedDiagOffMaterial = null)));
			this._sharedSmokeMaterial = null;
			this._sharedMaterialSets.Clear();
		}

		// Token: 0x04000004 RID: 4
		private EntitiesManager _entities;

		// Token: 0x04000005 RID: 5
		private IGameLoopEvents _gameLoop;

		// Token: 0x04000006 RID: 6
		private ISimLoopEvents _simLoop;

		// Token: 0x04000007 RID: 7
		private ISaveManager _saveManager;

		// Token: 0x04000008 RID: 8
		private InspectorsManager _inspectors;

		// Token: 0x04000009 RID: 9
		private object _machineInspector;

		// Token: 0x0400000A RID: 10
		private object _panel;

		// Token: 0x0400000B RID: 11
		private object _panelBody;

		// Token: 0x0400000C RID: 12
		private object _otherInspector;

		// Token: 0x0400000D RID: 13
		private object _otherPanel;

		// Token: 0x0400000E RID: 14
		private object _otherPanelBody;

		// Token: 0x0400000F RID: 15
		private readonly Dictionary<object, StorageAutoPauseMod.InspectorPanelBinding> _otherPanelBindings = new Dictionary<object, StorageAutoPauseMod.InspectorPanelBinding>(StorageAutoPauseMod.ReferenceObjectComparer.Instance);

		// Token: 0x04000010 RID: 16
		private Type _labelType;

		// Token: 0x04000011 RID: 17
		private Type _buttonTextType;

		// Token: 0x04000012 RID: 18
		private Type _uiComponentType;

		// Token: 0x04000013 RID: 19
		private readonly Dictionary<int, StorageAutoPauseMod.MachineRule> _rules = new Dictionary<int, StorageAutoPauseMod.MachineRule>();

		// Token: 0x04000014 RID: 20
		private readonly Dictionary<int, StorageAutoPauseMod.MineTowerRule> _towerRules = new Dictionary<int, StorageAutoPauseMod.MineTowerRule>();

		// Token: 0x04000015 RID: 21
		private readonly HashSet<int> _manualSemaphoreEntities = new HashSet<int>();

		// Token: 0x04000016 RID: 22
		private readonly Dictionary<int, StorageAutoPauseMod.AutomationMarker> _markers = new Dictionary<int, StorageAutoPauseMod.AutomationMarker>();

		// Token: 0x04000017 RID: 23
		private readonly Dictionary<int, StorageAutoPauseMod.MarkerState> _desiredMarkerStates = new Dictionary<int, StorageAutoPauseMod.MarkerState>();

		// Token: 0x04000018 RID: 24
		private readonly Dictionary<int, int> _diagnosticStateCache = new Dictionary<int, int>();

		// Token: 0x04000019 RID: 25
		private readonly Dictionary<int, int> _diagnosticLoggedState = new Dictionary<int, int>();

		private readonly Dictionary<int, StorageAutoPauseMod.DiagnosticLightState> _lastPowerStates = new Dictionary<int, StorageAutoPauseMod.DiagnosticLightState>();

		private readonly Dictionary<int, StorageAutoPauseMod.DiagnosticLightState> _lastWorkerStates = new Dictionary<int, StorageAutoPauseMod.DiagnosticLightState>();

		// Token: 0x0400001A RID: 26
		private readonly List<int> _markerRemovalBuffer = new List<int>();

		// Token: 0x0400001B RID: 27
		private readonly List<int> _ruleIterationBuffer = new List<int>();

		// Token: 0x0400001C RID: 28
		private AssetsDb _assetsDb;

		// Token: 0x0400001D RID: 29
		private Mesh _sharedMarkerMesh;

		private Mesh _sharedDiagnosticMarkerMesh;

		// Token: 0x0400001E RID: 30
		private Material _sharedMastMaterial;

		// Token: 0x0400001F RID: 31
		private Material _sharedTipGreenMaterial;

		// Token: 0x04000020 RID: 32
		private Material _sharedTipRedMaterial;

		// Token: 0x04000021 RID: 33
		private Material _sharedTipBlueMaterial;

		private Material _sharedTipCyanMaterial;

		// Token: 0x04000022 RID: 34
		private Material _sharedTipOrangeMaterial;

		// Token: 0x04000023 RID: 35
		private Material _sharedDiagGreenMaterial;

		// Token: 0x04000024 RID: 36
		private Material _sharedDiagRedPhaseAMaterial;

		// Token: 0x04000025 RID: 37
		private Material _sharedDiagRedPhaseBMaterial;

		// Token: 0x04000026 RID: 38
		private Material _sharedDiagOffMaterial;

		private Material _sharedSmokeMaterial;

		// Token: 0x04000027 RID: 39
		private readonly Dictionary<int, Material[]> _sharedMaterialSets = new Dictionary<int, Material[]>();

		// Token: 0x04000028 RID: 40
		private ProtoModelFactory _protoModelFactory;

		// Token: 0x04000029 RID: 41
		private const string NativeMastPrefabPath = "Assets/Base/Transports/StackerTower/StackerTower-pillar.prefab";

		// Token: 0x0400002A RID: 42
		private bool _markersDirty = true;

		private bool _persistenceDirty;

		private readonly object _stateLock = new object();

		// Token: 0x0400002B RID: 43
		private bool _orangePulseActive;

		// Token: 0x0400002C RID: 44
		private bool _brokenPulseActive;

		// Token: 0x0400002D RID: 45
		private int _orangePulseDivider;

		// Token: 0x0400002E RID: 46
		private bool _diagnosticSemaphoreEnabled = true;

		// Token: 0x0400002F RID: 47
		private int _waitingMachineId;

		// Token: 0x04000030 RID: 48
		private int _waitingMineTowerId;

		// Token: 0x04000031 RID: 49
		private int _expandedMachineId;

		// Token: 0x04000032 RID: 50
		private int _editMachineId;

		// Token: 0x04000033 RID: 51
		private int _lastActiveEntityId;

		// Token: 0x04000034 RID: 52
		private int _tickDivider;

		// Token: 0x04000035 RID: 53
		private int _pendingMachineRefreshId;

		// Token: 0x04000036 RID: 54
		private int _pendingOtherRefreshId;

		// Token: 0x04000037 RID: 55
		private string _selectionNotice;

		// Token: 0x04000038 RID: 56
		private string _otherSelectionNotice;

		// Token: 0x04000039 RID: 57
		private volatile bool _uiRefreshRequested;

		// Token: 0x0400003A RID: 58
		private int _uiRefreshDelayInputFrames;

		// Token: 0x0400003B RID: 59
		private string _dataPath;

		// Token: 0x0400003C RID: 60
		private Action<GameTime> _inputUpdate;

		// Token: 0x0400003D RID: 61
		private Action _simUpdate;

		// Token: 0x0400003E RID: 62
		private Action<IEntity, bool> _pauseChanged;

		private Action<IEntity, bool> _enabledChanged;

		// Token: 0x0400003F RID: 63
		private readonly HashSet<int> _modPauseMutations = new HashSet<int>();

		private readonly HashSet<int> _knownBrokenEntities = new HashSet<int>();

		private readonly HashSet<int> _pausedMaintainedEntities = new HashSet<int>();

		private readonly Queue<int> _pendingFireworks = new Queue<int>();

		private DateTime _fireworkSuppressUntilUtc;

		private bool _breakdownFireworksEnabled = true;

		private bool _breakdownFireworksSoundEnabled = true;

		private GameObject _fireworkAudioHost;

		public sealed class FireworkAudioLoader : MonoBehaviour
		{
			public static AudioClip SharedClip;

			public void Begin(string path)
			{
				base.StartCoroutine(this.Load(path));
			}

			private IEnumerator Load(string path)
			{
				string uri = new Uri(path).AbsoluteUri;
				UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS);
				yield return request.SendWebRequest();
				if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
				{
					Log.Warning("StorageAutoPause: firework audio load failed: " + request.error);
				}
				else
				{
					StorageAutoPauseMod.FireworkAudioLoader.SharedClip = DownloadHandlerAudioClip.GetContent(request);
				}
				request.Dispose();
			}
		}

		public sealed class FireworkController : MonoBehaviour
		{
			private Vector3 _start;
			private Vector3 _horizontal;
			private float _started;
			private float _flightDuration;
			private float _height;
			private Material _red;
			private Material _orange;
			private Material _green;
			private Material _smoke;
			private bool _exploded;
			private bool _soundEnabled;

			public static void Spawn(Vector3 position, int seed, Material red, Material orange, Material green, Material smoke, bool soundEnabled)
			{
				GameObject go = new GameObject("StorageAutoPause.BreakdownFirework." + seed.ToString());
				go.layer = 2;
				go.transform.position = position;
				StorageAutoPauseMod.FireworkController controller = go.AddComponent<StorageAutoPauseMod.FireworkController>();
				controller._start = position;
				System.Random random = new System.Random(seed ^ DateTime.UtcNow.Millisecond);
				float angle = (float)(random.NextDouble() * Math.PI * 2.0);
				float distance = 6f + (float)random.NextDouble() * 6f;
				controller._horizontal = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
				controller._flightDuration = 2.2f + (float)random.NextDouble() * 0.6f;
				controller._height = 50f + (float)random.NextDouble() * 20f;
				controller._red = red;
				controller._orange = orange;
				controller._green = green;
				controller._smoke = smoke;
				controller._soundEnabled = soundEnabled;
				controller._started = Time.realtimeSinceStartup;
				controller.CreateLaunchTrail();
				controller.CreateSmokeTrail();
				controller.PlaySpatialSound();
			}

			public static void SpawnVehicleSmoke(Vector3 position, Material smoke)
			{
				GameObject smokeGo = new GameObject("StorageAutoPause.VehicleBreakdownSmoke");
				smokeGo.layer = 2;
				smokeGo.transform.position = position;
				ParticleSystem ps = smokeGo.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = ps.main;
				main.loop = false;
				main.duration = 0.35f;
				main.startLifetime = new ParticleSystem.MinMaxCurve(1.8f, 2.8f);
				main.startSpeed = new ParticleSystem.MinMaxCurve(0.25f, 0.75f);
				main.startSize = new ParticleSystem.MinMaxCurve(0.18f, 0.42f);
				main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.015f, 0.015f, 0.015f, 0.7f), new Color(0.09f, 0.09f, 0.09f, 0.45f));
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				ParticleSystem.EmissionModule emission = ps.emission;
				emission.rateOverTime = 0f;
				emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)7) });
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.shapeType = ParticleSystemShapeType.Cone;
				shape.angle = 12f;
				shape.radius = 0.12f;
				ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
				renderer.material = smoke;
				ps.Play();
				UnityEngine.Object.Destroy(smokeGo, 3.2f);
			}

			private void CreateLaunchTrail()
			{
				ParticleSystem ps = base.gameObject.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = ps.main;
				main.loop = true;
				main.startLifetime = 0.65f;
				main.startSpeed = 0.15f;
				main.startSize = 0.13f;
				main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.26f, 0.02f, 1f), new Color(1f, 0.9f, 0.25f, 1f));
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				ParticleSystem.EmissionModule emission = ps.emission;
				emission.rateOverTime = 35f;
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.enabled = false;
				ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
				renderer.renderMode = ParticleSystemRenderMode.Billboard;
				renderer.material = this._orange;
				ps.Play();
			}

			private void CreateSmokeTrail()
			{
				GameObject smokeGo = new GameObject("ThinSmokeTrail");
				smokeGo.layer = 2;
				smokeGo.transform.SetParent(base.transform, false);
				ParticleSystem ps = smokeGo.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = ps.main;
				main.loop = true;
				main.startLifetime = new ParticleSystem.MinMaxCurve(1.3f, 2f);
				main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.12f);
				main.startSize = new ParticleSystem.MinMaxCurve(0.07f, 0.13f);
				main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.22f, 0.22f, 0.22f, 0.36f), new Color(0.5f, 0.5f, 0.5f, 0.18f));
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				ParticleSystem.EmissionModule emission = ps.emission;
				emission.rateOverTime = 17f;
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.enabled = false;
				ParticleSystem.SizeOverLifetimeModule sizeLife = ps.sizeOverLifetime;
				sizeLife.enabled = true;
				sizeLife.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.55f, 1f, 1.5f));
				ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
				renderer.material = this._smoke;
				ps.Play();
			}

			private void PlaySpatialSound()
			{
				if (!this._soundEnabled || StorageAutoPauseMod.FireworkAudioLoader.SharedClip == null) return;
				AudioSource source = base.gameObject.AddComponent<AudioSource>();
				source.clip = StorageAutoPauseMod.FireworkAudioLoader.SharedClip;
				source.spatialBlend = 1f;
				source.rolloffMode = AudioRolloffMode.Logarithmic;
				source.minDistance = 9f;
				source.maxDistance = 110f;
				source.volume = 1f;
				source.Play();
			}

			private void Update()
			{
				float elapsed = Time.realtimeSinceStartup - this._started;
				if (!this._exploded)
				{
					float n = Mathf.Clamp01(elapsed / this._flightDuration);
					float y = this._height * (2f * n - n * n);
					base.transform.position = this._start + this._horizontal * n + Vector3.up * y;
					if (n >= 1f) this.Explode();
				}
				else if (elapsed > this._flightDuration + 5.5f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}

			private void Explode()
			{
				this._exploded = true;
				ParticleSystem launch = base.GetComponent<ParticleSystem>();
				if (launch != null)
				{
					ParticleSystem.EmissionModule launchEmission = launch.emission;
					launchEmission.enabled = false;
				}
				foreach (ParticleSystem trail in base.GetComponentsInChildren<ParticleSystem>())
				{
					ParticleSystem.EmissionModule trailEmission = trail.emission;
					trailEmission.enabled = false;
				}
				GameObject burstGo = new GameObject("FlareBurst");
				burstGo.layer = 2;
				burstGo.transform.SetParent(base.transform, false);
				ParticleSystem burst = burstGo.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = burst.main;
				main.loop = false;
				main.duration = 0.12f;
				main.startLifetime = new ParticleSystem.MinMaxCurve(3f, 5f);
				main.startSpeed = new ParticleSystem.MinMaxCurve(3.2f, 7.5f);
				main.startSize = new ParticleSystem.MinMaxCurve(0.09f, 0.19f);
				main.gravityModifier = 0.22f;
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.08f, 0.02f, 1f), new Color(1f, 0.88f, 0.18f, 1f));
				ParticleSystem.EmissionModule emission = burst.emission;
				emission.rateOverTime = 0f;
				emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)120) });
				ParticleSystem.ShapeModule shape = burst.shape;
				shape.shapeType = ParticleSystemShapeType.Sphere;
				shape.radius = 0.25f;
				ParticleSystem.NoiseModule noise = burst.noise;
				noise.enabled = true;
				noise.strength = 0.45f;
				noise.frequency = 0.35f;
				ParticleSystem.ColorOverLifetimeModule colorLife = burst.colorOverLifetime;
				colorLife.enabled = true;
				Gradient gradient = new Gradient();
					gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.95f, 0.35f), 0f), new GradientColorKey(new Color(1f, 0.08f, 0.01f), 0.45f), new GradientColorKey(new Color(0.22f, 0.02f, 0.01f), 1f) };
					gradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.8f, 0.65f), new GradientAlphaKey(0f, 1f) };
				colorLife.color = gradient;
				ParticleSystemRenderer renderer = burst.GetComponent<ParticleSystemRenderer>();
				renderer.renderMode = ParticleSystemRenderMode.Billboard;
				renderer.material = this._red;
				burst.Play();
				this.CreateExplosionSmoke();
				this.CreateExplosionFlash();
			}

			private void CreateExplosionSmoke()
			{
				GameObject smokeGo = new GameObject("FlareBurstSmoke");
				smokeGo.layer = 2;
				smokeGo.transform.SetParent(base.transform, false);
				ParticleSystem ps = smokeGo.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = ps.main;
				main.loop = false;
				main.duration = 0.2f;
				main.startLifetime = new ParticleSystem.MinMaxCurve(2.4f, 4.2f);
				main.startSpeed = new ParticleSystem.MinMaxCurve(0.45f, 1.45f);
				main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.75f);
				main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.12f, 0.12f, 0.12f, 0.48f), new Color(0.45f, 0.42f, 0.38f, 0.25f));
				main.gravityModifier = -0.025f;
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				ParticleSystem.EmissionModule emission = ps.emission;
				emission.rateOverTime = 0f;
				emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)18) });
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.shapeType = ParticleSystemShapeType.Sphere;
				shape.radius = 0.35f;
				ParticleSystem.SizeOverLifetimeModule sizeLife = ps.sizeOverLifetime;
				sizeLife.enabled = true;
				sizeLife.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.65f, 1f, 1.8f));
				ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
				renderer.material = this._smoke;
				ps.Play();
			}

			private void CreateExplosionFlash()
			{
				GameObject flashGo = new GameObject("FlareFlash");
				flashGo.layer = 2;
				flashGo.transform.SetParent(base.transform, false);
				ParticleSystem ps = flashGo.AddComponent<ParticleSystem>();
				ParticleSystem.MainModule main = ps.main;
				main.loop = false;
				main.duration = 0.06f;
				main.startLifetime = 0.18f;
				main.startSpeed = 0f;
				main.startSize = new ParticleSystem.MinMaxCurve(1.2f, 2.4f);
				main.startColor = new Color(1f, 0.95f, 0.55f, 1f);
				ParticleSystem.EmissionModule emission = ps.emission;
				emission.rateOverTime = 0f;
				emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)2) });
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.enabled = false;
				ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
				renderer.material = this._orange;
				ps.Play();
			}
		}

		// Token: 0x02000004 RID: 4
		private sealed class AutomationMarker
		{
			// Token: 0x04000041 RID: 65
			public int EntityId;

			// Token: 0x04000042 RID: 66
			public bool IsStorage;

			// Token: 0x04000043 RID: 67
			public GameObject Root;

			public MeshFilter Filter;

			// Token: 0x04000044 RID: 68
			public MeshRenderer Renderer;

			// Token: 0x04000045 RID: 69
			public int MaterialKey;

			// Token: 0x04000046 RID: 70
			public bool MaterialStateApplied;
		}

		// Token: 0x02000005 RID: 5
		private struct MarkerState
		{
			// Token: 0x0600006C RID: 108 RVA: 0x0000804E File Offset: 0x0000624E
			public MarkerState(IEntity entity, bool isStorage, StorageAutoPauseMod.MarkerVisualState logisticsState, StorageAutoPauseMod.DiagnosticLightState powerState, StorageAutoPauseMod.DiagnosticLightState workerState)
			{
				this.Entity = entity;
				this.IsStorage = isStorage;
				this.LogisticsState = logisticsState;
				this.PowerState = powerState;
				this.WorkerState = workerState;
			}

			// Token: 0x04000047 RID: 71
			public readonly IEntity Entity;

			// Token: 0x04000048 RID: 72
			public readonly bool IsStorage;

			// Token: 0x04000049 RID: 73
			public readonly StorageAutoPauseMod.MarkerVisualState LogisticsState;

			// Token: 0x0400004A RID: 74
			public readonly StorageAutoPauseMod.DiagnosticLightState PowerState;

			// Token: 0x0400004B RID: 75
			public readonly StorageAutoPauseMod.DiagnosticLightState WorkerState;
		}

		// Token: 0x02000006 RID: 6
		private enum MarkerVisualState
		{
			// Token: 0x0400004D RID: 77
			Off,
			// Token: 0x0400004E RID: 78
			Normal,
			// Token: 0x0400004F RID: 79
			ApproachingThreshold,
			// Token: 0x04000050 RID: 80
			AutomationPaused,
			// Token: 0x04000051 RID: 81
			ManualPaused
			,
			FillingUp,
			DrainingDown
		}

		// Token: 0x02000007 RID: 7
		private enum DiagnosticLightState
		{
			// Token: 0x04000053 RID: 83
			Off,
			// Token: 0x04000054 RID: 84
			Ok,
			// Token: 0x04000055 RID: 85
			Warning,
			// Token: 0x04000056 RID: 86
			BrokenPhaseA,
			// Token: 0x04000057 RID: 87
			BrokenPhaseB
		}

		// Token: 0x02000008 RID: 8
		private sealed class MachineRule
		{
			// Token: 0x04000058 RID: 88
			public int MachineId;

			// Token: 0x04000059 RID: 89
			public readonly List<StorageAutoPauseMod.StorageCondition> Conditions = new List<StorageAutoPauseMod.StorageCondition>();

			// Token: 0x0400005A RID: 90
			public bool ElseResume = true;

			// Token: 0x0400005B RID: 91
			public bool PausedByMod;

			public bool PlayerOverrideUntilClear;
		}

		// Token: 0x02000009 RID: 9
		private sealed class StorageCondition
		{
			// Token: 0x0400005C RID: 92
			public int StorageId;

			// Token: 0x0400005D RID: 93
			public StorageAutoPauseMod.LogicJoin Join;

			// Token: 0x0400005E RID: 94
			public int PauseAtPercent = 100;

			// Token: 0x0400005F RID: 95
			public int ResumeAtPercent = 90;

			// Token: 0x04000060 RID: 96
			public int LastFillPercent;

			public bool HasFillSample;

			public int FillTrend;

			public int TrendHoldUpdates;

			// Token: 0x04000061 RID: 97
			public bool Active;
		}

		// Token: 0x0200000A RID: 10
		private sealed class InspectorPanelBinding
		{
			// Token: 0x0600006F RID: 111 RVA: 0x000080A7 File Offset: 0x000062A7
			public InspectorPanelBinding(object panel, object body)
			{
				this.Panel = panel;
				this.Body = body;
			}

			// Token: 0x04000062 RID: 98
			public readonly object Panel;

			// Token: 0x04000063 RID: 99
			public readonly object Body;
		}

		// Token: 0x0200000B RID: 11
		private sealed class ReferenceObjectComparer : IEqualityComparer<object>
		{
			// Token: 0x06000070 RID: 112 RVA: 0x000080BD File Offset: 0x000062BD
			private ReferenceObjectComparer()
			{
			}

			// Token: 0x06000071 RID: 113 RVA: 0x000080C5 File Offset: 0x000062C5
			public new bool Equals(object x, object y)
			{
				return x == y;
			}

			// Token: 0x06000072 RID: 114 RVA: 0x000080CB File Offset: 0x000062CB
			public int GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}

			// Token: 0x04000064 RID: 100
			public static readonly StorageAutoPauseMod.ReferenceObjectComparer Instance = new StorageAutoPauseMod.ReferenceObjectComparer();
		}

		// Token: 0x0200000C RID: 12
		private sealed class MineTowerRule
		{
			// Token: 0x04000065 RID: 101
			public int TowerId;

			// Token: 0x04000066 RID: 102
			public readonly List<StorageAutoPauseMod.SorterCondition> Sorters = new List<StorageAutoPauseMod.SorterCondition>();
		}

		// Token: 0x0200000D RID: 13
		private sealed class SorterCondition
		{
			// Token: 0x04000067 RID: 103
			public int SorterId;

			// Token: 0x04000068 RID: 104
			public int PauseAtPercent = 100;

			// Token: 0x04000069 RID: 105
			public int ResumeAtPercent = 90;

			// Token: 0x0400006A RID: 106
			public int LastFillPercent;

			public bool HasFillSample;

			public int FillTrend;

			public int TrendHoldUpdates;

			// Token: 0x0400006B RID: 107
			public bool Active;

			// Token: 0x0400006C RID: 108
			public bool PausedByMod;

			public bool PlayerOverrideUntilClear;
		}

		// Token: 0x0200000E RID: 14
		private enum LogicJoin
		{
			// Token: 0x0400006E RID: 110
			IF,
			// Token: 0x0400006F RID: 111
			AND,
			// Token: 0x04000070 RID: 112
			OR
		}
	}
}
