using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
//using CombatExtended;

namespace CthulhuFactions
{
    [StaticConstructorOnStartup]
    public static class HarmonyFactions
    {
        static HarmonyFactions()
        {
            var harmony = new Harmony("rimworld.jecrell.cthulhu.factions");

            harmony.Patch(AccessTools.Method(typeof(TileFinder), "RandomSettlementTileFor"),
                new HarmonyMethod(typeof(HarmonyFactions), nameof(RandomSettlementTileFor_PreFix)));
        }

        public static bool RandomSettlementTileFor_PreFix(Faction faction, ref int __result)

        {
            //if (faction.def.defName == "TheAgency")
            //{
            //    __result = RandomSettlementTileFor_TheAgency(faction);
            //    return false;
            //}
            if (faction?.def?.defName != "ROM_Townsfolk")
            {
                return true;
            }

            __result = RandomSettlementTileFor_Townsfolk(faction);
            return false;
        }


        public static int RandomSettlementTileFor_Townsfolk(Faction faction, bool mustBeAutoChoosable = false)
        {
            for (var i = 0; i < 500; i++)
            {
                if (!(from _ in Enumerable.Range(0, 100)
                    select Rand.Range(0, Find.WorldGrid.TilesCount)).TryRandomElementByWeight(delegate(int x)
                {
                    var tile = Find.WorldGrid[x];
                    if (!tile.biome.canBuildBase || tile.hilliness == Hilliness.Impassable)
                    {
                        return 0f;
                    }

                    var neighbors = new List<int>();
                    Find.WorldGrid.GetTileNeighbors(x, neighbors);
                    //Log.Message("Neighbors " + neighbors.Count.ToString());
                    if (neighbors.Count <= 0)
                    {
                        return tile.biome.settlementSelectionWeight;
                    }

                    foreach (var y in neighbors)
                    {
                        var tile2 = Find.WorldGrid[y];
                        if (tile2.biome == BiomeDefOf.IceSheet || tile2.biome == BiomeDef.Named("SeaIce"))
                        {
                            return 0f;
                        }

                        if (tile2.WaterCovered)
                        {
                            return 1000f;
                        }
                    }

                    return tile.biome.settlementSelectionWeight;
                }, out var num))
                {
                    continue;
                }

                if (TileFinder.IsValidTileForNewSettlement(num))
                {
                    return num;
                }
            }

            Log.Error("Failed to find faction base tile for " + faction);
            return 0;
        }
    }
}