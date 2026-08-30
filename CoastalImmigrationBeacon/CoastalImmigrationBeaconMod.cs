using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using Mafi;
using Mafi.Base;
using Mafi.Collections;
using Mafi.Core;
using Mafi.Core.Buildings.Beacons;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Game;
using Mafi.Core.GameLoop;
using Mafi.Core.Mods;
using Mafi.Core.Population.Refugees;
using Mafi.Core.Prototypes;
using Mafi.Core.Simulation;
using Mafi.Core.Terrain;
using Mafi.Localization;
using Mafi.Unity;
using Mafi.Unity.Buildings;
using Mafi.Unity.Entities;
using Mafi.Unity.InputControl.Factory;
using Mafi.Unity.Ui;
using Mafi.Unity.Ui.Library;
using Mafi.Unity.UiToolkit.Component;
using Mafi.Unity.UiToolkit.Library;
using UnityEngine;

namespace CoastalImmigrationBeacon {
public sealed class CoastalImmigrationBeaconMod : IMod, IDisposable {
 internal const float PierCenterX=2f,PierEndZ=34.6f,DockAreaNearZ=35.5f,DockAreaFarZ=58f,DockAreaCenterZ=46.75f,BoatHalfWidth=3.23f,DockGap=.25f;
 public ModManifest Manifest{get;private set;} public bool IsUiOnly=>false;
 [Obsolete] public Option<IConfig> ModConfig{get;set;} public ModJsonConfig JsonConfig{get;private set;}
 static CoastalImmigrationBeaconMod instance; Harmony harmony; DependencyResolver resolver; TerrainManager terrain; AssetsDb assetsDb; RefugeesManager refugees; ISimLoopEvents sim; IGameLoopEvents loop; InspectorsManager inspectors;
 static readonly Dictionary<Beacon,Transform> beaconVisuals=new Dictionary<Beacon,Transform>();
 Beacon beacon; RelTile2i seaDirection; bool coastal; long priorProgress; BoatVisual boat; object boundInspector,panel; Column panelBody; int uiHash;
 volatile bool boatExists,createBoatRequested,destroyBoatRequested,desiredBoatFrozen,departBoatRequested;
 volatile bool journeyCommitted;
 Beacon requestedBoatBeacon; RelTile2i requestedBoatDirection;
 readonly Dictionary<string,string> localization=new Dictionary<string,string>();
 string title="IMMIGRATION DOCK", valid="Coastal berth ready.", invalid="Immigration stopped: rebuild the Beacon on the coast with its docking area in the ocean.";
 public CoastalImmigrationBeaconMod(ModManifest manifest){Manifest=manifest;JsonConfig=new ModJsonConfig(this);}
 public void RegisterDependencies(DependencyResolverBuilder b,ProtosDb p,bool loaded){}
 public void RegisterPrototypes(ProtoRegistrator r){
  // Keep the vanilla 5x5 beacon footprint and add only the remote ocean berth.
  // The pier itself remains completely outside the placement layout.
  // Keeping the same prototype ID makes old saves load normally; inland legacy beacons
  // remain present but fail the coastal validation below.
  try{
   var proto=r.PrototypesDb.GetOrThrow<BeaconProto>(Ids.Buildings.Beacon);
   var parser=new EntityLayoutParser(r.PrototypesDb);
   Predicate<LayoutTile> filter=x=>x.Constraint.HasAnyConstraints(LayoutTileConstraint.Ocean);
   var tokens=new[]{
    new CustomLayoutToken("[0!",(p,h)=>new LayoutTokenSpec(0,3*h,LayoutTileConstraint.None,0,null,null,null,null,p.HardenedFloorSurfaceId,false,false,0)),
    new CustomLayoutToken("~0!",(p,h)=>new LayoutTokenSpec(-10,h,LayoutTileConstraint.Ocean,null,null,null,null,null,null,false,false,0))};
   var rows=new List<string>();
   // One layout tile equals two Unity metres. The green docking rectangle is
   // therefore represented by a separate 5x12 ocean-only area. It is shifted
   // one tile towards the pier axis. Every tile between it and the beacon is empty.
   // Layout Y is inverted by the vanilla parser, so the berth comes first.
   string berthRow="   ~0!~0!~0!~0!~0!";
   string emptyRow="                  ";
   string beaconRow="[7![7![7![7![7!   ";
   for(int i=0;i<12;i++)rows.Add(berthRow);
   for(int i=0;i<15;i++)rows.Add(emptyRow);
   for(int i=0;i<5;i++)rows.Add(beaconRow);
   var layout=parser.ParseLayoutOrThrow(new EntityLayoutParams(filter,tokens,false,null,null,null,null,null,null,default(Option<IEnumerable<KeyValuePair<char,int>>>),false,null,null),rows.ToArray());
   AccessTools.Field(typeof(LayoutEntityProto),"<Layout>k__BackingField").SetValue(proto,layout);
  }catch(Exception ex){Log.Error("CoastalImmigrationBeacon layout creation failed: "+ex);}
 }
 public void MigrateJsonConfig(VersionSlim v,Dict<string,object> c){} public void EarlyInit(DependencyResolver r){}
 public void Initialize(DependencyResolver r,bool loaded){instance=this;resolver=r;terrain=r.Resolve<TerrainManager>();assetsDb=r.Resolve<AssetsDb>();refugees=r.Resolve<RefugeesManager>();sim=r.Resolve<ISimLoopEvents>();loop=r.Resolve<IGameLoopEvents>();inspectors=r.Resolve<InspectorsManager>();loop.RegisterInitState(this,OnInit);}
 void OnInit(){LoadLocalization();string assets=Path.Combine(Manifest.RootDirectoryPath,"Assets");try{ObjLoader.Preload(Path.Combine(assets,"immigrant_boat.obj"),assets);}catch(Exception ex){Log.Error("Immigration boat preload failed: "+ex);}harmony=new Harmony("sirael.coastalimmigrationbeacon");harmony.Patch(AccessTools.Method(typeof(Beacon),"TryWork"),postfix:new HarmonyMethod(typeof(CoastalImmigrationBeaconMod),nameof(TryWorkPostfix)));harmony.Patch(AccessTools.Method(typeof(BeaconMbFactory),"Create"),postfix:new HarmonyMethod(typeof(CoastalImmigrationBeaconMod),nameof(BeaconVisualPostfix)));harmony.Patch(AccessTools.Method(typeof(LayoutEntityPreview),"Initialize"),postfix:new HarmonyMethod(typeof(CoastalImmigrationBeaconMod),nameof(PlacementPreviewPostfix)));var sapType=AccessTools.TypeByName("StorageAutoPause.StorageAutoPauseMod");var markerMethod=sapType==null?null:AccessTools.Method(sapType,"TryFindCanonicalMarkerPosition");if(markerMethod!=null)harmony.Patch(markerMethod,postfix:new HarmonyMethod(typeof(CoastalImmigrationBeaconMod),nameof(SemaphoreMarkerPositionPostfix)));sim.Update.AddNonSaveable<CoastalImmigrationBeaconMod>(this,OnSim);sim.BeforeSave.AddNonSaveable<CoastalImmigrationBeaconMod>(this,OnBeforeSave);loop.InputUpdate.AddNonSaveable<CoastalImmigrationBeaconMod>(this,OnInput);}
 public static void TryWorkPostfix(Beacon __instance,ref bool __result){if(instance!=null&&!instance.IsCoastal(__instance,out instance.seaDirection))__result=false;}
 public static void BeaconVisualPostfix(Beacon reactor,ref EntityMb __result){try{var mb=__result as BeaconMb;if(mb==null)return;beaconVisuals[reactor]=mb.transform;AddPierAndArea(mb.gameObject,false);var area=mb.gameObject.AddComponent<DockAreaVisual>();area.Initialize(reactor);}catch(Exception ex){Log.Warning("Immigration pier visual failed: "+ex.Message);}}
 internal static bool TryGetBeaconVisual(Beacon b,out Transform t){return beaconVisuals.TryGetValue(b,out t)&&t!=null;}
 public static void PlacementPreviewPostfix(LayoutEntityPreview __instance){try{if(__instance.LayoutEntityProto==null||__instance.LayoutEntityProto.Id!=Ids.Buildings.Beacon||__instance.EntityPreviewGameObject==null)return;AddPierAndArea(__instance.EntityPreviewGameObject,true);}catch(Exception ex){Log.Warning("Immigration placement preview failed: "+ex.Message);}}
 public static void SemaphoreMarkerPositionPostfix(IEntity entity,ref Vector3 result,ref bool __result){var b=entity as Beacon;Transform t;if(b!=null&&TryGetBeaconVisual(b,out t)){Vector3 edge=t.TransformPoint(new Vector3(-.35f,0f,2.5f));float topY=edge.y;var renderers=t.GetComponentsInChildren<Renderer>(true);foreach(var renderer in renderers)if(renderer!=null&&renderer.gameObject.name!="Immigration docking area")topY=Math.Max(topY,renderer.bounds.max.y);result=new Vector3(edge.x,topY+.35f,edge.z);__result=true;}}
 static void AddPierAndArea(GameObject parent,bool showArea){if(parent.transform.Find("ImmigrationPier")!=null)return;var root=new GameObject("ImmigrationPier");root.transform.SetParent(parent.transform,false);var wood=new Material(Shader.Find("Standard"));wood.color=new Color(.24f,.12f,.055f);for(int i=0;i<24;i++){var plank=GameObject.CreatePrimitive(PrimitiveType.Cube);plank.name="Pier plank";plank.transform.SetParent(root.transform,false);plank.transform.localPosition=new Vector3(2f,0.12f,5.3f+i*1.25f);plank.transform.localScale=new Vector3(2.4f,.22f,1.1f);plank.GetComponent<MeshRenderer>().sharedMaterial=wood;var col=plank.GetComponent<Collider>();if(col!=null)UnityEngine.Object.Destroy(col);plank.layer=2;if(i%4==1){AddPost(root.transform,.95f,5.3f+i*1.25f,wood);AddPost(root.transform,3.05f,5.3f+i*1.25f,wood);}}if(showArea){var line=root.AddComponent<LineRenderer>();ConfigureAreaLine(line);line.enabled=true;}root.layer=2;}
 static void AddPost(Transform parent,float x,float z,Material mat){var post=GameObject.CreatePrimitive(PrimitiveType.Cube);post.name="Pier support";post.transform.SetParent(parent,false);post.transform.localPosition=new Vector3(x,-2.45f,z);post.transform.localScale=new Vector3(.28f,5f,.28f);post.GetComponent<MeshRenderer>().sharedMaterial=mat;var col=post.GetComponent<Collider>();if(col!=null)UnityEngine.Object.Destroy(col);post.layer=2;}
 internal static void ConfigureAreaLine(LineRenderer line){line.name="Immigration docking area";line.useWorldSpace=false;line.loop=true;line.positionCount=4;line.startWidth=line.endWidth=.18f;line.material=new Material(Shader.Find("Sprites/Default"));line.startColor=line.endColor=new Color(.1f,1f,.35f,.95f);line.SetPosition(0,new Vector3(-3f,.28f,DockAreaNearZ));line.SetPosition(1,new Vector3(7f,.28f,DockAreaNearZ));line.SetPosition(2,new Vector3(7f,.28f,DockAreaFarZ));line.SetPosition(3,new Vector3(-3f,.28f,DockAreaFarZ));}
 bool IsCoastal(Beacon b,out RelTile2i dir){
  dir=RelTile2i.Zero;if(b==null||!b.IsConstructed)return false;Tile2i c=b.Position2f.Tile2i;
  RelTile2i preferred=b.Transform.TransformMatrix.Transform(new RelTile2i(0,1));RelTile2i[] ds={preferred,new RelTile2i(-preferred.X,-preferred.Y),new RelTile2i(-preferred.Y,preferred.X),new RelTile2i(preferred.Y,-preferred.X)};
  foreach(RelTile2i d in ds){RelTile2i tangent=new RelTile2i(-d.Y,d.X);int ocean=0,total=0;for(int forward=30;forward<=50;forward+=2)for(int side=-2;side<=2;side++){Tile2i q=c+d*forward+tangent*side;total++;if(q.X>=0&&q.Y>=0&&q.X<terrain.TerrainWidth&&q.Y<terrain.TerrainHeight&&terrain.IsOcean(q))ocean++;}if(ocean>=total*3/4){dir=d;return true;}}
  return false;
 }
 void OnSim(){
  beacon=refugees.Beacon.ValueOrNull;RelTile2i d;coastal=IsCoastal(beacon,out d);if(coastal)seaDirection=d;
  long progress=refugees.StepsDoneSoFar.Ticks;
  bool immigrationCompleted=priorProgress>0&&progress<priorProgress;
  if(immigrationCompleted&&(boatExists||createBoatRequested)){journeyCommitted=true;departBoatRequested=true;desiredBoatFrozen=false;priorProgress=progress;return;}
  if(!coastal||beacon==null){RequestBoatDestroy();priorProgress=progress;return;}
  if(refugees.NextReward.IsNone){if(boatExists||createBoatRequested){departBoatRequested=true;desiredBoatFrozen=false;}else RequestBoatDestroy();priorProgress=progress;return;}
  long total=refugees.NextReward.Value.Duration.Ticks,left=total-progress;
  long travel=1.Months().Ticks*31/15;
  desiredBoatFrozen=!journeyCommitted&&(beacon.IsPaused||!beacon.IsEnabled);
  if(!boatExists&&!createBoatRequested&&!journeyCommitted&&progress>0&&left<=travel){requestedBoatBeacon=beacon;requestedBoatDirection=seaDirection;destroyBoatRequested=false;createBoatRequested=true;}
  if(boatExists&&progress<priorProgress&&priorProgress>0){departBoatRequested=true;desiredBoatFrozen=false;}
  priorProgress=progress;
 }
 void OnInput(GameTime t){
  // Unity objects may only be created, changed, or destroyed on the main thread.
  // OnSim merely publishes commands; this callback executes all Unity work.
  if(destroyBoatRequested){destroyBoatRequested=false;createBoatRequested=false;departBoatRequested=false;if(boat!=null){boat.Dispose();boat=null;}boatExists=false;}
  if(createBoatRequested&&boat==null){createBoatRequested=false;try{boat=new BoatVisual(requestedBoatBeacon,requestedBoatDirection,Manifest.RootDirectoryPath,terrain,assetsDb);boatExists=true;}catch(Exception ex){boat=null;boatExists=false;Log.Error("Immigrant boat creation failed on main thread: "+ex);}}
  if(boat!=null){boat.Frozen=desiredBoatFrozen;if(departBoatRequested){departBoatRequested=false;boat.BeginDeparture();}boat.Update(t);if(boat.Finished){boat.Dispose();boat=null;boatExists=false;journeyCommitted=false;}}
  BindPanel();UpdatePanel();DockAreaVisual.SelectedBeacon=inspectors.GetFirstActiveEntityOrNull() as Beacon;
 }
 void BindPanel(){var e=inspectors.GetFirstActiveEntityOrNull() as Beacon;var i=inspectors.GetFirstActiveInspectorOrNull();if(e==null||i==null||i.GetType().FullName!="Mafi.Unity.Ui.Inspectors.BeaconInspector")return;if(boundInspector==i&&panelBody!=null)return;var m=i.GetType().GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).FirstOrDefault(x=>x.Name=="AddPanelWithHeader"&&x.GetParameters().Length==1&&x.GetParameters()[0].ParameterType.IsArray);if(m==null)return;panelBody=new Column(5.pt()).AlignItemsStretch<Column>();var a=Array.CreateInstance(typeof(UiComponent),1);a.SetValue(panelBody,0);panel=m.Invoke(i,new object[]{a});Invoke(panel,"Title",new object[]{new LocStrFormatted(title)});boundInspector=i;uiHash=-1;}
 void UpdatePanel(){if(panelBody==null||boundInspector!=inspectors.GetFirstActiveInspectorOrNull())return;int h=(coastal?1:0)+(boatExists?2:0)+(beacon!=null&&beacon.IsPaused?4:0);if(h==uiHash)return;uiHash=h;Invoke(panelBody,"Clear",new object[0]);panelBody.Add(new Label(new LocStrFormatted(coastal?valid:invalid)));}
 void RequestBoatDestroy(){destroyBoatRequested=true;desiredBoatFrozen=true;journeyCommitted=false;}
 void OnBeforeSave(){RequestBoatDestroy();}
 void LoadLocalization(){
  localization.Clear();
  LoadLocalizationFile("en");
  string culture="en-US";
  try{culture=LocalizationManager.CurrentLangInfo.CultureInfoId??"en-US";}catch(Exception ex){Log.Warning("CoastalImmigrationBeacon: could not read game language: "+ex.Message);}
  string code=MapCulture(culture);
  if(!string.Equals(code,"en",StringComparison.OrdinalIgnoreCase))LoadLocalizationFile(code);
  string value;
  if(localization.TryGetValue("panel_title",out value))title=value;
  if(localization.TryGetValue("coastal_valid",out value))valid=value;
  if(localization.TryGetValue("coastal_invalid",out value))invalid=value;
  Log.Info("CoastalImmigrationBeacon: localization selected '"+code+"' for game culture '"+culture+"'.");
 }
 void LoadLocalizationFile(string code){
  try{
   string path=Path.Combine(Manifest.RootDirectoryPath,"Localization",code+".json");
   if(!File.Exists(path)){Log.Warning("CoastalImmigrationBeacon: localization file not found: "+path);return;}
   string json=File.ReadAllText(path,Encoding.UTF8);
   foreach(Match match in Regex.Matches(json,"\\\"(?<k>(?:\\\\.|[^\\\"\\\\])*)\\\"\\s*:\\s*\\\"(?<v>(?:\\\\.|[^\\\"\\\\])*)\\\""))
    localization[Regex.Unescape(match.Groups["k"].Value)]=Regex.Unescape(match.Groups["v"].Value.Replace("\\/","/"));
  }catch(Exception ex){Log.Warning("CoastalImmigrationBeacon: failed to load localization '"+code+"': "+ex.Message);}
 }
 static string MapCulture(string culture){
  string c=(culture??"en-US").Replace('_','-').ToLowerInvariant();
  if(c.StartsWith("zh-hant")||c=="zh-tw"||c=="zh-hk")return "zh-Hant";
  if(c.StartsWith("zh"))return "zh-Hans";
  if(c.StartsWith("pt"))return "pt-BR";
  string two=c.Split('-')[0];
  switch(two){case "ca":case "cs":case "de":case "es":case "et":case "fr":case "hu":case "it":case "ja":case "ko":case "nb":case "nl":case "pl":case "ru":case "sv":case "tr":case "uk":return two;case "no":return "nb";default:return "en";}
 }
 static object Invoke(object o,string n,object[] a){if(o==null)return null;var m=o.GetType().GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).FirstOrDefault(x=>x.Name==n&&x.GetParameters().Length==a.Length);return m==null?null:m.Invoke(o,a);}
 public void Dispose(){if(boat!=null){boat.Dispose();boat=null;}boatExists=false;if(harmony!=null)harmony.UnpatchAll("sirael.coastalimmigrationbeacon");if(instance==this)instance=null;}
}

