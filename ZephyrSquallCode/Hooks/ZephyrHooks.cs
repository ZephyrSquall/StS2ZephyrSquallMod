using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public class ZephyrHooks
{
    public static async Task AfterExtraTurnStart(ICombatState combatState)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IAfterExtraTurnStart>())
            await model.AfterExtraTurnStart(combatState);
    }

    public static int ModifyAttackHitCountMultiplicative(ICombatState combatState, AttackCommand attackCommand,
        int originalHitCount)
    {
        int hitCount = originalHitCount;
        foreach (var model in combatState.IterateHookListeners().OfType<IModifyAttackHitCountMultiplicative>())
            hitCount = model.ModifyAttackHitCountMultiplicative(attackCommand, hitCount);
        return hitCount;
    }

    public static async Task OnAddHoned(CardModel card, int amount, AbstractModel source)
    {
        if (card.CombatState == null) throw new InvalidOperationException("card.CombatState is null");
        foreach (var model in card.CombatState.IterateHookListeners().OfType<IOnAddHoned>())
            await model.OnAddHoned(card, amount, source);
    }

    public static async Task OnBecomeWellRead(Player player)
    {
        if (player.Creature.CombatState == null) throw new InvalidOperationException("player.Creature.CombatState is null");
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnBecomeWellRead>())
            await model.OnBecomeWellRead(player);
    }

    public static async Task OnOverflow(PlayerChoiceContext choiceContext, Player player, bool fromHandDraw)
    {
        if (player.Creature.CombatState == null) throw new InvalidOperationException("player.Creature.CombatState is null");
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnOverflow>())
            await model.OnOverflow(choiceContext, player, fromHandDraw);
    }

    public static async Task OnRecord(List<CardModel> cards, AbstractModel source)
    {
        if (cards.Count > 0)
        {
            ICombatState combatState = cards[0].CombatState ?? throw new InvalidOperationException("cards[0].CombatState is null");
            foreach (var model in combatState.IterateHookListeners().OfType<IOnRecord>())
                await model.OnRecord(cards, source);
        }
    }
}