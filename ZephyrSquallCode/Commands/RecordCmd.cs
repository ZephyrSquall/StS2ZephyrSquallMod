using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Cards;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Commands;

public static class RecordCmd
{
    private static LocString RecordSelectionPrompt => new("card_selection", "TO_RECORD");
    private static LocString RecordUpToSelectionPrompt => new("card_selection", "TO_RECORD_UP_TO");
    private static LocString RecordAnySelectionPrompt => new("card_selection", "TO_RECORD_ANY");

    public static async Task<Book?> Record(PlayerChoiceContext choiceContext, Player player, int amount,
        ICombatState combatState, AbstractModel source)
    {
        var prefs = new CardSelectorPrefs(RecordSelectionPrompt, amount);
        return await RecordWithPrefs(choiceContext, player, prefs, combatState, source);
    }

    public static async Task<Book?> RecordUpTo(PlayerChoiceContext choiceContext, Player player, int maxAmount,
        ICombatState combatState, AbstractModel source)
    {
        var prefs = new CardSelectorPrefs(RecordUpToSelectionPrompt, 0, maxAmount);
        return await RecordWithPrefs(choiceContext, player, prefs, combatState, source);
    }

    public static async Task<Book?> RecordAny(PlayerChoiceContext choiceContext, Player player, ICombatState combatState,
        AbstractModel source)
    {
        var prefs = new CardSelectorPrefs(RecordAnySelectionPrompt, 0, int.MaxValue);
        return await RecordWithPrefs(choiceContext, player, prefs, combatState, source);
    }

    private static async Task<Book?> RecordWithPrefs(PlayerChoiceContext choiceContext, Player player, CardSelectorPrefs prefs,
        ICombatState combatState, AbstractModel source)
    {
        var selectedCards = (await CardSelectCmd.FromHand(choiceContext, player, prefs,
            (Func<CardModel, bool>)ZephyrQueries.CanBeRecorded, source)).ToList();
        Book? book = await Book.CreateInHand(player, selectedCards, combatState);
        await ZephyrHooks.OnRecord(selectedCards, source);
        return book;
    }
}