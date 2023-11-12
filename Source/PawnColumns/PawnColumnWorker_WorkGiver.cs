// PawnColumnWorker_WorkGiver.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using static WorkTab.Constants;
using static WorkTab.InteractionUtilities;
using static WorkTab.MainTabWindow_WorkTab;
using static WorkTab.Resources;

namespace WorkTab {
    public class PawnColumnWorker_WorkGiver: PawnColumnWorker, IAlternatingColumn {
        private string _headerTip;


        private Vector2 cachedLabelSize = Vector2.zero;

        public List<Pawn> CapablePawns => Instance
                                         .Table
                                         .PawnsListForReading
                                         .Where(ShouldDrawCell)
                                         .ToList();

        public PawnColumnDef_WorkGiver Def => def as PawnColumnDef_WorkGiver;

        public string Label => WorkGiver.label.NullOrEmpty() ? WorkGiver.gerund : WorkGiver.LabelCap.Resolve();

        public WorkGiverDef WorkGiver => Def?.workgiver;

        public Vector2 LabelSize {
            get {
                if (cachedLabelSize == Vector2.zero) {
                    cachedLabelSize = Text.CalcSize(Label);
                    cachedLabelSize.x = Mathf.Min(cachedLabelSize.x, (WorkGiverWidth * 2) - Margin);
                }

                return cachedLabelSize;
            }
        }

        public bool MoveDown { get; set; }

        public override int Compare(Pawn a, Pawn b) {
            return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
        }

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            // bail out if showing worksettings is nonsensical
            if (pawn.Dead
              || !pawn.workSettings.EverWork) {
                return;
            }

            WorkGiverDef workgiver = WorkGiver;

            if (Settings.highlightCurrentJobCell)
            {
                bool doingNow = (pawn.CurJob?.workGiverDef?.defName == workgiver?.defName);
                if (doingNow)
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(rect.ContractedBy(-2f), DrawUtilities.GetCurrentJobHighlightBox());
                }
            }

            // create rect in centre of cell, slightly offsetting left to give the appearance of aligning to worktype.
            Vector2 pos = rect.center - (new Vector2( WorkGiverBoxSize, WorkGiverBoxSize ) / 2f);
            Rect box = new Rect( pos.x - 2f, pos.y, WorkGiverBoxSize, WorkGiverBoxSize );

            // is the pawn currently capable of doing this job?
            bool incapable = !pawn.CapableOf( workgiver );

            // plop in the tooltip
            string tipGetter() { return DrawUtilities.TipForPawnWorker(pawn, workgiver, incapable); }
            TooltipHandler.TipRegion(box, tipGetter, pawn.thingIDNumber ^ workgiver.workType.GetHashCode());

            // bail out if pawn can't actually do this work
            if (!ShouldDrawCell(pawn)) {
                return;
            }

            // draw the workbox
            Text.Font = GameFont.Tiny;
            DrawWorkGiverBoxFor(box, pawn, workgiver, incapable);

