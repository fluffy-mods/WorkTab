// Karel Kroeze
// PawnColumnWorker_Mood.cs
// 2017-05-30

using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public class PawnColumnWorker_Mood: PawnColumnWorker_Icon
    {
        public override int Compare( Pawn a, Pawn b )
        {
            return GetValueToCompare( a ).CompareTo( GetValueToCompare( b ) );
        }

        public float GetValueToCompare( Pawn pawn )
        {
            return pawn.needs.mood.CurLevelPercentage - pawn.mindState.mentalBreaker.BreakThresholdMinor;
        }

        protected override Vector2 GetIconSize( Pawn pawn ) { return def.HeaderIconSize; }

        protected override Texture2D GetIconFor( Pawn pawn )
        {
            // broken
            if ( pawn.mindState.mentalStateHandler?.CurStateDef != null )
                return Resources.MoodBroken;

            // current level
            var mood = pawn.needs.mood.CurLevelPercentage;
            var softBreak = pawn.mindState.mentalBreaker.BreakThresholdMinor;

            // icon
            if (mood < softBreak)
                return Resources.MoodUnhappy;
            if (mood < .5)
                return Resources.MoodDiscontent;
            if (mood < .9)
                return Resources.MoodContent;
            return Resources.MoodHappy;
        }

        protected override Color GetIconColor( Pawn pawn )
        {
            // broken
            if ( pawn.mindState.mentalStateHandler?.CurStateDef != null )
            {
                switch ( pawn.mindState.mentalStateHandler.CurStateDef.category )
                {
                    case MentalStateCategory.Aggro:
                        return Color.red;
                    case MentalStateCategory.Sad:
                        return Color.cyan;
                    case MentalStateCategory.Panic:
                        return new Color( .4f, .008f, .235f );
                    case MentalStateCategory.Misc:
                    case MentalStateCategory.Indulgent:
                    case MentalStateCategory.Undefined:
                        return new Color( 207 / 256f, 83 / 256f, 0f );
                    default:
                        return Color.white;
                }
            }

            // current level
            var mood = pawn.needs.mood.CurLevelPercentage;
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

        protected override string GetIconTip( Pawn pawn ) { return pawn.needs.mood.GetTipString(); }
    }
}