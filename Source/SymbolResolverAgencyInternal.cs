using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolverAgencyInternal : SymbolResolver
    {

        public static Map instanceMap = null;
        public static Faction instanceFaction = null;
        public static IntVec3 tempIntVec = IntVec3.Invalid;
        public static Thing newThing = null;

        private static List<Thing> tmpThingsToDestroy = new List<Thing>();

        public override void Resolve(ResolveParams rp)
        {

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
                if (SymbolResolverAgencyInternal.instanceMap == null) return;
                foreach (IntVec3 cell in SymbolResolverAgencyInternal.instanceMap.AllCells)
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
                            SymbolResolverAgencyInternal.newThing = null;
                            SymbolResolverAgencyInternal.newThing = ThingMaker.MakeThing(ThingDefOf.Door, ThingDefOf.Steel);
                            SymbolResolverAgencyInternal.newThing.SetFaction(SymbolResolverAgencyInternal.instanceFaction, null);
                            //newThing.Rotation = thing.Rotation;
                            tempIntVec = new IntVec3(cell.x, 0, cell.z);
                            GenSpawn.Spawn(SymbolResolverAgencyInternal.newThing, SymbolResolverAgencyInternal.tempIntVec, SymbolResolverAgencyInternal.instanceMap);
                        }
                        thing.Destroy(DestroyMode.Vanish);
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