            // handle interactions
            HandleInteractions(rect, pawn);
        }

        public override void DoHeader(Rect rect, PawnTable table) {
            // make sure we're at the correct font size
            Text.Font = GameFont.Tiny;
            Rect labelRect = rect;

            if (Settings.verticalLabels) {
                DrawVerticalHeader(rect, table);
            } else {
                DrawHorizontalHeader(rect, table, out labelRect);
            }

            // handle interactions (click + scroll)
            HeaderInteractions(labelRect, table);

            // mouseover stuff
            Widgets.DrawHighlightIfMouseover(labelRect);
            TooltipHandler.TipRegion(labelRect, GetHeaderTip(table));

            // sort icon
            if (table.SortingBy == def) {
                Texture2D sortIcon = !table.SortingDescending ? SortingIcon : SortingDescendingIcon;
                Rect bottomRight = new Rect( rect.xMax - sortIcon.width - 1f, rect.yMax - sortIcon.height - 1f,
                                            sortIcon.width, sortIcon.height );
                GUI.DrawTexture(bottomRight, sortIcon);
            }
        }

        public void DrawHorizontalHeader(Rect rect, PawnTable table, out Rect labelRect) {
            // get offset rect
            labelRect = GetLabelRect(rect);

            // draw label, slightly greyed out to contrast with work types
            GUI.color = new Color(1f, 1f, 1f, .8f);
            Widgets.Label(labelRect, Label.Truncate(labelRect.width, TruncationCache));
            GUI.color = Color.white;

            // get bottom center of label, offset left to align with boxes
            Vector2 start  = new Vector2( labelRect.center.x - 2f, labelRect.yMax );
            float length = rect.yMax - start.y;

            // make sure we're not at a whole pixel
            if (start.x - (int) start.x < 1e-4) {
                start.x += .5f;
            }

            // draw the lines - two separate lines give a clearer edge than one 2px line...
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            Widgets.DrawLineVertical(Mathf.FloorToInt(start.x), start.y, length);
            Widgets.DrawLineVertical(Mathf.CeilToInt(start.x), start.y, length);
            GUI.color = Color.white;
        }

        public void DrawVerticalHeader(Rect rect, PawnTable table) {
            GUI.color = new Color(.8f, .8f, .8f);
            Text.Anchor = TextAnchor.MiddleLeft;
            LabelUtilities.VerticalLabel(rect, Label.Truncate(rect.height, VerticalTruncationCache));
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        public override int GetMinHeaderHeight(PawnTable table) {
            return Settings.verticalLabels ? VerticalHeaderHeight : HorizontalHeaderHeight;
        }

        public override int GetMinWidth(PawnTable table) {
            return WorkGiverWidth;
        }

        public void HeaderInteractions(Rect headerRect, PawnTable table) {
            if (!Mouse.IsOver(headerRect)) {
                return;
            }

            // handle interactions
            if (Shift) {
                // deal with clicks and scrolls
                if (Find.PlaySettings.useWorkPriorities) {
                    if (ScrolledUp(headerRect, true) || RightClicked(headerRect)) {
                        WorkGiver.IncrementPriority(CapablePawns, VisibleHour, SelectedHours);
                    }

                    if (ScrolledDown(headerRect, true) || LeftClicked(headerRect)) {
                        WorkGiver.DecrementPriority(CapablePawns, VisibleHour, SelectedHours);
                    }
                } else {
                    // this gets slightly more complicated
                    List<Pawn> pawns = CapablePawns;
                    if (ScrolledUp(headerRect, true) || RightClicked(headerRect)) {
                        if (pawns.Any(p => p.GetPriority(WorkGiver, VisibleHour) != 0)) {
                            if (Settings.playSounds) {
                                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            }

                            foreach (Pawn pawn in pawns) {
                                pawn.SetPriority(WorkGiver, 0, SelectedHours);
                            }
                        }
                    }

                    if (ScrolledDown(headerRect, true) || LeftClicked(headerRect)) {
                        if (pawns.Any(p => p.GetPriority(WorkGiver, VisibleHour) == 0)) {
                            if (Settings.playSounds) {
                                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                            }

                            foreach (Pawn pawn in pawns) {
                                pawn.SetPriority(WorkGiver, 3, SelectedHours);
                            }
                        }
                    }
                }
            } else if (def.sortable) {
                if (LeftClicked(headerRect)) {
                    Sort(table);
                }

                if (RightClicked(headerRect)) {
                    Sort(table, false);
                }
            }
        }

        public bool ShouldDrawCell(Pawn pawn) {
            if (pawn?.story == null) {
                return false;
            }

            return !pawn.WorkTypeIsDisabled(WorkGiver.workType);
        }

        protected virtual void DrawWorkGiverBoxFor(Rect box, Pawn pawn, WorkGiverDef workgiver, bool incapable) {
            // draw background
            GUI.color = incapable ? new Color(1f, .3f, .3f) : Color.white;
            DrawUtilities.DrawWorkBoxBackground(box, pawn, workgiver.workType);
            GUI.color = Color.white;

            // draw extras
            PriorityTracker tracker = PriorityManager.Get[pawn];
            if (tracker.TimeScheduled(workgiver)) {
                DrawUtilities.DrawTimeScheduled(box);
            }

            // draw priorities / checks
            DrawUtilities.DrawPriority(box, pawn.GetPriority(workgiver, VisibleHour), true);
        }

        protected override string GetHeaderTip(PawnTable table) {
            if (_headerTip.NullOrEmpty()) {
                _headerTip = CreateHeaderTip(table);
            }

            return _headerTip;
        }

        protected virtual void HandleInteractions(Rect rect, Pawn pawn) {
            if (Find.PlaySettings.useWorkPriorities) {
                HandleInteractionsDetailed(rect, pawn);
            } else {
                HandleInteractionsToggle(rect, pawn);
            }
        }


        protected override void HeaderClicked(Rect headerRect, PawnTable table) {
            // replaced with HeaderInteractions
        }

        protected virtual void Sort(PawnTable table, bool ascending = true) {
            if (ascending) {
                if (table.SortingBy != def) {
                    table.SortBy(def, true);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                } else if (table.SortingDescending) {
                    table.SortBy(def, false);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                } else {
                    table.SortBy(null, false);
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                }
            } else {
                if (table.SortingBy != def) {
                    table.SortBy(def, false);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                } else if (table.SortingDescending) {
                    table.SortBy(null, false);
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                } else {
                    table.SortBy(def, true);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }
            }
        }

        private string CreateHeaderTip(PawnTable table) {
            string tip = "";
            tip += Label + "\n";
            if (!WorkGiver.description.NullOrEmpty()) {
                tip += "\n" + WorkGiver.description + "\n";
            }

            tip += "\n" + "ClickToSortByThisColumn".Translate();
            if (Find.PlaySettings.useWorkPriorities) {
                tip += "\n" + "WorkTab.DetailedColumnTip".Translate();
            } else {
                tip += "\n" + "WorkTab.ToggleColumnTip".Translate();
            }

            return tip;
        }

        private Rect GetLabelRect(Rect headerRect) {
            float x = headerRect.center.x;
            // move down slightly since we're using a smaller font here.
            Rect result = new Rect( x - (LabelSize.x / 2f) - 2f, headerRect.y + 3f, LabelSize.x, LabelSize.y );
            if (MoveDown) {
                result.y += 20f;
            }

            return result;
        }

        private float GetValueToCompare(Pawn pawn) {
            if (pawn.workSettings == null || !pawn.workSettings.EverWork) {
                return -2f;
            }

            if (pawn.story != null && pawn.WorkTypeIsDisabled(WorkGiver.workType)) {
                return -1f;
            }

            return pawn.skills.AverageOfRelevantSkillsFor(WorkGiver.workType);
        }

        private void HandleInteractionsDetailed(Rect rect, Pawn pawn) {
            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.ScrollWheel)
              && Mouse.IsOver(rect)) {
                // track priority so we can play appropriate sounds
                int oldpriority = pawn.GetPriority( WorkGiver, VisibleHour );

                // deal with clicks and scrolls
                if (ScrolledUp(rect, true) || RightClicked(rect)) {
                    WorkGiver.IncrementPriority(pawn, VisibleHour, SelectedHours);
                }

                if (ScrolledDown(rect, true) || LeftClicked(rect)) {
                    WorkGiver.DecrementPriority(pawn, VisibleHour, SelectedHours);
                }

                // get new priority, play crunch if it wasn't pretty
                int newPriority = pawn.GetPriority( WorkGiver, VisibleHour );
                if (Settings.playSounds && Settings.playCrunch &&
                     oldpriority == 0 && newPriority > 0 &&
                     pawn.skills.AverageOfRelevantSkillsFor(WorkGiver.workType) <= 2f) {
                    SoundDefOf.Crunch.PlayOneShotOnCamera();
                }

                // update tutorials
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab,
                                                               KnowledgeAmount.SpecificInteraction);
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ManualWorkPriorities,
                                                               KnowledgeAmount.SmallInteraction);
            }
        }

        private void HandleInteractionsToggle(Rect rect, Pawn pawn) {
            if ((Event.current.type == EventType.MouseDown ||
                   Event.current.type == EventType.ScrollWheel && !Settings.disableScrollwheel)
              && Mouse.IsOver(rect)) {
                // track priority so we can play appropriate sounds
                bool active = pawn.GetPriority( WorkGiver, VisibleHour ) > 0;

                if (active) {
                    pawn.SetPriority(WorkGiver, 0, SelectedHours);
                    if (Settings.playSounds) {
                        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                    }
                } else {
                    pawn.SetPriority(WorkGiver, Mathf.Min(Settings.maxPriority, 3), SelectedHours);
                    if (Settings.playSounds) {
                        SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                    }

                    if (Settings.playSounds && Settings.playCrunch &&
                         pawn.skills.AverageOfRelevantSkillsFor(WorkGiver.workType) <= 2f) {
                        SoundDefOf.Crunch.PlayOneShotOnCamera();
                    }
                }

                // stop event propagation, update tutorials
                Event.current.Use();
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab,
                                                               KnowledgeAmount.SpecificInteraction);
            }
        }
    }
}
