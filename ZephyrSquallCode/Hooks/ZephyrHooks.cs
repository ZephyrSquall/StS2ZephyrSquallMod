using MegaCrit.Sts2.Core.Entities.Players;

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
}