// Karel Kroeze
// Constants.cs
// 2017-05-23

using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace WorkTab
{
    public static class Constants
    {
        public const int WorkTypeWidth = 32;
        public const int WorkTypeBoxSize = 25;
        public const int WorkGiverWidth = 25;
        public const int WorkGiverBoxSize = 20;
        public const int Margin = 4;
        public const int HeaderHeight = 50;
        public static Dictionary<string,string> TruncationCache = new Dictionary<string, string>();
        public const int TimeBarHeight = 30;
        public const int ExtraTopSpace = 40;
        public static readonly Vector2 PriorityLabelSize = new Vector2( 160, 30 );
    }
}