using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    /// <summary>
    ///     A large wing or hallway that creates -> Containment cells / Storage
    /// </summary>
    public class SymbolResolver_AgencyLivingQuartersWing : SymbolResolverAgencyWing
    {
        private bool greaterWidth;
        private bool spawnedCells;
        private bool spawnedStorage;

        public override void PostPassingParameters(ResolveParams rp)
        {
            //BaseGen.symbolStack.Push("agencyDoorsNSEW", rp);

            // If we're wider, then we're horizontal.
            // This is later passed so we can have vertical hallways intersect our center.
            if (rp.rect.Width > rp.rect.Height)
            {
                greaterWidth = true;
            }

            _ = new List<IntVec3>();
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
                /////////////////////////// Sleeping Quarters ///////////////////////////

                //if (!spawnedCells)
                //{
                //    spawnedCells = true;
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
                        BaseGen.symbolStack.Push("agencyBedroomsH", resolveParamsCells);
                    }
                    else
                    {
                        BaseGen.symbolStack.Push("agencyBedrooms", resolveParamsCells);
                    }
                }
                else
                {
                    if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                    {
                        BaseGen.symbolStack.Push("agencyBedrooms", resolveParamsCells);
                    }
                    else
                    {
                        BaseGen.symbolStack.Push("agencyBedroomsH", resolveParamsCells);
                    }
                }

                BaseGen.symbolStack.Push("floor", resolveParamsCells);
                //    continue;
                //}

                /////////////////////////// Mess Hall ///////////////////////////////

                //if (!spawnedStorage)
                //{
                //    spawnedStorage = true;
                //    ResolveParams resolveParamsCells = rp;
                //    resolveParamsCells.wallStuff = defaultWallDef;
                //    resolveParamsCells.floorDef = defaultFloorDef;
                //    resolveParamsCells.rect = current;
                //    ///ROOF
                //    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                //        BaseGen.symbolStack.Push("roof", resolveParamsCells);

                //    BaseGen.symbolStack.Push("edgeWalls", resolveParamsCells);
                //    if (greaterWidth)
                //    {
                //        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                //        {
                //            BaseGen.symbolStack.Push("agencyMessHallH", resolveParamsCells);
                //        }
                //        else BaseGen.symbolStack.Push("agencyMessHall", resolveParamsCells);
                //    }
                //    else
                //    {
                //        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                //        {
                //            BaseGen.symbolStack.Push("agencyMessHall", resolveParamsCells);
                //        }
                //        else BaseGen.symbolStack.Push("agencyMessHallH", resolveParamsCells);
                //    }
                //    BaseGen.symbolStack.Push("floor", resolveParamsCells);
                //    continue;
                //}

                ///////////////////////// Manufacture /////////////////

                /////ROOF
                //ResolveParams resolveParamsTemp = rp;
                //resolveParamsTemp.rect = current;
                //resolveParamsTemp.wallStuff = defaultWallDef;
                //resolveParamsTemp.floorDef = defaultFloorDef;
                //BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                ////BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsTemp);
                //BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                //if (greaterWidth)
                //{
                //    if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                //    {
                //        BaseGen.symbolStack.Push("agencyInterrogationsH", resolveParamsTemp);
                //    }
                //    else BaseGen.symbolStack.Push("agencyInterrogations", resolveParamsTemp);
                //}
                //else
                //{
                //    if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                //    {
                //        BaseGen.symbolStack.Push("agencyInterrogations", resolveParamsTemp);
                //    }
                //    else BaseGen.symbolStack.Push("agencyInterrogationsH", resolveParamsTemp);
                //}
                //BaseGen.symbolStack.Push("floor", resolveParamsTemp);
            }

            spawnedCells = false;
            spawnedStorage = false;
            FactionUtility.RectReport(rp.rect);
        }
    }
}