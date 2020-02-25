// Karel Kroeze
// DrawUtilities.cs
// 2017-05-25

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public static class DrawUtilities
    {
        private static MethodInfo _drawWorkBoxBackgroundMethodInfo;
        public static void DrawWorkBoxBackground(Rect box, Pawn pawn, WorkTypeDef worktype)
        {
            if (_drawWorkBoxBackgroundMethodInfo == null)
            {
                _drawWorkBoxBackgroundMethodInfo = typeof(WidgetsWork).GetMethod("DrawWorkBoxBackground",
                                                                                    AccessTools.all);
                if (_drawWorkBoxBackgroundMethodInfo == null)
                    throw new NullReferenceException("DrawWorkBoxBackground method info not found!");
            }

            _drawWorkBoxBackgroundMethodInfo.Invoke(null, new object[] { box, pawn, worktype });
        }

        public static void DrawPriority( Rect box, int priority, bool small = false )
        {
            // bail out if priority is 0
            if ( priority == 0 )
                return;

            GameFont font = small ? GameFont.Small : GameFont.Medium;

            // detailed mode
            if ( Find.PlaySettings.useWorkPriorities )
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = font;
                GUI.color = ColorOfPriority( priority );
                Widgets.Label( box, priority.ToStringCached() );
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            // toggle mode
            else
                GUI.DrawTexture( box.ContractedBy( 3f ), WidgetsWork.WorkBoxCheckTex );
        }

        public static void DrawTimeScheduled(Rect box)
        {
            // draw clock icon in bottom left
            var colour = GUI.color;
            GUI.color = Color.grey;
            GUI.DrawTexture(new Rect(box.xMin, box.yMin + box.height / 2f, box.height / 2f, box.width / 2f).ContractedBy(2f), Resources.Clock);
            GUI.color = colour;
        }

        public static void DrawPartScheduled(Rect box)
        {
            // draw clock icon in top left
            var colour = GUI.color;
            GUI.color = Color.grey;
            GUI.DrawTexture(new Rect(box.xMin, box.yMin, box.height / 2f, box.width / 2f).ContractedBy(2f), Resources.Half);
            GUI.color = colour;
        }

        private static Color ColorOfPriority(int priority)
        {
            if (priority == 0)
                return Color.grey;

            var halfway = Settings.maxPriority / 2f;

            if ( priority <= halfway )
                return Color.Lerp( Color.green, Color.white, Mathf.InverseLerp( 1, halfway, priority ) );
            return Color.Lerp( Color.white, Color.grey, Mathf.InverseLerp( halfway, Settings.maxPriority, priority ) );
        }

        public static string TipForPawnWorker(Pawn pawn, WorkGiverDef workgiver, bool incapableBecauseOfCapacities)
        {
            var tip = $"{workgiver.LabelCap} ({workgiver.workType.gerundLabel})";
            if (pawn.WorkTypeIsDisabled(workgiver.workType))
                tip += "\n\n" + "CannotDoThisWork".Translate( pawn.Name.ToStringShort );
            else
            {
                if ( workgiver.workType.relevantSkills.Count > 0 )
                {
                    var skills = string.Join(", ",
                                                  workgiver.workType.relevantSkills.Select(s => s.skillLabel)
                                                           .ToArray());
                    tip += "\n\n" + 
                           "RelevantSkills".Translate( skills, pawn.skills.AverageOfRelevantSkillsFor( workgiver.workType ), 20 );
                }
            }

            if (incapableBecauseOfCapacities)
                tip += "\n\n" + "IncapableOfWorkTypeBecauseOfCapacities".Translate();
            
            var tracker = PriorityManager.Get[pawn];
            if (tracker.EverScheduled(workgiver))
                tip += "\n\n" + "WorkTab.XIsAssignedToY".Translate(pawn.Name.ToStringShort, workgiver.label);

            if (tracker.TimeScheduled(workgiver))
                tip += tracker.TimeScheduledTip(workgiver);

            return tip;
        }

        public static string TipForPawnWorker(Pawn pawn, WorkTypeDef worktype, bool incapableBecauseOfCapacities)
        {
            string tip = WidgetsWork.TipForPawnWorker(pawn, worktype, incapableBecauseOfCapacities);

            var tracker = PriorityManager.Get[pawn];
            if (tracker.EverScheduled(worktype))
                tip += "\n\n" + "WorkTab.XIsAssignedToY".Translate(pawn.Name.ToStringShort, worktype.gerundLabel);

            if (tracker.TimeScheduled(worktype))
                tip += tracker.TimeScheduledTip(worktype);

            return tip;
        }

        public static string TimeScheduledTip( int[] priorities, string label )
        {
            string tip = "";
            int start = -1;
            int priority = -1;

            for (int hour = 0; hour < GenDate.HoursPerDay; hour++)
            {
                int curpriority = priorities[hour];

                // stop condition
                if (curpriority != priority && start >= 0)
                {
                    tip += "\n   ";
                    tip += start.FormatHour() + " - " + hour.FormatHour();
                    if (Find.PlaySettings.useWorkPriorities)
                        tip += " (" + priority + ")";

                    // reset start & priority
                    start = -1;
                    priority = -1;
                }

                // start condition
                if (curpriority > 0 && curpriority != priority && start < 0)
                {
                    priority = curpriority;
                    start = hour;
                }
            }

            // final check for x till midnight
            if (start > 0)
            {
                tip += "\n   ";
                tip += start.FormatHour() + " - " + 0.FormatHour();
                if (Find.PlaySettings.useWorkPriorities)
                    tip += " (" + priority + ")";
            }

            return tip;
        }
    }
}