using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_FillWithTypewriters : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams rp)
        {
            var defTypewriter = "Estate_TableTypewriter";
            var defBench = "SimpleResearchBench";
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var lampDef = Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            var lampStuffDef = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

            var map = BaseGen.globalSettings.map;
            var @bool = Rand.Bool;
            var thingDef = rp.singleThingDef ?? (Utility.IsIndustrialAgeLoaded()
                ? ThingDef.Named(defTypewriter)
                : ThingDef.Named(defBench));
            var chairDef = ThingDef.Named(defChair);
            foreach (var current in rp.rect)
            {
                if (@bool)
                {
                    if (current.x % 3 != 0 || current.z % 3 != 0)
                    {
                        continue;
                    }
                }
                else if (current.x % 3 != 0 || current.z % 3 != 0)
                {
                    continue;
                }

                var rot = !@bool ? Rot4.South : Rot4.West;
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
                    stuff = ThingDefOf.WoodLog;
                }

                var thing = ThingMaker.MakeThing(thingDef, stuff);
                thing.SetFaction(rp.faction);
                GenSpawn.Spawn(thing, current, map, rot);

                if (!thing.Spawned || !thing.Position.InBounds(map))
                {
                    continue;
                }

                //CHAIR
                //We need an interaction cell
                if (thing is not Building bld)
                {
                    continue;
                }

                var thing2 = ThingMaker.MakeThing(chairDef, stuff);
                thing2.SetFaction(rp.faction);
                GenSpawn.Spawn(thing2, bld.InteractionCell, map, rot.Opposite);
            }

            var corners = new List<int> {0, 1, 2, 3};
            foreach (var corner in corners.InRandomOrder())
            {
                var loc = Utility.GetCornerPos(rp.rect.ContractedBy(1), corner);
                if (GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map,
                    x => x.def.category == ThingCategory.Building))
                {
                    continue;
                }

                var singleThingDef3 = Utility.IsIndustrialAgeLoaded() ? lampDef : ThingDefOf.TorchLamp;
                var thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                GenSpawn.Spawn(thing3, loc, map);
                break;
            }
        }
    }
}