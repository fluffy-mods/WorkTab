// Karel Kroeze
// PawnTable_WorkTab.cs
// 2017-05-22

using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static WorkTab.Constants;
using static WorkTab.InteractionUtilities;
using static WorkTab.Resources;

namespace WorkTab
{
    public class MainTabWindow_WorkTab: MainTabWindow_PawnTable
    {
        protected override PawnTableDef PawnTableDef => PawnTableDefOf.Work;
        private static FieldInfo _tableFieldInfo;

        static MainTabWindow_WorkTab()
        {
            _tableFieldInfo = typeof(MainTabWindow_PawnTable).GetField("table",
                                                                          BindingFlags.Instance | BindingFlags.NonPublic);
            if (_tableFieldInfo == null)
                throw new NullReferenceException("table field not found!");
        }
        public MainTabWindow_WorkTab() { _instance = this; }

        private static MainTabWindow_WorkTab _instance;
        public static MainTabWindow_WorkTab Instance => _instance;

        public override void PostOpen()
        {
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
            RebuildTable();
        }
        
        public PawnTable Table
        {
            get { return _tableFieldInfo.GetValue(this) as PawnTable; }
            private set { _tableFieldInfo.SetValue(this, value); }
        }

        public static void RebuildTable()
        {
            if ( _instance == null )
            {
                Logger.Debug( "calling RebuildTable on a null instance" );
                return;
            }

            // get clean copy of base columns
            var columns = new List<PawnColumnDef>( _instance.PawnTableDef.columns );

            // add workgiver columns for expanded worktypes
            var templist = new List<PawnColumnDef>( columns );
            foreach ( PawnColumnDef column in templist )
            {
                var expandable = column.Worker as IExpandableColumn;
                if ( expandable != null && expandable.Expanded )
                {
                    var index = columns.IndexOf( column );
                    columns.InsertRange( index + 1, expandable.ChildColumns );
                }
            }
            
            // update alternating vertical positions
            bool moveNextDown = false;
            foreach ( PawnColumnDef columnDef in columns )
            {
                var worker = columnDef.Worker as IAlternatingColumn;
                if ( worker != null )
                {
                    worker.MoveDown = moveNextDown;
                    moveNextDown = !moveNextDown;
                }
                else
                {
                    moveNextDown = false;
                }
            }

            // rebuild table
            Instance.Table = new PawnTable( columns, () => Instance.Pawns, 998, UI.screenWidth - (int)(Instance.Margin * 2f), 0,
                                   (int)(UI.screenHeight - 35 - Instance.ExtraBottomSpace - Instance.ExtraTopSpace - Instance.Margin * 2f));
        }

        public override void DoWindowContents( Rect rect )
        {
            base.DoWindowContents( rect );
            if (Event.current.type == EventType.Layout)
                return;

            DoToggleButtons( rect );
            DoPriorityLabels( rect );
            DoTimeBar( rect );
        }

        private void DoTimeBar( Rect canvas )
        {
            Rect rect = new Rect( 0f, canvas.yMax - TimeBarHeight - base.ExtraBottomSpace, canvas.width, TimeBarHeight );
            //Widgets.DrawBox( rect );
        }

        private void DoPriorityLabels( Rect canvas )
        {
            Rect priorityLabelRect = new Rect( canvas.width / 3f - PriorityLabelSize.x / 2f,
                                               canvas.yMin,
                                               PriorityLabelSize.x,
                                               PriorityLabelSize.y );

            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Tiny;

            Widgets.Label( priorityLabelRect, "<= " + "HigherPriority".Translate());

            priorityLabelRect.x += canvas.width / 3f;
            Widgets.Label( priorityLabelRect, "LowerPriority".Translate() + " =>");

            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DoToggleButtons( Rect canvas )
        {
            Rect rect = new Rect( canvas.xMax - 30f, canvas.yMin, 30f, 30f );

            ButtonImageToggle( ref Find.PlaySettings.useWorkPriorities, rect,
                               "WorkTab.PrioritiesDetailed".Translate(), PrioritiesDetailed,
                               "WorkTab.PrioritiesSimple".Translate(), PrioritiesSimple );
            rect.x -= 30f + Margin;

            ButtonImageToggle( ref PriorityManager.Get.useScheduler, rect,
                               "WorkTab.PrioritiesTimed".Translate(), PrioritiesTimed,
                               "WorkTab.PrioritiesWholeDay".Translate(), PrioritiesWholeDay);
            rect.x -= 30f + Margin;
        }

        protected override float ExtraBottomSpace => base.ExtraBottomSpace + TimeBarHeight;
        protected override float ExtraTopSpace => base.ExtraTopSpace + Constants.ExtraTopSpace;
    }
}