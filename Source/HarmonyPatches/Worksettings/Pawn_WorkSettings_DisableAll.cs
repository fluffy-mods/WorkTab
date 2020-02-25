// Pawn_WorkSettings_DisableAll.cs
// Copyright Karel Kroeze, 2018-2018

using HarmonyLib;
using RimWorld;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings ), "DisableAll" )]
    public class Pawn_WorkSettings_DisableAll
    {
        public static void Prefix( Pawn_WorkSettings __instance )
        {
            __instance.Pawn().DisableAll();
        }
    }
}