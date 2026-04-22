using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.AfterCombatRoomLoaded))]
class SetUpOnBecomeWellReadHookPatch
{
    [HarmonyPostfix]
    static void AddActionToHandContentsChangedPatch(CombatManager __instance)
    {
        foreach (var player in __instance.DebugOnlyGetState().Players)
        {
            PileType.Hand.GetPile(player).ContentsChanged += async() =>
            {
                if (ZephyrQueries.IsWellRead(player))
                    await ZephyrHooks.OnBecomeWellRead(player);
            };
        }
    }
}