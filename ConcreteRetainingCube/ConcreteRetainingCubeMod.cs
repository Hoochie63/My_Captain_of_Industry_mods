using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mafi;
using Mafi.Base;
using Mafi.Base.Prototypes.Buildings;
using Mafi.Collections;
using Mafi.Collections.ImmutableCollections;
using Mafi.Core;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Static;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Economy;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using Mafi.Core.Research;
using Mafi.Core.Simulation;
using Mafi.Core.Vehicles;
using Mafi.Unity;
using UnityEngine;
using Mafi.Localization;

namespace ConcreteRetainingCube {

public sealed class ConcreteRetainingCubeMod : IMod, IDisposable {
 public static readonly StaticEntityProto.ID CubeId=new StaticEntityProto.ID("ConcreteRetainingCube");
 public static readonly StaticEntityProto.ID WeatheredCubeId=new StaticEntityProto.ID("ConcreteRetainingCubeWeathered");
 public static readonly StaticEntityProto.ID Slope1Id=new StaticEntityProto.ID("ConcreteRetainingCubeSlope1");
 public static readonly StaticEntityProto.ID Slope1WeatheredId=new StaticEntityProto.ID("ConcreteRetainingCubeSlope1Weathered");
 public static readonly StaticEntityProto.ID Slope2Id=new StaticEntityProto.ID("ConcreteRetainingCubeSlope2");
 public static readonly StaticEntityProto.ID Slope2WeatheredId=new StaticEntityProto.ID("ConcreteRetainingCubeSlope2Weathered");
 public static readonly StaticEntityProto.ID Slope3Id=new StaticEntityProto.ID("ConcreteRetainingCubeSlope3");
 public static readonly StaticEntityProto.ID Slope3WeatheredId=new StaticEntityProto.ID("ConcreteRetainingCubeSlope3Weathered");
 public static readonly StaticEntityProto.ID RailStraightId=new StaticEntityProto.ID("ConcreteRetainingCubeRailStraight");
 public static readonly StaticEntityProto.ID RailCornerId=new StaticEntityProto.ID("ConcreteRetainingCubeRailCorner");
 public static readonly StaticEntityProto.ID RailEndId=new StaticEntityProto.ID("ConcreteRetainingCubeRailEnd");
 public static readonly StaticEntityProto.ID RailTId=new StaticEntityProto.ID("ConcreteRetainingCubeRailT");
 public static readonly Proto.ID CubeCategoryId=new Proto.ID("ConcreteRetainingCubeCategory");
 public static readonly Proto.ID SlopeCategoryId=new Proto.ID("ConcreteRetainingCubeSlopeCategory");
 public const string WeatheredIconPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeWeathered.png";
 public const string RailStraightIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeRailStraight.png";
 public const string RailCornerIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeRailCorner.png";
 public const string RailEndIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeRailEnd.png";
 public const string RailTIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeRailT.png";
 public const string Slope1WeatheredIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeSlope1Weathered.png";
 public const string Slope2WeatheredIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeSlope2Weathered.png";
 public const string Slope3WeatheredIconPath="Assets/Mods/ConcreteRetainingCube/IconsV2/ConcreteRetainingCubeSlope3Weathered.png";
 public const string WeatheredPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeWeathered.prefab";
 public const string Slope1WeatheredPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeSlope1Weathered.prefab";
 public const string Slope2WeatheredPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeSlope2Weathered.prefab";
 public const string Slope3WeatheredPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeSlope3Weathered.prefab";
 public const string RailStraightPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeRailStraight.prefab";
 public const string RailCornerPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeRailCorner.prefab";
 public const string RailEndPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeRailEnd.prefab";
 public const string RailTPrefabPath="Assets/Mods/ConcreteRetainingCube/ConcreteRetainingCubeRailT.prefab";

 public ModManifest Manifest{get;private set;}
 public bool IsUiOnly{get{return false;}}
 [Obsolete] public Option<IConfig> ModConfig{get;set;}
 public ModJsonConfig JsonConfig{get;private set;}
 CubeVehicleSurfaceCoordinator m_surfaceCoordinator;
 readonly Dictionary<string,string> m_texts=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
 public ConcreteRetainingCubeMod(ModManifest manifest){Manifest=manifest;JsonConfig=new ModJsonConfig(this);}

