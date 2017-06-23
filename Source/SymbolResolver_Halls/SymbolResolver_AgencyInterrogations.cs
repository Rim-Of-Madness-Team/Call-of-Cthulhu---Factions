using System;
using Verse;
using RimWorld.BaseGen;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyInterrogations : SymbolResolverAgency
    {
        public virtual bool horizontalHallway
        {
            get { return false; }
        }

        public override void PassingParameters(ResolveParams rp)
        {

            Map map = BaseGen.globalSettings.map;

            CellRect hallway = new CellRect();
            IEnumerable<IntVec3> doorLocs = new List<IntVec3>();
            IEnumerable<CellRect> splitRooms = Utility.FourWaySplit(rp.rect, out hallway, out doorLocs, 2, horizontalHallway);


            ResolveParams hallwayParams = rp;
            hallwayParams.rect = hallway;

            foreach (IntVec3 doorLoc in doorLocs)
            {
                ResolveParams resolveParamsDoor = rp;
                resolveParamsDoor.rect = CellRect.SingleCell(doorLoc);
                resolveParamsDoor.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
                BaseGen.symbolStack.Push("thing", resolveParamsDoor);
            }

            foreach (CellRect current in splitRooms.InRandomOrder<CellRect>())
            {
                ResolveParams resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsTemp);
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                BaseGen.symbolStack.Push("agencyFillWithTablesAndChairs", resolveParamsTemp);
                UnfogRoomCenter(current.CenterCell);
            }

            Utility.RectReport(rp.rect);

            //Bring in Standard Pawns
            Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(rp.faction, new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map, null);

            ResolveParams resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.FactionBase;
            float points = 500;
            resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
            resolveParams.pawnGroupMakerParams.tile = map.Tile;
            resolveParams.pawnGroupMakerParams.faction = rp.faction;
            resolveParams.pawnGroupMakerParams.points = points;

            BaseGen.symbolStack.Push("pawnGroup", resolveParams);

        }

        private void UnfogRoomCenter(IntVec3 centerCell)
        {
            if (Current.ProgramState != ProgramState.MapInitializing)
            {
                return;
            }
            List<IntVec3> rootsToUnfog = MapGenerator.rootsToUnfog;
            rootsToUnfog.Add(centerCell);
        }
    }
}
