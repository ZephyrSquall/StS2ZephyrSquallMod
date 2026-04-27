using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

public class RecordedCardPreviewTracker
{
    public static List<NCard> RecordedCardsInPreview = [];
}

[HarmonyPatch(typeof(NCardHolder), "CreateHoverTips")]
public class DisplayRecordedCardsPatch
{
    private static readonly Vector2 PreviewCardSize = NCard.defaultSize * NCardHolder.smallScale;
    private const float XPadding = 8f;
    private const float YPadding = 8f;

    [HarmonyPrefix]
    static bool DisplayRecordedCards(NCardHolder __instance)
    {
        Log.Warn($"Viewport size: {NGame.Instance.GetViewportRect().Size.X}, {NGame.Instance.GetViewportRect().Size.Y}");
        Log.Warn($"Running a CreateHoverTips Prefix Patch on {__instance.CardModel}!");
        
        if (__instance.CardModel is Book book)
        {
            Log.Warn("Book detected!");

            for (int i = 0; i < book.RecordedCards.Count; i++)
            {
                var recordedCard = book.RecordedCards[i];
                
                Log.Warn("Book has recorded cards!");
                var nCard = NCard.FindOnTable(recordedCard);
        
                if (nCard is { } nonNullNCard)
                {
                    Log.Warn("Found an NCard for Book's first recorded card!");
                    __instance.AddChild(nCard);
                    nCard.UpdateVisuals(nCard.Model.Pile.Type, CardPreviewMode.Normal);
                    RecordedCardPreviewTracker.RecordedCardsInPreview.Add(nCard);

                    nCard.Scale = NCardHolder.smallScale;
                    
                    // Offset from the card being directly above the Book. Negative for being offset to the left.
                    var cardOffset = i + 0.5f - (book.RecordedCards.Count / 2f);
                    var xOffset = cardOffset * (PreviewCardSize.X + XPadding);
                    var yOffset = -((NCard.defaultSize.Y + PreviewCardSize.Y) / 2f + YPadding);
                    nCard.Position = new Vector2(xOffset, yOffset);
                    
                    Log.Warn($"Is this card ready? {nCard.IsNodeReady()}");
                }
            }
            
            var firstRecordedCard = book.RecordedCards.FirstOrDefault();
            if (firstRecordedCard != null)
            {

            }
        }
        
        return true;
    }
}

[HarmonyPatch(typeof(NCardHolder), "ClearHoverTips")]
public class HideRecordedCardsPatch
{
    [HarmonyPrefix]
    static bool HideRecordedCards(NCardHolder __instance)
    {
        Log.Warn($"Running a ClearHoverTips Prefix Patch on {__instance.CardModel}!");

        foreach (var nCard in RecordedCardPreviewTracker.RecordedCardsInPreview)
        {
            Log.Warn($"Removing NCard {nCard.Model.Title} from Book preview");
            nCard.QueueFree();
        }

        RecordedCardPreviewTracker.RecordedCardsInPreview.Clear();
        
        return true;
    }
}
