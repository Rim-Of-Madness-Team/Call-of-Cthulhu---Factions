using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolverAgency : SymbolResolver
    {
        
        public static Map instanceMap = null;
        public static Faction instanceFaction = null;
        public static IntVec3 tempIntVec = IntVec3.Invalid;
        public static Thing newThing = null;

        private static List<Thing> tmpThingsToDestroy = new List<Thing>();

        public virtual ThingDef defaultWallDef
        {
            get
            {
                return ThingDefOf.Steel;
            }
        }

        public virtual TerrainDef defaultFloorDef
        {
            get
            {
                return TerrainDefOf.MetalTile;
            }
        }

        public override void Resolve(ResolveParams rp)
        {
            CellRect.CellRectIterator iterator = rp.rect.ContractedBy(1).GetIterator();
            while (!iterator.Done())
            {
                if (rp.clearEdificeOnly.HasValue && rp.clearEdificeOnly.Value)
                {
                    Building edifice = iterator.Current.GetEdifice(BaseGen.globalSettings.map);
                    if (edifice != null && edifice.def.destroyable)
                    {
                        if (edifice.def != CthulhuFactionsDefOf.ROM_TemporaryDoorMarker)
                        edifice.Destroy(DestroyMode.Vanish);
                    }
                }
                else
                {
                    SymbolResolverAgency.tmpThingsToDestroy.Clear();
                    SymbolResolverAgency.tmpThingsToDestroy.AddRange(iterator.Current.GetThingList(BaseGen.globalSettings.map));
                    for (int i = 0; i < SymbolResolverAgency.tmpThingsToDestroy.Count; i++)
                    {
                        if (SymbolResolverAgency.tmpThingsToDestroy[i].def.destroyable)
                        {
                            if (SymbolResolverAgency.tmpThingsToDestroy[i].def != CthulhuFactionsDefOf.ROM_TemporaryDoorMarker)
                                SymbolResolverAgency.tmpThingsToDestroy[i].Destroy(DestroyMode.Vanish);
                        }
                    }
                }
                iterator.MoveNext();
            }

            foreach (IntVec3 cell in rp.rect.Cells)
            {
                ResolveParams resolveParams3 = rp;
                resolveParams3.rect = CellRect.SingleCell(cell);
                resolveParams3.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryRegionBarrier;
                BaseGen.symbolStack.Push("thing", resolveParams3);
            }

            PassingParameters(rp);
            instanceMap = map;
            instanceFaction = rp.faction ?? Find.FactionManager.FirstFactionOfDef(FactionDef.Named("TheAgency"));

            //ResolveParams resolveParams4 = rp;
            //BaseGen.symbolStack.Push("clear", resolveParams4);

            UnfogRoomCenter(rp.rect.CenterCell);

            LongEventHandler.QueueLongEvent(delegate
            {
                if (SymbolResolverAgency.instanceMap == null) return;
                foreach (IntVec3 cell in SymbolResolverAgency.instanceMap.AllCells)
                {
                    Thing thing = cell.GetThingList(instanceMap).Find((Thing x) => x.def == CthulhuFactionsDefOf.ROM_TemporaryDoorMarker);
                    if (thing == null)
                    {
                        //Log.Warning("Could not destroy temporary region barrier at " + cell + " because it's not here.");
                    }
                    else
                    {
                        if (thing.def == CthulhuFactionsDefOf.ROM_TemporaryDoorMarker)
                        {
                            SymbolResolverAgency.newThing = null;
                            SymbolResolverAgency.newThing = ThingMaker.MakeThing(ThingDefOf.Door, ThingDefOf.Steel);
                            SymbolResolverAgency.newThing.SetFaction(SymbolResolverAgency.instanceFaction, null);
                            //newThing.Rotation = thing.Rotation;
                            tempIntVec = new IntVec3(cell.x, 0, cell.z);
                            GenSpawn.Spawn(SymbolResolverAgency.newThing, SymbolResolverAgency.tempIntVec, SymbolResolverAgency.instanceMap);
                        }
                        thing.Destroy(DestroyMode.Vanish);
                    }
                }
                foreach (Pawn pawn in SymbolResolverAgency.instanceMap.mapPawns.AllPawnsSpawned.FindAll((Pawn x) => x != null && x.Spawned && x.RaceProps.intelligence >= Intelligence.ToolUser && !x.IsColonist && x.Faction != instanceFaction && x.guest != null && SymbolResolver_FillCells.IsCosmicHorrorFaction(x.Faction) == false))
                {
                    pawn.guest.SetGuestStatus(instanceFaction, true);
                }
                foreach (Building_Bed bed in SymbolResolverAgency.instanceMap.listerThings.AllThings.FindAll((Thing x) => x != null && x is Building_Bed && x.def == ThingDefOf.SleepingSpot && x.Faction == instanceFaction))
                {
                    bed.ForPrisoners = true;
                }

            }, "doorResolver", true, null);
        }


        private void UnfogRoomCenter(IntVec3 centerCell)
        {
            if (Current.ProgramState != ProgramState.MapInitializing)
            {
                return;
            }
            List<IntVec3> rootsToUnfog = MapGenerator.rootsToUnfog;
            rootsToUnfog.Add(centerCell);
        }

        public Map map
        {
            get
            {
                 return BaseGen.globalSettings.map;
            }
        }

        public virtual void PassingParameters(ResolveParams rp)
        {

        }

    }
}
