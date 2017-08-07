using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace WorkTab
{
    public static class Int_Extensions
    {
        public static string FormatHour( this int hour )
        {
            // 24-hour is simple
            if ( Settings.TwentyFourHourMode )
                return hour.ToString( "D2" ) + ":00";

            // noon/midnight are special
            int noon = GenDate.HoursPerDay / 2;
            if ( hour == 0 )
                return "midnight".Translate();
            if ( hour == noon )
                return "noon".Translate();

            // am/pm
            return hour % noon + ( hour > noon ? " p.m." : " a.m." );
        }
    }
}
