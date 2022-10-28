using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;
//using CustomFactionBase;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyChokepoint : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;


            //Manual Override this mofo
            var size = 9;
            var minX = rp.rect.CenterCell.x - size;
            _ = rp.rect.CenterCell.x + size;
            var minZ = rp.rect.CenterCell.z - size;
            _ = rp.rect.CenterCell.z + size;


            var rectOverride = new CellRect(minX, minZ - (int) RectSizeValues.BASEOFFSET, size, size);
            rp.rect = rectOverride;
            UnfogRoomCenter(rp.rect.CenterCell);

            //Bring in MERF
            var singlePawnLord = rp.singlePawnLord ??
                                 LordMaker.MakeNewLord(rp.faction,
                                     new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map);

            var resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.Combat; //CthulhuFactionsDefOf.ROM_AgencyMERF;
            float points = 5000;
            resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms
            {
                tile = map.Tile,
                faction = rp.faction,
                points = points
            };

            BaseGen.symbolStack.Push("pawnGroup", resolveParams);


            var thingDef = ThingDefOf.Steel;
            var floorDef = TerrainDefOf.MetalTile;

            //Faction values are read in reverse of their push.
            // Programmed like: Checkpoint doors -> Lobby -> Edge Walls -> Floor -> Clear
            // Read Like: Clear -> Floor -> Edge Walls -> Lobby -> Chokepoint doors
            //
            // Tip:
            // Doors must be defined in the program first to avoid region errors.

            //ROOF
            if (!rp.noRoof.HasValue || !rp.noRoof.Value)
            {
                BaseGen.symbolStack.Push("roof", rp);
            }

            //DOORS
            var resolveParams0 = rp;
            BaseGen.symbolStack.Push("agencyDoorsNS", resolveParams0);

            //LOBBY VALUES
            var superParams = rp;
            _ = IntVec3.Invalid;
            superParams.rect = FactionUtility.AdjacentRectMaker(rp.rect, Rot4.North, out var doorLoc,
                (int) RectSizeValues.LOBBY, (int) RectSizeValues.LOBBYHEIGHT);

            var resolveParamsDoor = rp;
            resolveParamsDoor.rect = CellRect.SingleCell(doorLoc);
            resolveParamsDoor.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor);

            BaseGen.symbolStack.Push("agencyLobby", superParams);
            //CustomBaseUtility.replaceFloor(superParams, Utility.factionFloorStuff);
            //CustomBaseUtility.replaceWalls(superParams, Utility.factionWallStuff);

            //CLEAR/FLOOR/WALLS
            var resolveParams2 = rp;
            resolveParams2.wallStuff = thingDef;
            BaseGen.symbolStack.Push("edgeWalls", resolveParams2);
            var resolveParams3 = rp;
            resolveParams3.floorDef = floorDef;
            BaseGen.symbolStack.Push("floor", resolveParams3);
            BaseGen.symbolStack.Push("clear", resolveParams);

            //CustomBaseUtility.replaceFloor(resolveParams3, Utility.factionFloorStuff);
            //CustomBaseUtility.replaceWalls(resolveParams3, Utility.factionWallStuff);

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