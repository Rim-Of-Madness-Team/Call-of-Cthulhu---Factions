using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    internal class SymbolResolver_AgencyDoorsNS : SymbolResolverAgencyInternal
    {
        public virtual IEnumerable<Rot4> Directions => new List<Rot4> {Rot4.North, Rot4.South};

        public override void PassingParameters(ResolveParams rp)
        {
            var unused = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction);
            var dirs = new List<Rot4>(Directions.InRandomOrder());
            foreach (var dir in dirs)
            {
                TryMakeDoor(rp.rect, dir);
            }
        }

        private void TryMakeDoor(CellRect rect, Rot4 dir)
        {
            //if (this.WallHasDoor(rect, dir))
            //{
            //    return;
            //}
            if (!TryFindRandomDoorCell(rect, dir, out var intVec))
            {
                return;
            }

            _ = intVec + dir.FacingCell;
            //if (this.IsOutdoorsAt(c))
            //{
            //    if (doorToOutsidePresent)
            //    {
            //        return;
            //    }
            //    doorToOutsidePresent = true;
            //}
            var thing = ThingMaker.MakeThing(CthulhuFactionsDefOf.ROM_TemporaryDoorMarker);
            //thing.SetFaction(faction, null);
            GenSpawn.Spawn(thing, intVec, BaseGen.globalSettings.map);
        }

        private bool WallHasDoor(CellRect rect, Rot4 dir)
        {
            var map = BaseGen.globalSettings.map;
            foreach (var current in rect.GetEdgeCells(dir))
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
            var map = BaseGen.globalSettings.map;
            if (dir == Rot4.North)
            {
                if (rect.Width <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }

                if (!Rand.TryRangeInclusiveWhere(rect.minX + 1, rect.maxX - 1, delegate(int x)
                {
                    var c = new IntVec3(x, 0, rect.maxZ + 1);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out var newX))
                {
                    found = IntVec3.Invalid;
                    return false;
                }

                found = new IntVec3(newX, 0, rect.maxZ);
                return true;
            }

            if (dir == Rot4.South)
            {
                if (rect.Width <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }

                if (!Rand.TryRangeInclusiveWhere(rect.minX + 1, rect.maxX - 1, delegate(int x)
                {
                    var c = new IntVec3(x, 0, rect.minZ - 1);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out var newX2))
                {
                    found = IntVec3.Invalid;
                    return false;
                }

                found = new IntVec3(newX2, 0, rect.minZ);
                return true;
            }

            if (dir == Rot4.West)
            {
                if (rect.Height <= 2)
                {
                    found = IntVec3.Invalid;
                    return false;
                }

                if (!Rand.TryRangeInclusiveWhere(rect.minZ + 1, rect.maxZ - 1, delegate(int z)
                {
                    var c = new IntVec3(rect.minX - 1, 0, z);
                    return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
                }, out var newZ))
                {
                    found = IntVec3.Invalid;
                    return false;
                }

                found = new IntVec3(rect.minX, 0, newZ);
                return true;
            }

            if (rect.Height <= 2)
            {
                found = IntVec3.Invalid;
                return false;
            }

            if (!Rand.TryRangeInclusiveWhere(rect.minZ + 1, rect.maxZ - 1, delegate(int z)
            {
                var c = new IntVec3(rect.maxX + 1, 0, z);
                return c.InBounds(map) && c.Walkable(map) && !c.Fogged(map);
            }, out var newZ2))
            {
                found = IntVec3.Invalid;
                return false;
            }

            found = new IntVec3(rect.maxX, 0, newZ2);
            return true;
        }

        private bool IsOutdoorsAt(IntVec3 c)
        {
            var map = BaseGen.globalSettings.map;
            return c.GetRegion(map) != null && c.GetRegion(map).Room.PsychologicallyOutdoors;
        }
    }
}