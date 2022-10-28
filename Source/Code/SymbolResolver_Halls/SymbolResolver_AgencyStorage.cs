using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyStorage : SymbolResolverAgency
    {
        //private bool spawnedArtifactRoom = false;
        public virtual bool HorizontalHallway => false;

        public override void PassingParameters(ResolveParams rp)
        {
            _ = BaseGen.globalSettings.map;

            //Faction values are read in reverse of their push.
            // Programmed like: Checkpoint doors -> Lobby -> Edge Walls -> Floor -> Clear
            // Read Like: Clear -> Floor -> Edge Walls -> Lobby -> Chokepoint doors
            //
            // Tip:
            // Roofs and doors must be defined in the program first to avoid region errors.

            //ROOF
            //if (!rp.noRoof.HasValue || !rp.noRoof.Value)
            //{
            //    BaseGen.symbolStack.Push("roof", rp);
            //}

            _ = new CellRect();
            _ = new List<IntVec3>();
            var splitRooms =
                FactionUtility.FourWaySplit(rp.rect, out var hallway, out var doorLocs, 2, HorizontalHallway);


            var hallwayParams = rp;
            hallwayParams.rect = hallway;

            foreach (var doorLoc in doorLocs)
            {
                var resolveParams3 = rp;
                resolveParams3.rect = CellRect.SingleCell(doorLoc);
                resolveParams3.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
                BaseGen.symbolStack.Push("thing", resolveParams3);
            }

            var num = 0;
            foreach (var current in splitRooms.InRandomOrder())
            {
                num++;
                if (num < 2)
                {
                    var resolveParamsArtifactRoom = rp;
                    resolveParamsArtifactRoom.rect = current;
                    BaseGen.symbolStack.Push("roof", resolveParamsArtifactRoom);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsArtifactRoom);
                    BaseGen.symbolStack.Push("agencyStorageArtifact", resolveParamsArtifactRoom);
                    UnfogRoomCenter(current.CenterCell);
                }

                var resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                BaseGen.symbolStack.Push("storage", resolveParamsTemp);
                UnfogRoomCenter(current.CenterCell);
            }

            FactionUtility.RectReport(rp.rect);
        }

        private void UnfogRoomCenter(IntVec3 centerCell)
        {
            if (Current.ProgramState != ProgramState.MapInitializing)
            {
                return;
            }

            var rootsToUnfog = MapGenerator.rootsToUnfog;
            rootsToUnfog.Add(centerCell);
        }
    }
}