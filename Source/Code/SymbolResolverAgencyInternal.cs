using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolverAgencyInternal : SymbolResolver
    {
        public static Map instanceMap;
        public static Faction instanceFaction;
        public static IntVec3 tempIntVec = IntVec3.Invalid;
        public static Thing newThing;

        private static readonly List<Thing> tmpThingsToDestroy = new List<Thing>();

        public Map Map => BaseGen.globalSettings.map;

        public override void Resolve(ResolveParams rp)
        {
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