using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

public class WellReadTracker
{
    public static bool WasWellReadAtStartOfCardPlay = false;
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
class PlayStartPatch
{
    [HarmonyPrefix]
    static bool TrackWellReadCardPlayStartPatch(
        PlayerChoiceContext choiceContext,
        Creature? target,
        bool isAutoPlay,
        ResourceInfo resources,
        bool skipCardPileVisuals,
        CardModel __instance)
    {
        WellReadTracker.WasWellReadAtStartOfCardPlay = ZephyrQueries.IsWellRead(__instance.Owner);
        return true;
    }
}

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.CheckForEmptyHand))]
class WellReadHookPatch
{
    [HarmonyPostfix]
    static void CheckForWellReadHandPatch(ref Task __result, PlayerChoiceContext choiceContext, Player player)
    {
        if (ZephyrQueries.IsWellRead(player))
            __result = AsyncWrapper(__result, choiceContext, player);
    }

    private static async Task AsyncWrapper(Task originalResult, PlayerChoiceContext choiceContext, Player player)
    {
        await originalResult;
        await ZephyrHooks.OnBecomeWellRead(choiceContext, player);
    }
}