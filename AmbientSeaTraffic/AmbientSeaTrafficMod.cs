using System;
using System.Collections.Generic;
using Mafi;
using Mafi.Base;
using Mafi.Collections;
using Mafi.Core;
using Mafi.Core.Buildings;
using Mafi.Core.Buildings.Cargo.Ships;
using Mafi.Core.Entities;
using Mafi.Core.Entities.Ships;
using Mafi.Core.Game;
using Mafi.Core.GameLoop;
using Mafi.Core.Maintenance;
using Mafi.Core.Mods;
using Mafi.Core.PathFinding;
using Mafi.Core.Prototypes;
using Mafi.Core.Simulation;
using Mafi.Core.Terrain;
using Mafi.Core.Vehicles;
using Mafi.Unity;
using Mafi.Unity.Entities;
using UnityEngine;
using CoreEntityId=Mafi.Core.EntityId;

namespace AmbientSeaTraffic {
public sealed class AmbientSeaTrafficMod : IMod,IDisposable {
 public ModManifest Manifest{get;private set;} public bool IsUiOnly=>false;
 [Obsolete] public Option<IConfig> ModConfig{get;set;} public ModJsonConfig JsonConfig{get;private set;}
 static AmbientSeaTrafficMod instance; DependencyResolver resolver; ISimLoopEvents sim; IGameLoopEvents loop; EntitiesManager entities; CoreEntityId.Factory ids; ProtosDb protos; TerrainManager terrain;
 readonly List<Traffic> traffic=new List<Traffic>();readonly List<PendingShip> pending=new List<PendingShip>();readonly System.Random random=new System.Random();int nextAutoDay;bool enabled; ICalendar calendar;
 public AmbientSeaTrafficMod(ModManifest manifest){Manifest=manifest;JsonConfig=new ModJsonConfig(this);}
 public void RegisterPrototypes(ProtoRegistrator r){} public void RegisterDependencies(DependencyResolverBuilder b,ProtosDb p,bool loaded){b.RegisterDependency<AmbientShipMbFactory>().AsAllInterfaces(false);} public void MigrateJsonConfig(VersionSlim v,Dict<string,object> c){} public void EarlyInit(DependencyResolver r){}
 public void Initialize(DependencyResolver r,bool loaded){instance=this;resolver=r;sim=r.Resolve<ISimLoopEvents>();loop=r.Resolve<IGameLoopEvents>();entities=r.Resolve<EntitiesManager>();ids=r.Resolve<CoreEntityId.Factory>();protos=r.Resolve<ProtosDb>();terrain=r.Resolve<TerrainManager>();calendar=r.Resolve<ICalendar>();loop.RegisterInitState(this,OnInit);}
 void OnInit(){enabled=JsonConfig.GetBool("enabled",true);ScheduleNextAutomatic();sim.Update.AddNonSaveable<AmbientSeaTrafficMod>(this,OnSim);sim.BeforeSave.AddNonSaveable<AmbientSeaTrafficMod>(this,OnBeforeSave);Log.Info("AmbientSeaTraffic initialized.");}
 void OnSim(){if(enabled&&calendar.CurrentDate.Value>=nextAutoDay&&traffic.Count+pending.Count<6){int count=Math.Min(random.Next(1,4),6-traffic.Count-pending.Count);var kinds=new int[count];for(int i=0;i<count;i++)kinds[i]=RandomKind();QueueGroup(kinds);ScheduleNextAutomatic();}for(int i=pending.Count-1;i>=0;i--){var p=pending[i];if(--p.Delay<=0){Spawn(p.Kind,p.Start,p.Target);pending.RemoveAt(i);}}for(int i=traffic.Count-1;i>=0;i--){var x=traffic[i];x.Ticks++;if(x.Ship==null){traffic.RemoveAt(i);continue;}if(x.Phase==0&&x.Ticks>30&&!x.Ship.HasTrueJob){if(x.Ship.NavigatedSuccessfully){x.Ship.LeaveToWorld();x.Phase=1;x.Ticks=0;}else{Remove(x);traffic.RemoveAt(i);}}else if(x.Phase==1&&x.Ship.IsAtWorld){Remove(x);traffic.RemoveAt(i);}}}
 void ScheduleNextAutomatic(){nextAutoDay=calendar.CurrentDate.Value+random.Next(60,91);}int RandomKind(){int r=random.Next(100);if(r<35)return random.Next(1,4);return new[]{0,4,5,6,7,8,9}[random.Next(7)];}
 void QueueGroup(int[] kinds){Tile2i start,target;if(!TryRoute(out start,out target)){Log.Warning("AmbientSeaTraffic: no ocean edge pair found.");return;}RelTile2i delta=target-start;RelTile2i side=Math.Abs(delta.X)>Math.Abs(delta.Y)?new RelTile2i(0,1):new RelTile2i(1,0);for(int i=0;i<kinds.Length;i++){int lane=(i-(kinds.Length-1)/2)*8;Tile2i s=start+side*lane,t=target+side*lane;if(!IsSafeOcean(s)||!IsSafeOcean(t)){s=start;t=target;}pending.Add(new PendingShip(kinds[i],s,t,i*12.Seconds().Ticks));}}
 bool TryRoute(out Tile2i start,out Tile2i target){int margin=9;for(int n=0;n<160;n++){int edge=random.Next(4),v;if(edge<2){v=(random.Next(margin,terrain.TerrainHeight-margin)/4)*4;start=new Tile2i(edge==0?margin:terrain.TerrainWidth-margin,v);target=new Tile2i(edge==0?terrain.TerrainWidth-margin:margin,v);}else{v=(random.Next(margin,terrain.TerrainWidth-margin)/4)*4;start=new Tile2i(v,edge==2?margin:terrain.TerrainHeight-margin);target=new Tile2i(v,edge==2?terrain.TerrainHeight-margin:margin);}if(IsSafeOcean(start)&&IsSafeOcean(target))return true;}start=target=Tile2i.Zero;return false;}
 bool IsSafeOcean(Tile2i p){for(int x=-8;x<=8;x+=4)for(int y=-8;y<=8;y+=4){var q=p+new RelTile2i(x,y);if(q.X<0||q.Y<0||q.X>=terrain.TerrainWidth||q.Y>=terrain.TerrainHeight||!terrain.IsOcean(q))return false;}return true;}
 void Spawn(int kind,Tile2i start,Tile2i target){try{var proto=protos.GetOrThrow<CargoShipProto>(Ids.Ships.CargoShipT1);var ship=new AmbientShip(ids.GetNextId(),proto,kind,Resolve<EntityContext>(),terrain,Resolve<ShipsPathFindingManager>(),Resolve<VehiclesManager>(),Resolve<ShipSurfaceProvider>(),Resolve<ShipJobsContext>(),Resolve<IEntityMaintenanceProvidersFactory>(),Resolve<ShipsClearancePathabilityProvider>());entities.AddEntityNoChecks(ship,EntityAddReason.New);AngleDegrees1f direction=new RelTile2f(target.X-start.X,target.Y-start.Y).Angle;ship.Spawn(start.CenterTile2f,direction);ship.GoTo(target);traffic.Add(new Traffic(ship,kind));Log.Info("AmbientSeaTraffic spawned kind="+kind+" from="+start+" target="+target);}catch(Exception ex){Log.Error("AmbientSeaTraffic spawn failed: "+ex);}}
 T Resolve<T>() where T:class{return resolver.Resolve<T>();}
 void Remove(Traffic x){if(x.Ship!=null)try{entities.TryRemoveAndDestroyEntityNoChecks(x.Ship,EntityRemoveReason.Remove);}catch{}x.Ship=null;x.DestroyVisual=true;}
 void OnBeforeSave(){pending.Clear();for(int i=traffic.Count-1;i>=0;i--)Remove(traffic[i]);traffic.Clear();}
 public void Dispose(){OnBeforeSave();if(instance==this)instance=null;}
 sealed class Traffic{public AmbientShip Ship;public int Kind,Phase,Ticks;public bool DestroyVisual;public Traffic(AmbientShip s,int k){Ship=s;Kind=k;}public void UpdateVisualRequest(){if(DestroyVisual)DestroyVisual=false;}}
 sealed class PendingShip{public int Kind,Delay;public Tile2i Start,Target;public PendingShip(int k,Tile2i s,Tile2i t,int d){Kind=k;Start=s;Target=t;Delay=d;}}
}

public sealed class AmbientShip:Ship {
 public readonly int VisualKind; public override Option<IDockEntity> AssignedDockEntity=>Option<IDockEntity>.None;
 public AmbientShip(CoreEntityId id,CargoShipProto proto,int kind,EntityContext context,TerrainManager terrain,ShipsPathFindingManager paths,VehiclesManager vehicles,ShipSurfaceProvider surface,ShipJobsContext jobs,IEntityMaintenanceProvidersFactory maintenance,ShipsClearancePathabilityProvider pathability):base(id,proto,context,terrain,paths,vehicles,surface,jobs,maintenance,pathability){VisualKind=kind;}
}

public sealed class AmbientShipMb: Mafi.Unity.Entities.Ships.ShipMb {
 AmbientShip entity; Mafi.Unity.Utils.OptionalParticlesWrapper engineParticles,exhaustParticles; ParticleSystem[] vanillaParticles; bool particlesRunning;
 public void Init(AmbientShip e,Mafi.Unity.Audio.EntityAudioManager audio,Mafi.Core.Environment.WeatherManager weather,IRandom rng,GameObject effectRoot){entity=e;Initialize(e,audio,weather,rng);engineParticles=new Mafi.Unity.Utils.OptionalParticlesWrapper(effectRoot,"RearEngineParticles",true);exhaustParticles=new Mafi.Unity.Utils.OptionalParticlesWrapper(effectRoot,"RearExhaustParticles",true);SetRearEngineParticles(engineParticles);SetRearExhaustParticles(exhaustParticles);vanillaParticles=effectRoot.GetComponentsInChildren<ParticleSystem>(true);Log.Info("AmbientSeaTraffic vanilla particle systems: "+vanillaParticles.Length);SetVanillaParticles(false);}
 public override void RenderUpdateAfterSync(GameTime time){base.RenderUpdateAfterSync(time);SetVanillaParticles(entity!=null&&entity.IsMoving&&entity.IsSpawned&&!entity.IsAtWorld);}
 void SetVanillaParticles(bool enabled){if(vanillaParticles==null||particlesRunning==enabled)return;particlesRunning=enabled;foreach(var ps in vanillaParticles){if(ps==null)continue;ps.gameObject.SetActive(true);var emission=ps.emission;emission.enabled=enabled;if(enabled){if(!ps.isPlaying)ps.Play(true);}else ps.Stop(true,ParticleSystemStopBehavior.StopEmitting);}}
}

public sealed class AmbientShipMbFactory:IEntityMbFactory<AmbientShip>,IFactory<AmbientShip,EntityMb>{
 readonly AssetsDb assets;readonly Mafi.Unity.Audio.EntityAudioManager audio;readonly Mafi.Core.Environment.WeatherManager weather;readonly IRandom rng;
 public AmbientShipMbFactory(AssetsDb a,Mafi.Unity.Audio.EntityAudioManager au,Mafi.Core.Environment.WeatherManager w,RandomProvider random){assets=a;audio=au;weather=w;rng=random.GetNonSimRandomFor(this,"ambient sea traffic");}
 public EntityMb Create(AmbientShip e){var root=new GameObject("AmbientSeaTraffic.Ship");GameObject effects=root;if(e.VisualKind==1||e.VisualKind==2||e.VisualKind==3){var g=assets.GetClonedPrefabOrEmptyGo("Assets/Base/Ships/BattleShip/BattleShipT"+e.VisualKind+".prefab");g.transform.SetParent(root.transform,false);SetLayer(g);effects=g;}else{string module=e.VisualKind==4?"CargoShipModule_Countable":e.VisualKind==5?"CargoShipModule_Loose":e.VisualKind==6?"CargoShipModule_Liquid":e.VisualKind==7?"CargoShipModule_Countable":e.VisualKind==8?"CargoShipModule_Gas":"CargoShipModule_Empty";string front=e.VisualKind==9?"HydrogenFront":"CargoShip_Front",rear=e.VisualKind==9?"HydrogenRear":"CargoShip_Rear";string[] p={"Assets/Base/Ships/CargoShip/"+front+".prefab","Assets/Base/Ships/CargoShip/"+module+".prefab","Assets/Base/Ships/CargoShip/"+rear+".prefab"};float[] x={5,0,-5};for(int i=0;i<3;i++){var g=assets.GetClonedPrefabOrEmptyGo(p[i]);g.transform.SetParent(root.transform,false);g.transform.localPosition=new Vector3(x[i],0,0);SetLayer(g);if(i==2)effects=g;}if(e.VisualKind==4)AddContainers(root);}SetLayer(root);var mb=root.AddComponent<AmbientShipMb>();mb.Init(e,audio,weather,rng,effects);return mb;}
 void AddContainers(GameObject root){Vector3[] positions={new Vector3(-2.2f,.35f,-1.6f),new Vector3(-2.2f,.35f,1.6f),new Vector3(0,.35f,-1.6f),new Vector3(0,.35f,1.6f),new Vector3(2.2f,.35f,-1.6f),new Vector3(2.2f,.35f,1.6f)};foreach(var pos in positions){var c=assets.GetClonedPrefabOrEmptyGo("Assets/Base/Ships/CargoShip/CargoShip_Container.prefab");c.transform.SetParent(root.transform,false);c.transform.localPosition=pos;SetLayer(c);}}
 static void SetLayer(GameObject g){g.layer=2;foreach(Transform c in g.transform)SetLayer(c.gameObject);}
}

}
