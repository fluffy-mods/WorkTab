using HugsLib;
using HugsLib.Utils;
using Verse;

namespace Fluffy_Tabs
{
    public class Controller : ModBase
    {
        public override string ModIdentifier => "WorkTab";
        private static ModLogger _logger;

        public Controller() : base() { _logger = base.Logger; }

        #region Overrides of ModBase

        public override void DefsLoaded()
        {
            base.DefsLoaded();
            WorldObject_Priorities.OnDefsLoaded();
        }

        #endregion

        public new static ModLogger Logger => _logger;

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
