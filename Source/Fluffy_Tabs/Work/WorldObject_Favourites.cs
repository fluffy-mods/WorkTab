using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HugsLib.Utils;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Fluffy_Tabs
{
    public class WorldObject_Favourites : UtilityWorldObject
    {
        #region Fields

        private static List<WorkFavourite> _favourites = new List<WorkFavourite>();
        private static WorldObject_Favourites _instance;

        #endregion Fields

        #region Properties

        public static WorldObject_Favourites Get
        {
            get
            {
                // if cache is null, get instance from HugsLib
                if ( _instance == null )
                    // TODO: Get instance from HugsLib
                    _instance = UtilityWorldObjectManager.GetUtilityWorldObject<WorldObject_Favourites>();

                return _instance;
            }
        }

        public List<WorkFavourite> Favourites => _favourites;

        #endregion Properties

        #region Methods

        public void Add( WorkFavourite favourite )
        {
            _favourites.Add( favourite );
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.LookList( ref _favourites, "favourites", LookMode.Deep );
        }

        #endregion Methods
        
        public void Remove( WorkFavourite favourite )
        {
            // remove from list
            _favourites.Remove( favourite );
            
            // remove from assigned pawns
            WorldObject_Priorities.Instance.Notify_FavouriteDeleted( favourite );
        }
    }
}