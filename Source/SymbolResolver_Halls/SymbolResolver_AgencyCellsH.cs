using System;
using Verse;
using RimWorld.BaseGen;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyCellsH : SymbolResolver_AgencyCells
    {
        public override bool horizontalHallway
        {
            get { return true; }
        }
    }
}