 public void RegisterPrototypes(ProtoRegistrator r){
  LoadLocalization();
  var concrete=r.PrototypesDb.GetOrThrow<ProductProto>(Ids.Products.ConcreteSlab);
  var iron=r.PrototypesDb.GetOrThrow<ProductProto>(Ids.Products.Iron);
  var costs=new EntityCosts(new AssetValue(concrete.WithQuantity(2),iron.WithQuantity(1)));

  // One occupied tile, one tile high. The four surrounding terrain vertices
  // use the same DisableTerrainPhysics constraint as vanilla retaining walls.
  var token=new CustomLayoutToken("(W)",(p,h)=>new LayoutTokenSpec(0,1,LayoutTileConstraint.Ground|LayoutTileConstraint.NoRubbleAfterCollapse));
  var layoutParams=new EntityLayoutParams(
   customTokens:new[]{token},
   customVertexDataLayout:new[]{"##","##"},
   customVertexTransformFn:(v,c)=>c=='#'?v.WithExtraConstraint(LayoutTileConstraint.DisableTerrainPhysics):v,
   customCollapseVerticesThreshold:1,
   customPlacementRange:new ThicknessIRange(0,32));
  var baseLayout=r.LayoutParser.ParseLayoutOrThrow(layoutParams,new[]{"(W)"});
  var flatLayout=WithVehicleSurface(baseLayout,1.0,1.0);
  var slope1Layout=WithVehicleSurface(baseLayout,0.0,1.0/3.0);
  var slope2Layout=WithVehicleSurface(baseLayout,1.0/3.0,2.0/3.0);
  var slope3Layout=WithVehicleSurface(baseLayout,2.0/3.0,1.0);
  var terraformingCategory=r.PrototypesDb.GetOrThrow<ToolbarCategoryProto>(IdsCore.ToolbarCategories.Terraforming);
  // Keep the vanilla Terraforming toolbar as the only category. The upgrade
  // chains below provide the two drop-down families without extra top tabs.
  var cubeCategories=ImmutableArray.Create(new ToolbarEntryData(terraformingCategory));
  var slopeCategories=ImmutableArray.Create(new ToolbarEntryData(terraformingCategory));
  var hiddenCategories=ImmutableArray<ToolbarEntryData>.Empty;

  // Preserve every old prototype ID for save compatibility. The former clean
  // IDs are the visible weathered family; old weathered duplicates are hidden.
  var cleanGfx=new LayoutEntityProto.Gfx(WeatheredPrefabPath,default(RelTile3f),WeatheredIconPath.SomeOption(),default(ColorRgba),false,null,cubeCategories,true);
  var cleanStrings=Proto.CreateStr(CubeId,T("cube_name","Weathered concrete retaining cube"),T("cube_desc","A stackable 1 x 1 x 1 weathered concrete retaining block."),null);
  var clean=r.PrototypesDb.Add<RetainingWallProto>(new RetainingWallProto(CubeId,cleanStrings,flatLayout,costs,cleanGfx),false);

  var weatheredGfx=new LayoutEntityProto.Gfx(WeatheredPrefabPath,default(RelTile3f),WeatheredIconPath.SomeOption(),default(ColorRgba),false,null,hiddenCategories,true);
  var weatheredStrings=Proto.CreateStr(WeatheredCubeId,T("legacy_cube_name","Legacy weathered concrete retaining cube"),T("legacy_desc","Compatibility prototype for existing saves."),null);
  var weathered=r.PrototypesDb.Add<RetainingWallProto>(new RetainingWallProto(WeatheredCubeId,weatheredStrings,flatLayout,costs,weatheredGfx),false);

  var railStraight=AddSlope(r,flatLayout,costs,cubeCategories,RailStraightId,T("rail_straight_name","Weathered cube - straight railing"),T("rail_straight_desc","Weathered cube with a centered straight safety railing."),RailStraightPrefabPath,RailStraightIconPath);
  var railCorner=AddSlope(r,flatLayout,costs,cubeCategories,RailCornerId,T("rail_corner_name","Weathered cube - corner railing"),T("rail_corner_desc","Weathered cube with a centered 90-degree safety railing."),RailCornerPrefabPath,RailCornerIconPath);
  var railEnd=AddSlope(r,flatLayout,costs,cubeCategories,RailEndId,T("rail_end_name","Weathered cube - railing end"),T("rail_end_desc","Weathered cube with a centered terminating safety railing."),RailEndPrefabPath,RailEndIconPath);
  var railT=AddSlope(r,flatLayout,costs,cubeCategories,RailTId,T("rail_t_name","Weathered cube - T railing"),T("rail_t_desc","Weathered cube with a centered T-junction safety railing."),RailTPrefabPath,RailTIconPath);

  var slope1=AddSlope(r,slope1Layout,costs,slopeCategories,Slope1Id,T("slope1_name","Weathered concrete ramp cube 1/3"),T("slope1_desc","Weathered ramp segment rising from 0% to 33%."),Slope1WeatheredPrefabPath,Slope1WeatheredIconPath);
  var slope1Weathered=AddSlope(r,slope1Layout,costs,hiddenCategories,Slope1WeatheredId,T("legacy_slope1_name","Legacy weathered concrete ramp cube 1/3"),T("legacy_desc","Compatibility prototype for existing saves."),Slope1WeatheredPrefabPath,Slope1WeatheredIconPath);
  var slope2=AddSlope(r,slope2Layout,costs,slopeCategories,Slope2Id,T("slope2_name","Weathered concrete ramp cube 2/3"),T("slope2_desc","Weathered ramp segment rising from 33% to 66%."),Slope2WeatheredPrefabPath,Slope2WeatheredIconPath);
  var slope2Weathered=AddSlope(r,slope2Layout,costs,hiddenCategories,Slope2WeatheredId,T("legacy_slope2_name","Legacy weathered concrete ramp cube 2/3"),T("legacy_desc","Compatibility prototype for existing saves."),Slope2WeatheredPrefabPath,Slope2WeatheredIconPath);
  var slope3=AddSlope(r,slope3Layout,costs,slopeCategories,Slope3Id,T("slope3_name","Weathered concrete ramp cube 3/3"),T("slope3_desc","Weathered ramp segment rising from 66% to 100%."),Slope3WeatheredPrefabPath,Slope3WeatheredIconPath);
  var slope3Weathered=AddSlope(r,slope3Layout,costs,hiddenCategories,Slope3WeatheredId,T("legacy_slope3_name","Legacy weathered concrete ramp cube 3/3"),T("legacy_desc","Compatibility prototype for existing saves."),Slope3WeatheredPrefabPath,Slope3WeatheredIconPath);

  clean.SetNextTierIndirect(railStraight,false,false);
  railStraight.SetNextTierIndirect(railCorner,false,false);
  railCorner.SetNextTierIndirect(railEnd,false,false);
  railEnd.SetNextTierIndirect(railT,false,false);
  slope1.SetNextTierIndirect(slope2,false,false);
  slope2.SetNextTierIndirect(slope3,false,false);

  var retainingWallsResearch=r.PrototypesDb.GetOrThrow<ResearchNodeProto>(Ids.Research.RetainingWalls);
  retainingWallsResearch.AddProtoToUnlock(clean,false);
  retainingWallsResearch.AddProtoToUnlock(railStraight,true);
  retainingWallsResearch.AddProtoToUnlock(railCorner,true);
  retainingWallsResearch.AddProtoToUnlock(railEnd,true);
  retainingWallsResearch.AddProtoToUnlock(railT,true);
  retainingWallsResearch.AddProtoToUnlock(slope1,false);
  retainingWallsResearch.AddProtoToUnlock(slope2,true);
  retainingWallsResearch.AddProtoToUnlock(slope3,true);
 }

