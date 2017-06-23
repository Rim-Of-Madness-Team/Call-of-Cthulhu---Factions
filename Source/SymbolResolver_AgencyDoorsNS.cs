using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CthulhuFactions
{
    class SymbolResolver_AgencyDoorsNS : SymbolResolverAgencyInternal
    {
        public virtual IEnumerable<Rot4> directions
        {
            get
            {
                return new List<Rot4>() { Rot4.North, Rot4.South };
            }
        }

        public override void PassingParameters(ResolveParams rp)
        {
            ThingDef doorStuff = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction, false);
            bool flag = false;
            List<Rot4> dirs = new List<Rot4>(directions.InRandomOrder<Rot4>());
            for (int i = 0; i < dirs.Count<Rot4>(); i++)
            {
                this.TryMakeDoor(rp.rect, dirs[i], rp.faction, doorStuff, ref flag);
            }
        }

        private void TryMakeDoor(CellRect rect, Rot4 dir, Faction faction, ThingDef doorStuff, ref bool doorToOutsidePresent)
        {
            //if (this.WallHasDoor(rect, dir))
            //{
            //    return;
            //}
            IntVec3 intVec;
            if (!this.TryFindRandomDoorCell(rect, dir, out intVec))
            {
                return;
            }
            IntVec3 c = intVec + dir.FacingCell;
            //if (this.IsOutdoorsAt(c))
            //{
            //    if (doorToOutsidePresent)
            //    {
            //        return;
            //    }
            //    doorToOutsidePresent = true;
            //}
            Thing thing = ThingMaker.MakeThing(CthulhuFactionsDefOf.ROM_TemporaryDoorMarker, null);
            //thing.SetFaction(faction, null);
            GenSpawn.Spawn(thing, intVec, BaseGen.globalSettings.map);
        }

        private bool WallHasDoor(CellRect rect, Rot4 dir)
        {
            Map map = BaseGen.globalSettings.map;
            foreach (IntVec3 current in rect.GetEdgeCells(dir))
            {
                if (current.GetDoor(map) != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryFindRandomDoorCell(CellRect rect, Rot4 dir, out IntVec3 found)
        {
            Map map = BaseGen.globalSettings.map;
            if (dir == Rot4.North)
            {
                if (rect.Width <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                int newX;
                if (!Rand.TryRangeInclusiveWhere(rect.minX + 1, rect.maxX - 1, delegate (int x)
                {
                    IntVec3 c = new IntVec3(x, 0, rect.maxZ + 1);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out newX))
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                found = new IntVec3(newX, 0, rect.maxZ);
                return true;
            }
            else if (dir == Rot4.South)
            {
                if (rect.Width <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                int newX2;
                if (!Rand.TryRangeInclusiveWhere(rect.minX + 1, rect.maxX - 1, delegate (int x)
                {
                    IntVec3 c = new IntVec3(x, 0, rect.minZ - 1);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out newX2))
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                found = new IntVec3(newX2, 0, rect.minZ);
                return true;
            }
            else if (dir == Rot4.West)
            {
                if (rect.Height <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                int newZ;
                if (!Rand.TryRangeInclusiveWhere(rect.minZ + 1, rect.maxZ - 1, delegate (int z)
                {
                    IntVec3 c = new IntVec3(rect.minX - 1, 0, z);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out newZ))
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                found = new IntVec3(rect.minX, 0, newZ);
                return true;
            }
            else
            {
                if (rect.Height <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                int newZ2;
                if (!Rand.TryRangeInclusiveWhere(rect.minZ + 1, rect.maxZ - 1, delegate (int z)
                {
                    IntVec3 c = new IntVec3(rect.maxX + 1, 0, z);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out newZ2))
                {
                    found = IntVec3.Invalid;
                    return false;
                }
                found = new IntVec3(rect.maxX, 0, newZ2);
                return true;
            }
        }

        private bool IsOutdoorsAt(IntVec3 c)
        {
            Map map = BaseGen.globalSettings.map;
            return c.GetRegion(map) != null && c.GetRegion(map).Room.PsychologicallyOutdoors;
        }
    }
}
