// Settings.cs
// Copyright Karel Kroeze, 2020-2020

using UnityEngine;
using Verse;

namespace WorkTab {
    public class Settings: ModSettings {
        public static int  maxPriority             = 9;
        public static int  defaultPriority         = 3;
        public static bool TwentyFourHourMode      = true;
        public static bool playSounds              = true;
        public static bool playCrunch              = true;
        public static bool disableScrollwheel      = false;
        public static bool jobTextMode             = false;
        public static bool highlightCurrentJobCell = true;
        public static bool verticalLabels          = true;
        private static bool _fontFix = true;
        // public static bool sharedFavourites = true;

        // Buffers will be initialized with current settings as soon as
        // DoWindowContents() → Listing_Standard.TextFieldNumericLabeled() → Widgets.TextFieldNumeric() will be called.
        private static string maxPriorityBuffer = null;
        private static string defaultPriorityBuffer = null;

        public Settings() {
            ApplyFontFix(_fontFix);
        }

        public static void ApplyFontFix(bool state) {
            LongEventHandler.ExecuteWhenFinished(delegate {
                Logger.Debug(state ? "Applying font fix" : "Disabling font fix");
                _fontFix = state;
                Text.fontStyles[0].font.material.mainTexture.filterMode =
                    state ? FilterMode.Point : FilterMode.Trilinear;
            });
        }

        public static void DoWindowContents(Rect rect) {
            Listing_Standard options = new Listing_Standard();
            options.Begin(rect);
            options.TextFieldNumericLabeled("WorkTab.MaxPriority".Translate(), ref maxPriority, ref maxPriorityBuffer,
                                             4, 9, "WorkTab.MaxPriorityTip".Translate(), 1 / 8f);
            options.TextFieldNumericLabeled("WorkTab.DefaultPriority".Translate(), ref defaultPriority,
                                             ref defaultPriorityBuffer, 1, 9, "WorkTab.DefaultPriorityTip".Translate(),
                                             1 / 8f);
            options.CheckboxLabeled("WorkTab.24HourMode".Translate(), ref TwentyFourHourMode,
                                     "WorkTab.24HourModeTip".Translate());
            options.CheckboxLabeled("WorkTab.PlaySounds".Translate(), ref playSounds,
                                     "WorkTab.PlaySoundsTip".Translate());
            playCrunch = playSounds && playCrunch; // disabling sounds also disables crunch.
            options.CheckboxLabeled("WorkTab.PlayCrunch".Translate(), ref playCrunch, !playSounds,
                                     "WorkTab.PlayCrunchTip".Translate());
            options.CheckboxLabeled("WorkTab.DisableScrollwheel".Translate(), ref disableScrollwheel,
                                     "WorkTab.DisableScrollwheelTip".Translate());
            options.CheckboxLabeled("WorkTab.JobTextMode".Translate(), ref jobTextMode,
                                     "WorkTab.JobTextModeTip".Translate());
            options.CheckboxLabeled("WorkTab.HighlightCurrentJobCell".Translate(), ref highlightCurrentJobCell,
                                     "WorkTab.HighlightCurrentJobCellTip".Translate());
            bool verticalLabelsBuffer = verticalLabels;
            options.CheckboxLabeled("WorkTab.VerticalLabels".Translate(), ref verticalLabelsBuffer,
                                     "WorkTab.VerticalLabelsTip".Translate());
            // options.CheckboxLabeled( "WorkTab.SharedFavourites".Translate(), ref sharedFavourites,
            //     "WorkTab.SharedFavouritesTip".Translate() );

            // vertical labels mess up unity's font positioning, and causes anti-aliasing blur
            // setting the filtermode to point removes the blur, but causes slight jitter in letter positioning. 
            // I still think it's the lesser of two evils...
            bool _fontFixBuffer = _fontFix;
            options.CheckboxLabeled("WorkTab.FontFix".Translate(), ref _fontFixBuffer,
                                     "WorkTab.FontFixTip".Translate());
            _fontFixBuffer =
                verticalLabels && _fontFixBuffer; // disabling vertical labels makes the font fix unnecessary.

            // apply any changes.
            if (_fontFixBuffer != _fontFix) {
                ApplyFontFix(_fontFixBuffer);
            }

            if (verticalLabelsBuffer != verticalLabels) {
                verticalLabels = verticalLabelsBuffer;
                MainTabWindow_WorkTab.Instance?.Table?.SetDirty();
            }

            options.End();
        }

        public override void ExposeData() {
            Scribe_Values.Look(ref maxPriority, "MaxPriority", 9);
            Scribe_Values.Look(ref defaultPriority, "DefaultPriority", 3);
            Scribe_Values.Look(ref TwentyFourHourMode, "TwentyFourHourMode", true);
            Scribe_Values.Look(ref playSounds, "PlaySounds", true);
            Scribe_Values.Look(ref playCrunch, "PlayCrunch", true);
            Scribe_Values.Look(ref disableScrollwheel, "DisableScrollwheel");
            Scribe_Values.Look(ref jobTextMode, "JobTextMode");
            Scribe_Values.Look(ref verticalLabels, "VerticalLabels", true);
            Scribe_Values.Look(ref highlightCurrentJobCell, "HighlightCurrentJobCell", true);
            Scribe_Values.Look(ref _fontFix, "FontFix", true);

            // apply font-fix on load
            if (Scribe.mode == LoadSaveMode.PostLoadInit) {
                ApplyFontFix(_fontFix);
            }
        }
    }
}
