// VanillaWorkSettings.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WorkTab
{
    public static class VanillaWorkSettings
    {
        private static FieldInfo pawnFieldInfo;
        private static FieldInfo prioritiesFieldInfo;

        public static int GetVanillaPriority( this Pawn pawn, WorkTypeDef worktype )
        {
            //public override float GetPriority(Pawn pawn)
            if ( pawn.workSettings == null || !pawn.workSettings.EverWork ) return 0;

            if ( prioritiesFieldInfo == null )
            {
                prioritiesFieldInfo = typeof( Pawn_WorkSettings ).GetField( "priorities", AccessTools.all );
                if ( prioritiesFieldInfo == null )
                    throw new NullReferenceException( "priorities field not found" );
            }

            int priority;
            try
            {
                priority = ( prioritiesFieldInfo.GetValue( pawn.workSettings ) as DefMap<WorkTypeDef, int> )[worktype];
            }
            catch ( ArgumentOutOfRangeException )
            {
                priority = 0;
                Logger.Message(
                    $"Priority requested for a workgiver that did not yet exist for {pawn.Name.ToStringShort}. Did you add mods in an existing game?" );
            }
            catch ( TargetException )
            {
                priority = 0;
                Logger.Message(
                    $"Priority requested for a pawn that did not have worksettings ({pawn.Name.ToStringShort})" );
            }

            return priority;
        }

        public static Pawn Pawn( this Pawn_WorkSettings worksettings )
        {
            if ( pawnFieldInfo == null )
            {
                pawnFieldInfo = typeof( Pawn_WorkSettings ).GetField( "pawn", AccessTools.all );
                if ( pawnFieldInfo == null )
                    throw new NullReferenceException( "could not get pawn field" );
            }

            return pawnFieldInfo.GetValue( worksettings ) as Pawn;
        }
    }
}