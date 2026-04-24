using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Commands;

public static class RecordCmd
{
    private static LocString RecordSelectionPrompt => new("card_selection", "TO_RECORD");
    private static LocString RecordUpToSelectionPrompt => new("card_selection", "TO_RECORD_UP_TO");
    private static LocString RecordAnySelectionPrompt => new("card_selection", "TO_RECORD_ANY");

    public static async Task Record(PlayerChoiceContext choiceContext, Player player, int amount,
        ICombatState combatState, AbstractModel source)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(RecordSelectionPrompt, amount);
        await RecordWithPrefs(choiceContext, player, prefs, combatState, source);
    }

    public static async Task RecordUpTo(PlayerChoiceContext choiceContext, Player player, int maxAmount,
        ICombatState combatState, AbstractModel source)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(RecordUpToSelectionPrompt, 0, maxAmount);
        await RecordWithPrefs(choiceContext, player, prefs, combatState, source);
    }

    public static async Task RecordAny(PlayerChoiceContext choiceContext, Player player, ICombatState combatState,
        AbstractModel source)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(RecordAnySelectionPrompt, 0, int.MaxValue);
        await RecordWithPrefs(choiceContext, player, prefs, combatState, source);
    }

    private static async Task RecordWithPrefs(PlayerChoiceContext choiceContext,
        Player player,
        CardSelectorPrefs prefs,
        ICombatState combatState,
        AbstractModel source)
    {
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(choiceContext, player, prefs,
            (Func<CardModel, bool>)(c =>
                c.Type != CardType.Status && c.Type != CardType.Curse && c.Type != CardType.Quest && c is not Book),
            source);
        await Book.CreateInHand(player, selectedCards, combatState);
    }
}