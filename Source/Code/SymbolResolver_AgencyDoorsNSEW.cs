using System.Collections.Generic;
using Verse;

namespace CthulhuFactions
{
    internal class SymbolResolver_AgencyDoorsNSEW : SymbolResolver_AgencyDoorsNS
    {
        public override IEnumerable<Rot4> Directions => new List<Rot4> {Rot4.North, Rot4.South, Rot4.East, Rot4.West};
    }
}