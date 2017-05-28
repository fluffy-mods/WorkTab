using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;

namespace WorkTab
{
    public static class TimeUtilities
    {
        public static List<int> FullDay
        {
            get
            {
                List<int> day = new List<int>();
                for (int hour = 0; hour < GenDate.HoursPerDay; hour++)
                    day.Add(hour);
                return day;
            }
        }
    }
}
