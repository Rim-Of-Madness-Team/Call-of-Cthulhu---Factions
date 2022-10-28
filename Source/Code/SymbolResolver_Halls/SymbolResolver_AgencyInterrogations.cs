using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyInterrogations : SymbolResolverAgency
    {
        public virtual bool HorizontalHallway => false;

        public override void PassingParameters(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;
            _ = new CellRect();
            _ = new List<IntVec3>();
            var splitRooms =
                FactionUtility.FourWaySplit(rp.rect, out var hallway, out var doorLocs, 2, HorizontalHallway);


            var hallwayParams = rp;
            hallwayParams.rect = hallway;

            foreach (var doorLoc in doorLocs)
            {
                var resolveParamsDoor = rp;
                resolveParamsDoor.rect = CellRect.SingleCell(doorLoc);
                resolveParamsDoor.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
                BaseGen.symbolStack.Push("thing", resolveParamsDoor);
            }

            foreach (var current in splitRooms.InRandomOrder())
            {
                var resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsTemp);
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                BaseGen.symbolStack.Push("agencyFillWithTablesAndChairs", resolveParamsTemp);
                UnfogRoomCenter(current.CenterCell);
            }

            FactionUtility.RectReport(rp.rect);

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