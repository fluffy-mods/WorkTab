// MainTabWindow_WorkTab.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static WorkTab.Constants;
using static WorkTab.InteractionUtilities;
using static WorkTab.Resources;

namespace WorkTab
{
    public class MainTabWindow_WorkTab : MainTabWindow_PawnTable
    {
        private static          List<int> _selectedHours = TimeUtilities.WholeDay;
        private static readonly FieldInfo _tableFieldInfo;
        private static          int       _visibleHour = -1;

        private bool _columnsChanged;


        private Rect _timeBarRect;

        static MainTabWindow_WorkTab()
        {
            _tableFieldInfo = typeof( MainTabWindow_PawnTable ).GetField( "table",
                                                                          BindingFlags.Instance |
                                                                          BindingFlags.NonPublic );
            if ( _tableFieldInfo == null )
                throw new NullReferenceException( "table field not found!" );
        }

        public MainTabWindow_WorkTab()
        {
            Instance = this;
        }

        public static MainTabWindow_WorkTab Instance { get; private set; }

        public static List<int> SelectedHours => PriorityManager.ShowScheduler
            ? _selectedHours
            : TimeUtilities.WholeDay;

        public static int VisibleHour => PriorityManager.ShowScheduler
            ? _visibleHour
            : -1;

        private static List<PawnColumnDef> Columns
        {
            get
            {
                // get clean copy of base columns
                var columns = new List<PawnColumnDef>( Controller.allColumns );

                // add workgiver columns for expanded worktypes
                var templist = new List<PawnColumnDef>( columns );
                foreach ( var column in templist )
                {
                    var expandable = column.Worker as IExpandableColumn;
                    if ( expandable != null && expandable.Expanded )
                    {
                        var index = columns.IndexOf( column );
                        columns.InsertRange( index + 1, expandable.ChildColumns );
                    }
                }

                return columns;
            }
        }

        public PawnTable Table
        {
            get => _tableFieldInfo.GetValue( this ) as PawnTable;
            private set => _tableFieldInfo.SetValue( this, value );
        }

        public Rect TimeBarRect
        {
            get
            {
                if ( _timeBarRect == default )
                    RecacheTimeBarRect();

                return _timeBarRect;
            }
        }

        protected override float ExtraBottomSpace => PriorityManager.ShowScheduler
            ? base.ExtraBottomSpace / 2f +
              TimeBarHeight // slightly less margin if we're showing the scheduler, as it already takes quite a sizeable chunk of screen space
            : base.ExtraBottomSpace;

        protected override float        ExtraTopSpace => base.ExtraTopSpace + Constants.ExtraTopSpace;
        protected override PawnTableDef PawnTableDef  => PawnTableDefOf.Work;

        private bool AnyExpanded => PawnTableDef.columns
                                                .Select( c => c.Worker )
                                                .OfType<IExpandableColumn>()
                                                .Any( w => w.Expanded );

        public static void AddSelectedHour( int hour, bool replace )
        {
            if ( replace )
                _selectedHours.Clear();
            if ( replace || !_selectedHours.Contains( hour ) )
                _selectedHours.Add( hour );
            _visibleHour = hour;
        }

        public static void RemoveSelectedHour( int hour )
        {
            if ( _selectedHours.Contains( hour ) )
                _selectedHours.Remove( hour );

            if ( _visibleHour == hour )
                _visibleHour = -1;
        }

        public static void SelectWholeDay()
        {
            _selectedHours = TimeUtilities.WholeDay;
            _visibleHour   = -1;
        }

        public override void DoWindowContents( Rect rect )
        {
            if ( _columnsChanged )
                RebuildTable();

            base.DoWindowContents( rect );
            if ( Event.current.type == EventType.Layout )
                return;

            DoToggleButtons( rect );
            DoPriorityLabels( rect );
            if ( PriorityManager.ShowScheduler )
                DoTimeBar( rect );
        }

        public void Notify_ColumnsChanged()
        {
            _columnsChanged = true;
        }

        public override void PostOpen()
        {
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
            RebuildTable();
        }

        public void RebuildTable()
        {
            if ( Instance == null )
            {
                Logger.Debug( "calling RebuildTable on a null instance" );
                return;
            }

            // get columns
            var columns = Columns;

            // update alternating vertical positions
            var moveNextDown = false;
            foreach ( var columnDef in columns )
                if ( columnDef.Worker is IAlternatingColumn worker )
                {
                    worker.MoveDown = moveNextDown;
                    moveNextDown    = !moveNextDown;
                }
                else
                {
                    moveNextDown = false;
                }

            // rebuild table
            PawnTableDefOf.Work.columns = columns;
            //Instance.Table = new PawnTable( PawnTableDefOf.Work, () => Instance.Pawns, UI.screenWidth - (int)(Instance.Margin * 2f), 
            //                       (int)(UI.screenHeight - 35 - Instance.ExtraBottomSpace - Instance.ExtraTopSpace - Instance.Margin * 2f));

            // force recache of table sizes: set the table to be dirty, then get the size - which calls the private recache routine.
            Instance.Table.SetDirty();
            var tmp = Instance.Table.Size;
            Instance.windowRect.width = Math.Min( tmp.x, UI.screenWidth);

            // force recache of timeBarRect
            Instance._timeBarRect = default;

            // clear dirty flag
            _columnsChanged = false;
        }

