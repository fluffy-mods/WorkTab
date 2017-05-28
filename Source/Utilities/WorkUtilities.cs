// Karel Kroeze
// WorkUtilities.cs
// 2017-05-25

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public static class WorkUtilities
    {
        private static MethodInfo _drawWorkBoxBackgroundMethodInfo;
        public static void DrawWorkBoxBackground(Rect box, Pawn pawn, WorkTypeDef worktype)
        {
            if (_drawWorkBoxBackgroundMethodInfo == null)
            {
                _drawWorkBoxBackgroundMethodInfo = typeof(WidgetsWork).GetMethod("DrawWorkBoxBackground",
                                                                                    AccessTools.all);
                if (_drawWorkBoxBackgroundMethodInfo == null)
                {
                    throw new NullReferenceException("DrawWorkBoxBackground method info not found!");
                }
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

        private static Color ColorOfPriority(int priority)
        {
            // first 1/3rd of priority levels lerp from green to white, then lerp to grey
            if (priority == 0)
                return Color.grey;

            // define sizes of colour ranges 
            int firstThird = Settings.maxPriority / 3;
            int rest = Settings.maxPriority - firstThird;

            // from green to white
            if ( priority <= firstThird )
                return Color.Lerp( Color.green, Color.white, ((float) priority) / firstThird );

            // white to grey
            return Color.Lerp( Color.white, Color.grey, ( (float) ( priority - firstThird ) ) / rest );
        }


        public static string TipForPawnWorker(Pawn pawn, WorkGiverDef workgiver, bool incapableBecauseOfCapacities)
        {
            var tip = $"{workgiver.LabelCap} ({workgiver.workType.gerundLabel})";
            if (pawn.story.WorkTypeIsDisabled(workgiver.workType))
                tip += "\n\n" + "CannotDoThisWork".Translate( pawn.NameStringShort );
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
            {
                tip += "\n\n" + "IncapableOfWorkTypeBecauseOfCapacities".Translate();
            }
            return tip;
        }
    }
}