// FavouriteManager.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluffyUI;
using FluffyUI.FloatMenu;
using UnityEngine;
using Verse;
using Widgets = Verse.Widgets;

namespace WorkTab
{
    public class FavouriteManager : GameComponent
    {
        public static  List<Favourite>             Favourites  = new List<Favourite>();
        private static Dictionary<Pawn, Favourite> _favourites = new Dictionary<Pawn, Favourite>();

        public FavouriteManager()
        {
            Get = this;
        }

        public FavouriteManager( Game game ) : this()
        {
        }

        public static FavouriteManager Get { get; private set; }

        private static string FavouriteBasePath
        {
            get
            {
                var path = Path.Combine( GenFilePaths.SaveDataFolderPath, "WorkTab_Favourites" );
                if ( !Directory.Exists( path ) ) Directory.CreateDirectory( path );
                return path;
            }
        }

        public Favourite this[ Pawn pawn ]
        {
            get
            {
                if ( _favourites.TryGetValue( pawn, out var favourite ) )
                    return favourite;
                return null;
            }
            set
            {
                if ( value == null )
                    _favourites.Remove( pawn );
                else
                    _favourites[pawn] = value;
            }
        }

        public static void Delete( Favourite favourite, bool remove = false )
        {
            if ( remove ) Remove( favourite );
            var path = FavouritePath( favourite );
            if ( File.Exists( path ) ) File.Delete( path );
        }


        public static void FavouriteFloatMenuFor( Pawn pawn )
        {
            // create new
            var options = new List<FloatMenuOption>();
            options.Add( new FloatMenuOption(
                             "Fluffy.WorkTab.CreateFavourite".Translate(),
                             () => Favourite.From( pawn, true ) ) );

            // load from disk
            options.Add( new FloatMenuOption(
                             "Fluffy.WorkTab.LoadFavourite".Translate(),
                             () => LoadFavouriteFloatMenu( pawn ) ) );

            // clear
            if ( Get[pawn] != null )
                options.Add( new FloatMenuOption(
                                 "Fluffy.WorkTab.ClearFavourite".Translate(),
                                 () => Get[pawn] = null ) );

            // set favourite in favourites
            foreach ( var favourite in Favourites )
                options.Add( new FloatMenuOption_Aligned(
                                 favourite.Label,
                                 () => Get[pawn] = favourite,
                                 extraPartWidth: 38,
                                 extraPartOnGUI: rect => ExtraPart( rect, favourite ) ) );
            Find.WindowStack.Add( new FloatMenu( options ) );
        }


        public static FailReason IsValidLabel( string label, string curLabel = null )
        {
            if ( label.NullOrEmpty() )
                return "Fluffy.WorkTab.FavouriteLabelCannotBeEmpty".Translate().Resolve();

            if ( !IsValidFileName( label ) )
                return "Fluffy.WorkTab.FavouriteInvalidFilename".Translate( label ).Resolve();

            if ( curLabel != label && File.Exists( FavouritePath( label ) ) )
                return "Fluffy.WorkTab.FavouriteAlreadyExists".Translate( label ).Resolve();

            return true;
        }

        public static void Remove( Favourite favourite )
        {
            Favourites.Remove( favourite );

            // remove from pawns
            var affected = _favourites
                          .Where( f => f.Value == favourite )
                          .Select( f => f.Key )
                          .ToArray();
            foreach ( var pawn in affected )
                _favourites.Remove( pawn );
        }

        public static void Save( Favourite favourite )
        {
            Scribe.saver.InitSaving( FavouritePath( favourite ), "Favourite" );
            favourite.ExposeData();
            Scribe.saver.FinalizeSaving();
        }

        private static bool ExtraPart( Rect rect, Favourite favourite )
        {
            var rects    = new Grid( rect, gutters: Vector2.zero ).Columns( 1, 1 );
            var size     = Mathf.Min( 16f, rects[0].Rect.width, rects[0].Rect.height );
            var iconRect = new Rect( 0f, 0f, size, size );
            GUI.DrawTexture( iconRect.CenteredIn( rects[0] ), favourite.Icon );
            if ( Widgets.ButtonImage( new Rect( 0f, 0f, size, size ).CenteredIn( rects[1] ), Resources.Edit ) )
                Find.WindowStack.Add( new Dialog_EditFavourite( favourite ) );

            return false;
        }

        private static string FavouritePath( Favourite favourite )
        {
            return FavouritePath( favourite.Label );
        }

        private static string FavouritePath( string label )
        {
            return Path.Combine( FavouriteBasePath, $"{label}.xml" );
        }

        private static bool IsValidFileName( string label )
        {
            return GenFile.SanitizedFileName( label ) == label;
        }

        private static void LoadFavourite( string path, Pawn pawn )
        {
            var favourite = new Favourite();
            Scribe.loader.InitLoading( path );
            favourite.ExposeData();
            Scribe.loader.crossRefs.RegisterForCrossRefResolve( favourite );
            Scribe.loader.initer.RegisterForPostLoadInit( favourite );
            Scribe.loader.FinalizeLoading();

            favourite.SetLoadId();
            Favourites.Add( favourite );
            if ( pawn != null )
                Get[pawn] = favourite;
        }

        private static void LoadFavouriteFloatMenu( Pawn pawn )
        {
            var options = Directory.GetFiles( FavouriteBasePath )
                                   .Select( f => new {path = f, label = Path.GetFileNameWithoutExtension( f )} )
                                   .Where( f => !Favourites.Any( F => F.Label == f.label ) )
                                   .Select( f => new FloatMenuOption( f.label,
                                                                      () => LoadFavourite( f.path, pawn ) ) )
                                   .ToList();
            if (options.Count() > 0)
            {
                Find.WindowStack.Add(new FloatMenu(options));
            }
            else
            {
                var noneLabel = new List<FloatMenuOption>();
                noneLabel.Add(new FloatMenuOption("No new favourites", null));
                Find.WindowStack.Add(new FloatMenu(noneLabel));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look( ref Favourites, "Favourites", LookMode.Deep );
            Scribe_Collections.Look( ref _favourites, "FavouriteAssignments", LookMode.Reference, LookMode.Reference );
        }
    }
}