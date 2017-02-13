using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HugsLib.Utils;
using UnityEngine;
using Verse;
using static Fluffy_Tabs.Controller;

namespace Fluffy_Tabs
{
    public class PawnPrioritiesTracker : IExposable
    {
        #region Fields

        public WorkFavourite currentFavourite;
        public Pawn pawn;
        private DefMap<WorkGiverDef, bool> _cacheDirty = new DefMap<WorkGiverDef, bool>();
        private DefMap<WorkGiverDef, bool> _timeDependentCache = new DefMap<WorkGiverDef, bool>();
        private Dictionary<WorkGiverDef, string> _timeDependentTipCache = new Dictionary<WorkGiverDef, string>();
        private List<Dictionary<WorkGiverDef, int>> priorities = new List<Dictionary<WorkGiverDef, int>>();
        private bool newFormat = true;

        private string _prioritiesScribeHelper;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// For scribe only!
        /// </summary>
        public PawnPrioritiesTracker()
        {
            InitPriorityCache();
        }

        public PawnPrioritiesTracker( Pawn pawn )
        {
            this.pawn = pawn;

            InitPriorityCache();
        }

        private void InitPriorityCache()
        {
            // create fresh list (just to be sure).
            priorities = new List<Dictionary<WorkGiverDef, int>>();

            // initialize from vanilla priorities, if available.
            // ( might no longer be available for priorities, in which case fall back to an empty defmap ).
            DefMap<WorkTypeDef, int> vanillaPriorities;
            if ( pawn != null )
                vanillaPriorities = Detours_WorkSettings.GetVanillaPriorities(pawn);
            else 
                vanillaPriorities = new DefMap<WorkTypeDef, int>();

            // loop over hours
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
            {
                // create map for this hour
                priorities.Add( new Dictionary<WorkGiverDef, int>() );
                
                // loop over worktypes
                foreach ( WorkTypeDef worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading )
                {
                    // add workgivers to dictionary, initialize at zero.
                    foreach ( WorkGiverDef workgiver in worktype.workGiversByPriority )
                        priorities[hour].Add( workgiver, 0 );

                    // copy over vanilla priorities
                    int priority = vanillaPriorities[worktype];
                    SetPriority( worktype, priority, hour );
                }
            }
        }

        #endregion Constructors

        #region Methods

