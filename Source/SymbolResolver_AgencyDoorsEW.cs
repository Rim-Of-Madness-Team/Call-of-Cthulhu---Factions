using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CthulhuFactions
{
    class SymbolResolver_AgencyDoorsEW : SymbolResolver_AgencyDoorsNS
    {
        public override IEnumerable<Rot4> directions
        {
            get
            {
                return new List<Rot4>() { Rot4.East, Rot4.West };
            }
        }
    }
}