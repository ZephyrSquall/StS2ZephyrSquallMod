using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Patches;

namespace ZephyrSquall.ZephyrSquallCode.Commands;

public class ModifierCmd
{
    public static async Task AddHoned(PlayerChoiceContext choiceContext, Player player, int honedAmount, int cardAmount, AbstractModel source)
    {
        LocString prompt = new("card_selection", "TO_HONED");
        prompt.Add("Honed", honedAmount);
        CardSelectorPrefs prefs = new CardSelectorPrefs(prompt, cardAmount);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source);
    }

    private static async Task AddHonedWithPrefs(PlayerChoiceContext choiceContext,
        Player player,
        int honedAmount,
        CardSelectorPrefs prefs,
        AbstractModel source)
    {
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(choiceContext, player, prefs,
            (Func<CardModel, bool>)(c => c.Type == CardType.Attack), source);
        foreach (CardModel card in selectedCards)
        {
            CardModifierTracker.HonedAmount[card] += honedAmount;
        }
    }
    
}