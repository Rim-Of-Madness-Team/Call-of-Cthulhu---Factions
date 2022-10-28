using RimWorld.BaseGen;
using Verse;
//using CustomFactionBase;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyLobby : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            _ = BaseGen.globalSettings.map;


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
            //ResolveParams resolveParams0 = rp;
            //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParams0);

            //East Wing
            var eastParams = rp;
            _ = IntVec3.Invalid;
            eastParams.rect = FactionUtility.AdjacentRectMaker(rp.rect, Rot4.East, out var doorLoc,
                (int) RectSizeValues.HALLWAYLENGTH, (int) RectSizeValues.HALLWAYSIZE);
            var resolveParamsDoor1 = rp;
            resolveParamsDoor1.rect = CellRect.SingleCell(doorLoc);
            resolveParamsDoor1.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor1);
            BaseGen.symbolStack.Push("agencyContainmentWing", eastParams);

            //West Wing
            var westParams = rp;
            _ = IntVec3.Invalid;
            westParams.rect = FactionUtility.AdjacentRectMaker(rp.rect, Rot4.West, out var doorLoc2,
                (int) RectSizeValues.HALLWAYLENGTH, (int) RectSizeValues.HALLWAYSIZE);
            var resolveParamsDoor2 = rp;
            resolveParamsDoor2.rect = CellRect.SingleCell(doorLoc2);
            resolveParamsDoor2.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor2);
            BaseGen.symbolStack.Push("agencyInvestigationWing", westParams);

            //North Wing
            //Hallway
            var northParams = rp;
            _ = IntVec3.Invalid;
            northParams.rect = FactionUtility.AdjacentRectMaker(rp.rect, Rot4.North, out var doorLoc3,
                (int) RectSizeValues.HALLWAYSIZE, (int) RectSizeValues.HALLWAYLENGTH);
            _ = ThingMaker.MakeThing(CthulhuFactionsDefOf.ROM_TemporaryDoorMarker);
            var resolveParamsDoor3 = rp;
            resolveParamsDoor3.rect = CellRect.SingleCell(doorLoc3);
            resolveParamsDoor3.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor3);

            //More
            var northParams2 = northParams;
            _ = IntVec3.Invalid;
            northParams2.rect = FactionUtility.AdjacentRectMaker(northParams.rect, Rot4.North, out var doorLoc4,
                (int) RectSizeValues.HALLWAYSIZE, (int) RectSizeValues.HALLWAYLENGTH);
            var resolveParamsDoor4 = rp;
            resolveParamsDoor4.rect = CellRect.SingleCell(doorLoc4);
            resolveParamsDoor4.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor4);


            BaseGen.symbolStack.Push("edgeWalls", northParams);
            BaseGen.symbolStack.Push("floor", northParams);

            BaseGen.symbolStack.Push("agencyLivingQuartersWing", northParams2);

            //Fill Lobby
            BaseGen.symbolStack.Push("agencyFillLobby", rp);

            //CLEAR/FLOOR/WALLS
            var resolveParams2 = rp;
            resolveParams2.wallStuff = DefaultWallDef;
            BaseGen.symbolStack.Push("edgeWalls", resolveParams2);
            var resolveParams3 = rp;
            resolveParams3.floorDef = DefaultFloorDef;
            BaseGen.symbolStack.Push("floor", resolveParams3);

            //ResolveParams resolveParams5 = rp;
            //IntVec3 bldLoc = resolveParams0.rect.CenterCell;
            //Thing regBar = ThingMaker.MakeThing(ThingDefOf.TemporaryRegionBarrier);
            //GenSpawn.Spawn(regBar, bldLoc, map);
            //GenSpawn.Spawn(ThingMaker.MakeThing(ThingDef.Named("Factions_AgencySymbol"), null), bldLoc, map);
            //regBar.Destroy(DestroyMode.Vanish);
        }
    }
}