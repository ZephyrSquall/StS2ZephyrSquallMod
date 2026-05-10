using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public class ZephyrHooks
{
    public static async Task OnAddDeft(CardModel card, int amount, AbstractModel source)
    {
        foreach (var model in card.CombatState.IterateHookListeners().OfType<IOnAddDeft>())
            await model.OnAddDeft(card, amount, source);
    }

    public static async Task OnAddHoned(CardModel card, int amount, AbstractModel source)
    {
        foreach (var model in card.CombatState.IterateHookListeners().OfType<IOnAddHoned>())
            await model.OnAddHoned(card, amount, source);
    }

    public static async Task OnBecomeWellRead(Player player)
    {
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnBecomeWellRead>())
            await model.OnBecomeWellRead(player);
    }

    public static async Task OnOverflow(PlayerChoiceContext choiceContext, Player player, bool fromHandDraw)
    {
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnOverflow>())
            await model.OnOverflow(choiceContext, player, fromHandDraw);
    }

    public static async Task OnRecord(List<CardModel> cards, AbstractModel source)
    {
        if (cards.Count > 0)
            foreach (var model in cards[0].CombatState.IterateHookListeners().OfType<IOnRecord>())
                await model.OnRecord(cards, source);
    }
}