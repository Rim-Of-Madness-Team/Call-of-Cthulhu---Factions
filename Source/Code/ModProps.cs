// ----------------------------------------------------------------------
// These are basic usings. Always let them be here.
// ----------------------------------------------------------------------

// ----------------------------------------------------------------------
// These are RimWorld-specific usings. Activate/Deactivate what you need:
// ----------------------------------------------------------------------
// Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
// RimWorld universal objects are here (like 'Building')
// Needed when you do something with the AI
// Needed when you do something with Sound
// Needed when you do something with Noises
// RimWorld specific functions are found here (like 'Building_Battery')
// RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

/// <summary>
/// Utility File for use between Cthulhu mods.
/// Last Update: 7/15/2017 NEWEST
/// </summary>
namespace Cthulhu
{
    public static class ModProps
    {
        public const string main = "Cthulhu";
        public const string mod = "Factions";
        public const string version = "1.0.0";
    }
}