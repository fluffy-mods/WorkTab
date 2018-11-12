// Dialog_Favourite.cs
// Copyright Karel Kroeze, 2018-2018

using FluffyUI;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public abstract class Dialog_Favourite: Window
    {
        protected Vector2 _size = new Vector2(350f, HEADER_HEIGHT + FIELD_HEIGHT + BUTTON_HEIGHT + StandardMargin * 2);
        protected string label;
        protected Favourite favourite;
        protected TextureChooser textureChooser;

        protected const float HEADER_HEIGHT = 42f;
        protected const float FIELD_HEIGHT = 30f;
        protected const float BUTTON_HEIGHT = 35f;

        public Dialog_Favourite( Favourite favourite )
        {
            this.favourite = favourite;
            label = favourite.Label ?? "Fluffy.WorkTab.DefaultFavouriteLabel".Translate();
            textureChooser = new TextureChooser(Resources.Icons);
        }

        public override Vector2 InitialSize => _size;
    }
}