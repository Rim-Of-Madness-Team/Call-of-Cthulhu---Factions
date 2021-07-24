using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolverAgency : SymbolResolver
    {
        public static Map instanceMap;
        public static Faction instanceFaction;
        public static IntVec3 tempIntVec = IntVec3.Invalid;
        public static Thing newThing;

        private static readonly List<Thing> tmpThingsToDestroy = new List<Thing>();

        public virtual ThingDef DefaultWallDef => ThingDefOf.Steel;

        public virtual TerrainDef DefaultFloorDef => TerrainDefOf.MetalTile;

        public Map Map => BaseGen.globalSettings.map;

        public override void Resolve(ResolveParams rp)
        {
            //CellRect.CellRectIterator iterator = rp.rect.ContractedBy(1).GetIterator();
            //while (!iterator.Done())
            foreach (var cellRect in rp.rect.ContractedBy(1))
            {
                if (rp.clearEdificeOnly.HasValue && rp.clearEdificeOnly.Value)
                {
                    var edifice = cellRect.GetEdifice(BaseGen.globalSettings.map);
                    if (edifice == null || !edifice.def.destroyable)
                    {
                        continue;
                    }

                    if (edifice.def != CthulhuFactionsDefOf.ROM_TemporaryDoorMarker)
                    {
                        edifice.Destroy();
                    }
                }
                else
                {
                    tmpThingsToDestroy.Clear();
                    tmpThingsToDestroy.AddRange(cellRect.GetThingList(BaseGen.globalSettings.map));
                    foreach (var thing in tmpThingsToDestroy)
                    {
                        if (!thing.def.destroyable)
                        {
                            continue;
                        }

                        if (thing.def != CthulhuFactionsDefOf.ROM_TemporaryDoorMarker)
                        {
                            thing.Destroy();
                        }
                    }
                }
            }

            foreach (var cell in rp.rect.Cells)
            {
                var resolveParams3 = rp;
                resolveParams3.rect = CellRect.SingleCell(cell);
                resolveParams3.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryRegionBarrier;
                BaseGen.symbolStack.Push("thing", resolveParams3);
            }

            PassingParameters(rp);
            instanceMap = Map;
            instanceFaction = rp.faction ?? Find.FactionManager.FirstFactionOfDef(FactionDef.Named("TheAgency"));

            //ResolveParams resolveParams4 = rp;
            //BaseGen.symbolStack.Push("clear", resolveParams4);

            UnfogRoomCenter(rp.rect.CenterCell);

            LongEventHandler.QueueLongEvent(delegate
            {
                if (instanceMap == null)
                {
                    return;
                }

                foreach (var cell in instanceMap.AllCells)
                {
                    var thing = cell.GetThingList(instanceMap)
                        .Find(x => x.def == CthulhuFactionsDefOf.ROM_TemporaryDoorMarker);
                    if (thing == null)
                    {
                        //Log.Warning("Could not destroy temporary region barrier at " + cell + " because it's not here.");
                    }
                    else
                    {
                        if (thing.def == CthulhuFactionsDefOf.ROM_TemporaryDoorMarker)
                        {
                            newThing = null;
                            newThing = ThingMaker.MakeThing(ThingDefOf.Door, ThingDefOf.Steel);
                            newThing.SetFaction(instanceFaction);
                            //newThing.Rotation = thing.Rotation;
                            tempIntVec = new IntVec3(cell.x, 0, cell.z);
                            GenSpawn.Spawn(newThing, tempIntVec, instanceMap);
                        }

                        thing.Destroy();
                    }
                }

                foreach (var pawn in instanceMap.mapPawns.AllPawnsSpawned.FindAll(x =>
                    x is {Spawned: true} && x.RaceProps.intelligence >= Intelligence.ToolUser && !x.IsColonist &&
                    x.Faction != instanceFaction && x.guest != null &&
                    SymbolResolver_FillCells.IsCosmicHorrorFaction(x.Faction) == false))
                {
                    pawn.guest.SetGuestStatus(instanceFaction, GuestStatus.Prisoner);
                }

                foreach (var thing in instanceMap.listerThings.AllThings.FindAll(x =>
                    x is Building_Bed && x.def == ThingDefOf.SleepingSpot && x.Faction == instanceFaction))
                {
                    var bed = (Building_Bed) thing;
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

            var rootsToUnfog = MapGenerator.rootsToUnfog;
            rootsToUnfog.Add(centerCell);
        }

        public virtual void PassingParameters(ResolveParams rp)
        {
        }
    }
}