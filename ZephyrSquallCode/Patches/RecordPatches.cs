using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using ZephyrSquall.ZephyrSquallCode.CardPiles;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

public class RecordedCardPreviewTracker
{
    public static List<NCard> RecordedCardsInPreview = [];
}

[HarmonyPatch(typeof(NCardHolder), "CreateHoverTips")]
public class DisplayRecordedCardsPatch
{
    private const float XPadding = 8f;
    private const float YPadding = 8f;

    [HarmonyPrefix]
    static bool DisplayRecordedCards(NCardHolder __instance)
    {
        if (__instance.CardModel is Book { RecordedCards.Count: > 0 } book)
        {
            Vector2 previewCardSize = NCard.defaultSize * NCardHolder.smallScale;
            // "Card offset" is how much a card preview has to be offset to the side to fit an additional card.
            float cardOffsetSize = previewCardSize.X + XPadding;
            float shrinkScale = 1;
            
            // Add offset in the x direction if cards scroll off-screen to keep them on screen . Or alternatively,
            // shrink the previewCardSize if cards scroll off-screen in both directions.
            float xOffset = 0;
            float cardsPreviewWidth = book.RecordedCards.Count * cardOffsetSize;
            if (cardsPreviewWidth > NGame.Instance.GetViewportRect().Size.X)
            {
                shrinkScale = NGame.Instance.GetViewportRect().Size.X / cardsPreviewWidth;
                previewCardSize *= shrinkScale;
                cardOffsetSize *= shrinkScale;
                cardsPreviewWidth *= shrinkScale;
            }
            
            float previewsLeftEdgeX = __instance.GlobalPosition.X - cardsPreviewWidth / 2;
            float previewsRightEdgeX = __instance.GlobalPosition.X + cardsPreviewWidth / 2;
            if (previewsLeftEdgeX < 0)
            {
                xOffset = -previewsLeftEdgeX;
            }
            else if (previewsRightEdgeX > NGame.Instance.GetViewportRect().Size.X)
            {
                xOffset = NGame.Instance.GetViewportRect().Size.X - previewsRightEdgeX;
            }
            
            var yOffset = -((NCard.defaultSize.Y + previewCardSize.Y) / 2f + YPadding);
            // If the Book is too close to the top of the screen (only possible when viewed in a pile other than the
            // Hand), show the previewed cards beneath the Book instead.
            if (__instance.GlobalPosition.Y + yOffset < 180)
                yOffset *= -1;
            
            for (int i = 0; i < book.RecordedCards.Count; i++)
            {
                var recordedCard = book.RecordedCards[i];
                var nCard = NCard.FindOnTable(recordedCard);
        
                if (nCard is not null)
                {
                    __instance.AddChild(nCard);
                    nCard.UpdateVisuals(nCard.Model.Pile.Type, CardPreviewMode.Normal);
                    RecordedCardPreviewTracker.RecordedCardsInPreview.Add(nCard);

                    nCard.Scale = NCardHolder.smallScale * shrinkScale;
                    
                    float individualCardOffset = i + 0.5f - (book.RecordedCards.Count / 2f);
                    float individualXOffset = xOffset + individualCardOffset * cardOffsetSize;
                    nCard.Position = new Vector2(individualXOffset, yOffset);
                }
            }
        }
        
        return true;
    }
}

[HarmonyPatch(typeof(NCardHolder), "ClearHoverTips")]
public class HideRecordedCardsPatch
{
    [HarmonyPrefix]
    static bool HideRecordedCards()
    {
        foreach (var nCard in RecordedCardPreviewTracker.RecordedCardsInPreview)
            nCard.QueueFree();

        RecordedCardPreviewTracker.RecordedCardsInPreview.Clear();
        return true;
    }
}

// Sets up an event listener on each player's Record Pile to tell all of that player's Books to check their Recorded
// cards anytime a card is removed from the Record Pile.
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.AfterCombatRoomLoaded))]
class WatchRecordPileForEscapingCardsPatch
{
    [HarmonyPostfix]
    static void AddActionToRecordPileCardRemoveFinished(CombatManager __instance)
    {
        // Is there a better way to get a list of players with only access to the CombatManager?
        foreach (var player in __instance.DebugOnlyGetState().Players)
        {
            RecordPile.Record.GetPile(player).CardRemoveFinished += () =>
            {
                foreach (var book in player.Creature.CombatState.IterateHookListeners().OfType<Book>() )
                {
                    book.CheckRecordedCards();
                }
            };
        }
    }
}