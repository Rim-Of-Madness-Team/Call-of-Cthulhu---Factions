using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyStorageArtifact : SymbolResolverAgency
    {
        public override void PassingParameters(ResolveParams rp)
        {
            _ = BaseGen.globalSettings.map;


            var rp1 = rp;
            rp1.rect = rp.rect.ContractedBy(2);
            _ = FactionUtility.CellRectBottomMiddle(rp1.rect);
            rp1.singleThingDef = CthulhuFactionsDefOf.ROM_TemporaryDoorMarker;
            BaseGen.symbolStack.Push("thing", rp1);

            SpawnRandomArtifact(rp);


            var rp2 = rp;
            rp2.rect = rp.rect.ContractedBy(2);
            rp2.wallStuff = ThingDefOf.Plasteel;
            BaseGen.symbolStack.Push("edgeWalls", rp2);
            //BaseGen.symbolStack.Push("agencyDoorsNSEW", rp2);
        }

        private void SpawnRandomArtifact(ResolveParams rp)
        {
            var unused = BaseGen.globalSettings.map;
            if (!(from x in DefDatabase<ThingDef>.AllDefsListForReading
                where x.BaseMarketValue > 1500f && x.EverHaulable && !x.destroyOnDrop
                select x).TryRandomElement(out var thingDef))
            {
                return;
            }

            var newParams = rp;
            newParams.rect = CellRect.SingleCell(rp.rect.CenterCell);
            newParams.singleThingDef = thingDef;
            BaseGen.symbolStack.Push("thing", newParams);
        }
    }
}