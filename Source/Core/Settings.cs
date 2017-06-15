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

        public static void DoWindowContents( Rect rect )
        {
            // todo; implement
        }

        #region Overrides of ModSettings

        public override void ExposeData()
        {
            // todo; implement
        }

        #endregion
    }
}