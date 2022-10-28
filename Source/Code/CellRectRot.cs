using Verse;

namespace CthulhuFactions
{
    public class CellRectRot
    {
        public CellRectRot(CellRect newRect, Rot4 newRot)
        {
            Rect = newRect;
            Rot = newRot;
        }

        public CellRect Rect { get; }

        public Rot4 Rot { get; }
    }
}