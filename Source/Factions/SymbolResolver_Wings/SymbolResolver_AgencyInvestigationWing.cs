using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;
//using CustomFactionBase;

namespace CthulhuFactions
{
    /// <summary>
    ///     A large wing or hallway that creates -> Workfloor & Chief's office / Interrogation chamber / Research station /
    ///     Artifact storage
    /// </summary>
    public class SymbolResolver_AgencyInvestigationWing : SymbolResolverAgencyWing
    {
        private bool greaterWidth;
        private bool spawnedOffice;
        private bool spawnedResearchStation;

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
                /////////////////////////// OFFICES ///////////////////////////

                if (!spawnedOffice)
                {
                    spawnedOffice = true;
                    var resolveParamsOffice = rp;
                    resolveParamsOffice.wallStuff = DefaultWallDef;
                    resolveParamsOffice.floorDef = DefaultFloorDef;
                    resolveParamsOffice.rect = current;
                    //ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                    {
                        BaseGen.symbolStack.Push("roof", resolveParamsOffice);
                    }

                    //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsOffice);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsOffice);
                    if (greaterWidth)
                    {
                        if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                        {
                            BaseGen.symbolStack.Push("agencyOfficeSpaceH", resolveParamsOffice);
                        }
                        else
                        {
                            BaseGen.symbolStack.Push("agencyOfficeSpace", resolveParamsOffice);
                        }
                    }
                    else
                    {
                        if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                        {
                            BaseGen.symbolStack.Push("agencyOfficeSpace", resolveParamsOffice);
                        }
                        else
                        {
                            BaseGen.symbolStack.Push("agencyOfficeSpaceH", resolveParamsOffice);
                        }
                    }

                    BaseGen.symbolStack.Push("floor", resolveParamsOffice);
                    BaseGen.symbolStack.Push("clear", resolveParamsOffice);
                    continue;
                }

                /////////////////////////// RESEARCH ///////////////////////////////

                if (!spawnedResearchStation)
                {
                    spawnedResearchStation = true;
                    var resolveParamsLab = rp;
                    resolveParamsLab.rect = current;
                    resolveParamsLab.floorDef = DefaultFloorDef;
                    resolveParamsLab.wallStuff = DefaultWallDef;
                    //ROOF
                    if (!rp.noRoof.HasValue || !rp.noRoof.Value)
                    {
                        BaseGen.symbolStack.Push("roof", resolveParamsLab);
                    }

                    //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsLab);
                    BaseGen.symbolStack.Push("edgeWalls", resolveParamsLab);
                    BaseGen.symbolStack.Push("fillWithResearchBenches", resolveParamsLab);
                    //BaseGen.symbolStack.Push("barracks", resolveParamsOffice);
                    BaseGen.symbolStack.Push("floor", resolveParamsLab);
                    BaseGen.symbolStack.Push("clear", resolveParamsLab);
                    continue;
                }

                ///////////////////////// INTERROGATIONS /////////////////

                //ROOF
                var resolveParamsTemp = rp;
                resolveParamsTemp.rect = current;
                resolveParamsTemp.wallStuff = DefaultWallDef;
                resolveParamsTemp.floorDef = DefaultFloorDef;
                BaseGen.symbolStack.Push("roof", resolveParamsTemp);
                //BaseGen.symbolStack.Push("agencyDoorsNSEW", resolveParamsTemp);
                BaseGen.symbolStack.Push("edgeWalls", resolveParamsTemp);
                if (greaterWidth)
                {
                    if (rectrot.Rot == Rot4.West || rectrot.Rot == Rot4.East)
                    {
                        BaseGen.symbolStack.Push("agencyInterrogationsH", resolveParamsTemp);
                    }
                    else
                    {
                        BaseGen.symbolStack.Push("agencyInterrogations", resolveParamsTemp);
                    }
                }
                else
                {
                    if (rectrot.Rot == Rot4.North || rectrot.Rot == Rot4.South)
                    {
                        BaseGen.symbolStack.Push("agencyInterrogations", resolveParamsTemp);
                    }
                    else
                    {
                        BaseGen.symbolStack.Push("agencyInterrogationsH", resolveParamsTemp);
                    }
                }

                BaseGen.symbolStack.Push("floor", resolveParamsTemp);
                BaseGen.symbolStack.Push("clear", resolveParamsTemp);
            }

            spawnedOffice = false;
            spawnedResearchStation = false;
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