 string T(string key,string fallback){string value;return m_texts.TryGetValue(key,out value)?value:fallback;}
 void LoadLocalization(){
  m_texts.Clear();
  LoadLocalizationFile("en");
  string culture="en-US";
  try{culture=LocalizationManager.CurrentLangInfo.CultureInfoId??"en-US";}catch{}
  string code=MapCulture(culture);
  if(!string.Equals(code,"en",StringComparison.OrdinalIgnoreCase))LoadLocalizationFile(code);
  Log.Info("ConcreteRetainingCube: localization selected '"+code+"' for game culture '"+culture+"'.");
 }
 void LoadLocalizationFile(string code){
  try{
   string path=Path.Combine(Manifest.RootDirectoryPath,"Localization",code+".json");
   if(!File.Exists(path)){Log.Warning("ConcreteRetainingCube: localization file not found: "+path);return;}
   string json=File.ReadAllText(path,Encoding.UTF8);
   foreach(Match match in Regex.Matches(json,"\\\"(?<k>(?:\\\\.|[^\\\"\\\\])*)\\\"\\s*:\\s*\\\"(?<v>(?:\\\\.|[^\\\"\\\\])*)\\\""))
    m_texts[Regex.Unescape(match.Groups["k"].Value)]=Regex.Unescape(match.Groups["v"].Value.Replace("\\/","/"));
  }catch(Exception ex){Log.Warning("ConcreteRetainingCube: failed to load localization '"+code+"': "+ex.Message);}
 }
 static string MapCulture(string culture){
  string c=(culture??"en-US").Replace('_','-').ToLowerInvariant();
  if(c.StartsWith("zh-hant")||c=="zh-tw"||c=="zh-hk")return "zh-Hant";
  if(c.StartsWith("zh"))return "zh-Hans";
  if(c.StartsWith("pt"))return "pt-BR";
  string two=c.Split('-')[0];
  switch(two){case "ca":case "cs":case "de":case "es":case "et":case "fr":case "hu":case "it":case "ja":case "ko":case "nb":case "nl":case "pl":case "ru":case "sv":case "tr":case "uk":return two;case "no":return "nb";default:return "en";}
 }

