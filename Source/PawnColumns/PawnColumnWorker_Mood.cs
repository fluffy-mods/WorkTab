// PawnColumnWorker_Mood.cs
// Copyright Karel Kroeze, 2020-2020

using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public class PawnColumnWorker_Mood : PawnColumnWorker_Icon
    {
        public override int Compare( Pawn a, Pawn b )
        {
            return GetValueToCompare( a ).CompareTo( GetValueToCompare( b ) );
        }

        public override void DoCell( Rect rect, Pawn pawn, PawnTable table )
        {
            if ( pawn.needs?.mood == null )
                return;
            base.DoCell( rect, pawn, table );
        }

        public float GetValueToCompare( Pawn pawn )
        {
            return pawn.needs.mood?.CurLevelPercentage - pawn.mindState.mentalBreaker.BreakThresholdMinor ?? 0f;
        }

        protected override Color GetIconColor( Pawn pawn )
        {
            // broken
            if ( pawn.mindState?.mentalStateHandler?.CurStateDef != null )
                switch ( pawn.mindState.mentalStateHandler.CurStateDef.category )
                {
                    case MentalStateCategory.Aggro:
                        return Color.red;
                    case MentalStateCategory.Malicious:
                        return Color.yellow;
                    case MentalStateCategory.Misc:
                    case MentalStateCategory.Undefined:
                        return new Color( 207 / 256f, 83 / 256f, 0f );
                    default:
                        return Color.white;
                }

            if ( pawn.needs?.mood == null )
                return Color.white;
            // current level
            var mood      = pawn.needs.mood.CurLevelPercentage;
            var hardBreak = pawn.mindState.mentalBreaker.BreakThresholdExtreme;
            var softBreak = pawn.mindState.mentalBreaker.BreakThresholdMinor;

            // color
            if ( mood < hardBreak )
                return Color.red;
            if ( mood < softBreak )
                return Color.Lerp( Color.red, Color.yellow, ( mood - hardBreak ) / ( softBreak - hardBreak ) );
            if ( mood < .5 )
                return Color.Lerp( Color.yellow, Color.grey, ( mood - softBreak ) / ( .5f - softBreak ) );
            if ( mood < .9 )
                return Color.Lerp( Color.grey, Color.green, ( mood - .5f ) / .4f );

            return Color.green;
        }

        protected override Texture2D GetIconFor( Pawn pawn )
        {
            // broken
            if ( pawn.mindState.mentalStateHandler?.CurStateDef != null )
                return Resources.MoodBroken;

            // current level
            var mood      = pawn.needs.mood?.CurLevelPercentage;
            var softBreak = pawn.mindState.mentalBreaker.BreakThresholdMinor;

            // icon
            if ( mood < softBreak )
                return Resources.MoodUnhappy;
            if ( mood < .5 )
                return Resources.MoodDiscontent;
            if ( mood < .9 )
                return Resources.MoodContent;
            return Resources.MoodHappy;
        }

        protected override Vector2 GetIconSize( Pawn pawn )
        {
            return def.HeaderIconSize;
        }

        protected override string GetIconTip( Pawn pawn )
        {
            return pawn.needs.mood?.GetTipString() ?? string.Empty;
        }
    }
}