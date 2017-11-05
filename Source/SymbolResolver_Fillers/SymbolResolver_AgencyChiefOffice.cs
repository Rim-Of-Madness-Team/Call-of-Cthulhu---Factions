using System;
using Verse;
using RimWorld.BaseGen;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using UnityEngine;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyChiefOffice : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            bool @bool = Rand.Bool;
            string defTypewriter = "Estate_TableTypewriter";
            string defBench = "SimpleResearchBench";
            string defChair = "DiningChair";
            string defLamp = "Jecrell_GasLamp";

            ThingDef wallDef = ThingDefOf.Steel;
            TerrainDef floorDef = TerrainDefOf.MetalTile;
            ThingDef lampDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            ThingDef lampStuffDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

            ThingDef thingDef = rp.singleThingDef ?? ((Cthulhu.Utility.IsIndustrialAgeLoaded()) ? ThingDef.Named(defTypewriter) : ThingDef.Named(defBench));
            ThingDef chairDef = ThingDef.Named(defChair);

            Rot4 rot = (!@bool) ? Rot4.South : Rot4.West;
            IntVec3 loc = rp.rect.CenterCell;
            if (!GenSpawn.WouldWipeAnythingWith(loc, rot, thingDef, map, (Thing x) => x.def.category == ThingCategory.Building))
            {
                bool flag = false;
                foreach (IntVec3 current2 in GenAdj.CellsOccupiedBy(loc, rot, thingDef.Size))
                {
                    if (BaseGenUtility.AnyDoorAdjacentCardinalTo(current2, map))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    ///RESEARCH BENCH
                    ThingDef stuff = null;
                    if (thingDef.MadeFromStuff)
                    {
                        stuff = ThingDefOf.WoodLog;
                    }
                    Thing thing = ThingMaker.MakeThing(thingDef, stuff);
                    thing.SetFaction(rp.faction, null);
                    GenSpawn.Spawn(thing, loc, map, rot);

                    if (thing != null)
                    {
                        if (thing.Spawned && thing.Position != IntVec3.Invalid)
                        {
                            ///CHAIR
                            Building bld = thing as Building; //We need an interaction cell
                            Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                            thing2.SetFaction(rp.faction, null);
                            GenSpawn.Spawn(thing2, bld.InteractionCell, map, rot.Opposite);
                        }

                        ///Adjacent lamps
                        ThingDef singleThingDef3 = (Cthulhu.Utility.IsIndustrialAgeLoaded()) ? lampDef : ThingDefOf.TorchLamp;
                        Thing thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                        IntVec3 thingLoc = Cthulhu.Utility.GetCornerPos(thing.OccupiedRect(), 0) + GenAdj.AdjacentCells[Rot4.West.AsInt];
                        GenSpawn.Spawn(thing3, thingLoc, map);

                        
                    }
                }
            }

            UnfogRoomCenter(rp.rect.CenterCell);
            Utility.RectReport(rp.rect);

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
    }
}
