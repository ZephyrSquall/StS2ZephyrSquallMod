using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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
    private static bool DisplayRecordedCards(NCardHolder __instance)
    {
        if (__instance.CardModel is Book { RecordedCards.Count: > 0 } book)
        {
            var previewCardSize = NCard.defaultSize * NCardHolder.smallScale;
            // "Card offset" is how much a card preview has to be offset to the side to fit an additional card.
            var cardOffsetSize = previewCardSize.X + XPadding;
            float shrinkScale = 1;

            // Add offset in the x direction if cards scroll off-screen to keep them on screen . Or alternatively,
            // shrink the previewCardSize if cards scroll off-screen in both directions.
            float xOffset = 0;
            var cardsPreviewWidth = book.RecordedCards.Count * cardOffsetSize;
            if (NGame.Instance == null) throw new InvalidOperationException("NGame.Instance is null");
            if (cardsPreviewWidth > NGame.Instance.GetViewportRect().Size.X)
            {
                shrinkScale = NGame.Instance.GetViewportRect().Size.X / cardsPreviewWidth;
                previewCardSize *= shrinkScale;
                cardOffsetSize *= shrinkScale;
                cardsPreviewWidth *= shrinkScale;
            }

            var previewsLeftEdgeX = __instance.GlobalPosition.X - cardsPreviewWidth / 2;
            var previewsRightEdgeX = __instance.GlobalPosition.X + cardsPreviewWidth / 2;
            if (previewsLeftEdgeX < 0)
                xOffset = -previewsLeftEdgeX;
            else if (previewsRightEdgeX > NGame.Instance.GetViewportRect().Size.X)
                xOffset = NGame.Instance.GetViewportRect().Size.X - previewsRightEdgeX;

            var yOffset = -((NCard.defaultSize.Y + previewCardSize.Y) / 2f + YPadding);
            // If the Book is too close to the top of the screen (only possible when viewed in a pile other than the
            // Hand), show the previewed cards beneath the Book instead.
            if (__instance.GlobalPosition.Y + yOffset < 180) yOffset *= -1;

            for (var i = 0; i < book.RecordedCards.Count; i++)
            {
                var recordedCard = book.RecordedCards[i];
                var nCard = NCard.FindOnTable(recordedCard);

                if (nCard != null && nCard.Model?.Pile != null)
                {
                    __instance.AddChild(nCard);
                    nCard.UpdateVisuals(nCard.Model.Pile.Type, CardPreviewMode.Normal);
                    RecordedCardPreviewTracker.RecordedCardsInPreview.Add(nCard);

                    nCard.Scale = NCardHolder.smallScale * shrinkScale;

                    var individualCardOffset = i + 0.5f - book.RecordedCards.Count / 2f;
                    var individualXOffset = xOffset + individualCardOffset * cardOffsetSize;
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
    private static bool HideRecordedCards()
    {
        foreach (var nCard in RecordedCardPreviewTracker.RecordedCardsInPreview) nCard.QueueFree();

        RecordedCardPreviewTracker.RecordedCardsInPreview.Clear();
        return true;
    }
}

// Sets up an event listener on each player's Record Pile to tell all of that player's Books to check their Recorded
// cards anytime a card is removed from the Record Pile.
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.AfterCombatRoomLoaded))]
internal class WatchRecordPileForEscapingCardsPatch
{
    [HarmonyPostfix]
    private static void AddActionToRecordPileCardRemoveFinished(CombatTurnState? ____turnState)
    {
        if (____turnState == null) return;
        foreach (Player player in ____turnState.State.Players)
            RecordPile.Record.GetPile(player).CardRemoveFinished += () =>
            {
                if (player.Creature.CombatState == null) throw new InvalidOperationException("player.Creature.CombatState is null");
                foreach (Book book in player.Creature.CombatState.IterateHookListeners().OfType<Book>())
                    book.CheckRecordedCards();
            };
    }
}