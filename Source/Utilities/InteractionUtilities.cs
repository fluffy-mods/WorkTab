// Karel Kroeze
// InteractionUtilities.cs
// 2017-05-25

using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public enum ScrollDirection
    {
        Up,
        Down
    }
    public static class InteractionUtilities
    {
        public static bool Clicked( Rect rect, int button = 0 )
        {
            return Event.current.type == EventType.MouseDown && Event.current.button == button && Mouse.IsOver( rect );
        }

        public static bool LeftClicked( Rect rect )
        {
            return Clicked( rect, 0 );
        }

        public static bool RightClicked( Rect rect )
        {
            return Clicked( rect, 1 );
        }

        public static bool Scrolled( Rect rect, ScrollDirection direction, bool stopPropagation )
        {
            bool scrolled = Event.current.type == EventType.ScrollWheel &&
                   ( ( Event.current.delta.y > 0 && direction == ScrollDirection.Up ) ||
                     ( Event.current.delta.y < 0 && direction == ScrollDirection.Down ) ) &&
                   Mouse.IsOver( rect );

            if (scrolled && stopPropagation)
                Event.current.Use();

            return scrolled;
        }

        public static bool ScrolledUp( Rect rect, bool stopPropagation = false ) { return Scrolled( rect, ScrollDirection.Up, stopPropagation ); }
        public static bool ScrolledDown( Rect rect, bool stopPropagation = false ) { return Scrolled( rect, ScrollDirection.Down, stopPropagation ); }

        public static void ButtonImageToggle( Func<bool> getter, Action<bool> setter, Rect canvas,
                                              string tipOn, Texture2D texOn,
                                              string tipOff, Texture2D texOff )
        {
            if ( setter == null || getter == null )
                throw new NullReferenceException( "getter and setter must not be null" );

            TooltipHandler.TipRegion( canvas, getter() ? tipOff : tipOn );
            if ( Widgets.ButtonImage( canvas, getter() ? texOff : texOn, Color.white, GenUI.MouseoverColor ) )
                setter( !getter() );
        }
    }
}