        public void RecacheTimeBarRect()
        {
            var   widths  = Traverse.Create( Table ).Field( "cachedColumnWidths" ).GetValue<List<float>>();
            var   columns = Table.ColumnsListForReading;
            float start   = 0;
            float width   = 0;

            // loop over columns, initially add any column that is not a workbox to the start, but not after we've seen a workbox.
            // Add widths for workboxes to width. 
            // NOTE: This assumes a single contiguous block of workboxes!
            for ( var i = 0; i < columns.Count; i++ )
            {
                var column = columns[i].Worker;
                if ( column is PawnColumnWorker_WorkType || column is PawnColumnWorker_WorkGiver )
                    width += widths[i];
                else if ( width < 1 )
                    start += widths[i];
            }

            // build the rect
            _timeBarRect = new Rect(
                start,
                windowRect.height -
                base.ExtraBottomSpace, // note that we're not subtracting the time bar height itself, as at this point the window's height has not yet been updated to include it.
                width,
                TimeBarHeight );

            // limit maximum size
            _timeBarRect.xMax = Mathf.Min( _timeBarRect.xMax, UI.screenWidth - StandardMargin );

            Logger.Debug( "created time bar rect: " + _timeBarRect );
        }

        private void DoPriorityLabels( Rect canvas )
        {
            var priorityLabelRect = new Rect( canvas.width / 3f - PriorityLabelSize.x / 2f,
                                              canvas.yMin,
                                              PriorityLabelSize.x,
                                              PriorityLabelSize.y );

            GUI.color   = new Color( 1f, 1f, 1f, 0.5f );
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font   = GameFont.Tiny;

            Widgets.Label( priorityLabelRect, "<= " + "HigherPriority".Translate() );

            priorityLabelRect.x += canvas.width / 3f;
            Widgets.Label( priorityLabelRect, "LowerPriority".Translate() + " =>" );

            GUI.color   = Color.white;
            Text.Font   = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DoTimeBar( Rect rect )
        {
            // set up rects
            var bar = TimeBarRect;
            var buttons = new Rect( rect.xMin, bar.yMin + bar.height / 3f, bar.xMin - rect.xMin,
                                    bar.height                       * 2 / 3f );
            var button = new Rect( buttons.xMax - buttons.height * 2f, buttons.yMin, buttons.height,
                                   buttons.height );

            // split the available area into rects. bottom 2/3's are used for 'buttons', with text for times.
            var hourWidth         = bar.width      / GenDate.HoursPerDay;
            var barheight         = bar.height * 2 / 3f;
            var timeIndicatorSize = bar.height * 2 / 3f;
            var lastLabelPosition = 0f;
            var hourRect          = new Rect( bar.xMin, bar.yMax - barheight, hourWidth, barheight );

            // draw buttons
            TooltipHandler.TipRegion( button, "WorkTab.SelectWholeDayTip".Translate() );
            if ( Widgets.ButtonImage( button, PrioritiesWholeDay, Color.white, GenUI.MouseoverColor ) )
                SelectWholeDay();
            button.x -= button.height + Constants.Margin;
            TooltipHandler.TipRegion( button, "WorkTab.SelectCurrentHourTip".Translate() );
            if ( Widgets.ButtonImage( button, Now, Color.white, GenUI.MouseoverColor ) )
                AddSelectedHour( GenLocalDate.HourOfDay( Find.CurrentMap ), true );

            // draw first tick
            GUI.color = Color.grey;
            Widgets.DrawLineVertical( hourRect.xMin, hourRect.yMin + hourRect.height * 1 / 2f,
                                      hourRect.height                                    * 1 / 2f );

            // draw horizontal line ( y - 1 because canvas gets clipped on bottom )
            Widgets.DrawLineHorizontal( bar.xMin, bar.yMax - 1, bar.width );
            GUI.color = Color.white;

            // label and rect
            string label;
            Rect   labelRect;

            for ( var hour = 0; hour < GenDate.HoursPerDay; hour++ )
            {
                var selected = SelectedHours.Contains( hour );
                var focused  = hour == VisibleHour;

                // print major tick
                GUI.color = Color.grey;
                Widgets.DrawLineVertical( hourRect.xMax, hourRect.yMin + hourRect.height * 1 / 2f,
                                          hourRect.height                                    * 1 / 2f );

                // print minor ticks
                Widgets.DrawLineVertical( hourRect.xMin + hourRect.width  * 1 / 4f,
                                          hourRect.yMin + hourRect.height * 3 / 4f, hourRect.height * 1 / 4f );
                Widgets.DrawLineVertical( hourRect.xMin + hourRect.width  * 2 / 4f,
                                          hourRect.yMin + hourRect.height * 3 / 4f, hourRect.height * 1 / 4f );
                Widgets.DrawLineVertical( hourRect.xMin + hourRect.width  * 3 / 4f,
                                          hourRect.yMin + hourRect.height * 3 / 4f, hourRect.height * 1 / 4f );
                GUI.color = Color.white;

                // create and draw labelrect - but only if the last label isn't too close
                if ( hourRect.xMin - lastLabelPosition > MinTimeBarLabelSpacing )
                {
                    label = hour.FormatHour();
                    labelRect = new Rect( 0f, bar.yMin + bar.height * 1 / 3f, label.NoWrapWidth(),
                                          bar.height                    * 2 / 3f );
                    labelRect.x = hourRect.xMin - labelRect.width / 2f;
                    LabelUtilities.Label( labelRect, label, Color.grey, GameFont.Tiny, TextAnchor.UpperCenter );

                    lastLabelPosition = labelRect.xMax;
                }

                // draw hour rect with mouseover + interactions
                Widgets.DrawHighlightIfMouseover( hourRect );

                // set/remove focus (LMB and any other MB respectively)
                if ( Mouse.IsOver( hourRect ) )
                {
                    if ( Input.GetMouseButton( 0 ) )
                        AddSelectedHour( hour, Shift );

                    if ( Input.GetMouseButton( 1 ) )
                        RemoveSelectedHour( hour );

                    // handle tooltip
                    var selectedString = selected
                        ? "WorkTab.Selected".Translate()
                        : "WorkTab.NotSelected".Translate();
                    var interactionString = "";
                    if ( selected )
                    {
                        interactionString += "WorkTab.RightClickToDeselect".Translate();
                        if ( focused )
                            interactionString += "\n" + "WorkTab.ClickToFocus".Translate();
                    }
                    else
                    {
                        interactionString += "WorkTab.ClickToSelect".Translate();
                    }

                    TooltipHandler.TipRegion( hourRect,
                                              "WorkTab.SchedulerHourTip".Translate( hour.FormatHour(),
                                                                                    ( hour + 1 % GenDate.HoursPerDay )
                                                                                   .FormatHour(),
                                                                                    selectedString,
                                                                                    interactionString ) );
                }

                // if this is currently the 'main' timeslot, and not the actual time, draw an eye
                if ( focused && hour != GenLocalDate.HourOfDay( Find.CurrentMap ) )
                {
                    var eyeRect = new Rect( hourRect.center.x                 - timeIndicatorSize * 1 / 2f,
                                            hourRect.yMax - timeIndicatorSize - hourRect.height   * 1 / 6f,
                                            timeIndicatorSize, timeIndicatorSize );
                    GUI.DrawTexture( eyeRect, PinEye );
                }

                // also highlight all selected timeslots
                if ( selected )
                    Widgets.DrawHighlightSelected( hourRect );

                // advance rect
                hourRect.x += hourRect.width;
            }

            // draw final label
            label       = 0.FormatHour();
            labelRect   = new Rect( 0f, bar.yMin + bar.height * 1 / 3f, label.NoWrapWidth(), bar.height * 2 / 3f );
            labelRect.x = hourRect.xMin - labelRect.width / 2f;
            LabelUtilities.Label( labelRect, label, Color.grey, GameFont.Tiny, TextAnchor.UpperCenter );

            // draw current time indicator
            var curTimeX = GenLocalDate.DayPercent( Find.CurrentMap ) * bar.width;
            var curTimeRect = new Rect( bar.xMin + curTimeX               - timeIndicatorSize * 1 / 2f,
                                        hourRect.yMax - timeIndicatorSize - hourRect.height   * 1 / 6f,
                                        timeIndicatorSize,
                                        timeIndicatorSize );
            GUI.DrawTexture( curTimeRect, PinClock );
        }

        private void DoToggleButtons( Rect canvas )
        {
            var button = new Rect( canvas.xMax - 30f, canvas.yMin, 30f, 30f );

            ButtonImageToggle( () => PriorityManager.ShowPriorities, val => PriorityManager.ShowPriorities = val,
                               button,
                               "WorkTab.PrioritiesDetailed".Translate(), PrioritiesDetailed,
                               "WorkTab.PrioritiesSimple".Translate(), PrioritiesSimple );
            button.x -= 30f + Margin;

            ButtonImageToggle( () => PriorityManager.ShowScheduler, val => PriorityManager.ShowScheduler = val, button,
                               "WorkTab.PrioritiesTimed".Translate(), PrioritiesTimed,
                               "WorkTab.PrioritiesWholeDay".Translate(), PrioritiesWholeDay );
            button.x -= 30f + Margin;

            ButtonImageToggle( () => AnyExpanded, ExpandAll, button,
                               "WorkTab.ExpandAll".Translate(), Expand,
                               "WorkTab.CollapseAll".Translate(), Collapse );
        }

        private void ExpandAll( bool expand )
        {
            foreach ( var expandableColumn in PawnTableDef.columns.Select( c => c.Worker ).OfType<IExpandableColumn>() )
                if ( expandableColumn.Expanded != expand && expandableColumn.CanExpand )
                    expandableColumn.Expanded = expand;
        }
    }
}