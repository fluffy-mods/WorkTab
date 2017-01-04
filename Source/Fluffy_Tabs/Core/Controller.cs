using HugsLib;
using Verse;

namespace Fluffy_Tabs
{
    public class Controller : ModBase
    {
        public override string ModIdentifier => "WorkTab";

        public override void MapLoaded( Map map )
        {
            base.MapLoaded( map );

            // make sure each loaded map has our timekeeper component
            // this will inject the component into existing save games.
            if ( map.GetComponent<MapComponent_Timekeeper>() == null )
                map.components.Add( new MapComponent_Timekeeper( map ) );
        }
    }
}