 static EntityLayout WithVehicleSurface(EntityLayout source,double low,double high){
  // Match vanilla apron / bridge vehicle surfaces.  HasVehicleSurface alone is
  // only metadata; the overlap constraint is what makes the occupied layout
  // participate in the vanilla vehicle-surface clearance rules.
  var tiles=source.LayoutTiles.Map(t=>new LayoutTile(t.Coord,t.SourceStrIndex,t.OccupiedThickness,t.TerrainHeight,t.MinTerrainHeight,t.MaxTerrainHeight,t.Constraint|LayoutTileConstraint.OverlappableVehicleSurface,t.TerrainMaterialProto,t.TileSurfaceProto,true));
  var vertices=source.TerrainVertices.Map(v=>{
   double x=v.Coord.X<=source.CoreMin.X?0.0:1.0;
   var surface=(0.05+low+(high-low)*x).TilesThick();
   return new TerrainVertexRel(v.Coord,v.OccupiedThickness,v.Constraint|LayoutTileConstraint.OverlappableVehicleSurface,v.TerrainMaterial,v.TerrainHeight,v.MinTerrainHeight,v.MaxTerrainHeight,surface,v.ContributingTiles,v.LowestTileIndex);
  });
  return new EntityLayout(source.SourceLayoutStr,tiles,vertices,source.Ports,source.LayoutParams,source.CollapseVerticesThreshold,source.OriginTile);
 }

 static RetainingWallProto AddSlope(ProtoRegistrator r,EntityLayout layout,EntityCosts costs,ImmutableArray<ToolbarEntryData> categories,StaticEntityProto.ID id,string name,string description,string prefab,string icon){
  var gfx=new LayoutEntityProto.Gfx(prefab,default(RelTile3f),icon.SomeOption(),default(ColorRgba),false,null,categories,true);
  return r.PrototypesDb.Add<RetainingWallProto>(new RetainingWallProto(id,Proto.CreateStr(id,name,description,null),layout,costs,gfx),false);
 }

 public void RegisterDependencies(DependencyResolverBuilder b,ProtosDb p,bool loaded){}
 public void MigrateJsonConfig(VersionSlim v,Dict<string,object> c){}
 public void EarlyInit(DependencyResolver r){CubeAssets.RegisterAll(r.Resolve<AssetsDb>());}
 public void Initialize(DependencyResolver r,bool loaded){m_surfaceCoordinator=new CubeVehicleSurfaceCoordinator(r);}
 public void Dispose(){if(m_surfaceCoordinator!=null)m_surfaceCoordinator.Dispose();m_surfaceCoordinator=null;}
}

// VehicleSurfaceProvider keeps the height contributed by the first entity at a
// shared vertex.  Stacked retaining blocks can therefore leave a lower height
// at some vertices and create isolated impassable slopes.  Recalculate only
// vertices touched by this mod and retain the highest completed surface.
internal sealed class CubeVehicleSurfaceCoordinator : IDisposable {
 readonly IEntitiesManager m_entities;
 readonly IConstructionManager m_construction;
 readonly ISimLoopEvents m_sim;
 readonly Dict<Tile2i,VehicleSurfaceProvider.SurfaceHeights> m_providerHeights;
 readonly Event<Tile2i> m_changed;
 readonly HashSet<Tile2i> m_previousTouched=new HashSet<Tile2i>();
 bool m_dirty=true;

 public CubeVehicleSurfaceCoordinator(DependencyResolver resolver){
  m_entities=resolver.Resolve<IEntitiesManager>();
  m_construction=resolver.Resolve<IConstructionManager>();
  m_sim=resolver.Resolve<ISimLoopEvents>();
  var provider=resolver.Resolve<VehicleSurfaceProvider>();
  var heightsField=typeof(VehicleSurfaceProvider).GetField("m_surfaceHeights",BindingFlags.Instance|BindingFlags.NonPublic);
  var changedField=typeof(VehicleSurfaceProvider).GetField("m_onVehicleSurfaceChanged",BindingFlags.Instance|BindingFlags.NonPublic);
  m_providerHeights=(Dict<Tile2i,VehicleSurfaceProvider.SurfaceHeights>)heightsField.GetValue(provider);
  m_changed=(Event<Tile2i>)changedField.GetValue(provider);
  m_entities.StaticEntityAdded.AddNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_entities.StaticEntityRemoved.AddNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_construction.EntityConstructed.AddNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_construction.EntityStartedDeconstruction.AddNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_construction.EntityConstructionStateChanged.AddNonSaveable<CubeVehicleSurfaceCoordinator>(this,onConstructionStateChanged);
  m_sim.Update.AddNonSaveable<CubeVehicleSurfaceCoordinator>(this,onUpdate);
 }

