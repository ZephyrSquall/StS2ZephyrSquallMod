using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Patches;

namespace ZephyrSquall.ZephyrSquallCode.Commands;

public class HonedCmd
{
    public static async Task AddHoned(PlayerChoiceContext choiceContext, Player player, int honedAmount, int cardAmount,
        AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_HONED");
        prompt.Add("Honed", honedAmount);
        var prefs = new CardSelectorPrefs(prompt, cardAmount);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source, pileType);
    }

    public static async Task AddHonedToUpTo(PlayerChoiceContext choiceContext, Player player, int honedAmount,
        int maxCardAmount, AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_HONED_TO_UP_TO");
        prompt.Add("Honed", honedAmount);
        var prefs = new CardSelectorPrefs(prompt, 0, maxCardAmount);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source, pileType);
    }

    public static async Task AddHonedToAny(PlayerChoiceContext choiceContext, Player player, int honedAmount,
        AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_HONED_TO_ANY");
        prompt.Add("Honed", honedAmount);
        var prefs = new CardSelectorPrefs(prompt, 0, int.MaxValue);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source, pileType);
    }

    private static async Task AddHonedWithPrefs(PlayerChoiceContext choiceContext, Player player, int honedAmount,
        CardSelectorPrefs prefs, AbstractModel source, PileType pileType = PileType.Hand)
    {
        var selectedCards = pileType == PileType.Hand
            ? await CardSelectCmd.FromHand(choiceContext, player, prefs, null, source)
            : await CardSelectCmd.FromSimpleGrid(choiceContext,
                pileType.GetPile(player)
                    .Cards
                    .OrderBy(c => c.Rarity)
                    .ThenBy(c => c.Id)
                    .ToList(), player, prefs);
        await AddHonedToSpecific(selectedCards, honedAmount, source);
    }

    public static async Task AddHonedToSpecific(CardModel card, int honedAmount, AbstractModel source)
    {
        await AddHonedToSpecific([card], honedAmount, source);
    }

    public static async Task AddHonedToSpecific(IEnumerable<CardModel> cards, int honedAmount, AbstractModel source)
    {
        foreach (var card in cards)
        {
            CardModifierTracker.HonedAmount[card] += honedAmount;
            await ZephyrHooks.OnAddHoned(card, honedAmount, source);
        }
    }
}