using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_FillWithResearchBenches : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams paramsIn)
        {
            var defBench = "SimpleResearchBench";
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var lampDef = Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            var lampStuffDef = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;
            var furnitureStuff = ThingDefOf.Steel;

            var rp = paramsIn;
            rp.rect = paramsIn.rect.ContractedBy(3);

            var map = BaseGen.globalSettings.map;
            var @bool = Rand.Bool;
            var thingDef = ThingDef.Named(defBench);
            var chairDef = ThingDef.Named(defChair);
            foreach (var current in rp.rect)
            {
                if (@bool)
                {
                    if (current.x % 4 != 0 || current.z % 4 != 0)
                    {
                        continue;
                    }
                }
                else if (current.x % 4 != 0 || current.z % 4 != 0)
                {
                    continue;
                }

                var rot = Rot4.Random;
                if (GenSpawn.WouldWipeAnythingWith(current, rot, thingDef, map,
                    x => x.def.category == ThingCategory.Building))
                {
                    continue;
                }

                var nearDoor = false;
                foreach (var current2 in GenAdj.CellsOccupiedBy(current, rot, thingDef.Size))
                {
                    if (!BaseGenUtility.AnyDoorAdjacentCardinalTo(current2, map))
                    {
                        continue;
                    }

                    nearDoor = true;
                    break;
                }

                if (nearDoor)
                {
                    continue;
                }

                //RESEARCH BENCH
                ThingDef stuff = null;
                if (thingDef.MadeFromStuff)
                {
                    stuff = furnitureStuff;
                }

                var thing = ThingMaker.MakeThing(thingDef, stuff);
                thing.SetFaction(rp.faction);
                GenSpawn.Spawn(thing, current, map, rot);

                if (!thing.Spawned || !thing.Position.InBounds(map))
                {
                    continue;
                }

                //CHAIR
                var bld = thing as Building; //We need an interaction cell
                var thing2 = ThingMaker.MakeThing(chairDef, stuff);
                thing2.SetFaction(rp.faction);
                if (bld != null)
                {
                    GenSpawn.Spawn(thing2, bld.InteractionCell, map, rot);
                }
            }

            var corners = new List<int> {0, 1, 2, 3};
            foreach (var corner in corners.InRandomOrder())
            {
                var loc = Utility.GetCornerPos(paramsIn.rect.ContractedBy(1), corner);
                if (GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map,
                    x => x.def.category == ThingCategory.Building))
                {
                    continue;
                }

                var singleThingDef3 = Utility.IsIndustrialAgeLoaded() ? lampDef : ThingDefOf.TorchLamp;
                var thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                GenSpawn.Spawn(thing3, loc, map);
            }
        }
    }
}