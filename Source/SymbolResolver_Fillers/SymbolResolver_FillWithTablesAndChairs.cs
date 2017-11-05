using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_FillWithTablesAndChairs : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams paramsIn)
        {
            string defBench = "TableLong";
            string defChair = "DiningChair";
            string defLamp = "Jecrell_GasLamp";

            ThingDef lampDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            ThingDef furnitureStuff = ThingDefOf.Steel;
            ThingDef lampStuffDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

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
                        ///TABLE
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
                                CellRect rectToEdit = thing.OccupiedRect().ExpandedBy(1);

                                List<IntVec3> poss = new List<IntVec3>();
                                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.North));
                                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.South));
                                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.East));
                                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.West));
                                poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 0));
                                poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 1));
                                poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 2));
                                poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 3));

                                for (int i = 0; i < 4; i++)
                                {
                                    ///CHAIR
                                    IntVec3 currentPos = poss.InRandomOrder<IntVec3>().RandomElement<IntVec3>();
                                    poss.Remove(currentPos);

                                    Rot4 newRot = Rot4.North;
                                    if (currentPos.x > thing.Position.x) newRot = Rot4.West;
                                    else if (currentPos.x < thing.Position.x) newRot = Rot4.East;
                                    else if (currentPos.z > thing.Position.z) newRot = Rot4.South;
                                    else newRot = Rot4.North; 

                                    Building bld = thing as Building; //We need an interaction cell
                                    Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                                    thing2.SetFaction(rp.faction, null);
                                    GenSpawn.Spawn(thing2, currentPos, map, newRot);
                                }
                            }
                        }
                    }
                }
            }
            List<int> corners = new List<int>() { 0, 1, 2, 3 };
            int count = 0;
            foreach (int corner in corners.InRandomOrder<int>())
            {
                if (count == 1) break;
                IntVec3 loc = Cthulhu.Utility.GetCornerPos(paramsIn.rect.ContractedBy(1), corner);
                if (!GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map, (Thing x) => x.def.category == ThingCategory.Building))
                {
                    ThingDef singleThingDef3 = (Cthulhu.Utility.IsIndustrialAgeLoaded()) ? lampDef : ThingDefOf.TorchLamp;
                    Thing thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                    GenSpawn.Spawn(thing3, loc, map);
                }
                count++;
            }
        }
    }
}
