using RimWorld;
using RimWorld.BaseGen;
using System;
using Verse;
using Verse.AI.Group;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyFillLobby : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        { 
            string defLogo  = "ROM_AgencySymbolFloorPainting";
            string defTable = "ROM_AgencyReceptionistTable";
            string defChair = "DiningChair";
            string defLamp  = "Jecrell_GasLamp";

            Map map = BaseGen.globalSettings.map;
            Rot4 rot = Rot4.North;
            int deskOffset = 4;
            ThingDef logoDef = ThingDef.Named(defLogo);
            ThingDef thingDef = ThingDef.Named(defTable);
            ThingDef chairDef = ThingDef.Named(defChair);

            ThingDef lampDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDef.Named(defLamp) : ThingDefOf.TorchLamp;
            ThingDef lampStuffDef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : null;


            IntVec3 locationCenter = rp.rect.CenterCell;
            for (int i = 0; i < deskOffset; i++)
            {
                locationCenter -= GenAdj.AdjacentCells[rot.AsInt];
            }

            IntVec3 receptionCenter = rp.rect.CenterCell;
            for (int i = 0; i < deskOffset; i++)
            {
                receptionCenter += GenAdj.AdjacentCells[rot.AsInt];
            }


            ///Center logo
            ThingDef stuff = null;
            Thing logo = ThingMaker.MakeThing(logoDef, stuff);
            logo.SetFaction(rp.faction, null);
            GenSpawn.Spawn(logo, locationCenter, map, rot);
            
            ///Reception table
            if (thingDef.MadeFromStuff)
            {
                stuff = ThingDefOf.WoodLog;
            }
            Thing thing = ThingMaker.MakeThing(thingDef, stuff);
            thing.SetFaction(rp.faction, null);
            GenSpawn.Spawn(thing, receptionCenter, map, rot);

            ///Adjacent lamps
            ThingDef stuffdef = Cthulhu.Utility.IsIndustrialAgeLoaded() ? ThingDefOf.Steel : ThingDefOf.WoodLog;
            IntVec3 loc1 = Cthulhu.Utility.GetCornerPos(thing.OccupiedRect(), 0) + GenAdj.AdjacentCells[Rot4.West.AsInt];
            Thing thing3 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing3, loc1, map);
            
            IntVec3 loc2 = Cthulhu.Utility.GetCornerPos(thing.OccupiedRect(), 1) + GenAdj.AdjacentCells[Rot4.East.AsInt];
            Thing thing4 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing4, loc2, map);

            ///Reception chair
            if (thing != null)
            {
                if (thing.Spawned && thing.Position != IntVec3.Invalid)
                {
                    ///CHAIR
                    Building bld = thing as Building; //We need an interaction cell
                    Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                    thing2.SetFaction(rp.faction, null);
                    GenSpawn.Spawn(thing2, bld.Position + GenAdj.AdjacentCells[rot.AsInt] + GenAdj.AdjacentCells[rot.AsInt], map, rot.Opposite);
                }
            }
            
            ///Four corners, four gas lamps.
            for (int i = 0; i < 4; i++)
            {
                Thing thing5 = ThingMaker.MakeThing(lampDef, lampStuffDef);
                IntVec3 loc = Cthulhu.Utility.GetCornerPos(rp.rect.ContractedBy(1), i);
                GenSpawn.Spawn(thing5, loc, map);
            }

            //Bring in Standard Pawns
            Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(rp.faction, new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map, null);

            ResolveParams resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.FactionBase;
            float points = 500;
            resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
            resolveParams.pawnGroupMakerParams.tile = map.Tile;
            resolveParams.pawnGroupMakerParams.faction = rp.faction;
            resolveParams.pawnGroupMakerParams.points = points;

            BaseGen.symbolStack.Push("pawnGroup", resolveParams);

        }
    }
}