sealed class DockAreaVisual:MonoBehaviour{
 public static Beacon SelectedBeacon;Beacon entity;LineRenderer line;
 public void Initialize(Beacon b){entity=b;line=gameObject.AddComponent<LineRenderer>();CoastalImmigrationBeaconMod.ConfigureAreaLine(line);}
 void Update(){if(line!=null)line.enabled=entity!=null&&(!entity.IsConstructed||SelectedBeacon==entity);}
}

// Purely visual and deliberately non-saveable. The vanilla RefugeesManager remains
// the sole owner of immigration and population rewards.
sealed class BoatVisual:IDisposable{
 readonly GameObject root;readonly Vector3 dock,approach,start,departureControl1,departureControl2;readonly Quaternion inboundRotation,berthRotation;readonly AudioSource engineAudio,arrivalHorn,departureHorn;readonly ParticleSystem[] vanillaParticles;float t,dockWait;bool departureRequested,arrivalPlayed,effectsRunning;public bool Frozen,Departing;public bool Finished{get;private set;}
 public BoatVisual(Beacon beacon,RelTile2i dir,string modRoot,TerrainManager terrain,AssetsDb assetsDb){
  Transform beaconTransform;bool hasVisual=CoastalImmigrationBeaconMod.TryGetBeaconVisual(beacon,out beaconTransform);Vector3 origin=hasVisual?beaconTransform.TransformPoint(new Vector3(CoastalImmigrationBeaconMod.PierCenterX,0f,0f)):beacon.Position3f.ToVector3();Vector3 axisPerTile=hasVisual?beaconTransform.TransformVector(Vector3.forward):dir.ExtendZ(0).ToVector3();Vector3 heading=axisPerTile.normalized;RelTile2i tangent=new RelTile2i(-dir.Y,dir.X);Tile2i c=beacon.Position2f.Tile2i;
  Tile2i sideTile=c+dir*27+tangent*6;if(!terrain.IsValidCoord(sideTile)||!terrain.IsOcean(sideTile)){tangent=new RelTile2i(-tangent.X,-tangent.Y);sideTile=c+dir*27+tangent*6;}
  Vector3 tangentPerTile=hasVisual?beaconTransform.TransformVector(Vector3.right):tangent.ExtendZ(0).ToVector3();Vector3 tangentHeading=tangentPerTile.normalized;int edgeDistance=dir.X>0?terrain.TerrainWidth-1-c.X:dir.X<0?c.X:dir.Y>0?terrain.TerrainHeight-1-c.Y:c.Y;int spawnTiles=Math.Max(0,edgeDistance);float waterY=terrain.GetHeightOrOceanSurface(sideTile).ToUnityUnits()-.75f;
  float dockZ=CoastalImmigrationBeaconMod.PierEndZ+CoastalImmigrationBeaconMod.BoatHalfWidth+CoastalImmigrationBeaconMod.DockGap;dock=hasVisual?beaconTransform.TransformPoint(new Vector3(CoastalImmigrationBeaconMod.PierCenterX,0f,dockZ)):origin+axisPerTile*dockZ;dock.y=waterY;approach=hasVisual?beaconTransform.TransformPoint(new Vector3(CoastalImmigrationBeaconMod.PierCenterX,0f,CoastalImmigrationBeaconMod.DockAreaFarZ)):origin+axisPerTile*CoastalImmigrationBeaconMod.DockAreaFarZ;approach.y=waterY;Tile2i spawnTile=c+dir*spawnTiles;start=spawnTile.CenterTile2f.ExtendZ(TerrainManager.HEIGHT_JUST_ABOVE_OCEAN.Value).ToVector3();start.y=waterY;departureControl1=dock+tangentHeading*60f;departureControl1.y=waterY;departureControl2=start-heading*45f;departureControl2.y=waterY;
  inboundRotation=Quaternion.LookRotation(heading,Vector3.up);berthRotation=Quaternion.LookRotation(-tangentHeading,Vector3.up);string assets=Path.Combine(modRoot,"Assets");root=ObjLoader.Load(Path.Combine(assets,"immigrant_boat.obj"),assets);root.name="CoastalImmigrationBeacon.ImmigrantBoat";root.layer=2;SetLayer(root);root.transform.localScale=new Vector3(.01f,.01f,.01f);vanillaParticles=AttachVanillaRearEffects(root,assetsDb);engineAudio=AddVanillaAudio(root,assetsDb,"Assets/Base/Ships/CargoShip/Audio/Engine.prefab",true);arrivalHorn=AddVanillaAudio(root,assetsDb,"Assets/Base/Ships/CargoShip/Audio/Arrival.prefab",false);departureHorn=AddVanillaAudio(root,assetsDb,"Assets/Base/Ships/CargoShip/Audio/Departure.prefab",false);root.transform.position=start;root.transform.rotation=inboundRotation;SetMovingEffects(true);
  Log.Info("Immigration boat route: beacon="+c+", map="+terrain.TerrainWidth+"x"+terrain.TerrainHeight+", direction="+dir+", edgeDistance="+edgeDistance+", spawnTiles="+spawnTiles+", start="+start+", approach="+approach+", dock="+dock);
 }
 public void BeginDeparture(){departureRequested=true;Frozen=false;}
 public void Update(GameTime time){if(Finished||Frozen)return;float dt=time.DeltaTimeMs/1000f;
  if(!Departing){t=Mathf.Clamp01(t+dt/41.6667f);if(t<.78f){root.transform.position=Vector3.Lerp(start,approach,Smooth(t/.78f));root.transform.rotation=inboundRotation;}else{float dockingT=Smooth((t-.78f)/.22f);root.transform.position=Vector3.Lerp(approach,dock,dockingT);root.transform.rotation=Quaternion.Slerp(inboundRotation,berthRotation,dockingT);}if(t>=1f&&!arrivalPlayed){arrivalPlayed=true;if(arrivalHorn!=null)arrivalHorn.Play();SetMovingEffects(false);}if(t>=1f&&departureRequested){dockWait+=dt;if(dockWait>=5f){Departing=true;t=0f;if(departureHorn!=null)departureHorn.Play();SetMovingEffects(true);}}return;}
  t=Mathf.Clamp01(t+dt/41.6667f);float u=t;root.transform.position=Bezier(dock,departureControl1,departureControl2,start,u);Vector3 tangent=BezierTangent(dock,departureControl1,departureControl2,start,u);if(tangent.sqrMagnitude>.001f)root.transform.rotation=Quaternion.LookRotation(-tangent.normalized,Vector3.up);if(t>=1f)Finished=true;
 }
 void SetMovingEffects(bool enabled){if(effectsRunning==enabled)return;effectsRunning=enabled;if(engineAudio!=null){if(enabled){if(!engineAudio.isPlaying)engineAudio.Play();}else engineAudio.Stop();}if(vanillaParticles==null)return;foreach(var ps in vanillaParticles){if(ps==null)continue;ps.gameObject.SetActive(true);var emission=ps.emission;emission.enabled=enabled;if(enabled)ps.Play(true);else ps.Stop(true,ParticleSystemStopBehavior.StopEmitting);}}
 static ParticleSystem[] AttachVanillaRearEffects(GameObject root,AssetsDb assetsDb){try{var rear=assetsDb.GetClonedPrefabOrEmptyGo("Assets/Base/Ships/CargoShip/CargoShip_Rear.prefab");rear.name="Vanilla ship particles";rear.transform.SetParent(root.transform,false);rear.transform.localPosition=new Vector3(0f,0f,-480f);rear.transform.localRotation=Quaternion.Euler(0f,180f,0f);rear.transform.localScale=Vector3.one*100f;foreach(var r in rear.GetComponentsInChildren<Renderer>(true))if(!(r is ParticleSystemRenderer))r.enabled=false;foreach(var c in rear.GetComponentsInChildren<Collider>(true))UnityEngine.Object.Destroy(c);SetLayer(rear);var particles=rear.GetComponentsInChildren<ParticleSystem>(true);foreach(var ps in particles){string hierarchy=ParticlePath(ps.transform).ToLowerInvariant();if(hierarchy.Contains("exhaust")||hierarchy.Contains("smoke")){Quaternion rotation=ps.transform.rotation;ps.transform.SetParent(root.transform,true);ps.transform.localPosition=new Vector3(0f,806f,149f);ps.transform.rotation=rotation;}}Log.Info("Immigration boat vanilla particle systems found: "+particles.Length);return particles;}catch(Exception ex){Log.Warning("Immigration boat vanilla particles unavailable: "+ex.Message);return new ParticleSystem[0];}}
 static string ParticlePath(Transform t){string result=t.name;while(t.parent!=null){t=t.parent;result=t.name+"/"+result;}return result;}
 static AudioSource AddVanillaAudio(GameObject root,AssetsDb assetsDb,string path,bool loop){try{var go=assetsDb.GetClonedPrefabOrEmptyGo(path);go.transform.SetParent(root.transform,false);SetLayer(go);var a=go.GetComponentInChildren<AudioSource>(true);if(a!=null){a.loop=loop;a.spatialBlend=1f;a.rolloffMode=AudioRolloffMode.Logarithmic;a.minDistance=8f;a.maxDistance=100f;}return a;}catch(Exception ex){Log.Warning("Immigration boat vanilla audio unavailable: "+ex.Message);return null;}}
 static float Smooth(float x){return x*x*(3-2*x);}static Vector3 Bezier(Vector3 a,Vector3 b,Vector3 c,Vector3 d,float t){float s=1f-t;return s*s*s*a+3f*s*s*t*b+3f*s*t*t*c+t*t*t*d;}static Vector3 BezierTangent(Vector3 a,Vector3 b,Vector3 c,Vector3 d,float t){float s=1f-t;return 3f*s*s*(b-a)+6f*s*t*(c-b)+3f*t*t*(d-c);}
 static void SetLayer(GameObject g){g.layer=2;foreach(Transform c in g.transform)SetLayer(c.gameObject);}public void Dispose(){if(root!=null)UnityEngine.Object.Destroy(root);}
}

