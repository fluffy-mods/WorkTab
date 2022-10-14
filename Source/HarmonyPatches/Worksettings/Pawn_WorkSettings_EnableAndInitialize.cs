// Pawn_WorkSettings_EnableAndInitialize.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace WorkTab {
    [HarmonyPatch(typeof(Pawn_WorkSettings), nameof(Pawn_WorkSettings.EnableAndInitialize))]
    public class Pawn_WorkSettings_EnableAndInitialize {
        private static int GetDefaultPriority() {
            return Settings.defaultPriority;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> _instr) {
            List<CodeInstruction> instructions = _instr.ToList();
            System.Reflection.MethodInfo setPriorityMI =
                AccessTools.Method( typeof( Pawn_WorkSettings ), nameof( Pawn_WorkSettings.SetPriority ) );
            System.Reflection.MethodInfo getDefaultPriorityMI =
                AccessTools.Method( typeof( Pawn_WorkSettings_EnableAndInitialize ), nameof( GetDefaultPriority ) );

            for (int i = 0; i < instructions.Count; i++) {
                if (instructions[i].opcode == OpCodes.Ldc_I4_3) {
                    if (instructions[i + 1].Calls(setPriorityMI)) {
                        yield return new CodeInstruction(OpCodes.Call, getDefaultPriorityMI);
                        continue;
                    }
                }

                yield return instructions[i];
            }
        }
    }
}