 static bool IsOurProto(StaticEntityProto.ID id){
  return id==ConcreteRetainingCubeMod.CubeId||id==ConcreteRetainingCubeMod.WeatheredCubeId||
   id==ConcreteRetainingCubeMod.RailStraightId||id==ConcreteRetainingCubeMod.RailCornerId||
   id==ConcreteRetainingCubeMod.RailEndId||id==ConcreteRetainingCubeMod.RailTId||
   id==ConcreteRetainingCubeMod.Slope1Id||id==ConcreteRetainingCubeMod.Slope1WeatheredId||
   id==ConcreteRetainingCubeMod.Slope2Id||id==ConcreteRetainingCubeMod.Slope2WeatheredId||
   id==ConcreteRetainingCubeMod.Slope3Id||id==ConcreteRetainingCubeMod.Slope3WeatheredId;
 }
 static bool IsSurfaceActive(IStaticEntity entity){
  return entity.ConstructionState>=ConstructionState.Constructed&&entity.ConstructionState<=ConstructionState.PendingDeconstruction;
 }
 void onEntityChanged(IStaticEntity entity){if(IsOurProto(entity.Prototype.Id))m_dirty=true;}
 void onConstructionStateChanged(IStaticEntity entity,ConstructionState state){if(IsOurProto(entity.Prototype.Id))m_dirty=true;}
 void onUpdate(){if(!m_dirty)return;m_dirty=false;recalculate();}

 void recalculate(){
  var modTouched=new HashSet<Tile2i>();
  foreach(var entity in m_entities.GetAllEntitiesOfType<IStaticEntity>()){
   if(!IsOurProto(entity.Prototype.Id))continue;
   foreach(var surface in entity.VehicleSurfaceHeights)modTouched.Add(surface.Key);
  }
  var touched=new HashSet<Tile2i>(m_previousTouched);
  foreach(var coord in modTouched)touched.Add(coord);
  if(touched.Count==0)return;

  var maximaAll=new Dictionary<Tile2i,HeightTilesF>();
  var maximaCompleted=new Dictionary<Tile2i,HeightTilesF>();
  var totals=new Dictionary<Tile2i,int>();
  var completed=new Dictionary<Tile2i,int>();
  foreach(var entity in m_entities.GetAllEntitiesOfType<IStaticEntity>()){
   foreach(var surface in entity.VehicleSurfaceHeights){
    if(!touched.Contains(surface.Key))continue;
    int count;
    totals.TryGetValue(surface.Key,out count);
    totals[surface.Key]=count+1;
    HeightTilesF current;
    if(!maximaAll.TryGetValue(surface.Key,out current)||surface.Value>current)maximaAll[surface.Key]=surface.Value;
    if(IsSurfaceActive(entity)){
     completed.TryGetValue(surface.Key,out count);
     completed[surface.Key]=count+1;
     if(!maximaCompleted.TryGetValue(surface.Key,out current)||surface.Value>current)maximaCompleted[surface.Key]=surface.Value;
    }
   }
  }
  foreach(var coord in touched){
    VehicleSurfaceProvider.SurfaceHeights state;
   if(!m_providerHeights.TryGetValue(coord,out state))continue;
   int totalValue;
   totals.TryGetValue(coord,out totalValue);
    int doneValue;
   completed.TryGetValue(coord,out doneValue);
   HeightTilesF height;
   sbyte total;
   sbyte done;
   if(modTouched.Contains(coord)&&doneValue>0){
    // A stack of blocks is one physical top surface, not N overlapping
    // surfaces. Keep the highest completed contribution and expose it as one
    // completed vanilla surface. Unfinished blocks remain blocked by their
    // construction occupancy, without invalidating adjacent finished blocks.
    height=maximaCompleted[coord];
    total=1;
    done=1;
   }else{
    if(totalValue<=0)continue;
    height=maximaAll[coord];
    total=(sbyte)Math.Min(sbyte.MaxValue,totalValue);
    done=(sbyte)Math.Min(sbyte.MaxValue,doneValue);
   }
   if(state.Height==height&&state.Count==total&&state.ConstructedCount==done)continue;
   state.Height=height;
   state.Count=total;
   state.ConstructedCount=done;
   m_providerHeights[coord]=state;
   m_changed.Invoke(coord);
  }
  m_previousTouched.Clear();
  foreach(var coord in modTouched)m_previousTouched.Add(coord);
 }

