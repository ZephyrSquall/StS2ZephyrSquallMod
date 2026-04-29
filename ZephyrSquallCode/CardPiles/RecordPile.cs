using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace ZephyrSquall.ZephyrSquallCode.CardPiles;

// This pile is purely a holding space for cards that have been Recorded, so they can be kept out of combat until the
// Book that returns them is played.
public class RecordPile() : CustomPile(Record)
{
    public Vector2 BookPosition { get; set; } = Vector2.Zero;

    [CustomEnum]
    public static PileType Record;
    
    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }

    // Move the target position to where the generated Book is so cards visually move into the Book. This method
    // specifically only seems to help with telling cards where to go as they're being stored inside the Book, and gets
    // ignored when returning the cards from the Book.
    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return BookPosition;
    }

    public override NCard? GetNCard(CardModel card)
    {
        NCard nCard = NCard.Create(card);
        // Setting the global position here ensures that the card animation for getting placed back in the hand starts
        // from the Book's position. Without this line, the animation always shows the cards coming from the top-left
        // corner of the screen, presumably coming from a default position of 0, 0.
        nCard.SetGlobalPosition(BookPosition);
        return nCard;
    }
}