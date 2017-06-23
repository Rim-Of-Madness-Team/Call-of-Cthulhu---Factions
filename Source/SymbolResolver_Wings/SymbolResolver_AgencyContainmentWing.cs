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
    /// <summary>
    /// A large wing or hallway that creates -> Containment cells / Storage
    /// </summary>
    public class SymbolResolver_AgencyContainmentWing : SymbolResolverAgencyWing
    {
        bool spawnedCells = false;
        bool spawnedStorage = false;
        bool greaterWidth = false;

        public override TerrainDef defaultFloorDef
        {
            get
            {
                return TerrainDefOf.Concrete;
            }
        }

        public override void PostPassingParameters(ResolveParams rp)
        {
            rp.faction = Find.FactionManager.AllFactions.FirstOrDefault((Faction x) => x.def.defName == "TheAgency");

            //BaseGen.symbolStack.Push("agencyDoorsNSEW", rp);

            // If we're wider, then we're horizontal.
            // This is later passed so we can have vertical hallways intersect our center.
            if (rp.rect.Width > rp.rect.Height) greaterWidth = true;



            IEnumerable<IntVec3> doorLocs = new List<IntVec3>();
            List<CellRectRot> rectRots = new List<CellRectRot>(Utility.AdjacentHallwayAreas(rp.rect, out doorLocs));

            foreach (IntVec3 doorLoc in doorLocs)
            {
                ResolveParams resolveParamsDoor = rp;
                resolveParamsDoor.rect = CellRect.SingleCell(doorLoc);
                resolveParamsDoor.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
                BaseGen.symbolStack.Push("thing", resolveParamsDoor);

            }

            foreach (CellRectRot rectrot in rectRots.InRandomOrder<CellRectRot>())
            {
                CellRect current = rectrot.Rect;
                /////////////////////////// Containment Cells ///////////////////////////

                if (!spawnedCells)
                {
                    spawnedCells = true;
                    ResolveParams resolveParamsCells = rp;
                    resolveParamsCells.wallStuff = defaultWallDef;
                    resolveParamsCells.floorDef = defaultFloorDef;
                    resolveParamsCells.rect = current;
                    ///ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                        BaseGen.symbolStack.Push("roof", resolveParamsCells);
                    //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsCells);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsCells);
                    if (greaterWidth)
                    {
                        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                        {
                            BaseGen.symbolStack.Push("agencyCellsH", resolveParamsCells);
                        }
                        else BaseGen.symbolStack.Push("agencyCells", resolveParamsCells);
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyCells", resolveParamsCells);
                        }
                        else BaseGen.symbolStack.Push("agencyCellsH", resolveParamsCells);
                    }
                    BaseGen.symbolStack.Push("floor", resolveParamsCells);
                    continue;
                }

                /////////////////////////// Storage ///////////////////////////////

                if (!spawnedStorage)
                {
                    spawnedStorage = true;
                    ResolveParams resolveParamsCells = rp;
                    resolveParamsCells.floorDef = defaultFloorDef;
                    resolveParamsCells.wallStuff = defaultWallDef;
                    resolveParamsCells.rect = current;
                    ///ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                        BaseGen.symbolStack.Push("roof", resolveParamsCells);

                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsCells);
                    if (greaterWidth)
                    {
                        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                        {
                            BaseGen.symbolStack.Push("agencyStorageH", resolveParamsCells);
                        }
                        else BaseGen.symbolStack.Push("agencyStorage", resolveParamsCells);
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyStorage", resolveParamsCells);
                        }
                        else BaseGen.symbolStack.Push("agencyStorageH", resolveParamsCells);
                    }
                    BaseGen.symbolStack.Push("floor", resolveParamsCells);
                    continue;
                }

                ///////////////////////// MERF OPERATIONS ROOM /////////////////

                //Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(rp.faction, new LordJob_DefendBase(rp.faction, current.CenterCell), map, null);
                //ResolveParams resolveParams = rp;
                //resolveParams.rect = current.ContractedBy(2);
                //for (int i = 0; i < Rand.Range(4, 8); i++)
                //{
                //    Cthulhu.Utility.SpawnPawnsOfCountAt(PawnKindDef.Named("MERF"), resolveParams.rect.Cells.RandomElement<IntVec3>(), map, 1, rp.faction);
                //}

                ///ROOF
                ResolveParams resolveParamsMERF = rp;
                resolveParamsMERF.rect = current;
                resolveParamsMERF.floorDef = defaultFloorDef;
                resolveParamsMERF.wallStuff = defaultWallDef;
                ///ROOF
                if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                    BaseGen.symbolStack.Push("roof", resolveParamsMERF);
                //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsMERF);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsMERF);
                BaseGen.symbolStack.Push("agencyFillMERF", resolveParamsMERF);
                //BaseGen.symbolStack.Push("barracks", resolveParamsOffice);
                BaseGen.symbolStack.Push("floor", resolveParamsMERF);
                BaseGen.symbolStack.Push("clear", resolveParamsMERF);
            }
            spawnedCells = false;
            spawnedStorage = false;
            Utility.RectReport(rp.rect);

        }
    }
}
