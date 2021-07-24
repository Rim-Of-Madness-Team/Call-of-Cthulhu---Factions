using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace CthulhuFactions
{
    [StaticConstructorOnStartup]
    public class PawnComponentInjectorBehavior : MonoBehaviour
    {
        private int lastTicks;

        protected float reinjectTime;

        static PawnComponentInjectorBehavior()
        {
            var initializer = new GameObject("AgencyPawnCompInjector");
            initializer.AddComponent<PawnComponentInjectorBehavior>();
            DontDestroyOnLoad(initializer);
        }

        public void FixedUpdate()
        {
            try
            {
                if (Find.TickManager == null)
                {
                    return;
                }

                if (Find.TickManager.TicksGame <= lastTicks + 10)
                {
                    return;
                }

                lastTicks = Find.TickManager.TicksGame;
                reinjectTime -= Time.fixedDeltaTime;
                if (!(reinjectTime <= 0))
                {
                    return;
                }

                reinjectTime = 0;
                if (Find.Maps != null)
                {
                    Find.Maps.ForEach(delegate(Map map)
                    {
                        var pawns = map.mapPawns.AllPawnsSpawned.Where(p => p.Faction != null).ToList();
                        pawns.Where(p => p.Name != null && p.TryGetComp<PawnComponent_Agency>() == null &&
                                         p.Faction.def.defName.EqualsIgnoreCase("ROM_TheAgency")).ToList()
                            .ForEach(
                                delegate(Pawn p)
                                {
                                    var pca = new PawnComponent_Agency
                                    {
                                        parent = p
                                    };
                                    p.AllComps.Add(pca);
                                });
                        pawns.Where(p =>
                            p.Name != null && p.Faction.def.defName.EqualsIgnoreCase("Townsfolk") &&
                            p.needs?.rest?.CurLevelPercentage < 40f).ToList().ForEach(
                            delegate(Pawn p) { p.needs.rest.CurLevelPercentage = 100; });
                    });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}