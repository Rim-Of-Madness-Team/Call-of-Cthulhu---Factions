using RimWorld;
using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace CthulhuFactions
{
    public class SymbolResolver_FillWithTypewriters : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams rp)
        {
            string defTypewriter = "Estate_TableTypewriter";
            string defBench      = "SimpleResearchBench";
            string defChair      = "DiningChair";
            string defLamp = "Jecrell_GasLamp";
          
            ThingDef lampDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            ThingDef lampStuffDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

            Map map = BaseGen.globalSettings.map;
            bool @bool = Rand.Bool;
            ThingDef thingDef = rp.singleThingDef ?? ((Cthulhu.Utility.IsIndustrialAgeLoaded()) ? ThingDef.Named(defTypewriter) : ThingDef.Named(defBench));
            ThingDef chairDef = ThingDef.Named(defChair);
            foreach (IntVec3 current in rp.rect)
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
                Rot4 rot = (!@bool) ? Rot4.South : Rot4.West;
                if (!GenSpawn.WouldWipeAnythingWith(current, rot, thingDef, map, (Thing x) => x.def.category == ThingCategory.Building))
                {
                    bool flag = false;
                    foreach (IntVec3 current2 in GenAdj.CellsOccupiedBy(current, rot, thingDef.Size))
                    {
                        if (BaseGenUtility.AnyDoorCardinalAdjacentTo(current2, map))
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
                        GenSpawn.Spawn(thing, current, map, rot);

                        if (thing != null)
                        {
                            if (thing.Spawned && thing.Position.InBounds(map))
                            {
                                ///CHAIR
                                Building bld = thing as Building; //We need an interaction cell
                                if (bld != null)
                                {
                                    if (rp.rect.Cells.FirstOrDefault((IntVec3 cell) => cell == bld.InteractionCell) != null)
                                    {
                                        Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                                        thing2.SetFaction(rp.faction, null);
                                        GenSpawn.Spawn(thing2, bld.InteractionCell, map, rot.Opposite);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            List<int> corners = new List<int>() { 0, 1, 2, 3 };
            foreach (int corner in corners.InRandomOrder<int>())
            {
                IntVec3 loc = Cthulhu.Utility.GetCornerPos(rp.rect.ContractedBy(1), corner);
                if (!GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map, (Thing x) => x.def.category == ThingCategory.Building))
                {
                    ThingDef singleThingDef3 = (Cthulhu.Utility.IsIndustrialAgeLoaded()) ? lampDef : ThingDefOf.TorchLamp;
                    Thing thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                    GenSpawn.Spawn(thing3, loc, map);
                    break;
                }
            }
        }
    }
}
