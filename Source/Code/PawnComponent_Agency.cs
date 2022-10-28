using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using Verse;
using Verse.AI;

namespace CthulhuFactions
{
    [StaticConstructorOnStartup]
    internal class PawnComponent_Agency : ThingComp
    {
        private static readonly bool loadedCults;

        static PawnComponent_Agency()
        {
            loadedCults = false;
            foreach (var ResolvedMod in LoadedModManager.RunningMods)
            {
                if (!ResolvedMod.Name.Contains("Call of Cthulhu - Cults"))
                {
                    continue;
                }

                loadedCults = true;
                break;
            }
        }

        public CompProperties_Agency Props => (CompProperties_Agency) props;

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (parent.Faction == Faction.OfPlayer)
            {
                return;
            }

            if (parent.Faction.HostileTo(Faction.OfPlayer))
            {
                return;
            }

            if (parent is not Pawn parentPawn)
            {
                return;
            }

            bool predicate(Thing t)
            {
                if (t == null)
                {
                    return false;
                }

                if (t == parent)
                {
                    return false;
                }

                if (!t.Spawned)
                {
                    return false;
                }

                if (t.def == null)
                {
                    return false;
                }

                if (t is Corpse corpse)
                {
                    t = corpse.InnerPawn;
                }

                if (t is MinifiedThing thing)
                {
                    t = thing.InnerThing;
                }

                if (!loadedCults)
                {
                    return false;
                }

                if (Utility.CultlikeStructures().Contains(t.def.defName))
                {
                    return true;
                }
                //if (t.def.defName.Contains("ROM_Cult")) return true; //covers most things
                //        if (t.def.defName.EqualsIgnoreCase("ForbiddenKnowledgeCenter")) return true; //specifically called because Jecrell's name consistency is awful ^^
                //        if (t.def.defName.Contains("Dagon")) return true;
                //if (t.def.defName.Contains("Nyarlathotep")) return true;
                //if (t.def.defName.Contains("Shub")) return true;

                return false;
            }

            if (parentPawn.Dead || parentPawn.Map == null)
            {
                return;
            }

            var things = new List<Thing>();
            parentPawn.Map.listerBuildings.allBuildingsColonist.ForEach(x => things.Add(x));
            things.AddRange(parentPawn.Map.listerThings.AllThings);
            var thing2 = GenClosest.ClosestThing_Global_Reachable(parentPawn.Position, parentPawn.Map, things,
                PathEndMode.OnCell,
                TraverseParms.For(parentPawn, Danger.Deadly, TraverseMode.PassDoors), 20,
                predicate);

            /*ClosestThingReachable(parent.Position, parent.Map, ThingRequest.ForGroup(ThingRequestGroup.Everything), PathEndMode.OnCell,
                            TraverseParms.For(parent as Pawn, Danger.Deadly, TraverseMode.PassDoors, false), 20, predicate, null, 50, true);*/
            if (thing2 == null || parentPawn.PositionHeld == IntVec3.Invalid ||
                thing2.PositionHeld == IntVec3.Invalid || parentPawn.MapHeld == null)
            {
                return;
            }

            if (!GenSight.LineOfSight(parentPawn.PositionHeld, thing2.PositionHeld, parentPawn.MapHeld))
            {
                return;
            }

            parentPawn.Faction.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Hostile);
            Log.Message("Agency discovered: " + thing2.def.label);
            Find.LetterStack.ReceiveLetter("AgencyDiscoveredCult".Translate(),
                "AgencyDiscoveredCultDesc".Translate(parentPawn.gender.GetPossessive(), parentPawn.LabelShort,
                    thing2.def.label), LetterDefOf.NeutralEvent);
            //Messages.Message("Agency discovered: " + thing2.def.label, MessageSound.Negative);
        }
    }
}