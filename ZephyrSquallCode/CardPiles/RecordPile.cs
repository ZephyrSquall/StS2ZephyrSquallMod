using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.CardPiles;

// This pile is purely a holding space for cards that have been Recorded, so they can be kept out of combat until the
// Book that returns them is played.
public class RecordPile() : CustomPile(Record)
{
    public static Book? TargetBook { get; set; }

    [CustomEnum]
    public static PileType Record;
    
    public override bool CardShouldBeVisible(CardModel card)
    {
        return true;
    }

    // Move the target position to where the generated Book is so cards visually move into the Book.
    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        if (TargetBook == null)
        {
            Log.Error("NO TARGET BOOK ASSIGNED");
            return Vector2.Zero;
        }
        NCard? nCard = NCard.FindOnTable(TargetBook);
        if (nCard == null)
        {
            Log.Error("COULD NOT FIND TARGET BOOK NCARD");
            return Vector2.Zero;
        }
        return nCard.GlobalPosition;
    }
    
    public override NCard? GetNCard(CardModel card)
    {
        return NCard.Create(card);
    }
}