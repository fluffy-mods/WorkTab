// PawnColumnWorker_Favourite.cs
// Copyright Karel Kroeze, 2018-2020

using FluffyUI;
using RimWorld;
using UnityEngine;
using Verse;
using Widgets = Verse.Widgets;

namespace WorkTab {
    public class PawnColumnWorker_Favourite: PawnColumnWorker_Icon {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table) {
            Favourite favourite = FavouriteManager.Get[pawn];
            Vector2 size      = GetIconSize( pawn );
            Rect iconRect  = new Rect( 0f, 0f, size.x, size.y ).CenteredIn( rect );
            Texture2D icon      = favourite?.Icon ?? ( Mouse.IsOver( iconRect ) ? Resources.Star : Resources.StarHollow );

            if (Widgets.ButtonImage(iconRect, icon)) {
                ClickedIcon(pawn);
            }

            TooltipHandler.TipRegion(rect, GetIconTip(pawn));
        }

        protected override void ClickedIcon(Pawn pawn) {
            FavouriteManager.FavouriteFloatMenuFor(pawn);
        }

        protected override Texture2D GetIconFor(Pawn pawn) {
            return FavouriteManager.Get[pawn]?.Icon;
        }

        protected override Vector2 GetIconSize(Pawn pawn) {
            return def.HeaderIconSize;
        }
    }
}
