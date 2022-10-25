// VanillaWorkSettings.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace WorkTab {
    public static class VanillaWorkSettings {
        private static FieldInfo pawnFieldInfo;

        public static int GetVanillaPriority(this Pawn pawn, WorkTypeDef worktype) {
            //public override float GetPriority(Pawn pawn)
            if (worktype == null || pawn.workSettings == null || !pawn.workSettings.EverWork) {
                return 0;
            }

            DefMap<WorkTypeDef, int> priorities = Traverse.Create(pawn.workSettings).Field<DefMap<WorkTypeDef, int>>("priorities").Value;
            if (priorities == null) {
                throw new Exception("priorities field not found, or value is null");
            }

            int priority;
            try {
                priority = priorities[worktype];
            } catch (ArgumentOutOfRangeException) {
                priority = 0;
                Logger.Message(
                    $"Priority requested for a workgiver that did not yet exist for {pawn.Name.ToStringShort}. Did you add mods in an existing game?");
            } catch (TargetException) {
                priority = 0;
                Logger.Message(
                    $"Priority requested for a pawn that did not have worksettings ({pawn.Name.ToStringShort})");
            }

            return priority;
        }

        public static Pawn Pawn(this Pawn_WorkSettings worksettings) {
            if (pawnFieldInfo == null) {
                pawnFieldInfo = typeof(Pawn_WorkSettings).GetField("pawn", AccessTools.all);
                if (pawnFieldInfo == null) {
                    throw new NullReferenceException("could not get pawn field");
                }
            }

            return pawnFieldInfo.GetValue(worksettings) as Pawn;
        }
    }
}
