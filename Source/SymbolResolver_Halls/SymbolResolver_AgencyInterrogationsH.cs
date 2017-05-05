using System;
using Verse;
using RimWorld.BaseGen;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using UnityEngine;

namespace CthulhuFactions
{
    public class SymbolResolver_AgencyInterrogationsH : SymbolResolver_AgencyInterrogations
    {
        public override bool horizontalHallway
        {
            get
            {
                return true;
            }
        }
    }
}
