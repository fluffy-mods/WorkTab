// Karel Kroeze
// Settings.cs
// 2017-05-22

using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public class Settings: ModSettings
    {

        public int maxPriority = 9;
        public bool showPriorityColors = false;
        public List<string> priorityColors;
        public bool playCrunch = true;
        public bool playSounds = true;
        public bool TwentyFourHourMode = true;
        public bool disableScrollwheel = false;
        public bool verticalLabels = true;
        private bool _fontFix = true;

        public static Settings Get()
        {
            return LoadedModManager.GetMod<WorkTab.Mod>().GetSettings<Settings>();
        }
        public Settings()
        {
            ApplyFontFix( _fontFix );
            priorityColors = new List<string> { "00ff00", "e6cf89", "808080" };
        }

        public void ApplyFontFix( bool state )
        {
            LongEventHandler.ExecuteWhenFinished( delegate
            {
                Logger.Debug( state ? "Applying font fix" : "Disabling font fix" );
                _fontFix = state;
                Text.fontStyles[0].font.material.mainTexture.filterMode =
                    state ? FilterMode.Point : FilterMode.Trilinear;
            } );
        }

        // buffers;

        public void DoWindowContents( Rect rect )
        {
            var options = new Listing_Standard();
            options.Begin(rect);
            options.TextFieldNumericLabeled<int>("WorkTab.MaxPriority".Translate(), ref maxPriority, maxPriority.ToString(), 4, 9, "WorkTab.MaxPriorityTip".Translate(), 1 / 8f);
            options.CheckboxLabeled("WorkTab.ShowPriorityColors".Translate(), ref showPriorityColors, "WorkTab.PriorityColorsTip".Translate()); 
            if (showPriorityColors)
                options.ColorBoxes(ref priorityColors, "WorkTab.PriorityColorsTip".Translate());
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
            Scribe_Values.Look(ref showPriorityColors, "ShowCustomColors", false);
            Scribe_Collections.Look(ref priorityColors, "PriorityColors");    // Doesn't seem to accept defaults
            Scribe_Values.Look(ref TwentyFourHourMode, "TwentyFourHourMode", true);
            Scribe_Values.Look(ref playSounds, "PlaySounds", true);
            Scribe_Values.Look(ref playCrunch, "PlayCrunch", true);
            Scribe_Values.Look(ref disableScrollwheel, "DisableScrollwheel", false);
            Scribe_Values.Look(ref verticalLabels, "VerticalLabels", true );
            Scribe_Values.Look(ref _fontFix, "FontFix", true );

            // apply font-fix on load
            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
                ApplyFontFix( _fontFix );

            if (priorityColors == null)
                priorityColors = new List<string> { "00ff00", "e6cf89", "808080" };
        }

        #endregion
    }
}