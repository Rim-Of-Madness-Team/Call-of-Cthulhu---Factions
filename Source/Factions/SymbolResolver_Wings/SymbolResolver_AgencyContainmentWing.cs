using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    /// <summary>
    ///     A large wing or hallway that creates -> Containment cells / Storage
    /// </summary>
    public class SymbolResolver_AgencyContainmentWing : SymbolResolverAgencyWing
    {
        private bool greaterWidth;
        private bool spawnedCells;
        private bool spawnedStorage;

        public override TerrainDef DefaultFloorDef => TerrainDefOf.Concrete;

        public override void PostPassingParameters(ResolveParams rp)
        {
            rp.faction = Find.FactionManager.AllFactions.FirstOrDefault(x => x.def.defName == "TheAgency");

            //BaseGen.symbolStack.Push("agencyDoorsNSEW", rp);

            // If we're wider, then we're horizontal.
            // This is later passed so we can have vertical hallways intersect our center.
            if (rp.rect.Width > rp.rect.Height)
            {
                greaterWidth = true;
            }

            var rectRots = new List<CellRectRot>(FactionUtility.AdjacentHallwayAreas(rp.rect, out var doorLocs));

            foreach (var doorLoc in doorLocs)
            {
                var resolveParamsDoor = rp;
                resolveParamsDoor.rect = CellRect.SingleCell(doorLoc);
                resolveParamsDoor.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
                BaseGen.symbolStack.Push("thing", resolveParamsDoor);
            }

            foreach (var rectrot in rectRots.InRandomOrder())
            {
                var current = rectrot.Rect;
                /////////////////////////// Containment Cells ///////////////////////////

                if (!spawnedCells)
                {
                    spawnedCells = true;
                    var resolveParamsCells = rp;
                    resolveParamsCells.wallStuff = DefaultWallDef;
                    resolveParamsCells.floorDef = DefaultFloorDef;
                    resolveParamsCells.rect = current;
                    //ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                    {
                        BaseGen.symbolStack.Push("roof", resolveParamsCells);
                    }

                    //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsCells);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsCells);
                    if (greaterWidth)
                    {
                        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                        {
                            BaseGen.symbolStack.Push("agencyCellsH", resolveParamsCells);
                        }
                        else
                        {
                            BaseGen.symbolStack.Push("agencyCells", resolveParamsCells);
                        }
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyCells", resolveParamsCells);
                        }
                        else
                        {
                            BaseGen.symbolStack.Push("agencyCellsH", resolveParamsCells);
                        }
                    }

                    BaseGen.symbolStack.Push("floor", resolveParamsCells);
                    continue;
                }

                /////////////////////////// Storage ///////////////////////////////

                if (!spawnedStorage)
                {
                    spawnedStorage = true;
                    var resolveParamsCells = rp;
                    resolveParamsCells.floorDef = DefaultFloorDef;
                    resolveParamsCells.wallStuff = DefaultWallDef;
                    resolveParamsCells.rect = current;
                    //ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                    {
                        BaseGen.symbolStack.Push("roof", resolveParamsCells);
                    }

                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsCells);
                    if (greaterWidth)
                    {
                        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                        {
                            BaseGen.symbolStack.Push("agencyStorageH", resolveParamsCells);
                        }
                        else
                        {
                            BaseGen.symbolStack.Push("agencyStorage", resolveParamsCells);
                        }
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyStorage", resolveParamsCells);
                        }
                        else
                        {
                            BaseGen.symbolStack.Push("agencyStorageH", resolveParamsCells);
                        }
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

                //ROOF
                var resolveParamsMERF = rp;
                resolveParamsMERF.rect = current;
                resolveParamsMERF.floorDef = DefaultFloorDef;
                resolveParamsMERF.wallStuff = DefaultWallDef;
                //ROOF
                if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                {
                    BaseGen.symbolStack.Push("roof", resolveParamsMERF);
                }

                //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsMERF);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsMERF);
                BaseGen.symbolStack.Push("agencyFillMERF", resolveParamsMERF);
                //BaseGen.symbolStack.Push("barracks", resolveParamsOffice);
                BaseGen.symbolStack.Push("floor", resolveParamsMERF);
                BaseGen.symbolStack.Push("clear", resolveParamsMERF);
            }

            spawnedCells = false;
            spawnedStorage = false;
            FactionUtility.RectReport(rp.rect);
        }
    }
}