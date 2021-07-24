using System.Collections.Generic;
using Verse;

namespace CthulhuFactions
{
    internal class SymbolResolver_AgencyDoorsEW : SymbolResolver_AgencyDoorsNS
    {
        public override IEnumerable<Rot4> Directions => new List<Rot4> {Rot4.East, Rot4.West};
    }
}