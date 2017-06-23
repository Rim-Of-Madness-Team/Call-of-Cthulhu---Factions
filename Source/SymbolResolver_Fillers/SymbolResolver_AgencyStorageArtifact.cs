using System;
using RimWorld.BaseGen;
using Verse;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyStorageArtifact : SymbolResolverAgency
    {

        public override void PassingParameters(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;


            ResolveParams rp1 = rp;
            rp1.rect = rp.rect.ContractedBy(2);
            IntVec3 doorLoc = CthulhuFactions.Utility.CellRectBottomMiddle(rp1.rect);
            rp1.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", rp1);

            SpawnRandomArtifact(rp);
            

            ResolveParams rp2 = rp;
            rp2.rect = rp.rect.ContractedBy(2);
            rp2.wallStuff = ThingDefOf.Plasteel;
            BaseGen.symbolStack.Push("edgeWalls", rp2);
            //BaseGen.symbolStack.Push("agencyDoorsNSEW", rp2);


        }

        private void SpawnRandomArtifact(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            ThingDef thingDef;
            if (!(from x in DefDatabase<ThingDef>.AllDefsListForReading
                  where x.BaseMarketValue > 1500f && x.EverHaulable && !x.destroyOnDrop
                  select x).TryRandomElement(out thingDef))
            {
                return;
            }
            ResolveParams newParams = rp;
            newParams.rect = CellRect.SingleCell(rp.rect.CenterCell);
            newParams.singleThingDef = thingDef;
            BaseGen.symbolStack.Push("thing", newParams);
        }
    }
}
