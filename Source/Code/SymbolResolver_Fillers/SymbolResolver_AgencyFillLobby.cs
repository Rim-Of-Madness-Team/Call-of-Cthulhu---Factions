using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyFillLobby : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            var defLogo = "ROM_AgencySymbolFloorPainting";
            var defTable = "ROM_AgencyReceptionistTable";
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var map = BaseGen.globalSettings.map;
            var rot = Rot4.North;
            var deskOffset = 4;
            var logoDef = ThingDef.Named(defLogo);
            var thingDef = ThingDef.Named(defTable);
            var chairDef = ThingDef.Named(defChair);

            var lampDef = Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            var lampStuffDef = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;


            var locationCenter = rp.rect.CenterCell;
            for (var i = 0; i < deskOffset; i++)
            {
                locationCenter -= GenAdj.AdjacentCells[rot.AsInt];
            }

            var receptionCenter = rp.rect.CenterCell;
            for (var i = 0; i < deskOffset; i++)
            {
                receptionCenter += GenAdj.AdjacentCells[rot.AsInt];
            }


            //Center logo
            ThingDef stuff = null;
            var logo = ThingMaker.MakeThing(logoDef);
            logo.SetFaction(rp.faction);
            GenSpawn.Spawn(logo, locationCenter, map, rot);

            //Reception table
            if (thingDef.MadeFromStuff)
            {
                stuff = ThingDefOf.WoodLog;
            }

            var thing = ThingMaker.MakeThing(thingDef, stuff);
            thing.SetFaction(rp.faction);
            GenSpawn.Spawn(thing, receptionCenter, map, rot);
            //Adjacent lamps
            _ = Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : ThingDefOf.WoodLog;
            var loc1 = Utility.GetCornerPos(thing.OccupiedRect(), 0) + GenAdj.AdjacentCells[Rot4.West.AsInt];
            var thing3 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing3, loc1, map);

            var loc2 = Utility.GetCornerPos(thing.OccupiedRect(), 1) + GenAdj.AdjacentCells[Rot4.East.AsInt];
            var thing4 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing4, loc2, map);

            //Reception chair
            if (thing.Spawned && thing.Position != IntVec3.Invalid)
            {
                //CHAIR
                var bld = thing as Building; //We need an interaction cell
                var thing2 = ThingMaker.MakeThing(chairDef, stuff);
                thing2.SetFaction(rp.faction);
                if (bld != null)
                {
                    GenSpawn.Spawn(thing2,
                        bld.Position + GenAdj.AdjacentCells[rot.AsInt] + GenAdj.AdjacentCells[rot.AsInt], map,
                        rot.Opposite);
                }
            }

            //Four corners, four gas lamps.
            for (var i = 0; i < 4; i++)
            {
                var thing5 = ThingMaker.MakeThing(lampDef, lampStuffDef);
                var loc = Utility.GetCornerPos(rp.rect.ContractedBy(1), i);
                GenSpawn.Spawn(thing5, loc, map);
            }

            //Bring in Standard Pawns
            var singlePawnLord = rp.singlePawnLord ??
                                 LordMaker.MakeNewLord(rp.faction,
                                     new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map);

            var resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.Settlement;
            float points = 500;
            resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms
            {
                tile = map.Tile,
                faction = rp.faction,
                points = points
            };

            BaseGen.symbolStack.Push("pawnGroup", resolveParams);
        }
    }
}