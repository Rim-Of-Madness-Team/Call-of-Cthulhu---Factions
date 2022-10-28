using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyFillMERF : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            //string defLogo = "AgencySymbolFloorPainting";
            var defTable = "TableLong";
            var defChair = "DiningChair";
            var defLamp = "Jecrell_GasLamp";

            var map = BaseGen.globalSettings.map;
            var rot = Rot4.North;
            var deskOffset = 1;
            //ThingDef logoDef = ThingDef.Named(defLogo);
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
            //ThingDef stuff = null;
            //Thing logo = ThingMaker.MakeThing(logoDef, stuff);
            //logo.SetFaction(rp.faction, null);
            //GenSpawn.Spawn(logo, locationCenter, map, rot);

            //Reception table
            ThingDef stuff = null;
            if (thingDef.MadeFromStuff)
            {
                stuff = ThingDefOf.WoodLog;
            }

            var thing = ThingMaker.MakeThing(thingDef, stuff);
            thing.SetFaction(rp.faction);
            GenSpawn.Spawn(thing, receptionCenter, map, rot);

            //Adjacent lamps
            var loc1 = Utility.GetCornerPos(thing.OccupiedRect(), 0) + GenAdj.AdjacentCells[Rot4.West.AsInt] +
                       GenAdj.AdjacentCells[Rot4.West.AsInt];
            var thing3 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing3, loc1, map);

            var loc2 = Utility.GetCornerPos(thing.OccupiedRect(), 1) + GenAdj.AdjacentCells[Rot4.East.AsInt] +
                       GenAdj.AdjacentCells[Rot4.East.AsInt];
            var thing4 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing4, loc2, map);

            if (thing.Spawned && thing.Position.InBounds(map))
            {
                var rectToEdit = thing.OccupiedRect().ExpandedBy(1);

                var poss = new List<IntVec3>();
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.North));
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.South));
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.East));
                poss.AddRange(rectToEdit.GetEdgeCells(Rot4.West));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 0));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 1));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 2));
                poss.Remove(Utility.GetCornerPos(rectToEdit, 3));

                foreach (var currentPos in poss.InRandomOrder())
                {
                    //CHAIR


                    var angle = (currentPos - thing.Position).ToVector3().AngleFlat();
                    var newRot = Pawn_RotationTracker.RotFromAngleBiased(angle).Opposite;
                    var thing2 = ThingMaker.MakeThing(chairDef, stuff);
                    thing2.SetFaction(rp.faction);
                    GenSpawn.Spawn(thing2, currentPos, map, newRot);
                }
            }

            //Four corners, four gas lamps.
            for (var i = 0; i < 1; i++)
            {
                var thing5 = ThingMaker.MakeThing(lampDef, lampStuffDef);
                var loc = Utility.GetCornerPos(rp.rect.ContractedBy(1), i);
                GenSpawn.Spawn(thing5, loc, map);
            }

            //Bring in MERF
            var singlePawnLord = rp.singlePawnLord ??
                                 LordMaker.MakeNewLord(rp.faction,
                                     new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map);

            var resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.Combat; //CthulhuFactionsDefOf.ROM_AgencyMERF;
            float points = 10000;
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