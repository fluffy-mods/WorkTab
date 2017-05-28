// Karel Kroeze
// PawnTable_WorkTab.cs
// 2017-05-22

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace WorkTab
{
    public class MainTabWindow_WorkTab: MainTabWindow_Work
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
    }
}