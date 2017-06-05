// Karel Kroeze
// Widgets_BeginScrollView.cs
// 2017-06-05

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Widgets), "BeginScrollView" )]
    public class Widgets_BeginScrollView
    {
        static IEnumerable<CodeInstruction> Transpiler( IEnumerable<CodeInstruction> instructions )
        {
            var Event_Use = AccessTools.Method( typeof(Event), "Use" );
            var all = instructions.ToList();

            foreach ( CodeInstruction instruction in all )
            {
                if ( instruction.opcode == OpCodes.Callvirt && instruction.operand == Event_Use )
                    yield return new CodeInstruction( OpCodes.Pop );
                else yield return instruction;
            }
        }
    }
}