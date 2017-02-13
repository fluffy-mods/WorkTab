using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Tabs
{
    public class WorkFavourite : IExposable, ILoadReferenceable
    {
        #region Fields

        public static int count = 0;
        public int ID;
        public string label;
        public PawnPrioritiesTracker workgiverPriorities = new PawnPrioritiesTracker();
        private Texture2D _icon;
        private string _iconpath;

        #endregion Fields

        #region Constructors

        // scribe
        public WorkFavourite()
        {
            ID = count++;
        }

        public WorkFavourite( Pawn pawn, string label, string iconpath ) : this()
        {
            this.label = label;
            this._icon = ContentFinder<Texture2D>.Get( iconpath );
            this._iconpath = iconpath;

            // create new tracker (copies from vanilla, so doesn't include workgiver level priorities).
            workgiverPriorities = new PawnPrioritiesTracker( pawn );

            // populate all workgiver priorities
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ )
                foreach ( var workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
                    workgiverPriorities.SetPriority( workgiver, pawn.Priorities().GetPriority( workgiver, hour ), hour );
        }

        #endregion Constructors

        #region Properties

        public Texture2D Icon
        {
            get
            {
                if ( _icon == null )
                    _icon = ContentFinder<Texture2D>.Get( _iconpath );
                return _icon;
            }
        }

        #endregion Properties

        #region Methods

        public void ExposeData()
        {
            Scribe_Values.LookValue( ref label, "label" );
            Scribe_Values.LookValue( ref ID, "ID" );
            Scribe_Values.LookValue( ref _iconpath, "iconpath" );
            Scribe_Deep.LookDeep( ref workgiverPriorities, "workgiverPriorities" );
        }

        public string GetUniqueLoadID()
        {
            return "Favourite_" + label + "_" + ID;
        }

        #endregion Methods
    }
}
