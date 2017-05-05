using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CthulhuFactions
{
    public class CellRectRot
    {
        private CellRect rect;
        private Rot4 rot;

        public CellRect Rect
        {
            get { return rect; }
        }
        public Rot4 Rot
        {
            get { return rot; }
        }

        public CellRectRot(CellRect newRect, Rot4 newRot)
        {
            rect = newRect;
            rot = newRot;
        }
    }
}