        public void AssignFavourite( WorkFavourite favourite )
        {
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
                foreach ( var workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
                    if ( !pawn.story.WorkTypeIsDisabled( workgiver.workType ) )
                        SetPriority( workgiver, favourite.workgiverPriorities.priorities[hour][workgiver], hour );
            currentFavourite = favourite;
        }

        public void ExposeData()
        {
            Scribe_References.LookReference( ref pawn, "pawn" );
            Scribe_References.LookReference( ref currentFavourite, "currentFavourite" );

            // handle priorities, first try to see what style the old save was in.
            // note that while saving, newFormat is set to true. While loading,
            // it defaults to false. The value of newFormat should be immediately available.
            if ( Scribe.mode == LoadSaveMode.Saving )
                newFormat = true;
            Scribe_Values.LookValue( ref newFormat, "newFormat", false );
            //Controller.Logger.Message( "Loading priorities from {1} format save: {0}", Scribe.mode,
            //                           newFormat ? "NEW" : "OLD" );
            if ( newFormat )
            {
                switch ( Scribe.mode )
                {
                    case LoadSaveMode.Saving:
                        // this one is fairly straightforward, create string block that has one line per hour and just joins all the priorities
                        // note; this means a hard requirement for priorities to be single digits.
                        _prioritiesScribeHelper = GetSaveString();
                        Scribe_Values.LookValue(ref _prioritiesScribeHelper, "priorities", string.Empty, true);
                        break;
                    case LoadSaveMode.LoadingVars:
                        // just read out the string block, we'll process it in the PostLoadInit
                        Scribe_Values.LookValue(ref _prioritiesScribeHelper, "priorities", string.Empty, true);
                        break;
                    case LoadSaveMode.PostLoadInit:
                        // create priorities data, and fill with defaults
                        InitPriorityCache(); 

                        // fill in saved priorities
                        LoadPrioritiesFromString();
                        break;
                }
            }
            else
            {
                // fall back to old style
                Scribe_Collections.LookList(ref priorities, "priorities", LookMode.Deep);
            }

            // clear tip cache so it gets rebuild after load
            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
                foreach ( var workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
                    _cacheDirty[workgiver] = true;
        }

        private string GetSaveString()
        {
            var Workgivers = DefDatabase<WorkGiverDef>.AllDefsListForReading;
            string savestring = string.Empty;

            // for each hour
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
            {
                // for each workgiver
                foreach ( WorkGiverDef workgiver in Workgivers )
                {
                    savestring += GetPriority( workgiver, hour );
                }

                // next line
                savestring += "\n";
            }

            return savestring;
        }

        private void LoadPrioritiesFromString()
        {
            // fetch priorities from the string block
            List<List<int>> _priorities = _prioritiesScribeHelper
                // first off, split the lines to get a string per hour
                .Split( "\n".ToCharArray() )
                // split lines into individual priorities
                .Select( IntsFromString ).ToList();

            // fill priority tracker
            foreach ( WorkGiverDef workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
            {
                int savedWorkgiverIndex = WorldObject_Priorities.GetSavedWorkgiverIndex( workgiver );
                if ( savedWorkgiverIndex >= 0 )
                {
                    try
                    {
                        // we made an error in judgment when storing favourites where 
                        // we assumed they would be valid and include all workgivers. 
                        // This is however not necessarily the case, so now we have
                        // incomplete favourites stored in the new format. Yuck.
                        for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
                            priorities[hour][workgiver] = _priorities[hour][savedWorkgiverIndex];
                    }
                    catch ( ArgumentOutOfRangeException )
                    {
                        // yep, here we go!
                        Log.Warning( $"Corrupted work priorities for {pawn.NameStringShort}. " + 
                            "This is likely actually caused by a favourite created from the pawn before v0.16.2.1. " + 
                            "You may want to verify your priorities and favourites are still correct." );
                    }
                }
            }
        }

        private List<int> IntsFromString( string line )
        {
            return line.ToCharArray() // break into individual characters
                       .Select( c => int.Parse( c.ToString() ) ) // parse as ints
                       .ToList(); 
        }

        public int GetPriority( WorkGiverDef workgiver )
        {
            return GetPriority( workgiver, GenLocalDate.HourOfDay( pawn.Map ) );
        }

        public int GetPriority( WorkGiverDef workgiver, int hour )
        {
            try
            {
                var priority = Find.PlaySettings.useWorkPriorities ? priorities[hour][workgiver] : priorities[hour][workgiver] > 0 ? 1 : 0;
                if ( !pawn.CapableOf( workgiver ) && priority > 0 )
                {
                    // log it
                    Log.Error( $"Found priority {priority} for a pawn incapable of {workgiver.defName}. Did you add/change mods without starting a new game?" );

                    // force priority back to 0.
                    priority = 0;
                    SetPriority( workgiver, priority, hour );
                }
                return priority;
            }
            catch ( ArgumentOutOfRangeException )
            {
                // workgiver-priority defmap is really just an ordered list indexed by a dynamically generated workgiver index int
                // if the number of workgivers increases, this means errors.
                Log.Error( "WorkGiver database corrupted, resetting priorities for " + pawn.NameStringShort + ". Did you add mods during the game?" );
                priorities = new List<Dictionary<WorkGiverDef, int>>();
                InitPriorityCache();

                throw;
            }
        }

        public int GetPriority( WorkTypeDef worktype, int hour = -1 )
        {
            // get current hour if left default
            // check if pawn has a registered Map we can use for the local time (99% of cases).
            if ( hour < 0 && pawn?.Map != null )
                hour = GenLocalDate.HourOfDay( pawn.Map );
            // if not, try use the visible map
            if ( hour < 0 && Find.VisibleMap != null )
                hour = GenLocalDate.HourOfDay( Find.VisibleMap );
            // if that still didnt work (before game init), fall back to 0.
            if ( hour < 0 )
                hour = 0;

            // get workgiver priorities
            var _priorities = worktype.workGiversByPriority.Select( wg => GetPriority( wg, hour ) );

            // if any priority is not 0, select highest (lowest) priority
            if ( _priorities.Any() && _priorities.Any( prio => prio != 0 ) )
                return _priorities.Where( prio => prio > 0 ).Min();
            else
                return 0;
        }

        /// <summary>
        /// returns true if pawn is scheduled for this workgiver, but only if not always active.
        /// </summary>
        /// <param name="workgiver"></param>
        /// <returns></returns>
        public bool IsTimeDependent( WorkGiverDef workgiver )
        {
            if ( _cacheDirty[workgiver] )
            {
                // initialize at false
                _timeDependentCache[workgiver] = false;

                // if there is more than one unique priority, this workgiver is partially scheduled
                int priority = GetPriority( workgiver, 0 );

                for ( int hour = 1; hour < GenDate.HoursPerDay; hour++ )
                {
                    // get prio
                    int curPriority = GetPriority( workgiver, hour );

                    if ( priority != curPriority )
                    {
                        _timeDependentCache[workgiver] = true;
                        break;
                    }
                }

                // mark clean
                _cacheDirty[workgiver] = false;

                // update tip
                CreatePartiallyScheduledTip( workgiver );
            }

            return _timeDependentCache[workgiver];
        }

        public bool IsTimeDependent( WorkTypeDef worktype )
        {
            return worktype.workGiversByPriority.Any( wg => IsTimeDependent( wg ) );
        }

        public string TimeDependentTip( WorkGiverDef workgiver )
        {
            if ( !_timeDependentTipCache.ContainsKey( workgiver ) || _cacheDirty[workgiver] )
                CreatePartiallyScheduledTip( workgiver );

            return _timeDependentTipCache[workgiver];
        }

        public string TimeDependentTip( WorkTypeDef worktype )
        {
            return "FluffyTabs.XIsPartiallyScheduledForY".Translate( pawn.NameStringShort, worktype.label );
        }

        /// <summary>
        /// Set priority for workgiver and hour of day.
        /// 
        /// All the various overloads eventually call this method to set priorities on a workgiver level.
        /// </summary>
        /// <param name="workgiver"></param>
        /// <param name="priority"></param>
        /// <param name="hour"></param>
        public void SetPriority( WorkGiverDef workgiver, int priority, int hour )
        {
            // check if pawn is allowed to do this job
            if ( pawn != null && priority > 0 && !pawn.CapableOf( workgiver ) )
            {
                Log.Error( $"tried to enable work {workgiver.workType.defName} for {pawn.NameStringShort}, who is incapable of said work." );
                
                // force to 0
                priority = 0;
            }

            // mark our partially scheduled cache dirty if changed
            if ( priority != priorities[hour][workgiver] )
                _cacheDirty[workgiver] = true;

            // change priority
            priorities[hour][workgiver] = priority;

            // clear current favourite, if not currently scribing
            if ( Scribe.mode == LoadSaveMode.Inactive )
                currentFavourite = null;

            // notify pawn to recache it's work order
            pawn?.workSettings.Notify_UseWorkPrioritiesChanged();

            // notify pawn that he might have to stop current job
            if ( pawn != null && pawn.Spawned && priority == 0 && hour == GenLocalDate.HourOfDay( pawn.Map ) )
                pawn.mindState.Notify_WorkPriorityDisabled( workgiver.workType );
        }

        /// <summary>
        ///  Set priority for all workgivers in worktype for hour of day.
        /// </summary>
        /// <param name="worktype"></param>
        /// <param name="priority"></param>
        /// <param name="hour"></param>
        public void SetPriority( WorkTypeDef worktype, int priority, int hour )
        {
            foreach ( var workgiver in worktype.workGiversByPriority )
                SetPriority( workgiver, priority, hour );
        }

        /// <summary>
        /// Set priority for workgiver for all hours of day.
        /// </summary>
        /// <param name="workgiver"></param>
        /// <param name="priority"></param>
        public void SetPriority( WorkGiverDef workgiver, int priority )
        {
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
                SetPriority( workgiver, priority, hour );
        }
        
        /// <summary>
        /// Set priority of all workgivers in worktype for all hours of the day.
        /// </summary>
        /// <param name="worktype"></param>
        /// <param name="priority"></param>
        public void SetPriority( WorkTypeDef worktype, int priority )
        {
            // propagate to vanilla
            Detours_WorkSettings.SetVanillaPriority( pawn, worktype, priority );

            foreach ( var workgiver in worktype.workGiversByPriority )
                SetPriority( workgiver, priority );
        }

        /// <summary>
        /// Set priority of workgiver for multiple hours of day.
        /// </summary>
        /// <param name="workgiver"></param>
        /// <param name="priority"></param>
        /// <param name="hours"></param>
        public void SetPriority( WorkGiverDef workgiver, int priority, List<int> hours )
        {
            foreach ( int hour in hours )
                SetPriority( workgiver, priority, hour );
        }

        /// <summary>
        /// Set priority of all workgivers in worktype for multiple hours of day.
        /// </summary>
        /// <param name="worktype"></param>
        /// <param name="priority"></param>
        /// <param name="hours"></param>
        public void SetPriority( WorkTypeDef worktype, int priority, List<int> hours )
        {
            foreach ( var workgiver in worktype.workGiversByPriority )
                SetPriority( workgiver, priority, hours );
        }

        private void CreatePartiallyScheduledTip( WorkGiverDef workgiver )
        {
            string tip = "FluffyTabs.XIsScheduledForY".Translate( pawn.Name.ToStringShort, Settings.WorkgiverLabels[workgiver] );

            int start = -1;
            int priority = -1;

            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
            {
                int curpriority = GetPriority( workgiver, hour );

                // stop condition
                if ( curpriority != priority && start >= 0 )
                {
                    tip += start.FormatHour() + " - " + hour.FormatHour();
                    if ( Find.PlaySettings.useWorkPriorities )
                        tip += " (" + priority + ")";
                    tip += "\n";

                    // reset start & priority
                    start = -1;
                    priority = -1;
                }

                // start condition
                if ( curpriority > 0 && curpriority != priority && start < 0 )
                {
                    priority = curpriority;
                    start = hour;
                }
            }

            // final check for x till midnight
            if ( start > 0 )
            {
                tip += start.FormatHour() + " - " + 0.FormatHour();
                if ( Find.PlaySettings.useWorkPriorities )
                    tip += " (" + priority + ")";
                tip += "\n";
            }

            // add or replace tip cache
            if ( !_timeDependentTipCache.ContainsKey( workgiver ) )
                _timeDependentTipCache.Add( workgiver, tip );
            else
                _timeDependentTipCache[workgiver] = tip;
        }

        #endregion Methods
    }
}
