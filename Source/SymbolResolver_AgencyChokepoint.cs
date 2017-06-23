using System;
using Verse;
using RimWorld.BaseGen;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using UnityEngine;
using CustomFactionBase;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyChokepoint : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {

            Map map = BaseGen.globalSettings.map;
            

            //Manual Override this mofo
            int size = 9;
            int minX = rp.rect.CenterCell.x - size;
            int maxX = rp.rect.CenterCell.x + size;
            int minZ = rp.rect.CenterCell.z - size;
            int maxZ = rp.rect.CenterCell.z + size;
            

            CellRect rectOverride = new CellRect(minX, minZ - (int)RectSizeValues.BASEOFFSET, size, size);
            rp.rect = rectOverride;
            UnfogRoomCenter(rp.rect.CenterCell);

            //Bring in MERF
            Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(rp.faction, new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map, null);
            
            ResolveParams resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = CthulhuFactionsDefOf.ROM_AgencyMERF;
                float points = 5000;
                resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
                resolveParams.pawnGroupMakerParams.tile = map.Tile;
                resolveParams.pawnGroupMakerParams.faction = rp.faction;
                resolveParams.pawnGroupMakerParams.points = points;
            
            BaseGen.symbolStack.Push("pawnGroup", resolveParams);



            ThingDef thingDef = ThingDefOf.Steel;
            TerrainDef floorDef = TerrainDefOf.MetalTile;

            //Faction values are read in reverse of their push.
            // Programmed like: Checkpoint doors -> Lobby -> Edge Walls -> Floor -> Clear
            // Read Like: Clear -> Floor -> Edge Walls -> Lobby -> Chokepoint doors
            //
            // Tip:
            // Doors must be defined in the program first to avoid region errors.

            ///ROOF
            if (!rp.noRoof.HasValue || !rp.noRoof.Value)
            {
                BaseGen.symbolStack.Push("roof", rp);
            }

            ///DOORS
            ResolveParams resolveParams0 = rp;
            BaseGen.symbolStack.Push("agencyDoorsNS", resolveParams0);

            ///LOBBY VALUES
            ResolveParams superParams = rp;
            IntVec3 doorLoc = IntVec3.Invalid;
            superParams.rect = CthulhuFactions.Utility.AdjacentRectMaker(rp.rect, Rot4.North, out doorLoc, (int)RectSizeValues.LOBBY, (int)RectSizeValues.LOBBYHEIGHT);

            ResolveParams resolveParamsDoor = rp;
            resolveParamsDoor.rect = CellRect.SingleCell(doorLoc);
            resolveParamsDoor.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor);

            BaseGen.symbolStack.Push("agencyLobby", superParams);
            //CustomBaseUtility.replaceFloor(superParams, Utility.factionFloorStuff);
            //CustomBaseUtility.replaceWalls(superParams, Utility.factionWallStuff);

            ///CLEAR/FLOOR/WALLS
            ResolveParams resolveParams2 = rp;
            resolveParams2.wallStuff = thingDef;
            BaseGen.symbolStack.Push("edgeWalls", resolveParams2);
            ResolveParams resolveParams3 = rp;
            resolveParams3.floorDef = floorDef;
            BaseGen.symbolStack.Push("floor", resolveParams3);
            BaseGen.symbolStack.Push("clear", resolveParams);

            //CustomBaseUtility.replaceFloor(resolveParams3, Utility.factionFloorStuff);
            //CustomBaseUtility.replaceWalls(resolveParams3, Utility.factionWallStuff);

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
