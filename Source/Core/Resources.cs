// Karel Kroeze
// Resources.cs
// 2017-05-28

using System.Linq;
using UnityEngine;
using Verse;

namespace WorkTab
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static readonly Texture2D
            SortingIcon,
            SortingDescendingIcon,
            MoodHappy,
            MoodContent,
            MoodDiscontent,
            MoodUnhappy,
            MoodBroken,
            PrioritiesDetailed,
            PrioritiesSimple,
            PrioritiesTimed,
            PrioritiesWholeDay,
            PinEye,
            PinClock,
            Clock,
            Half,
            Now,
            Expand,
            Collapse,
            LeftArrow,
            RightArrow,
            StarHollow,
            Star,
            Edit;

        public static readonly Texture2D[] Icons;

        static Resources()
        {
            SortingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
            SortingDescendingIcon = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
            MoodHappy = ContentFinder<Texture2D>.Get( "UI/Icons/Mood/happy" );
            MoodContent = ContentFinder<Texture2D>.Get( "UI/Icons/Mood/content" );
            MoodDiscontent = ContentFinder<Texture2D>.Get( "UI/Icons/Mood/discontent" );
            MoodUnhappy = ContentFinder<Texture2D>.Get( "UI/Icons/Mood/unhappy" );
            MoodBroken = ContentFinder<Texture2D>.Get( "UI/Icons/Mood/broken" );
            PrioritiesDetailed = ContentFinder<Texture2D>.Get( "UI/Icons/numbers" );
            PrioritiesSimple = ContentFinder<Texture2D>.Get( "UI/Icons/checks" );
            PrioritiesTimed = ContentFinder<Texture2D>.Get( "UI/Icons/clock-scheduler" );
            PrioritiesWholeDay = ContentFinder<Texture2D>.Get( "UI/Icons/whole-day" );
            PinEye = ContentFinder<Texture2D>.Get( "UI/Icons/pin-eye" );
            PinClock = ContentFinder<Texture2D>.Get( "UI/Icons/pin-clock" );
            Clock = ContentFinder<Texture2D>.Get("UI/Icons/clock");
            Half = ContentFinder<Texture2D>.Get("UI/Icons/half");
            Now = ContentFinder<Texture2D>.Get("UI/Icons/now");
            Expand = ContentFinder<Texture2D>.Get("UI/Icons/expand");
            Collapse = ContentFinder<Texture2D>.Get("UI/Icons/collapse");
            LeftArrow = ContentFinder<Texture2D>.Get( "UI/Icons/LeftArrow" );
            RightArrow = ContentFinder<Texture2D>.Get( "UI/Icons/RightArrow" );
            StarHollow = ContentFinder<Texture2D>.Get( "UI/Icons/star-hollow" );
            Star = ContentFinder<Texture2D>.Get( "UI/Icons/star" );
            Edit = ContentFinder<Texture2D>.Get( "UI/Icons/edit" );
            Icons = ContentFinder<Texture2D>.GetAllInFolder( "UI/Icons/Various" ).ToArray();
        }
    }
}