 public void Dispose(){
  m_entities.StaticEntityAdded.RemoveNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_entities.StaticEntityRemoved.RemoveNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_construction.EntityConstructed.RemoveNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_construction.EntityStartedDeconstruction.RemoveNonSaveable<CubeVehicleSurfaceCoordinator>(this,onEntityChanged);
  m_construction.EntityConstructionStateChanged.RemoveNonSaveable<CubeVehicleSurfaceCoordinator>(this,onConstructionStateChanged);
  m_sim.Update.RemoveNonSaveable<CubeVehicleSurfaceCoordinator>(this,onUpdate);
 }
}

internal static class CubeAssets {
 public static void RegisterAll(AssetsDb db){
  Add(db,ConcreteRetainingCubeMod.WeatheredIconPath,LoadTexture("ConcreteRetainingCube.WeatheredIcon.png","Weathered concrete cube icon"));
  Add(db,ConcreteRetainingCubeMod.RailStraightIconPath,LoadTexture("ConcreteRetainingCube.RailStraightIcon.png","Weathered concrete cube straight railing icon"));
  Add(db,ConcreteRetainingCubeMod.RailCornerIconPath,LoadTexture("ConcreteRetainingCube.RailCornerIcon.png","Weathered concrete cube corner railing icon"));
  Add(db,ConcreteRetainingCubeMod.RailEndIconPath,LoadTexture("ConcreteRetainingCube.RailEndIcon.png","Weathered concrete cube railing end icon"));
  Add(db,ConcreteRetainingCubeMod.RailTIconPath,LoadTexture("ConcreteRetainingCube.RailTIcon.png","Weathered concrete cube T railing icon"));
  Add(db,ConcreteRetainingCubeMod.Slope1WeatheredIconPath,LoadTexture("ConcreteRetainingCube.Slope1WeatheredIcon.png","Weathered concrete ramp cube 1/3 icon"));
  Add(db,ConcreteRetainingCubeMod.Slope2WeatheredIconPath,LoadTexture("ConcreteRetainingCube.Slope2WeatheredIcon.png","Weathered concrete ramp cube 2/3 icon"));
  Add(db,ConcreteRetainingCubeMod.Slope3WeatheredIconPath,LoadTexture("ConcreteRetainingCube.Slope3WeatheredIcon.png","Weathered concrete ramp cube 3/3 icon"));
  Add(db,ConcreteRetainingCubeMod.WeatheredPrefabPath,CreatePrefab(true));
  Add(db,ConcreteRetainingCubeMod.Slope1WeatheredPrefabPath,CreateSlopePrefab(true,0f,1f/3f,"Weathered concrete ramp cube 1/3"));
  Add(db,ConcreteRetainingCubeMod.Slope2WeatheredPrefabPath,CreateSlopePrefab(true,1f/3f,2f/3f,"Weathered concrete ramp cube 2/3"));
  Add(db,ConcreteRetainingCubeMod.Slope3WeatheredPrefabPath,CreateSlopePrefab(true,2f/3f,1f,"Weathered concrete ramp cube 3/3"));
  Add(db,ConcreteRetainingCubeMod.RailStraightPrefabPath,CreateRailingPrefab(RailingShape.Straight,"Weathered cube - straight railing"));
  Add(db,ConcreteRetainingCubeMod.RailCornerPrefabPath,CreateRailingPrefab(RailingShape.Corner,"Weathered cube - corner railing"));
  Add(db,ConcreteRetainingCubeMod.RailEndPrefabPath,CreateRailingPrefab(RailingShape.End,"Weathered cube - railing end"));
  Add(db,ConcreteRetainingCubeMod.RailTPrefabPath,CreateRailingPrefab(RailingShape.T,"Weathered cube - T railing"));
 }

 static void Add(AssetsDb db,string path,UnityEngine.Object asset){
  if(db.ContainsAsset(path))return;
  var bundleField=typeof(AssetsDb).GetField("m_bundleLoader",BindingFlags.Instance|BindingFlags.NonPublic);
  var bundle=bundleField.GetValue(db);
  var loadedField=bundle.GetType().GetField("m_loadedAssets",BindingFlags.Instance|BindingFlags.NonPublic);
  var dict=loadedField.GetValue(bundle);
  MethodInfo add=null;
  foreach(var method in dict.GetType().GetMethods())if(method.Name=="Add"&&method.GetParameters().Length==2){add=method;break;}
  if(add==null)throw new MissingMethodException("AssetsDb asset dictionary Add method was not found.");
  add.Invoke(dict,new object[]{path,asset});
 }

 static GameObject CreatePrefab(bool weathered){
  var root=new GameObject(weathered?"Weathered concrete retaining cube":"Concrete retaining cube");
  var meshGo=new GameObject("Cube mesh");
  meshGo.transform.SetParent(root.transform,false);
  meshGo.transform.localPosition=new Vector3(0f,1f,0f);
  meshGo.AddComponent<MeshFilter>().sharedMesh=CreateMesh();
  meshGo.AddComponent<MeshRenderer>().sharedMaterial=CreateMaterial(weathered);
  root.SetActive(false);
  return root;
 }

 static GameObject CreateSlopePrefab(bool weathered,float low,float high,string name){
  var root=new GameObject(name);
  var meshGo=new GameObject("Ramp cube mesh");
  meshGo.transform.SetParent(root.transform,false);
  meshGo.transform.localPosition=new Vector3(0f,1f,0f);
  meshGo.AddComponent<MeshFilter>().sharedMesh=CreateSlopeMesh(low,high);
  meshGo.AddComponent<MeshRenderer>().sharedMaterial=CreateMaterial(weathered);
  root.SetActive(false);
  return root;
 }

 enum RailingShape{Straight,Corner,End,T}

 static GameObject CreateRailingPrefab(RailingShape shape,string name){
  var root=CreatePrefab(true);root.name=name;
  var railGo=new GameObject("Centered safety railing");
  railGo.transform.SetParent(root.transform,false);
  railGo.AddComponent<MeshFilter>().sharedMesh=CreateRailingMesh(shape);
  railGo.AddComponent<MeshRenderer>().sharedMaterial=CreateRailingMaterial();
  return root;
 }

