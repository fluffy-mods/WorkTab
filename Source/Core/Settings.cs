// Karel Kroeze
// Settings.cs
// 2017-05-22

using System.Resources;
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
        public static bool disableScrollwheel = false;
        public static bool verticalLabels = true;
        private static bool _fontFix = true;

        public Settings()
        {
            ApplyFontFix( _fontFix );
        }

        public static void ApplyFontFix( bool state )
        {
            Logger.Debug( state ? "Applying font fix" : "Disabling font fix" );
            _fontFix = state;
            Text.fontStyles[0].font.material.mainTexture.filterMode = state ? FilterMode.Point : FilterMode.Trilinear;
        }

        // buffers
        private static bool _fontFixBuffer = _fontFix;
        private static string _maxPriorityBuffer = maxPriority.ToString();

        public static void DoWindowContents( Rect rect )
        { 
            var options = new Listing_Standard();
            options.Begin(rect);
            options.TextFieldNumericLabeled<int>("WorkTab.MaxPriority".Translate(), ref maxPriority, ref _maxPriorityBuffer, 4, 9, "WorkTab.MaxPriorityTip".Translate(), 1 / 8f);
            options.CheckboxLabeled("WorkTab.24HourMode".Translate(), ref TwentyFourHourMode, "WorkTab.24HourModeTip".Translate() );
            options.CheckboxLabeled("WorkTab.PlaySounds".Translate(), ref playSounds, "WorkTab.PlaySoundsTip".Translate());
            playCrunch = playSounds && playCrunch; // disabling sounds also disables crunch.
            options.CheckboxLabeled("WorkTab.PlayCrunch".Translate(), ref playCrunch, !playSounds, "WorkTab.PlayCrunchTip".Translate());
            options.CheckboxLabeled("WorkTab.DisableScrollwheel".Translate(), ref disableScrollwheel, "WorkTab.DisableScrollwheelTip".Translate());
            bool verticalLabelsBuffer = verticalLabels;
            options.CheckboxLabeled( "WorkTab.VerticalLabels".Translate(), ref verticalLabelsBuffer,
                "WorkTab.VerticalLabelsTip".Translate() );

            // vertical labels mess up unity's font positioning, and causes anti-aliasing blur
            // setting the filtermode to point removes the blur, but causes slight jitter in letter positioning. 
            // I still think it's the lesser of two evils...
            bool _fontFixBuffer = _fontFix;
            options.CheckboxLabeled("WorkTab.FontFix".Translate(), ref _fontFixBuffer, "WorkTab.FontFixTip".Translate());
            _fontFixBuffer = verticalLabels && _fontFixBuffer; // disabling vertical labels makes the font fix unnecesary.

            // apply any changes.
            if ( _fontFixBuffer != _fontFix )
                ApplyFontFix( _fontFixBuffer );
            if ( verticalLabelsBuffer != verticalLabels )
            {
                verticalLabels = verticalLabelsBuffer;
                MainTabWindow_WorkTab.Instance.Table.SetDirty();
            }

            options.End();
        }

        #region Overrides of ModSettings

        public override void ExposeData()
        {
            Scribe_Values.Look(ref maxPriority, "MaxPriority", 9);
            Scribe_Values.Look(ref TwentyFourHourMode, "TwentyFourHourMode", true);
            Scribe_Values.Look(ref playSounds, "PlaySounds", true);
            Scribe_Values.Look(ref playCrunch, "PlayCrunch", true);
            Scribe_Values.Look(ref disableScrollwheel, "DisableScrollwheel", false);
            Scribe_Values.Look(ref verticalLabels, "VerticalLabels", true );
            Scribe_Values.Look(ref _fontFix, "FontFix", true );

            // apply font-fix on load
            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
                ApplyFontFix( _fontFix );
        }

        #endregion
    }
}