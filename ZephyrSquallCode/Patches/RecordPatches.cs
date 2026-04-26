using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
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
    [HarmonyPrefix]
    static bool DisplayRecordedCards(NCardHolder __instance)
    {
        Log.Warn($"Running a CreateHoverTips Prefix Patch on {__instance.CardModel}!");
        
        if (__instance.CardModel is Book book)
        {
            Log.Warn("Book detected!");
            var firstRecordedCard = book.RecordedCards.FirstOrDefault();
            if (firstRecordedCard != null)
            {
                Log.Warn("Book has recorded cards!");
                var nCard = NCard.FindOnTable(firstRecordedCard);
        
                if (nCard is { } nonNullNCard)
                {
                    Log.Warn("Found an NCard for Book's first recorded card!");
                    __instance.AddChild(nCard);
                    nCard.UpdateVisuals(nCard.Model.Pile.Type, CardPreviewMode.Normal);
                    nCard.Position = new Vector2(0f, -500f);
                    RecordedCardPreviewTracker.RecordedCardsInPreview.Add(nCard);
                    
                    Log.Warn($"Is this card ready? {nCard.IsNodeReady()}");
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
