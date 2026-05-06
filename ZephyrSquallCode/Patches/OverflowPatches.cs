using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), new Type[] {
    typeof(PlayerChoiceContext),
    typeof(Decimal),
    typeof(Player),
    typeof(bool)
})]
class SetUpOverflowHookPatch
{
    [HarmonyPostfix]
    static void CallOverflowHookPatch(ref Task<IEnumerable<CardModel>> __result, PlayerChoiceContext choiceContext, Decimal count, Player player, bool fromHandDraw)
    {
        __result = AsyncWrapper(__result, choiceContext, count, player, fromHandDraw);
    }
    
    private static async Task<IEnumerable<CardModel>> AsyncWrapper(Task<IEnumerable<CardModel>> originalTask, PlayerChoiceContext choiceContext, Decimal count, Player player, bool fromHandDraw)
    {
        var result = await originalTask;
        await DecrementLaserFocus(player);
        var timesOverflowed = count - result.Count();
        for(int i = 0; i < timesOverflowed; i++)
            await ZephyrHooks.OnOverflow(choiceContext, player, fromHandDraw);
        return result;
    }
    
    // A power cannot be decremented outside an async function, and the ShouldSkipIndividualDraw check cannot be placed
    // where it is while being async (to the best of my knowledge). So as a workaround, that check increments an
    // internal counter of the Laser Focus power for each time a card is skipped, which then gets checked at this async
    // method at the end of the Draw method to decrement the power if needed.
    private static async Task DecrementLaserFocus(Player player)
    {
        var laserFocusPower = player.Creature.GetPower<LaserFocusPower>();
        if (laserFocusPower != null)
        {
            await laserFocusPower.DecrementUnhandledSkips();
        }
    }
}