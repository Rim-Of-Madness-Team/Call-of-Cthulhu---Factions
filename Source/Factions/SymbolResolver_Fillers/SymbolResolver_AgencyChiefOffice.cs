using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyChiefOffice : SymbolResolverAgencyInternal
    {
        public override void PassingParameters(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;
            var @bool = Rand.Bool;
            var defTypewriter = "Estate_TableTypewriter";
            var defBench = "SimpleResearchBench";
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var wallDef = ThingDefOf.Steel;
            var floorDef = TerrainDefOf.MetalTile;
            var lampDef = Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            var lampStuffDef = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;

            var thingDef = rp.singleThingDef ?? (Utility.IsIndustrialAgeLoaded()
                ? ThingDef.Named(defTypewriter)
                : ThingDef.Named(defBench));
            var chairDef = ThingDef.Named(defChair);

            var rot = !@bool ? Rot4.South : Rot4.West;
            var loc = rp.rect.CenterCell;
            if (!GenSpawn.WouldWipeAnythingWith(loc, rot, thingDef, map, x => x.def.category == ThingCategory.Building))
            {
                var doorNear = false;
                foreach (var current2 in GenAdj.CellsOccupiedBy(loc, rot, thingDef.Size))
                {
                    if (!BaseGenUtility.AnyDoorAdjacentCardinalTo(current2, map))
                    {
                        continue;
                    }

                    doorNear = true;
                    break;
                }

                if (!doorNear)
                {
                    //RESEARCH BENCH
                    ThingDef stuff = null;
                    if (thingDef.MadeFromStuff)
                    {
                        stuff = ThingDefOf.WoodLog;
                    }

                    var thing = ThingMaker.MakeThing(thingDef, stuff);
                    thing.SetFaction(rp.faction);
                    GenSpawn.Spawn(thing, loc, map, rot);

                    if (thing.Spawned && thing.Position != IntVec3.Invalid)
                    {
                        //CHAIR
                        var bld = thing as Building; //We need an interaction cell
                        var thing2 = ThingMaker.MakeThing(chairDef, stuff);
                        thing2.SetFaction(rp.faction);
                        if (bld != null)
                        {
                            GenSpawn.Spawn(thing2, bld.InteractionCell, map, rot.Opposite);
                        }
                    }

                    //Adjacent lamps
                    var singleThingDef3 = Utility.IsIndustrialAgeLoaded() ? lampDef : ThingDefOf.TorchLamp;
                    var thing3 = ThingMaker.MakeThing(singleThingDef3, lampStuffDef);
                    var thingLoc = Utility.GetCornerPos(thing.OccupiedRect(), 0) +
                                   GenAdj.AdjacentCells[Rot4.West.AsInt];
                    GenSpawn.Spawn(thing3, thingLoc, map);
                }
            }

            UnfogRoomCenter(rp.rect.CenterCell);
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