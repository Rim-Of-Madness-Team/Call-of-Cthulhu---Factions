using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CthulhuFactions
{
    public static class RectSizeValues
    {
        public const float BASEOFFSET = 30;
        public const float CHOKEPOINT = 9;
        public const float LOBBY = 19;
        public const float LOBBYHEIGHT = 19;
        public const float HALLWAYLENGTH = 19;
        public const float HALLWAYSIZE = 5;
        public const float CENTERDESK = 8;
        public const float LOGO = 8;
        public const float SITTINGAREA = 4;
    }

    public static class Utility
    {
        public static ThingDef factionWallStuff = ThingDefOf.Steel;
        public static TerrainDef factionFloorStuff = TerrainDefOf.MetalTile;

        public static IEnumerable<CellRect> TwoWaySplit(CellRect startRect, out CellRect hallway, out IEnumerable<IntVec3> doorLocs, int hallwayOffset = 0, bool horizontalHallway = false)
        {
            List<CellRect> result = new List<CellRect>();
            List<IntVec3> doorLocResults = new List<IntVec3>();
            hallway = new CellRect();
            int horizontalOffset = horizontalHallway ? hallwayOffset : 0;
            int verticalOffset = horizontalHallway ? 0 : hallwayOffset;

            int z = startRect.CenterCell.z;
            CellRect half1 = new CellRect(startRect.minX, startRect.minZ, startRect.Width, (z - (startRect.minZ) + 1) - horizontalOffset);
            CellRect half2 = new CellRect(startRect.minX, z + horizontalOffset, startRect.Width, ((startRect.maxZ - z) + 1) - horizontalOffset);
            Log.Message("Half1");
            RectReport(half1);
            Log.Message("Half2");
            RectReport(half2);
            
            if (!horizontalHallway) doorLocResults.Add(CellRectBottomMiddle(half1));
            else doorLocResults.Add(CellRectRightMiddle(half1));
            
            result.Add(half1);
            result.Add(half2);
            if (hallwayOffset != 0)
            {
                if (horizontalHallway)
                {
                    hallway = new CellRect(startRect.minX, (z - horizontalOffset), startRect.Width, (horizontalOffset * 2));
                }
                else hallway = new CellRect((startRect.CenterCell.x - verticalOffset), startRect.minZ, (verticalOffset * 2), startRect.Height);
                if (hallway != null) { Log.Message("Hallway"); RectReport(hallway); }
            }
            foreach (CellRect current in result) RectReport(current);

            doorLocs = new List<IntVec3>(doorLocResults);

            return result;
        }

        public static IEnumerable<CellRect> FourWaySplit(CellRect startRect, out CellRect hallway, out IEnumerable<IntVec3> doorLocs, int hallwayOffset = 0, bool horizontalHallway = false)
        {
            List<CellRect> result = new List<CellRect>();
            List<IntVec3> doorLocResults = new List<IntVec3>();
            hallway = new CellRect();
            int horizontalOffset = horizontalHallway ? hallwayOffset : 0;
            int verticalOffset = horizontalHallway ? 0 : hallwayOffset;

            int z = startRect.CenterCell.z;
            CellRect half1 = new CellRect(startRect.minX, startRect.minZ, startRect.Width, (z - (startRect.minZ) + 1) - horizontalOffset);
            CellRect half2 = new CellRect(startRect.minX, z + horizontalOffset, startRect.Width, ((startRect.maxZ - z) + 1) - horizontalOffset);
                
            int x = half1.CenterCell.x;
            CellRect quarter1 = new CellRect(half1.minX, half1.minZ, ((x - half1.minX) + 1) - verticalOffset, half1.Height);
            if (!horizontalHallway) doorLocResults.Add(CellRectRightMiddle(quarter1));
            else doorLocResults.Add(CellRectTopMiddle(quarter1));

            CellRect quarter2 = new CellRect(x + verticalOffset, half1.minZ, (half1.maxX - x + 1) - verticalOffset, half1.Height);
            if (!horizontalHallway) doorLocResults.Add(CellRectLeftMiddle(quarter2));
            else doorLocResults.Add(CellRectTopMiddle(quarter2));

            x = half2.CenterCell.x;
            CellRect quarter3 = new CellRect(half2.minX, half2.minZ, ((x - half2.minX) + 1) - verticalOffset, half2.Height);
            if (!horizontalHallway) doorLocResults.Add(CellRectRightMiddle(quarter3));
            else doorLocResults.Add(CellRectBottomMiddle(quarter3));

            CellRect quarter4 = new CellRect(x + verticalOffset, half2.minZ, (half2.maxX - x + 1) - verticalOffset, half2.Height);
            if (!horizontalHallway) doorLocResults.Add(CellRectLeftMiddle(quarter4));
            else doorLocResults.Add(CellRectBottomMiddle(quarter4));

            result.Add(quarter1);
            result.Add(quarter2);
            result.Add(quarter3);
            result.Add(quarter4);
            if (hallwayOffset != 0)
            {
                if (horizontalHallway)
                {
                    hallway = new CellRect(startRect.minX, (z - horizontalOffset), startRect.Width, (horizontalOffset * 2));
                }
                else hallway = new CellRect((startRect.CenterCell.x - verticalOffset) , startRect.minZ, (verticalOffset * 2), startRect.Height);
                if (hallway != null) { Log.Message("Hallway"); RectReport(hallway); }
            }
            foreach (CellRect current in result) RectReport(current);

            doorLocs = new List<IntVec3>(doorLocResults);

            return result;
        }

        public static IntVec3 CellRectLeftMiddle(CellRect cellRect)
        {
            IntVec3 result = new IntVec3(cellRect.minX, 0, cellRect.CenterCell.z);
            Log.Message("LM :: " + result.ToString());
            return result;
        }
        public static IntVec3 CellRectRightMiddle(CellRect cellRect)
        {
            IntVec3 result = new IntVec3(cellRect.maxX, 0, cellRect.CenterCell.z);
            Log.Message("RM :: " + result.ToString());
            return result;
        }
        public static IntVec3 CellRectBottomMiddle(CellRect cellRect)
        {
            IntVec3 result = new IntVec3(cellRect.CenterCell.x, 0, cellRect.minZ);
            Log.Message("BM :: " + result.ToString());
            return result;
        }
        public static IntVec3 CellRectTopMiddle(CellRect cellRect)
        {
            IntVec3 result = new IntVec3(cellRect.CenterCell.x, 0, cellRect.maxZ);
            Log.Message("TM :: " + result.ToString());
            return result;
        }

        public static CellRect AdjacentRectMaker(CellRect startRect, Rot4 dir, out IntVec3 doorLoc, int newSizeX, int newSizeZ=0)
        {
            doorLoc = IntVec3.Invalid;

            CellRect adjRect = new CellRect();
            
            int oldX = startRect.GetEdgeCells(Rot4.North).Count<IntVec3>();
            int oldZ = startRect.GetEdgeCells(Rot4.West).Count<IntVec3>();

            ///Make a rectangle to the North?
            if (dir == Rot4.North)
            {
                int heightCalc = newSizeX;
                if (newSizeZ != 0) heightCalc = newSizeZ;

                int newMinX = startRect.CenterCell.x - (newSizeX / 2);
                int newMinZ = startRect.CenterCell.z + (oldZ / 2);

                adjRect = new CellRect(newMinX, newMinZ, newSizeX, heightCalc);
                doorLoc = CellRectTopMiddle(startRect);
            }
            //South?
            else if (dir == Rot4.South)
            {

                int heightCalc = newSizeX;
                if (newSizeZ != 0) heightCalc = newSizeZ;

                int newMinX = startRect.CenterCell.x - (newSizeX / 2);
                int newMinZ = (startRect.CenterCell.z - ((oldZ / 2) - 1)) - heightCalc;

                adjRect = new CellRect(newMinX, newMinZ, newSizeX, heightCalc);
                doorLoc = CellRectBottomMiddle(startRect);
            }
            //East?
            else if (dir == Rot4.East)
            {
                int heightCalc = newSizeX;
                if (newSizeZ != 0) heightCalc = newSizeZ;
                
                int newMinX = startRect.CenterCell.x + (oldX / 2);
                int newMinZ = startRect.CenterCell.z - (heightCalc / 2);

                adjRect = new CellRect(newMinX, newMinZ, newSizeX, heightCalc);
                doorLoc = CellRectRightMiddle(startRect);
            }
            //West?
            else
            {
                int heightCalc = newSizeX;
                if (newSizeZ != 0) heightCalc = newSizeZ;

                int newMinX = ((startRect.CenterCell.x - (oldX / 2)) - newSizeX) + 1;
                int newMinZ = startRect.CenterCell.z - (heightCalc / 2);

                adjRect = new CellRect(newMinX, newMinZ, newSizeX, heightCalc);
                doorLoc = CellRectLeftMiddle(startRect);
            }

            RectReport(adjRect);
            return adjRect;
        }

        public static IEnumerable<CellRectRot> AdjacentHallwayAreas(CellRect hallway, out IEnumerable<IntVec3> doorLocs)
        {
            List<IntVec3> doorLocsResult = new List<IntVec3>();
            IntVec3 doorLoc = IntVec3.Invalid;
            Map map = BaseGen.globalSettings.map;

            bool greaterWidth = false;
            List<CellRectRot> returnList = new List<CellRectRot>();
            if (hallway.Width > hallway.Height) greaterWidth = true;

            if (greaterWidth)
            {
                if (hallway.CenterCell.x <= map.Center.x)
                {
                    //Westbound
                    returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.West, out doorLoc, hallway.Width), Rot4.West));
                    doorLocsResult.Add(doorLoc);
                }
                else if (hallway.CenterCell.x >= map.Center.x)
                {
                    //Eastbound
                    returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.East, out doorLoc, hallway.Width), Rot4.East));
                    doorLocsResult.Add(doorLoc);
                }

                //Northbound
                returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.North, out doorLoc, hallway.Width), Rot4.North));
                doorLocsResult.Add(doorLoc);

                //Southbound
                returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.South, out doorLoc, hallway.Width), Rot4.South));
                doorLocsResult.Add(doorLoc);
            }
            else
            {
                if (hallway.CenterCell.z >= map.Center.z)
                {
                    //Northbound
                    returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.North, out doorLoc, hallway.Height), Rot4.North));
                    doorLocsResult.Add(doorLoc);
                }
                else if (hallway.CenterCell.z <= map.Center.z)
                {
                    //Southbound
                    returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.South, out doorLoc, hallway.Height), Rot4.South));
                    doorLocsResult.Add(doorLoc);
                }

                //Eastbound
                returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.East, out doorLoc, hallway.Height), Rot4.East));
                doorLocsResult.Add(doorLoc);

                //Westbound
                returnList.Add(new CellRectRot(CthulhuFactions.Utility.AdjacentRectMaker(hallway, Rot4.West, out doorLoc, hallway.Height), Rot4.West));
                doorLocsResult.Add(doorLoc);
            }
            doorLocs = new List<IntVec3>(doorLocsResult);
            return returnList;
        }

        public static void RectReport(CellRect rect, string prefix ="")
        {
            Log.Message(prefix + "Adj Rect Maker :: MinX=" + rect.minX + " MinZ=" + rect.minZ + " Width=" + rect.Width + " Height=" + rect.Height);
        }
    }

}
