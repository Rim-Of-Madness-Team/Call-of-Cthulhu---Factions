using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_FillWithTablesAndChairs : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams paramsIn)
        {
            var defBench = "TableLong";
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var lampDef = Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            var furnitureStuff = ThingDefOf.Steel;
            var lampStuffDef = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

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

                //TABLE
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

                var rectToEdit = thing.OccupiedRect().ExpandedBy(1);

                var poss = new List<IntVec3>();
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.North));
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.South));
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.East));
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.West));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 0));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 1));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 2));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 3));

                for (var i = 0; i < 4; i++)
                {
                    //CHAIR
                    var currentPos = poss.InRandomOrder().RandomElement();
                    poss.Remove(currentPos);

                    Rot4 newRot;
                    if (currentPos.x > thing.Position.x)
                    {
                        newRot = Rot4.West;
                    }
                    else if (currentPos.x < thing.Position.x)
                    {
                        newRot = Rot4.East;
                    }
                    else if (currentPos.z > thing.Position.z)
                    {
                        newRot = Rot4.South;
                    }
                    else
                    {
                        newRot = Rot4.North;
                    }

                    var unused = thing as Building; //We need an interaction cell
                    var thing2 = ThingMaker.MakeThing(chairDef, stuff);
                    thing2.SetFaction(rp.faction);
                    GenSpawn.Spawn(thing2, currentPos, map, newRot);
                }
            }

            var corners = new List<int> {0, 1, 2, 3};
            var count = 0;
            foreach (var corner in corners.InRandomOrder())
            {
                if (count == 1)
                {
                    break;
                }

                var loc = Utility.GetCornerPos(paramsIn.rect.ContractedBy(1), corner);
                if (!GenSpawn.WouldWipeAnythingWith(loc, Rot4.South, lampDef, map,
                    x => x.def.category == ThingCategory.Building))
                {
                    var singleThingDef3 = Utility.IsIndustrialAgeLoaded() ? lampDef : ThingDefOf.TorchLamp;
                    var thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                    GenSpawn.Spawn(thing3, loc, map);
                }

                count++;
            }
        }
    }
}