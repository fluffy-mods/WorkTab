// Karel Kroeze
// Resources.cs
// 2017-05-28

using UnityEngine;
using Verse;

namespace WorkTab
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static readonly Texture2D SortingIcon,
                                         SortingDescendingIcon;

        static Resources()
        {
            SortingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
            SortingDescendingIcon = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        }
    }
}