static class ObjLoader{
 static GameObject template;
 public static void Preload(string path,string assetsDir){if(template!=null)return;template=Build(path,assetsDir);template.name="Immigration boat template";template.hideFlags=HideFlags.HideAndDontSave;template.SetActive(false);}
 public static GameObject Load(string path,string assetsDir){Preload(path,assetsDir);var go=UnityEngine.Object.Instantiate(template);go.hideFlags=HideFlags.None;go.SetActive(true);return go;}
 static GameObject Build(string path,string assetsDir){var go=new GameObject("Immigration boat");try{
  var verts=new List<Vector3>();var uvs=new List<Vector2>();var outV=new List<Vector3>();var outU=new List<Vector2>();
  var groups=new Dictionary<string,List<int>>(StringComparer.OrdinalIgnoreCase);var order=new List<string>();string current="FrontColor";
  Action<string> select=name=>{current=name;if(!groups.ContainsKey(name)){groups[name]=new List<int>();order.Add(name);}};select(current);
  foreach(string raw in File.ReadLines(path)){string s=raw.Trim();if(s.StartsWith("v ")){var p=s.Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);verts.Add(new Vector3(F(p[2]),F(p[3]),F(p[1])));}else if(s.StartsWith("vt ")){var p=s.Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);uvs.Add(new Vector2(F(p[1]),F(p[2])));}else if(s.StartsWith("usemtl "))select(s.Substring(7).Trim());else if(s.StartsWith("f ")){var p=s.Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);for(int k=1;k<p.Length-2;k++){var tr=groups[current];tr.Add(Add(p[1],current,verts,uvs,outV,outU));tr.Add(Add(p[k+1],current,verts,uvs,outV,outU));tr.Add(Add(p[k+2],current,verts,uvs,outV,outU));}}}
  var mesh=new Mesh();mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;mesh.SetVertices(outV);mesh.SetUVs(0,outU);mesh.subMeshCount=order.Count;for(int i=0;i<order.Count;i++)mesh.SetTriangles(groups[order[i]],i);mesh.RecalculateNormals();mesh.RecalculateBounds();go.AddComponent<MeshFilter>().sharedMesh=mesh;
  var mats=new Material[order.Count];for(int i=0;i<order.Count;i++){string n=order[i];var m=new Material(Shader.Find("Standard"));m.color=ColorFor(n);string tex=TextureFor(n);if(tex!=null)m.mainTexture=LoadTexture(Path.Combine(assetsDir,tex));mats[i]=m;}go.AddComponent<MeshRenderer>().sharedMaterials=mats;
 }catch(Exception ex){Log.Error("Immigrant boat OBJ load failed: "+ex);}return go;}
 static string TextureFor(string n){if(n.Equals("boat_buffer",StringComparison.OrdinalIgnoreCase))return "boat_buffer_diffuse.jpg";if(n.Equals("boat_roof_accessory",StringComparison.OrdinalIgnoreCase))return "boat_roof_accessory_diffuse.jpg";if(n.Equals("boat_body",StringComparison.OrdinalIgnoreCase))return "boat_body_diffuse.jpg";return null;}
 static Texture2D LoadTexture(string path){if(!File.Exists(path))return null;var t=new Texture2D(2,2);t.LoadImage(File.ReadAllBytes(path));return t;}
 static Color ColorFor(string n){if(n.IndexOf("glass",StringComparison.OrdinalIgnoreCase)>=0)return new Color(.11f,.22f,.25f,1f);if(n.IndexOf("black",StringComparison.OrdinalIgnoreCase)>=0)return new Color(.025f,.025f,.025f,1f);if(n.IndexOf("bronze",StringComparison.OrdinalIgnoreCase)>=0)return new Color(.38f,.25f,.06f,1f);if(n.IndexOf("silver",StringComparison.OrdinalIgnoreCase)>=0)return new Color(.55f,.57f,.58f,1f);return Color.white;}
 static Vector2 FixUv(string material,Vector2 value){if(!material.Equals("boat_roof_accessory",StringComparison.OrdinalIgnoreCase)||value.x<.7285f||value.y<.64f)return value;const float first=.7285f,width=.0905f;int column=Mathf.Clamp(Mathf.FloorToInt((value.x-first)/width),0,2);float center=first+(column+.5f)*width;value.x=2f*center-value.x;return value;}
 static int Add(string token,string material,List<Vector3> v,List<Vector2> uv,List<Vector3> ov,List<Vector2> ou){var p=token.Split('/');int vi=int.Parse(p[0],CultureInfo.InvariantCulture)-1,ti=p.Length>1&&!string.IsNullOrEmpty(p[1])?int.Parse(p[1],CultureInfo.InvariantCulture)-1:-1;ov.Add(v[vi]);ou.Add(ti>=0&&ti<uv.Count?FixUv(material,uv[ti]):Vector2.zero);return ov.Count-1;}static float F(string s){return float.Parse(s,CultureInfo.InvariantCulture);}
}
}
