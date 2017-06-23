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
    /// <summary>
    /// A large wing or hallway that creates -> Workfloor & Chief's office / Interrogation chamber / Research station / Artifact storage 
    /// </summary>
    public class SymbolResolver_AgencyInvestigationWing : SymbolResolverAgencyWing
    {
        bool spawnedOffice = false;
        bool spawnedResearchStation = false;
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
                /////////////////////////// OFFICES ///////////////////////////

                if (!spawnedOffice)
                {
                    spawnedOffice = true;
                    ResolveParams resolveParamsOffice = rp;
                    resolveParamsOffice.wallStuff = defaultWallDef;
                    resolveParamsOffice.floorDef = defaultFloorDef;
                    resolveParamsOffice.rect = current;
                    ///ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                    BaseGen.symbolStack.Push("roof", resolveParamsOffice);
                    //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsOffice);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsOffice);
                    if (greaterWidth)
                    {
                        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                        {
                            BaseGen.symbolStack.Push("agencyOfficeSpaceH", resolveParamsOffice);
                        }
                        else BaseGen.symbolStack.Push("agencyOfficeSpace", resolveParamsOffice);
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyOfficeSpace", resolveParamsOffice);
                        }
                        else BaseGen.symbolStack.Push("agencyOfficeSpaceH", resolveParamsOffice);
                    }
                    BaseGen.symbolStack.Push("floor", resolveParamsOffice);
                    BaseGen.symbolStack.Push("clear", resolveParamsOffice);
                    continue;
                }

                /////////////////////////// RESEARCH ///////////////////////////////

                if (!spawnedResearchStation)
                {
                    spawnedResearchStation = true;
                    ResolveParams resolveParamsLab = rp;
                    resolveParamsLab.rect = current;
                    resolveParamsLab.floorDef = defaultFloorDef;
                    resolveParamsLab.wallStuff = defaultWallDef;
                    ///ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                        BaseGen.symbolStack.Push("roof", resolveParamsLab);
                    //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsLab);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsLab);
                    BaseGen.symbolStack.Push("fillWithResearchBenches", resolveParamsLab);
                    //BaseGen.symbolStack.Push("barracks", resolveParamsOffice);
                    BaseGen.symbolStack.Push("floor", resolveParamsLab);
                    BaseGen.symbolStack.Push("clear", resolveParamsLab);
                    continue;
                }

                ///////////////////////// INTERROGATIONS /////////////////

                ///ROOF
                ResolveParams resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                resolveParamsTemp.wallStuff = defaultWallDef;
                resolveParamsTemp.floorDef = defaultFloorDef;
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                if (greaterWidth)
                {
                    if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                    {
                        BaseGen.symbolStack.Push("agencyInterrogationsH", resolveParamsTemp);
                    }
                    else BaseGen.symbolStack.Push("agencyInterrogations", resolveParamsTemp);
                }
                else
                {
                    if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                    {
                        BaseGen.symbolStack.Push("agencyInterrogations", resolveParamsTemp);
                    }
                    else BaseGen.symbolStack.Push("agencyInterrogationsH", resolveParamsTemp);
                }
                BaseGen.symbolStack.Push("floor", resolveParamsTemp);
                BaseGen.symbolStack.Push("clear", resolveParamsTemp);
            }
            spawnedOffice = false;
            spawnedResearchStation = false;
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
