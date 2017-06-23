using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CthulhuFactions
{ 
    [StaticConstructorOnStartup]
    public class PawnComponentInjectorBehavior : MonoBehaviour
    {
        static PawnComponentInjectorBehavior()
        {
            GameObject initializer = new UnityEngine.GameObject("AgencyPawnCompInjector");
            initializer.AddComponent<PawnComponentInjectorBehavior>();
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)initializer);
        }

        protected float reinjectTime = 0;
        int lastTicks;

        public void FixedUpdate()
        {
            try
            {
                if (Find.TickManager != null)
                {
                    if (Find.TickManager.TicksGame > lastTicks + 10)
                    {
                        lastTicks = Find.TickManager.TicksGame;
                        reinjectTime -= Time.fixedDeltaTime;
                        if (reinjectTime <= 0)
                        {
                            reinjectTime = 0;
                            if (Find.Maps != null)
                            {
                                Find.Maps.ForEach(delegate (Map map)
                                {
                                    List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where((Pawn p) => p.Faction != null).ToList();
                                    pawns.Where((Pawn p) => p.Name != null && p.TryGetComp<PawnComponent_Agency>() == null &&
                                            p.Faction.def.defName.EqualsIgnoreCase("ROM_TheAgency")).ToList().ForEach(
                                        delegate (Pawn p)
                                        {
                                            PawnComponent_Agency pca = new PawnComponent_Agency();
                                            pca.parent = p;
                                            p.AllComps.Add(pca);
                                        });
                                    pawns.Where((Pawn p) => p.Name != null && p.Faction.def.defName.EqualsIgnoreCase("Townsfolk") &&
                                    p.needs?.rest?.CurLevelPercentage < 40f).ToList().ForEach(
                                            delegate (Pawn p)
                                            {
                                                p.needs.rest.CurLevelPercentage = 100;
                                            });
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

        }
    }
}