using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyBedrooms : SymbolResolverAgency
    {
        public virtual bool HorizontalHallway => false;

        public override void PassingParameters(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;

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

            foreach (var current in splitRooms.InRandomOrder())
            {
                var resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                var resolveParamsTemp2 = rp;
                resolveParamsTemp2.rect = current.ContractedBy(2);
                BaseGen.symbolStack.Push("agencyFillBedroom", resolveParamsTemp2);
                UnfogRoomCenter(current.CenterCell);
            }

            //Bring in Standard Pawns
            var singlePawnLord = rp.singlePawnLord ??
                                 LordMaker.MakeNewLord(rp.faction,
                                     new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map);

            var resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.Settlement;
            float points = 500;
            resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms
            {
                tile = map.Tile,
                faction = rp.faction,
                points = points
            };

            BaseGen.symbolStack.Push("pawnGroup", resolveParams);

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