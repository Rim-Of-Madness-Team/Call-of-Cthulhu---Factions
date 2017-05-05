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
    public class SymbolResolver_AgencyStorageH : SymbolResolver_AgencyStorage
    {
        public override bool horizontalHallway
        {
            get { return true; }
        }
    }
}