 static Material CreateRailingMaterial(){
  var material=new Material(Shader.Find("Standard"));
  material.name="Dark mustard safety railing";
  var texture=new Texture2D(4,4,TextureFormat.RGBA32,false);texture.name="Dark mustard railing albedo";
  var pixels=new Color[16];
  for(int i=0;i<pixels.Length;i++)pixels[i]=new Color(0.72f,0.54f,0.025f,1f);
  texture.SetPixels(pixels);texture.Apply(false,true);texture.wrapMode=TextureWrapMode.Repeat;texture.filterMode=FilterMode.Bilinear;
  material.mainTexture=texture;material.color=Color.white;
  material.SetFloat("_Metallic",0.18f);material.SetFloat("_Glossiness",0.12f);
  return material;
 }

 static Mesh CreateRailingMesh(RailingShape shape){
  var vertices=new List<Vector3>();var triangles=new List<int>();
  Action<Vector3,Vector3> box=(min,max)=>AddBox(vertices,triangles,min,max);
  var nodes=new List<Vector3>();var segments=new List<Vector3[]>();
  Action<float,float,float,float> segment=(x0,z0,x1,z1)=>{
   var a=new Vector3(x0,0f,z0);var b=new Vector3(x1,0f,z1);
   segments.Add(new[]{a,b});
   if(!nodes.Contains(a))nodes.Add(a);if(!nodes.Contains(b))nodes.Add(b);
  };
  if(shape==RailingShape.Straight)segment(-1f,0f,1f,0f);
  else if(shape==RailingShape.Corner){segment(-1f,0f,0f,0f);segment(0f,0f,0f,1f);}
  else if(shape==RailingShape.End)segment(-1f,0f,0f,0f);
  else {segment(-1f,0f,1f,0f);segment(0f,0f,0f,1f);}
  // 25% lower than v0.4.9 and 50% thicker in both profile axes.
  const float baseY=2.02f,postTop=2.815f,halfPost=0.0825f,halfRail=0.0675f;
  foreach(var node in nodes)box(new Vector3(node.x-halfPost,baseY,node.z-halfPost),new Vector3(node.x+halfPost,postTop,node.z+halfPost));
  foreach(var s in segments){
   bool alongX=Math.Abs(s[1].x-s[0].x)>0.01f;
   float minX=Math.Min(s[0].x,s[1].x),maxX=Math.Max(s[0].x,s[1].x);
   float minZ=Math.Min(s[0].z,s[1].z),maxZ=Math.Max(s[0].z,s[1].z);
   if(alongX){minZ-=halfRail;maxZ+=halfRail;}else{minX-=halfRail;maxX+=halfRail;}
   box(new Vector3(minX,postTop-0.135f,minZ),new Vector3(maxX,postTop,maxZ));
   box(new Vector3(minX,baseY+0.3675f,minZ),new Vector3(maxX,baseY+0.5025f,maxZ));
  }
  var mesh=new Mesh();mesh.name="Centered modular safety railing";mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;
 }

 static void AddBox(List<Vector3> vertices,List<int> triangles,Vector3 min,Vector3 max){
  int n=vertices.Count;
  vertices.Add(new Vector3(min.x,min.y,min.z));vertices.Add(new Vector3(max.x,min.y,min.z));
  vertices.Add(new Vector3(max.x,max.y,min.z));vertices.Add(new Vector3(min.x,max.y,min.z));
  vertices.Add(new Vector3(min.x,min.y,max.z));vertices.Add(new Vector3(max.x,min.y,max.z));
  vertices.Add(new Vector3(max.x,max.y,max.z));vertices.Add(new Vector3(min.x,max.y,max.z));
  int[] inds={0,2,1,0,3,2,4,5,6,4,6,7,0,1,5,0,5,4,3,7,6,3,6,2,1,2,6,1,6,5,0,4,7,0,7,3};
  for(int i=0;i<inds.Length;i++)triangles.Add(n+inds[i]);
 }

 static Material CreateMaterial(bool weathered){
  var material=new Material(Shader.Find("Standard"));
  material.name=weathered?"Weathered concrete cube":"Concrete cube";
  var albedo=LoadTexture(weathered?"ConcreteRetainingCube.WeatheredAlbedo.png":"ConcreteRetainingCube.Albedo.png","Concrete cube albedo");
  albedo.anisoLevel=8;albedo.mipMapBias=-0.35f;material.mainTexture=albedo;
  var normal=LoadTexture("ConcreteRetainingCube.Normal.png","Concrete cube normal");
  material.SetTexture("_BumpMap",normal);material.SetFloat("_BumpScale",weathered?0.58f:0.42f);material.EnableKeyword("_NORMALMAP");
  if(weathered)material.color=Color.white;
  material.SetFloat("_Glossiness",weathered?0.0f:0.18f);material.SetFloat("_Smoothness",weathered?0.0f:0.18f);material.SetFloat("_Metallic",0f);
  if(weathered)material.SetColor("_SpecColor",new Color(0.015f,0.015f,0.015f,1f));
  return material;
 }

