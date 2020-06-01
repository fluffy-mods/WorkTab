// Dialog_Favourite.cs
// Copyright Karel Kroeze, 2018-2020

using FluffyUI;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public abstract class Dialog_Favourite : Window
    {
        protected const float BUTTON_HEIGHT = 35f;
        protected const float FIELD_HEIGHT  = 30f;

        protected const float HEADER_HEIGHT = 42f;

        protected Vector2 _size =
            new Vector2( 350f, HEADER_HEIGHT + FIELD_HEIGHT + BUTTON_HEIGHT + StandardMargin * 2 );

        protected Favourite      favourite;
        protected string         label;
        protected TextureChooser textureChooser;

        public Dialog_Favourite( Favourite favourite )
        {
            this.favourite = favourite;
            label          = favourite.Label ?? "Fluffy.WorkTab.DefaultFavouriteLabel".Translate();
            textureChooser = new TextureChooser( Resources.Icons );
        }

        public override Vector2 InitialSize => _size;
    }
}