using System;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace WorkTab
{
    [StaticConstructorOnStartup]
    public static class WorkTabMPCompat
    {
        
        static WorkTabMPCompat()
        {
            if (!MP.enabled) return;

            MP.RegisterAll();
            
            Logger.Debug("MP WorkTab Enabled");
        }
    }
}
