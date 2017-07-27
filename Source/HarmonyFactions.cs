using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld.Planet;
using RimWorld;

namespace CthulhuFactions
{
    [StaticConstructorOnStartup]
    public static class HarmonyFactions
    {
        static HarmonyFactions()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.cthulhu.factions");

            harmony.Patch(AccessTools.Method(typeof(TileFinder), "RandomFactionBaseTileFor"), new HarmonyMethod(typeof(HarmonyFactions).GetMethod("RandomFactionBaseTileFor_PreFix")), null);
        }

        public static bool RandomFactionBaseTileFor_PreFix(ref int __result, Faction faction)
        {
            //if (faction.def.defName == "TheAgency")
            //{
            //    __result = RandomFactionBaseTileFor_TheAgency(faction);
            //    return false;
            //}
            if (faction?.def?.defName == "ROM_Townsfolk")
            {
                __result = RandomFactionBaseTileFor_Townsfolk(faction);
                return false;
            }
            return true;
        }
        


        public static int RandomFactionBaseTileFor_Townsfolk(Faction faction, bool mustBeAutoChoosable = false)
        {
            for (int i = 0; i < 500; i++)
            {
                int num;
                if ((from _ in Enumerable.Range(0, 100)
                     select Rand.Range(0, Find.WorldGrid.TilesCount)).TryRandomElementByWeight(delegate (int x)
                     {
                         Tile tile = Find.WorldGrid[x];
                         if (!tile.biome.canBuildBase || tile.hilliness == Hilliness.Impassable)
                         {
                             return 0f;
                         }
                         List<int> neighbors = new List<int>();
                         Find.WorldGrid.GetTileNeighbors(x, neighbors);
                         //Log.Message("Neighbors " + neighbors.Count.ToString());
                         if (neighbors != null && neighbors.Count > 0)
                         {
                             foreach (int y in neighbors)
                             {
                                 Tile tile2 = Find.WorldGrid[y];
                                 if (tile2.biome == BiomeDefOf.IceSheet || tile2.biome == BiomeDef.Named("SeaIce"))
                                     return 0f;
                                 if (tile2.WaterCovered)
                                     return 1000f;
                             }
                         }

                         return tile.biome.factionBaseSelectionWeight;
                     }, out num))
                {
                    if (TileFinder.IsValidTileForNewSettlement(num, null))
                    {
                        return num;
                    }
                }
            }
            Log.Error("Failed to find faction base tile for " + faction);
            return 0;
        }
        
    }
}
