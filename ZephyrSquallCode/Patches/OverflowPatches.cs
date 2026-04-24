using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Hooks;

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
        var timesOverflowed = count - result.Count();
        for(int i = 0; i < timesOverflowed; i++)
            await ZephyrHooks.OnOverflow(choiceContext, player, fromHandDraw);
        return result;
    }
}