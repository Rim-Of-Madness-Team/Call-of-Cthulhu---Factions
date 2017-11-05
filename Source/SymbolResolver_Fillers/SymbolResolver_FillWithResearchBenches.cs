using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_FillWithResearchBenches : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams paramsIn)
        {
            string defBench = "SimpleResearchBench";
            string defChair = "DiningChair";
            string defLamp = "Jecrell_GasLamp";

            ThingDef lampDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            ThingDef lampStuffDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;
            ThingDef furnitureStuff = ThingDefOf.Steel;

            ResolveParams rp = paramsIn;
            rp.rect = paramsIn.rect.ContractedBy(3);

            Map map = BaseGen.globalSettings.map;
            bool @bool = Rand.Bool;
            ThingDef thingDef = ThingDef.Named(defBench);
            ThingDef chairDef = ThingDef.Named(defChair);
            foreach (IntVec3 current in rp.rect)
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
                Rot4 rot = Rot4.Random;
                if (!GenSpawn.WouldWipeAnythingWith(current, rot, thingDef, map, (Thing x) => x.def.category == ThingCategory.Building))
                {
                    bool flag = false;
                    foreach (IntVec3 current2 in GenAdj.CellsOccupiedBy(current, rot, thingDef.Size))
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
                            stuff = furnitureStuff;
                        }
                        Thing thing = ThingMaker.MakeThing(thingDef, stuff);
                        thing.SetFaction(rp.faction, null);
                        GenSpawn.Spawn(thing, current, map, rot);

                        if (thing != null)
                        {
                            if (thing.Spawned && thing.Position.InBounds(map))
                            {
                                ///CHAIR
                                Building bld = thing as Building; //We need an interaction cell
                                Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                                thing2.SetFaction(rp.faction, null);
                                GenSpawn.Spawn(thing2, bld.InteractionCell, map, rot);
                            }
                        }
                    }
                }
            }
            List<int> corners = new List<int>() { 0, 1, 2, 3 };
            foreach (int corner in corners.InRandomOrder<int>())
            {
                IntVec3 loc = Cthulhu.Utility.GetCornerPos(paramsIn.rect.ContractedBy(1), corner);
                if (!GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map, (Thing x) => x.def.category == ThingCategory.Building))
                {
                    ThingDef singleThingDef3 = (Cthulhu.Utility.IsIndustrialAgeLoaded()) ? lampDef : ThingDefOf.TorchLamp;
                    Thing thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                    GenSpawn.Spawn(thing3, loc, map);
                }
            }

        }
    }
}
