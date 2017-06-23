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
    public class SymbolResolver_AgencyLobby : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {

            Map map = BaseGen.globalSettings.map;


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
            //ResolveParams resolveParams0 = rp;
            //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParams0);
            
            ///East Wing
            ResolveParams eastParams = rp;
            IntVec3 doorLoc = IntVec3.Invalid;
            eastParams.rect = CthulhuFactions.Utility.AdjacentRectMaker(rp.rect, Rot4.East, out doorLoc, (int)RectSizeValues.HALLWAYLENGTH, (int)RectSizeValues.HALLWAYSIZE);
            ResolveParams resolveParamsDoor1 = rp;
            resolveParamsDoor1.rect = CellRect.SingleCell(doorLoc);
            resolveParamsDoor1.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor1);
            BaseGen.symbolStack.Push("agencyContainmentWing", eastParams);
            
            ///West Wing
            ResolveParams westParams = rp;
            IntVec3 doorLoc2 = IntVec3.Invalid;
            westParams.rect = CthulhuFactions.Utility.AdjacentRectMaker(rp.rect, Rot4.West, out doorLoc2, (int)RectSizeValues.HALLWAYLENGTH, (int)RectSizeValues.HALLWAYSIZE);
            ResolveParams resolveParamsDoor2 = rp;
            resolveParamsDoor2.rect = CellRect.SingleCell(doorLoc2);
            resolveParamsDoor2.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor2);
            BaseGen.symbolStack.Push("agencyInvestigationWing", westParams);

            ///North Wing
            ///Hallway
            ResolveParams northParams = rp;
            IntVec3 doorLoc3 = IntVec3.Invalid;
            northParams.rect = CthulhuFactions.Utility.AdjacentRectMaker(rp.rect, Rot4.North, out doorLoc3, (int)RectSizeValues.HALLWAYSIZE, (int)RectSizeValues.HALLWAYLENGTH);
            Thing thing3 = ThingMaker.MakeThing(CthulhuFactionsDefOf.ROM_TemporaryDoorMarker, null);
            ResolveParams resolveParamsDoor3 = rp;
            resolveParamsDoor3.rect = CellRect.SingleCell(doorLoc3);
            resolveParamsDoor3.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor3);

            ///More
            ResolveParams northParams2 = northParams;
            IntVec3 doorLoc4 = IntVec3.Invalid;
            northParams2.rect = CthulhuFactions.Utility.AdjacentRectMaker(northParams.rect, Rot4.North, out doorLoc4, (int)RectSizeValues.HALLWAYSIZE, (int)RectSizeValues.HALLWAYLENGTH);
            ResolveParams resolveParamsDoor4 = rp;
            resolveParamsDoor4.rect = CellRect.SingleCell(doorLoc4);
            resolveParamsDoor4.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", resolveParamsDoor4);


            BaseGen.symbolStack.Push("edgeWalls", northParams);
            BaseGen.symbolStack.Push("floor", northParams);

            BaseGen.symbolStack.Push("agencyLivingQuartersWing", northParams2);

            ///Fill Lobby
            ResolveParams fillParams = rp;
            BaseGen.symbolStack.Push("agencyFillLobby", fillParams);

            ///CLEAR/FLOOR/WALLS
            ResolveParams resolveParams2 = rp;
            resolveParams2.wallStuff = defaultWallDef;
            BaseGen.symbolStack.Push("edgeWalls", resolveParams2);
            ResolveParams resolveParams3 = rp;
            resolveParams3.floorDef = defaultFloorDef;
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
