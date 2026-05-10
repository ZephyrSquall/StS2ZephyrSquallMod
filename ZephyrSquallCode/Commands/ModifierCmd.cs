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

public class ModifierCmd
{
    public static async Task AddDeft(PlayerChoiceContext choiceContext, Player player, int deftAmount, int cardAmount,
        AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_DEFT");
        prompt.Add("Deft", deftAmount);
        var prefs = new CardSelectorPrefs(prompt, cardAmount);
        await AddDeftWithPrefs(choiceContext, player, deftAmount, prefs, source, pileType);
    }

    public static async Task AddDeftToUpTo(PlayerChoiceContext choiceContext, Player player, int deftAmount,
        int maxCardAmount, AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_DEFT_TO_UP_TO");
        prompt.Add("Deft", deftAmount);
        var prefs = new CardSelectorPrefs(prompt, 0, maxCardAmount);
        await AddDeftWithPrefs(choiceContext, player, deftAmount, prefs, source, pileType);
    }

    public static async Task AddDeftToAny(PlayerChoiceContext choiceContext, Player player, int deftAmount,
        AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_DEFT_TO_ANY");
        prompt.Add("Deft", deftAmount);
        var prefs = new CardSelectorPrefs(prompt, 0, int.MaxValue);
        await AddDeftWithPrefs(choiceContext, player, deftAmount, prefs, source, pileType);
    }

    private static async Task AddDeftWithPrefs(PlayerChoiceContext choiceContext, Player player, int deftAmount,
        CardSelectorPrefs prefs, AbstractModel source, PileType pileType = PileType.Hand)
    {
        var selectedCards = pileType == PileType.Hand
            ? await CardSelectCmd.FromHand(choiceContext, player, prefs, (Func<CardModel, bool>)(c => c.GainsBlock),
                source)
            : await CardSelectCmd.FromSimpleGrid(choiceContext,
                pileType.GetPile(player)
                    .Cards.Where(c => c.GainsBlock)
                    .OrderBy(c => c.Rarity)
                    .ThenBy(c => c.Id)
                    .ToList(), player, prefs);
        await AddDeftToSpecific(selectedCards, deftAmount, source);
    }

    public static async Task AddDeftToSpecific(CardModel card, int deftAmount, AbstractModel source)
    {
        await AddDeftToSpecific([card], deftAmount, source);
    }

    public static async Task AddDeftToSpecific(IEnumerable<CardModel> cards, int deftAmount, AbstractModel source)
    {
        foreach (var card in cards)
        {
            CardModifierTracker.DeftAmount[card] += deftAmount;
            await ZephyrHooks.OnAddDeft(card, deftAmount, source);
        }
    }

    public static async Task AddHoned(PlayerChoiceContext choiceContext, Player player, int honedAmount, int cardAmount,
        AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_HONED");
        prompt.Add("Honed", honedAmount);
        var prefs = new CardSelectorPrefs(prompt, cardAmount);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source);
    }

    public static async Task AddHonedToUpTo(PlayerChoiceContext choiceContext, Player player, int honedAmount,
        int maxCardAmount, AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_HONED_TO_UP_TO");
        prompt.Add("Honed", honedAmount);
        var prefs = new CardSelectorPrefs(prompt, 0, maxCardAmount);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source);
    }

    public static async Task AddHonedToAny(PlayerChoiceContext choiceContext, Player player, int honedAmount,
        AbstractModel source, PileType pileType = PileType.Hand)
    {
        LocString prompt = new("card_selection", "TO_ADD_HONED_TO_ANY");
        prompt.Add("Honed", honedAmount);
        var prefs = new CardSelectorPrefs(prompt, 0, int.MaxValue);
        await AddHonedWithPrefs(choiceContext, player, honedAmount, prefs, source);
    }

    private static async Task AddHonedWithPrefs(PlayerChoiceContext choiceContext, Player player, int honedAmount,
        CardSelectorPrefs prefs, AbstractModel source, PileType pileType = PileType.Hand)
    {
        var selectedCards = pileType == PileType.Hand
            ? await CardSelectCmd.FromHand(choiceContext, player, prefs,
                (Func<CardModel, bool>)(c => c.Type == CardType.Attack), source)
            : await CardSelectCmd.FromSimpleGrid(choiceContext,
                pileType.GetPile(player)
                    .Cards.Where(c => c.Type == CardType.Attack)
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