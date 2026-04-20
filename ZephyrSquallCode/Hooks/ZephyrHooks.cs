using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public class ZephyrHooks
{
    public static async Task OnBecomeWellRead(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnBecomeWellRead>() )
        {
            await model.OnBecomeWellRead(choiceContext, player);
        }
    }
}