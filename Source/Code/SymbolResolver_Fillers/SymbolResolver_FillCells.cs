using System.Collections.Generic;
using System.Linq;
using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_FillCells : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams rp)
        {
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var lampDef = Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            var lampStuffDef = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

            var map = BaseGen.globalSettings.map;
            var @bool = Rand.Bool;
            var hasLamp = false;
            var thingDef = ThingDefOf.SleepingSpot;
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

                {
                    var doorNear = false;
                    foreach (var current2 in GenAdj.CellsOccupiedBy(current, rot, thingDef.Size))
                    {
                        if (!BaseGenUtility.AnyDoorAdjacentCardinalTo(current2, map))
                        {
                            continue;
                        }

                        doorNear = true;
                        break;
                    }

                    if (doorNear)
                    {
                        continue;
                    }

                    //Bed
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

                    var thingRect = thing.OccupiedRect();

                    thingRect = thingRect.ExpandedBy(1);
                    var possibleCells = new List<IntVec3>();
                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.North));
                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.South));
                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.East));
                    possibleCells.AddRange(thingRect.GetEdgeCells(Rot4.West));
                    possibleCells.Remove(Utility.GetCornerPos(thingRect, 0));
                    possibleCells.Remove(Utility.GetCornerPos(thingRect, 1));
                    possibleCells.Remove(Utility.GetCornerPos(thingRect, 2));
                    possibleCells.Remove(Utility.GetCornerPos(thingRect, 3));

                    var spawnPos = possibleCells.InRandomOrder()
                        .FirstOrDefault(x => x.InBounds(map) && x.Walkable(map));
                    possibleCells.Remove(spawnPos);
                    var thing2 = ThingMaker.MakeThing(chairDef, ThingDefOf.WoodLog);
                    thing2.SetFaction(rp.faction);
                    GenSpawn.Spawn(thing2, spawnPos, map, rot.Opposite);

                    if (hasLamp)
                    {
                        continue;
                    }

                    hasLamp = true;
                    spawnPos = possibleCells.FirstOrDefault(x =>
                        x.InBounds(map) && x.Walkable(map));

                    possibleCells.Remove(spawnPos);
                    var thing3 = ThingMaker.MakeThing(lampDef, lampStuffDef);
                    thing3.SetFaction(rp.faction);
                    GenSpawn.Spawn(thing3, spawnPos, map, rot.Opposite);
                }
            }

            //if (!Cthulhu.Utility.IsCosmicHorrorsLoaded())
            //{
            var unused = rp.pawnGroupKindDef ?? PawnGroupKindDefOf.Combat;
            var pawnGroupMakerParms = new PawnGroupMakerParms
            {
                faction = (from x in Find.FactionManager.AllFactions
                    where x.HostileTo(rp.faction) && x.def.pawnGroupMakers is {Count: > 0} && x.def.humanlikeFaction &&
                          IsCosmicHorrorFaction(x) == false && x.def.pawnGroupMakers.FirstOrDefault(y =>
                              y.kindDef == PawnGroupKindDefOf.Combat) != null
                    select x).RandomElement(),
                tile = map.Tile,
                points = 5000,
                generateFightersOnly = false
            };
            var num = 0;
            foreach (var current in PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms))
            {
                num++;
                var resolveParams = rp;
                resolveParams.singlePawnToSpawn = current;
                BaseGen.symbolStack.Push("pawn", resolveParams);
                if (num == 3)
                {
                    break;
                }
            }
            //}
            //else
            //{
            //    Faction tempFaction = Find.FactionManager.AllFactions.InRandomOrder<Faction>().FirstOrDefault((Faction z) => IsCosmicHorrorFaction(z));
            //    PawnGroupKindDef groupKind = rp.pawnGroupKindDef ?? PawnGroupKindDefOf.Combat;
            //    PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms()
            //    {
            //        faction = tempFaction,
            //        map = map,
            //        points = 5000,
            //        generateFightersOnly = false,
            //        generateMeleeOnly = false
            //    };
            //    int num = 0;
            //    foreach (Pawn current in PawnGroupMakerUtility.GeneratePawns(groupKind, pawnGroupMakerParms, true))
            //    {
            //        num++;
            //        ResolveParams resolveParams = rp;
            //        resolveParams.singlePawnToSpawn = current;
            //        resolveParams.singlePawnLord = null;
            //        BaseGen.symbolStack.Push("pawn", resolveParams);
            //        if (num == 3) break;
            //    }
            //}

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

        public static bool IsCosmicHorrorFaction(Faction f)
        {
            var factions = new List<string>
            {
                "StarSpawn",
                "Shoggoth",
                "MiGo",
                "DeepOne"
            };
            foreach (var s in factions)
            {
                if (f.def.defName.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }
    }
}