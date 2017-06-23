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
    public class SymbolResolver_AgencyCells : SymbolResolverAgency
    {
        public virtual bool horizontalHallway
        {
            get { return false; }
        }

        public override void PassingParameters(ResolveParams rp)
        {

            Map map = BaseGen.globalSettings.map;

            //Faction values are read in reverse of their push.
            // Programmed like: Checkpoint doors -> Lobby -> Edge Walls -> Floor -> Clear
            // Read Like: Clear -> Floor -> Edge Walls -> Lobby -> Chokepoint doors
            //
            // Tip:
            // Roofs and doors must be defined in the program first to avoid region errors.

            ///ROOF
            //if (!rp.noRoof.HasValue || !rp.noRoof.Value)
            //{
            //    BaseGen.symbolStack.Push("roof", rp);
            //}

            CellRect hallway = new CellRect();
            IEnumerable<IntVec3> doorLocs = new List<IntVec3>();
            IEnumerable<CellRect> splitRooms = Utility.FourWaySplit(rp.rect, out hallway, out doorLocs, 2, horizontalHallway);


            ResolveParams hallwayParams = rp;
            hallwayParams.rect = hallway;

            foreach (IntVec3 doorLoc in doorLocs)
            {
                ResolveParams resolveParams3 = rp;
                resolveParams3.rect = CellRect.SingleCell(doorLoc);
                resolveParams3.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
                BaseGen.symbolStack.Push("thing", resolveParams3);
            }
            foreach (CellRect current in splitRooms.InRandomOrder<CellRect>())
            {
                ResolveParams resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                BaseGen.symbolStack.Push("fillCells", resolveParamsTemp);
                UnfogRoomCenter(current.CenterCell);
            }

            Utility.RectReport(rp.rect);

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
