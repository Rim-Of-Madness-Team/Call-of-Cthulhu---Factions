using System;
using Verse;
using RimWorld.BaseGen;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using UnityEngine;

namespace CthulhuFactions
{
    /// <summary>
    /// A large wing or hallway that creates -> Containment cells / Storage
    /// </summary>
    public class SymbolResolver_AgencyLivingQuartersWing : SymbolResolverAgencyWing
    {
        bool spawnedCells = false;
        bool spawnedStorage = false;
        bool greaterWidth = false;

        public override void PostPassingParameters(ResolveParams rp)
        {
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
                /////////////////////////// Sleeping Quarters ///////////////////////////

                //if (!spawnedCells)
                //{
                //    spawnedCells = true;
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
                            BaseGen.symbolStack.Push("agencyBedroomsH", resolveParamsCells);
                        }
                        else BaseGen.symbolStack.Push("agencyBedrooms", resolveParamsCells);
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyBedrooms", resolveParamsCells);
                        }
                        else BaseGen.symbolStack.Push("agencyBedroomsH", resolveParamsCells);
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
            Utility.RectReport(rp.rect);

        }
    }
}
