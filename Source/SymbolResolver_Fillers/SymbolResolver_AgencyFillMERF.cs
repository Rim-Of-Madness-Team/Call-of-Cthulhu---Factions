using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyFillMERF : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            //string defLogo = "AgencySymbolFloorPainting";
            string defTable = "TableLong";
            string defChair = "DiningChair";
            string defLamp = "Jecrell_GasLamp";

            Map map = BaseGen.globalSettings.map;
            Rot4 rot = Rot4.North;
            int deskOffset = 1;
            //ThingDef logoDef = ThingDef.Named(defLogo);
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
            //ThingDef stuff = null;
            //Thing logo = ThingMaker.MakeThing(logoDef, stuff);
            //logo.SetFaction(rp.faction, null);
            //GenSpawn.Spawn(logo, locationCenter, map, rot);

            ///Reception table
            ThingDef stuff = null;
            if (thingDef.MadeFromStuff)
            {
                stuff = ThingDefOf.WoodLog;
            }
            Thing thing = ThingMaker.MakeThing(thingDef, stuff);
            thing.SetFaction(rp.faction, null);
            GenSpawn.Spawn(thing, receptionCenter, map, rot);

            ///Adjacent lamps
            IntVec3 loc1 = Cthulhu.Utility.GetCornerPos(thing.OccupiedRect(), 0) + GenAdj.AdjacentCells[Rot4.West.AsInt] + GenAdj.AdjacentCells[Rot4.West.AsInt];
            Thing thing3 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing3, loc1, map);

            IntVec3 loc2 = Cthulhu.Utility.GetCornerPos(thing.OccupiedRect(), 1) + GenAdj.AdjacentCells[Rot4.East.AsInt] + GenAdj.AdjacentCells[Rot4.East.AsInt];
            Thing thing4 = ThingMaker.MakeThing(lampDef, lampStuffDef);
            GenSpawn.Spawn(thing4, loc2, map);
            
            if (thing != null)
            {
                if (thing.Spawned && thing.Position.InBounds(map))
                {
                    CellRect rectToEdit = thing.OccupiedRect().ExpandedBy(1);

                    List<IntVec3> poss = new List<IntVec3>();
                    poss.AddRange(rectToEdit.GetEdgeCells(Rot4.North));
                    poss.AddRange(rectToEdit.GetEdgeCells(Rot4.South));
                    poss.AddRange(rectToEdit.GetEdgeCells(Rot4.East));
                    poss.AddRange(rectToEdit.GetEdgeCells(Rot4.West));
                    poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 0));
                    poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 1));
                    poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 2));
                    poss.Remove(Cthulhu.Utility.GetCornerPos(rectToEdit, 3));

                    foreach (IntVec3 currentPos in poss.InRandomOrder<IntVec3>())
                    {
                        ///CHAIR

             
                        float angle = (currentPos - thing.Position).ToVector3().AngleFlat();
                        Rot4 newRot = PawnRotator.RotFromAngleBiased(angle).Opposite;

                        Building bld = thing as Building; //We need an interaction cell
                        Thing thing2 = ThingMaker.MakeThing(chairDef, stuff);
                        thing2.SetFaction(rp.faction, null);
                        GenSpawn.Spawn(thing2, currentPos, map, newRot);
                    }
                }
            }

            ///Four corners, four gas lamps.
            for (int i = 0; i < 1; i++)
            {
                Thing thing5 = ThingMaker.MakeThing(lampDef, lampStuffDef);
                IntVec3 loc = Cthulhu.Utility.GetCornerPos(rp.rect.ContractedBy(1), i);
                GenSpawn.Spawn(thing5, loc, map);
            }

            //Bring in MERF
            Lord singlePawnLord = rp.singlePawnLord ?? LordMaker.MakeNewLord(rp.faction, new LordJob_DefendBase(rp.faction, rp.rect.CenterCell), map, null);

            ResolveParams resolveParams = rp;
            resolveParams.rect = rp.rect.ExpandedBy(1);
            resolveParams.faction = rp.faction;
            resolveParams.singlePawnLord = singlePawnLord;
            resolveParams.pawnGroupKindDef = CthulhuFactionsDefOf.ROM_AgencyMERF;
            float points = 10000;
            resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
            resolveParams.pawnGroupMakerParams.tile = map.Tile;
            resolveParams.pawnGroupMakerParams.faction = rp.faction;
            resolveParams.pawnGroupMakerParams.points = points;

            BaseGen.symbolStack.Push("pawnGroup", resolveParams);

        }
    }
}
