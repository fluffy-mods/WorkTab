// Karel Kroeze
// Widgets_BeginScrollView.cs
// 2017-06-05

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Widgets ), "BeginScrollView") ]
    public class Widgets_BeginScrollView
    {
        static bool Prefix (Rect outRect, ref Vector2 scrollPosition, Rect viewRect, bool showScrollbars )
        {
            if (showScrollbars)
                scrollPosition = GUI.BeginScrollView(outRect, scrollPosition, viewRect);
            else
                scrollPosition = GUI.BeginScrollView(outRect, scrollPosition, viewRect, GUIStyle.none, GUIStyle.none);
            
            return false;
        }
    }
}