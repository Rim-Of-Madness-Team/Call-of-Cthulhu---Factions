using RimWorld;
using RimWorld.BaseGen;
using Verse;
//using CustomFactionBase;

namespace CthulhuFactions
{
    /// <summary>
    ///     A large wing or hallway that creates -> Workfloor & Chief's office / Interrogation chamber / Research station /
    ///     Artifact storage
    /// </summary>
    public class SymbolResolverAgencyWing : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            //ROOF
            if (!rp.noRoof.HasValue || !rp.noRoof.Value)
            {
                BaseGen.symbolStack.Push("roof", rp);
            }

            /////DOORS
            //ThingDef doorStuff = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction, false);
            //List<Rot4> dirs = new List<Rot4>() { Rot4.North, Rot4.South, Rot4.East, Rot4.West };
            //for (int i = 0; i < dirs.Count; i++)
            //{
            //    this.TryMakeDoor(rp.rect, dirs[i], rp.faction, doorStuff);
            //    Thing thing = ThingMaker.MakeThing(ThingDefOf.Door, doorStuff);
            //    thing.SetFaction(rp.faction, null);
            //}


            //THINGS TO PUSH
            PostPassingParameters(rp);

            //CLEAR/FLOOR/WALLS
            var resolveParams2 = rp;
            resolveParams2.wallStuff = FactionUtility.factionWallStuff;
            BaseGen.symbolStack.Push("edgeWalls", resolveParams2);
            var resolveParams3 = rp;
            resolveParams3.floorDef = FactionUtility.factionFloorStuff;
            BaseGen.symbolStack.Push("floor", resolveParams3);
        }

        private void TryMakeDoor(CellRect rect, Rot4 dir, Faction faction, ThingDef doorStuff)
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
            GenSpawn.Spawn(CthulhuFactionsDefOf.ROM_TemporaryRegionBarrier, intVec, Map);

            var thing = ThingMaker.MakeThing(ThingDefOf.Door, doorStuff);
            thing.SetFaction(faction);
            GenSpawn.Spawn(thing, intVec, Map);
        }

        //private bool WallHasDoor(CellRect rect, Rot4 dir)
        //{
        //    Map map = BaseGen.globalSettings.map;
        //    foreach (IntVec3 current in rect.GetEdgeCells(dir))
        //    {
        //        if (current.GetDoor(map) != null)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

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


        public virtual void PostPassingParameters(ResolveParams rp)
        {
        }
    }
}