// Dialog_EditFavourite.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using System.Data;
using FluffyUI;
using RimWorld;
using UnityEngine;
using Verse;
using Widgets = FluffyUI.Widgets;

namespace WorkTab
{
    public class Dialog_EditFavourite: Dialog_Favourite
    {
        public Dialog_EditFavourite( Favourite favourite ) : base( favourite ){}

        public override void DoWindowContents(Rect canvas)
        {
            var rows = new Grid(canvas).Rows(HEADER_HEIGHT, FIELD_HEIGHT, BUTTON_HEIGHT);
            var titleRect = rows[0].Rect;
            var nameRect = rows[1].Column(9);
            var iconRect = rows[1].Column(3);
            var deleteRect = rows[2].Column( 3 );
            rows[2].Column(3); // skip 3
            var cancelRect = rows[2].Column(3);
            var okRect = rows[2].Column(3);

            // title and a little text
            Widgets.Label(titleRect, "Fluffy.WorkTab.EditFavouritesDialogTitle".Translate( favourite.Label ), font: GameFont.Medium);

            // favourites label
            var valid = FavouriteManager.IsValidLabel( label, favourite.Label );
            GUI.color = valid ? Color.white : Color.red;
            label = GUI.TextField( nameRect, label );
            GUI.color = Color.white;

            // icon tumbler
            textureChooser.DrawAt(iconRect);

            // ok/cancel button.
            GUI.color = new Color(1f, 0.3f, 0.35f);
            if ( Verse.Widgets.ButtonText( deleteRect, "Delete".Translate() ) )
                DeleteAndClose();
            GUI.color = Color.white;
            if (Verse.Widgets.ButtonText(cancelRect, "CancelButton".Translate()))
                Close();
            if (Verse.Widgets.ButtonText(okRect, "AcceptButton".Translate()))
                ApplyAndClose();
        }

        public override void OnAcceptKeyPressed()
        {
            ApplyAndClose();
        }

        public void ApplyAndClose()
        {
            var valid = FavouriteManager.IsValidLabel( label, favourite.Label );
            if ( !valid )
            {
                Messages.Message( valid.Reason, MessageTypeDefOf.RejectInput, false );
                return;
            }
            
            if ( label != favourite.Label )
                FavouriteManager.Delete( favourite );

            favourite.Icon = textureChooser.Choice;
            favourite.Label = label;
            FavouriteManager.Save( favourite );
            Close();
        }

        public void DeleteAndClose()
        {
            var options = new List<FloatMenuOption>();
            options.Add( new FloatMenuOption( "Fluffy.WorkTab.UnloadFavourite".Translate(),
                () => { FavouriteManager.Remove( favourite ); Close(); } ) );
            options.Add( new FloatMenuOption( "Fluffy.WorkTab.DeleteFavourite".Translate(),
                () => { FavouriteManager.Delete( favourite, true ); Close(); } ) );
            Find.WindowStack.Add( new FloatMenu( options ) );
        }
    }
}