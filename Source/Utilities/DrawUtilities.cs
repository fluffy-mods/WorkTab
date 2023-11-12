// DrawUtilities.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab {
    [StaticConstructorOnStartup]
    public static class DrawUtilities {
        private static MethodInfo _drawWorkBoxBackgroundMethodInfo;

        public static void DrawPartScheduled(Rect box) {
            // draw clock icon in top left
            Color colour = GUI.color;
            GUI.color = Color.grey;
            GUI.DrawTexture(new Rect(box.xMin, box.yMin, box.height / 2f, box.width / 2f).ContractedBy(2f),
                             Resources.Half);
            GUI.color = colour;
        }

        public static void DrawPriority(Rect box, int priority, bool small = false) {
            // bail out if priority is 0
            if (priority == 0) {
                return;
            }

            GameFont font = small ? GameFont.Small : GameFont.Medium;

            // detailed mode
            if (Find.PlaySettings.useWorkPriorities) {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = font;
                GUI.color = ColorOfPriority(priority);
                Widgets.Label(box, priority.ToStringCached());
                GUI.color = Color.white;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            // toggle mode
            else {
                GUI.DrawTexture(box.ContractedBy(3f), WidgetsWork.WorkBoxCheckTex);
            }
        }

        public static void DrawTimeScheduled(Rect box) {
            // draw clock icon in bottom left
            Color colour = GUI.color;
            GUI.color = Color.grey;
            GUI.DrawTexture(
                new Rect(box.xMin, box.yMin + (box.height / 2f), box.height / 2f, box.width / 2f).ContractedBy(2f),
                Resources.Clock);
            GUI.color = colour;
        }

        public static void DrawWorkBoxBackground(Rect box, Pawn pawn, WorkTypeDef worktype) {
            if (_drawWorkBoxBackgroundMethodInfo == null) {
                _drawWorkBoxBackgroundMethodInfo = typeof(WidgetsWork).GetMethod("DrawWorkBoxBackground",
                                                                                    AccessTools.all);
                if (_drawWorkBoxBackgroundMethodInfo == null) {
                    throw new NullReferenceException("DrawWorkBoxBackground method info not found!");
                }
            }

            _drawWorkBoxBackgroundMethodInfo.Invoke(null, new object[] { box, pawn, worktype });
        }

        private static Texture2D currentJobHighlightBox;
        public static Texture2D GetCurrentJobHighlightBox()
        {
            if (currentJobHighlightBox != null)
            {
                return currentJobHighlightBox;
            }
            Color color = Color.Lerp(Color.black, Color.magenta, 0.7f);
            Texture2D startingExample = WidgetsWork.WorkBoxOverlay_PreceptWarning;
            int width = startingExample.width;
            int height = startingExample.height;
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = Enumerable.Repeat(color, width * height).ToArray();
            texture.SetPixels(pixels);
            texture.Apply();
            currentJobHighlightBox = texture;
            return currentJobHighlightBox;
        }

        public static string PriorityLabel(int priority) {
            /**
             *                  9   8   7   6   5   4
             * top              1   1   1   1   1   1
             * very high        2   2   2   2
             * high             3   3   3   3   2   2
             * above normal     4   4
             * normal           5       4       3   3
             * below normal     6   5
             * low              7   6   5   4   4   4
             * very low         8   7   6   5
             * lowest           9   8   7   6   5
             *
             * KILL ME NOW.
             */

            string label;
            switch (priority) {
                case 0:
                    label = "Fluffy.WorkTab.Priority.None".Translate();
                    break;
                case 1:
                    label = "Fluffy.WorkTab.Priority.Top".Translate();
                    break;
                case 2 when Settings.maxPriority >= 6:
                    label = "Fluffy.WorkTab.Priority.VeryHigh".Translate();
                    break;
                case 2 when Settings.maxPriority <= 5:
                    label = "Fluffy.WorkTab.Priority.High".Translate();
                    break;
                case 3 when !Find.PlaySettings.useWorkPriorities:
                    label = "Fluffy.WorkTab.Priority.Normal".Translate();
                    break;
                case 3 when Settings.maxPriority >= 6:
                    label = "Fluffy.WorkTab.Priority.High".Translate();
                    break;
                case 3 when Settings.maxPriority <= 5:
                    label = "Fluffy.WorkTab.Priority.Normal".Translate();
                    break;
                case 4 when Settings.maxPriority >= 8:
                    label = "Fluffy.WorkTab.Priority.AboveNormal".Translate();
                    break;
                case 4 when Settings.maxPriority == 7:
                    label = "Fluffy.WorkTab.Priority.Normal".Translate();
                    break;
                case 4 when Settings.maxPriority <= 6:
                    label = "Fluffy.WorkTab.Priority.Low".Translate();
                    break;
                case 5 when Settings.maxPriority == 9:
                    label = "Fluffy.WorkTab.Priority.Normal".Translate();
                    break;
                case 5 when Settings.maxPriority == 8:
                    label = "Fluffy.WorkTab.Priority.BelowNormal".Translate();
                    break;
                case 5 when Settings.maxPriority == 7:
                    label = "Fluffy.WorkTab.Priority.Low".Translate();
                    break;
                case 5 when Settings.maxPriority == 6:
                    label = "Fluffy.WorkTab.Priority.VeryLow".Translate();
                    break;
                case 5 when Settings.maxPriority == 5:
                    label = "Fluffy.WorkTab.Priority.Lowest".Translate();
                    break;
                case 6 when Settings.maxPriority == 9:
                    label = "Fluffy.WorkTab.Priority.BelowNormal".Translate();
                    break;
                case 6 when Settings.maxPriority == 8:
                    label = "Fluffy.WorkTab.Priority.Low".Translate();
                    break;
                case 6 when Settings.maxPriority == 7:
                    label = "Fluffy.WorkTab.Priority.VeryLow".Translate();
                    break;
                case 6 when Settings.maxPriority == 6:
                    label = "Fluffy.WorkTab.Priority.Lowest".Translate();
                    break;
                case 7 when Settings.maxPriority == 9:
                    label = "Fluffy.WorkTab.Priority.Low".Translate();
                    break;
                case 7 when Settings.maxPriority == 8:
                    label = "Fluffy.WorkTab.Priority.VeryLow".Translate();
                    break;
                case 7 when Settings.maxPriority == 7:
                    label = "Fluffy.WorkTab.Priority.Lowest".Translate();
                    break;
                case 8 when Settings.maxPriority == 9:
                    label = "Fluffy.WorkTab.Priority.VeryLow".Translate();
                    break;
                case 8 when Settings.maxPriority == 8:
                    label = "Fluffy.WorkTab.Priority.Lowest".Translate();
                    break;
                case 9:
                    label = "Fluffy.WorkTab.Priority.Lowest".Translate();
                    break;
                default:
                    label = "Fluffy.WorkTab.Priority.Unknown".Translate();
                    break;
            }

            return label.Colorize(ColorOfPriority(priority));
        }

        public static string TimeScheduledTip(int[] priorities, string label) {
            string tip = "";
            int start = -1;
            int priority = -1;

            for (int hour = 0; hour < GenDate.HoursPerDay; hour++) {
                int curpriority = priorities[hour];

                // stop condition
                if (curpriority != priority && start >= 0) {
                    tip += "\n   ";
                    tip += start.FormatHour() + " - " + hour.FormatHour();
                    if (Find.PlaySettings.useWorkPriorities) {
                        tip += " (" + priority + ")";
                    }

                    // reset start & priority
                    start = -1;
                    priority = -1;
                }

                // start condition
                if (curpriority > 0 && curpriority != priority && start < 0) {
                    priority = curpriority;
                    start = hour;
                }
            }

            // final check for x till midnight
            if (start > 0) {
                tip += "\n   ";
                tip += start.FormatHour() + " - " + 0.FormatHour();
                if (Find.PlaySettings.useWorkPriorities) {
                    tip += " (" + priority + ")";
                }
            }

            return tip;
        }

        public static string TipForPawnWorker(Pawn pawn, WorkGiverDef workgiver, bool incapableBecauseOfCapacities) {
            StringBuilder tip = new StringBuilder();
            tip.Append(workgiver.LabelCap);
            tip.Append(": " + PriorityLabel(pawn.GetPriority(workgiver, -1)));
            tip.AppendLine();

            if (pawn.WorkTypeIsDisabled(workgiver.workType) ||
                 (workgiver.workTags & pawn.story.DisabledWorkTagsBackstoryAndTraits) != WorkTags.None) {
                tip.Append("CannotDoThisWork".Translate(pawn.LabelShort, pawn));
            } else {
                float skill = pawn.skills.AverageOfRelevantSkillsFor(workgiver.workType);
                if (workgiver.workType.relevantSkills.Any()) {
                    tip.AppendLine("RelevantSkills".Translate(
                                        workgiver.workType.relevantSkills.Select(s => s.skillLabel).ToCommaList(),
                                        skill.ToString("0.#"), 20));
                }

                if (!workgiver.description.NullOrEmpty()) {
                    tip.AppendLine();
                    tip.Append(workgiver.description);
                }

                if (incapableBecauseOfCapacities) {
                    tip.AppendLine();
                    tip.AppendLine();
                    tip.Append("IncapableOfWorkTypeBecauseOfCapacities".Translate());
                }

                PriorityTracker tracker = PriorityManager.Get[pawn];
                if (tracker.TimeScheduled(workgiver)) {
                    tip.AppendLine();
                    tip.Append("WorkTab.XIsAssignedToY".Translate(pawn.NameShortColored, workgiver.LabelCap));
                    tip.Append(tracker.TimeScheduledTip(workgiver));
                }
            }

            return tip.ToString();
        }

        public static string TipForPawnWorker(Pawn pawn, WorkTypeDef worktype, bool incapableBecauseOfCapacities) {
            StringBuilder tip = new StringBuilder();
            tip.Append(worktype.gerundLabel.CapitalizeFirst());
            tip.Append(": " + PriorityLabel(pawn.workSettings.GetPriority(worktype)));
            tip.AppendLine();

            if (pawn.WorkTypeIsDisabled(worktype)) {
                tip.AppendLine("CannotDoThisWork".Translate(pawn.LabelShort, pawn));

                foreach (string reason in pawn.GetReasonsForDisabledWorkType(worktype)) {
                    tip.AppendLine();
                    tip.Append("- " + reason);
                }
            } else {
                float num = pawn.skills.AverageOfRelevantSkillsFor(worktype);
                if (worktype.relevantSkills.Any()) {
                    tip.AppendLine("RelevantSkills".Translate(
                                        worktype.relevantSkills.Select(s => s.skillLabel).ToCommaList(),
                                        num.ToString("0.#"), 20));
                }

                tip.AppendLine();
                tip.Append(worktype.description);

                if (incapableBecauseOfCapacities) {
                    tip.AppendLine();
                    tip.AppendLine();
                    tip.Append("IncapableOfWorkTypeBecauseOfCapacities".Translate());
                }

                if (worktype.relevantSkills.Any() && num <= 2f && pawn.workSettings.WorkIsActive(worktype)) {
                    tip.AppendLine();
                    tip.AppendLine();
                    tip.Append("SelectedWorkTypeWithVeryBadSkill".Translate());
                }

                PriorityTracker tracker = PriorityManager.Get[pawn];
                if (tracker.TimeScheduled(worktype)) {
                    tip.AppendLine();
                    tip.Append("WorkTab.XIsAssignedToY".Translate(pawn.NameShortColored, worktype.LabelCap));
                    tip.Append(tracker.TimeScheduledTip(worktype));
                }
            }

            return tip.ToString();
        }

        private static Color ColorOfPriority(int priority) {
            if (priority == 0) {
                return Color.grey;
            }

            float halfway = Settings.maxPriority / 2f;

            if (priority <= halfway) {
                return Color.Lerp(Color.green, Color.white, Mathf.InverseLerp(1, halfway, priority));
            }

            return Color.Lerp(Color.white, Color.grey, Mathf.InverseLerp(halfway, Settings.maxPriority, priority));
        }
    }
}
