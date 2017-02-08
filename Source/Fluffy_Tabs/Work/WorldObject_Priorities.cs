using System;
using HugsLib.Utils;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using static Fluffy_Tabs.Controller;

namespace Fluffy_Tabs
{
    public class WorldObject_Priorities : UtilityWorldObject
    {
        #region Fields

        private static bool _24Hours = true;

        private static bool _dwarfTherapistMode = false;

        // making everything static should solve our weird load issue, although it is cheating a bit.
        private static WorldObject_Priorities _instance;

        private static bool _schedulerMode = false;
        private static Dictionary<Pawn, PawnPrioritiesTracker> _trackers = new Dictionary<Pawn, PawnPrioritiesTracker>();
        private static List<PawnPrioritiesTracker> _trackersScribeHelper;
        private static List<string> currentworkgiverDefs;
        private static List<string> savedworkgiverDefs;

        #endregion Fields

        #region Properties

        public static void OnDefsLoaded()
        {
            // initialize current list of workgiverDefs
            currentworkgiverDefs = DefDatabase<WorkGiverDef>.AllDefsListForReading
                .OrderBy( def => def.index )
                .Select( def => def.defName )
                .ToList();
        }

        public static int GetSavedWorkgiverIndex( WorkGiverDef workgiver )
        {
            if ( currentworkgiverDefs.NullOrEmpty() || savedworkgiverDefs.NullOrEmpty() )
                throw new Exception( "current/saved workgiver lists not initialized." );

            return savedworkgiverDefs.IndexOf( workgiver.defName );
        }

        public static WorldObject_Priorities Instance
        {
            get
            {
                if ( _instance == null )
                    _instance = UtilityWorldObjectManager.GetUtilityWorldObject<WorldObject_Priorities>();

                return _instance;
            }
        }

        public bool DwarfTherapistMode
        {
            get { return _dwarfTherapistMode; }
            set { _dwarfTherapistMode = value; }
        }

        /// <summary>
        /// Note that time assignment is _always_ used internally, it is really only relevant from a UI perspective.
        /// </summary>
        public bool SchedulerMode
        {
            get { return _schedulerMode; }
            set { _schedulerMode = value; }
        }

        public bool TwentyFourHourMode
        {
            get { return _24Hours; }
            set { _24Hours = value; }
        }

        #endregion Properties

        #region Methods

        public static void NotifyAll_PrioritiesChanged()
        {
            foreach ( var pawn in Find.VisibleMap.mapPawns.FreeColonistsSpawned )
                pawn.workSettings.Notify_UseWorkPrioritiesChanged();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            // scribe per-game settings
            Scribe_Values.LookValue( ref _dwarfTherapistMode, "DwarfTherapistMode", false );
            Scribe_Values.LookValue( ref _24Hours, "TwentyFourHourClock", true );
            Scribe_Values.LookValue( ref _schedulerMode, "SchedulerMode", false );

            // scribe the current list of workgivers if saving
            if ( Scribe.mode == LoadSaveMode.Saving )
            {
                Scribe_Collections.LookList( ref currentworkgiverDefs, "workgivers" );
            }
            // otherwise, load the list as the saved workgivers, current will have been assigned on defsloaded
            else
            {
                Scribe_Collections.LookList( ref savedworkgiverDefs, "workgivers" );
                // Logger.Message( "old: {0}, new {1}", savedworkgiverDefs?.Count.ToString() ?? "NULL", currentworkgiverDefs?.Count.ToString() ?? "NULL" );

                if ( Scribe.mode == LoadSaveMode.PostLoadInit )
                {
                    if ( savedworkgiverDefs == null )
                    {
                        Logger.Message( "Migrating to new save format..." );
                    }
                    else
                    {
                        Logger.Message( "Validating stored priorities..." );
                        foreach (string saved in savedworkgiverDefs)
                            if (!currentworkgiverDefs.Contains(saved))
                                Logger.Error("Workgiver {0} was removed from the game. Removing defs from an ongoing game is almost always a bad idea.", saved);

                        foreach (string current in currentworkgiverDefs)
                            if (!savedworkgiverDefs.Contains(current))
                                Logger.Warning("Workgiver {0} was added to the game. Adding defs to an ongoing game can lead to errors.", current);
                    }
                }
            }

            // scribe only the actual trackers, since pawns don't want to be saved in dicts
            if ( Scribe.mode == LoadSaveMode.Saving )
            {
                _trackersScribeHelper = _trackers
                    .Where( p => p.Key != null && p.Key.Spawned && !p.Key.Dead )
                    .Select( p => p.Value ).ToList();
            }

            // do the scribing
            Scribe_Collections.LookList( ref _trackersScribeHelper, "workgiverPriorities", LookMode.Deep );

            // reconstruct the full dict, drop null pawns (these were probably leftovers from killed or otherwise no longer available pawns).
            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
            {
                _trackers = _trackersScribeHelper.Where( t => t.pawn != null ).ToDictionary( k => k.pawn, v => v );
            }
        }

        public void Notify_FavouriteDeleted( WorkFavourite favourite )
        {
            foreach ( var tracker in _trackers )
                if ( tracker.Value.currentFavourite == favourite )
                    tracker.Value.currentFavourite = null;
        }

        public PawnPrioritiesTracker WorkgiverTracker( Pawn pawn )
        {
            if ( !_trackers.ContainsKey( pawn ) )
                _trackers.Add( pawn, new PawnPrioritiesTracker( pawn ) );
            return _trackers[pawn];
        }

        #endregion Methods
    }
}
