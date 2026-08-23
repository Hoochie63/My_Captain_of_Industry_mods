using System;
using System.Globalization;
using System.IO;
using Mafi;
using Mafi.Base;
using Mafi.Base.Prototypes.Buildings;
using Mafi.Collections.ImmutableCollections;
using Mafi.Collections;
using Mafi.Core;
using Mafi.Core.Buildings.Beacons;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Priorities;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Entities.Static;
using Mafi.Core.Game;
using Mafi.Core.Factory.ElectricPower;
using Mafi.Core.Population;
using Mafi.Serialization;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;

namespace BeaconLights {
public sealed class BeaconLightsMod : IMod, IDisposable {
 public ModManifest Manifest { get; private set; }
 public bool IsUiOnly => false;
 [Obsolete] public Option<IConfig> ModConfig { get; set; }
 public ModJsonConfig JsonConfig { get; private set; }
 public BeaconLightsMod(ModManifest manifest) { Manifest=manifest; JsonConfig=new ModJsonConfig(this); }
 public void RegisterPrototypes(ProtoRegistrator r) {
   var source=r.PrototypesDb.GetOrThrow<BeaconProto>(Ids.Buildings.Beacon);
   var cats=r.GetCategoriesProtos(new[]{Ids.ToolbarCategories.Decorations_Landmarks});
   // At mod prototype-registration time the vanilla Gfx has not been initialized yet,
   // therefore source.IconPath is still null. Point directly to the already generated
   // core Beacon icon instead of asking the game to generate a non-existent mod icon.
   var gfx=new LayoutEntityProto.Gfx("Assets/Base/Buildings/Beacon.prefab", default(RelTile3f), "Assets/Unity/Generated/Icons/LayoutEntity/Beacon.png".SomeOption(), default(ColorRgba), false, null, cats);
   var localized=GetLocalizedStrings();
   var strings=Proto.CreateStr(new Proto.ID("DecorativeBeaconLights"), localized[0], localized[1], null);
   var proto=new DecorativeBeaconProto(new StaticEntityProto.ID("DecorativeBeaconLights"), strings, source.Layout, source.Costs, gfx, 10.Kw());
   r.PrototypesDb.Add<DecorativeBeaconProto>(proto, false);
 }
 private string[] GetLocalizedStrings(){
  string fallback="Decorative beacon|An unlimited decorative lighthouse model requiring one worker and 10 kW of electricity. It has no refugee beacon function.";
  try{
   string lang=CultureInfo.CurrentUICulture.Name; string shortLang=CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
   if(lang.Equals("zh-CN",StringComparison.OrdinalIgnoreCase)||lang.Equals("zh-SG",StringComparison.OrdinalIgnoreCase))lang="zh-Hans";
   if(lang.Equals("zh-TW",StringComparison.OrdinalIgnoreCase)||lang.Equals("zh-HK",StringComparison.OrdinalIgnoreCase)||lang.Equals("zh-MO",StringComparison.OrdinalIgnoreCase))lang="zh-Hant";
   string path=Path.Combine(Manifest.RootDirectoryPath,"Localization","index.txt");
   foreach(string raw in File.ReadAllLines(path)){if(String.IsNullOrWhiteSpace(raw)||raw.StartsWith("#"))continue; string[] p=raw.Split(new[]{'|'},3);if(p.Length==3&&(String.Equals(p[0],lang,StringComparison.OrdinalIgnoreCase)||String.Equals(p[0],shortLang,StringComparison.OrdinalIgnoreCase)))return new[]{p[1],p[2]};}
  }catch(Exception ex){Log.Warning("BeaconLights localization fallback: "+ex.Message);}
  string[] f=fallback.Split(new[]{'|'},2);return f;
 }
 public void RegisterDependencies(DependencyResolverBuilder b, ProtosDb p, bool loaded) {}
 public void MigrateJsonConfig(VersionSlim version, Dict<string,object> config) {}
 public void EarlyInit(DependencyResolver r) {}
 public void Initialize(DependencyResolver r, bool loaded) {}
 public void Dispose() {}
}

public sealed class DecorativeBeaconProto : LayoutEntityProto, IProtoWithPowerConsumption {
 public override Type EntityType => typeof(DecorativeBeacon);
 public Electricity ElectricityConsumed { get; private set; }
 public DecorativeBeaconProto(StaticEntityProto.ID id, Proto.Str strings, EntityLayout layout, EntityCosts costs, Gfx gfx, Electricity power)
  : base(id,strings,layout,costs,gfx,isUnique:false) { ElectricityConsumed=power; }
}

[GenerateSerializer(false)]
public sealed class DecorativeBeacon : DecorationEntity, IEntityWithWorkers, IEntityWithGeneralPriority, IElectricityConsumingEntity, IEntityWithSimUpdate {
 public new readonly DecorativeBeaconProto Prototype;
 private readonly IElectricityConsumer consumer;
 [DoNotSave] bool IEntityWithWorkers.HasWorkersCached { get; set; }
 int IEntityWithWorkers.WorkersNeeded => Prototype.Costs.Workers;
 Electricity IElectricityConsumingEntity.PowerRequired => Prototype.ElectricityConsumed;
 public Option<IElectricityConsumerReadonly> ElectricityConsumer => consumer.SomeOption<IElectricityConsumerReadonly>();
 public bool NotEnoughPower => consumer.NotEnoughPower;
 [DoNotSave] public bool IsOperational { get; private set; }
 public override bool CanBePaused => true;
 public DecorativeBeacon(Mafi.Core.EntityId id, DecorativeBeaconProto proto, TileTransform transform, EntityContext context) : base(id,proto,transform,context){Prototype=proto;consumer=Context.ElectricityConsumerFactory.CreateConsumer(this);}
 public void SimUpdate(){IsOperational=IsEnabled&&((IEntityWithWorkers)this).HasWorkersCached&&consumer.TryConsume(false);}
 public static void Serialize(DecorativeBeacon v, BlobWriter w){if(w.TryStartClassSerialization<DecorativeBeacon>(v))w.EnqueueDataSerialization(v,SerializeDelayed);}
 protected override void SerializeData(BlobWriter w){base.SerializeData(w);w.WriteGeneric<IElectricityConsumer>(consumer);w.WriteGeneric<DecorativeBeaconProto>(Prototype);}
 public new static DecorativeBeacon Deserialize(BlobReader r){DecorativeBeacon v;if(r.TryStartClassDeserialization<DecorativeBeacon>(out v,null,null,false))r.EnqueueDataDeserialization(v,DeserializeDelayed,null);return v;}
 protected override void DeserializeData(BlobReader r){base.DeserializeData(r);r.SetField<DecorativeBeacon>(this,"consumer",r.ReadGenericAs<IElectricityConsumer>());r.SetField<DecorativeBeacon>(this,"Prototype",r.ReadGenericAs<DecorativeBeaconProto>());}
 private static readonly Action<object,BlobWriter> SerializeDelayed=(o,w)=>((DecorativeBeacon)o).SerializeData(w);
 private static readonly Action<object,BlobReader> DeserializeDelayed=(o,r)=>((DecorativeBeacon)o).DeserializeData(r);
}
}
