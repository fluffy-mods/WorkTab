// Karel Kroeze
// Settings.cs
// 2017-05-22

using UnityEngine;
using Verse;

namespace WorkTab
{
    public class Settings: ModSettings
    {
        public static int maxPriority = 9;
        public static bool playCrunch = true;
        public static bool playSounds = true;
        public static bool TwentyFourHourMode = true;

        // buffers
        private static string maxPriorityBuffer = maxPriority.ToString();
        
        public static void DoWindowContents( Rect rect )
        { 
            var options = new Listing_Standard();
            options.Begin(rect);
            options.TextFieldNumericLabeled<int>("WorkTab.MaxPriority".Translate(), ref maxPriority, ref maxPriorityBuffer, 4, 9, "WorkTab.MaxPriorityTip".Translate(), 1 / 8f);
            options.CheckboxLabeled("WorkTab.24HourMode".Translate(), ref TwentyFourHourMode, "WorkTab.24HourModeTip".Translate() );
            options.CheckboxLabeled("WorkTab.PlaySounds".Translate(), ref playSounds, "WorkTab.PlaySoundsTip".Translate());
            options.CheckboxLabeled("WorkTab.PlayCrunch".Translate(), ref playCrunch, playSounds, "WorkTab.PlayCrunchTip".Translate());
            options.End();
        }

        #region Overrides of ModSettings

        public override void ExposeData()
        {
            Scribe_Values.Look(ref maxPriority, "MaxPriority", 9);
            Scribe_Values.Look(ref TwentyFourHourMode, "TwentyFourHourMode", true);
            Scribe_Values.Look(ref playSounds, "PlaySounds", true);
            Scribe_Values.Look(ref playCrunch, "PlayCrunch", true);
        }

        #endregion
    }
}