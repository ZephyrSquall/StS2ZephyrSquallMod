using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public class ZephyrHooks
{
    public static async Task OnBecomeWellRead(Player player)
    {
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnBecomeWellRead>() )
        {
            await model.OnBecomeWellRead(player);
        }
    }
    
    public static async Task OnOverflow(PlayerChoiceContext choiceContext, Player player, bool fromHandDraw)
    {
        foreach (var model in player.Creature.CombatState.IterateHookListeners().OfType<IOnOverflow>() )
        {
            await model.OnOverflow(choiceContext, player, fromHandDraw);
        }
    }
}