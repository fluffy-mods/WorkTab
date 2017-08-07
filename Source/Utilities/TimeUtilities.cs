using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace WorkTab
{
    public static class TimeUtilities
    {
        private static List<int> _day;

        public static List<int> WholeDay
        {
            get
            {
                if (_day.NullOrEmpty())
                {
                    _day = new List<int>();
                    for (int hour = 0; hour < GenDate.HoursPerDay; hour++)
                        _day.Add(hour);
                }
                return new List<int>( _day );
            }
        }
    }
}
