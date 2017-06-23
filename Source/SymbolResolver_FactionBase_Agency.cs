using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using RimWorld.BaseGen;
using CustomFactionBase;
using static CustomFactionBase.CustomBaseUtility;

namespace CthulhuFactions
{
    [StaticConstructorOnStartup]
    public class SymbolResolver_FactionBase_Agency : SymbolResolver
    {

        //RimWorld variables
        public const int MinDistBetweenSandbagsAndFactionBase = 4;

        private const int MaxRoomCells = 1200;

        private const int MinTotalRoomsNonWallCellsCount = 850;

        private const float ChanceToSkipSandbag = 0.25f;

        private static readonly IntRange CampfiresCount = new IntRange(1, 1);

        private static readonly IntRange FirefoamPoppersCount = new IntRange(1, 3);

        private static readonly IntRange RoomDivisionsCount = new IntRange(3, 3); //MODIFIED

        private static readonly IntRange FinalRoomsCount = new IntRange(3, 3); //MODIFIED

        private static readonly FloatRange AgencyPawnPoints = new FloatRange(1320f, 1980f);

        private static CellRect spawnZone;

        private static Map instanceMap;

        private static Faction instanceFaction;

        static SymbolResolver_FactionBase_Agency()
        {
            AddMapResolver(new ResolverStruct(rp => rp.faction.def.defName == "ROM_TheAgency", "agencyFactionBase", 1.0f));
        }

        public override void Resolve(ResolveParams rp)
        {

            Map map = BaseGen.globalSettings.map;
            Faction faction = rp.faction ?? Find.FactionManager.RandomEnemyFaction(false, false);

            ///CUSTOM
            rp.rect.maxX += 20;
            rp.rect.maxZ += 20;
            //Cthulhu.Utility.DebugReport("Faction square :: minZ=" + rp.rect.minZ + " minX=" + rp.rect.minX + "maxZ=" + rp.rect.maxZ + "maxX=" + rp.rect.maxX);
            ///

            spawnZone = rp.rect;
            instanceMap = map;
            instanceFaction = faction;


            CellRect cellRect = rp.rect;

            ResolveParams resolveParams12 = rp;
            resolveParams12.rect = rp.rect;
            resolveParams12.faction = faction;
            BaseGen.symbolStack.Push("agencyChokepoint", resolveParams12);


            //LongEventHandler.QueueLongEvent(delegate
            //{
            //    float randPoints = SymbolResolver_FactionBase_Agency.AgencyPawnPoints.RandomInRange;
            //    if (SymbolResolver_FactionBase_Agency.instanceMap == null) return;
            //    PawnGroupMakerParms parms = new PawnGroupMakerParms()
            //    {
            //        faction = instanceFaction,
            //        map = instanceMap,
            //        points = randPoints,
            //    };
            //    foreach (Pawn current in PawnGroupMakerUtility.GeneratePawns(PawnGroupKindDefOf.FactionBase, parms, true))
            //    {
            //        foreach (IntVec3 cell in SymbolResolver_FactionBase_Agency.spawnZone.Cells.InRandomOrder<IntVec3>())
            //        {
            //            if (cell.Walkable(instanceMap))
            //            {
            //                GenSpawn.Spawn(current, cell, instanceMap);
            //                break;
            //            }
            //        }
            //    }

            //}, "pawnGroupResolver", true, null);

        }

        private bool CanReachAnyRoom(IntVec3 root, List<RoomOutline> allRooms)
        {
            Map map = BaseGen.globalSettings.map;
            for (int i = 0; i < allRooms.Count; i++)
            {
                if (map.reachability.CanReach(root, allRooms[i].rect.RandomCell, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddRoomCentersToRootsToUnfog(List<RoomOutline> allRooms)
        {
            if (Current.ProgramState != ProgramState.MapInitializing)
            {
                return;
            }
            List<IntVec3> rootsToUnfog = MapGenerator.rootsToUnfog;
            for (int i = 0; i < allRooms.Count; i++)
            {
                rootsToUnfog.Add(allRooms[i].rect.CenterCell);
            }
        }
    }
}
