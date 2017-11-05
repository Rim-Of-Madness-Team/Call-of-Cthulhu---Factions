using RimWorld;
using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace CthulhuFactions
{
    public class SymbolResolver_FillBedroom : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams rp)
        {
            string defChair = "DiningChair";
            string defLamp = "Jecrell_GasLamp";

            ThingDef lampDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            ThingDef lampStuffDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

            Map map = BaseGen.globalSettings.map;
            bool @bool = Rand.Bool;
            bool hasLamp = false;
            ThingDef thingDef = ThingDefOf.Bed;
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
                        if (BaseGenUtility.AnyDoorAdjacentCardinalTo(current2, map))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        ///Bed
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
                                CellRect thingRect = thing.OccupiedRect();
                                if (thingRect != null)
                                {
                                    thingRect = thingRect.ExpandedBy(1);
                                    List<IntVec3> possibleCells = new List<IntVec3>();
                                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.North));
                                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.South));
                                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.East));
                                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.West));
                                    possibleCells.Remove(Cthulhu.Utility.GetCornerPos(thingRect, 0));
                                    possibleCells.Remove(Cthulhu.Utility.GetCornerPos(thingRect, 1));
                                    possibleCells.Remove(Cthulhu.Utility.GetCornerPos(thingRect, 2));
                                    possibleCells.Remove(Cthulhu.Utility.GetCornerPos(thingRect, 3));

                                    IntVec3 spawnPos = IntVec3.Invalid;
                                    spawnPos = possibleCells.FirstOrDefault((IntVec3 x) => x.InBounds(map) && x.Walkable(map));
                                    if (spawnPos != null)
                                    {
                                        possibleCells.Remove(spawnPos);
                                        Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                                        thing2.SetFaction(rp.faction, null);
                                        GenSpawn.Spawn(thing2, spawnPos, map, rot.Opposite);
                                    }
                                    if (!hasLamp)
                                    {
                                        hasLamp = true;
                                        spawnPos = possibleCells.FirstOrDefault((IntVec3 x) => x.InBounds(map) && x.Walkable(map));
                                        if (spawnPos != null)
                                        {
                                            possibleCells.Remove(spawnPos);
                                            Thing thing3 = ThingMaker.MakeThing(lampDef, lampStuffDef);
                                            thing3.SetFaction(rp.faction, null);
                                            GenSpawn.Spawn(thing3, spawnPos, map, rot.Opposite);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //List<int> corners = new List<int>() { 0, 1, 2, 3 };
            //foreach (int corner in corners.InRandomOrder<int>())
            //{
            //    IntVec3 loc = Cthulhu.Utility.GetCornerPos(rp.rect.ContractedBy(1), corner);
            //    if (!GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map, (Thing x) => x.def.category == ThingCategory.Building))
            //    {
            //        ThingDef singleThingDef3 = (Cthulhu.Utility.IsIndustrialAgeLoaded()) ? lampDef : ThingDefOf.TorchLamp;
            //        Thing thing3 = ThingMaker.MakeThing(singleThingDef3, ThingDefOf.Steel);
            //        GenSpawn.Spawn(thing3, loc, map);
            //        break;
            //    }
            //}
        }
    }
}