 static Texture2D LoadTexture(string resource,string name){
  using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream(resource)){
   if(stream==null)throw new InvalidOperationException("Missing embedded resource: "+resource);
   var data=new byte[stream.Length];int offset=0,read;
   while(offset<data.Length&&(read=stream.Read(data,offset,data.Length-offset))>0)offset+=read;
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,true);texture.name=name;
   if(!ImageConversion.LoadImage(texture,data,false))throw new InvalidOperationException("Invalid embedded texture: "+resource);
   texture.wrapMode=TextureWrapMode.Clamp;texture.filterMode=FilterMode.Trilinear;return texture;
  }
 }

 static Mesh CreateMesh(){
  var vertices=new List<Vector3>();var uvs=new List<Vector2>();var triangles=new List<int>();
  Action<Vector3,Vector3,Vector3,Vector3,int> face=(a,b,c,d,cell)=>{
   int n=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);vertices.Add(d);
   float u0=(cell%3)/3f,v0=(cell/3)/2f,u1=u0+1f/3f,v1=v0+1f/2f;
   uvs.Add(new Vector2(u0,v0));uvs.Add(new Vector2(u1,v0));uvs.Add(new Vector2(u1,v1));uvs.Add(new Vector2(u0,v1));
   triangles.Add(n);triangles.Add(n+1);triangles.Add(n+2);triangles.Add(n);triangles.Add(n+2);triangles.Add(n+3);
  };
  face(new Vector3(-1,-1,1),new Vector3(1,-1,1),new Vector3(1,1,1),new Vector3(-1,1,1),3);
  face(new Vector3(1,-1,-1),new Vector3(-1,-1,-1),new Vector3(-1,1,-1),new Vector3(1,1,-1),4);
  face(new Vector3(1,-1,1),new Vector3(1,-1,-1),new Vector3(1,1,-1),new Vector3(1,1,1),0);
  face(new Vector3(-1,-1,-1),new Vector3(-1,-1,1),new Vector3(-1,1,1),new Vector3(-1,1,-1),5);
  face(new Vector3(-1,1,1),new Vector3(1,1,1),new Vector3(1,1,-1),new Vector3(-1,1,-1),1);
  face(new Vector3(-1,-1,-1),new Vector3(1,-1,-1),new Vector3(1,-1,1),new Vector3(-1,-1,1),2);
  var mesh=new Mesh();mesh.name="Concrete cube";mesh.SetVertices(vertices);mesh.SetUVs(0,uvs);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;
 }

 static Mesh CreateSlopeMesh(float low,float high){
  float yLow=-1f+2f*low,yHigh=-1f+2f*high;
  var vertices=new List<Vector3>();var uvs=new List<Vector2>();var triangles=new List<int>();
  Action<Vector3,Vector3,Vector3,Vector3,int> face=(a,b,c,d,cell)=>{
   int n=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);vertices.Add(d);
   float u0=(cell%3)/3f,v0=(cell/3)/2f,u1=u0+1f/3f,v1=v0+1f/2f;
   uvs.Add(new Vector2(u0,v0));uvs.Add(new Vector2(u1,v0));uvs.Add(new Vector2(u1,v1));uvs.Add(new Vector2(u0,v1));
   triangles.Add(n);triangles.Add(n+1);triangles.Add(n+2);triangles.Add(n);triangles.Add(n+2);triangles.Add(n+3);
  };
  face(new Vector3(-1,-1,1),new Vector3(1,-1,1),new Vector3(1,yHigh,1),new Vector3(-1,yLow,1),3);
  face(new Vector3(1,-1,-1),new Vector3(-1,-1,-1),new Vector3(-1,yLow,-1),new Vector3(1,yHigh,-1),4);
  face(new Vector3(1,-1,1),new Vector3(1,-1,-1),new Vector3(1,yHigh,-1),new Vector3(1,yHigh,1),0);
  face(new Vector3(-1,-1,-1),new Vector3(-1,-1,1),new Vector3(-1,yLow,1),new Vector3(-1,yLow,-1),5);
  face(new Vector3(-1,yLow,1),new Vector3(1,yHigh,1),new Vector3(1,yHigh,-1),new Vector3(-1,yLow,-1),1);
  face(new Vector3(-1,-1,-1),new Vector3(1,-1,-1),new Vector3(1,-1,1),new Vector3(-1,-1,1),2);
  var mesh=new Mesh();mesh.name="Concrete ramp cube";mesh.SetVertices(vertices);mesh.SetUVs(0,uvs);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;
 }
}
}
