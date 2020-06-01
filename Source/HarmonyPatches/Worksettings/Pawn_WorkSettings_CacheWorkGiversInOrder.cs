// Pawn_WorkSettings_CacheWorkGiversInOrder.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using static HarmonyLib.AccessTools;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings ), "CacheWorkGiversInOrder" )]
    public class Pawn_WorkSettings_CacheWorkGiversInOrder
    {
        public static FieldInfo pawnField = typeof( Pawn_WorkSettings )
           .GetField( "pawn", all );

        public static FieldInfo workgiversDirtyField = typeof( Pawn_WorkSettings )
           .GetField( "workGiversDirty", all );

        public static FieldInfo workgiversEmergencyField = typeof( Pawn_WorkSettings )
           .GetField( "workGiversInOrderEmerg", all );

        public static FieldInfo workgiversNormalField = typeof( Pawn_WorkSettings )
           .GetField( "workGiversInOrderNormal", all );

        static Pawn_WorkSettings_CacheWorkGiversInOrder()
        {
            if ( pawnField == null )
                throw new NullReferenceException( "pawn field not found" );
            if ( workgiversDirtyField == null )
                throw new NullReferenceException( "workGiversDirty field not found" );
            if ( workgiversEmergencyField == null )
                throw new NullReferenceException( "workGiversInOrderEmerg field not found" );
            if ( workgiversNormalField == null )
                throw new NullReferenceException( "workGiversInOrderNormal field not found" );
        }

        public static bool Prefix( Pawn_WorkSettings __instance )
        {
            var pawn = pawnField.GetValue( __instance ) as Pawn;
            var allWorkgivers = DefDatabase<WorkGiverDef>.AllDefsListForReading
                                                         .Select( wgd => wgd.Worker )
                                                         .Where( wg => pawn.GetPriority( wg.def, -1 ) > 0 );
            var normalWorkgivers    = new List<WorkGiver>();
            var emergencyWorkgivers = new List<WorkGiver>();

            // sort by player set workgiver priorities => worktype order => workgiver order
            if ( allWorkgivers.Any() )
            {
                allWorkgivers = allWorkgivers
                               .OrderBy( wg => pawn.GetPriority( wg.def, -1 ) )
                               .ThenByDescending( wg => wg.def.workType.naturalPriority )
                               .ThenByDescending( wg => wg.def.priorityInType ).ToList();

                // lowest priority non-emergency job
                var maxEmergPrio = allWorkgivers
                                  .Where( wg => !wg.def.emergency )
                                  .Min( wg => pawn.GetPriority( wg.def, -1 ) );

                // create lists of workgivers
                normalWorkgivers = allWorkgivers
                                  .Where( wg => !wg.def.emergency || pawn.GetPriority( wg.def, -1 ) > maxEmergPrio )
                                  .ToList();
                emergencyWorkgivers = allWorkgivers
                                     .Where( wg => wg.def.emergency && pawn.GetPriority( wg.def, -1 ) <= maxEmergPrio )
                                     .ToList();
            }

            Logger.Debug( $"Updating priorities for {pawn.LabelShort};\n\t"                                      +
                          $"{string.Join( "\n\t", emergencyWorkgivers.Select( wg => wg.def.label ).ToArray() )}" +
                          $"{string.Join( "\n\t", normalWorkgivers.Select( wg => wg.def.label ).ToArray() )}" );

            // update cached lists of workgivers
            workgiversNormalField.SetValue( __instance, normalWorkgivers );
            workgiversEmergencyField.SetValue( __instance, emergencyWorkgivers );
            workgiversDirtyField.SetValue( __instance, false );

            // stop vanilla execution
            return false;
        }
    }
}