using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using ZephyrSquall.ZephyrSquallCode.Hooks;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

public class ExtraTurnTracker
{
    public static List<Player> PlayersTakingExtraTurn = [];
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterTakingExtraTurn))]
internal class ExtraTurnStartPatch
{
    [HarmonyPostfix]
    private static void TrackExtraTurnExtraTurnStart(ref Task __result, ICombatState combatState, Player player,
        AbstractModel source)
    {
        __result = AsyncWrapper(__result, combatState, player, source);
    }

    private static async Task AsyncWrapper(Task originalTask, ICombatState combatState, Player player,
        AbstractModel source)
    {
        if (ExtraTurnTracker.PlayersTakingExtraTurn.Count == 0) await ZephyrHooks.AfterExtraTurnStart(combatState);
        ExtraTurnTracker.PlayersTakingExtraTurn.Add(player);
        await originalTask;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCombatStart))]
internal class CombatStartPatch
{
    [HarmonyPrefix]
    private static bool TrackExtraTurnCombatStart(IRunState runState, ICombatState? combatState)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Clear();
        return true;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeSideTurnEnd))]
internal class TurnEndPatch
{
    [HarmonyPrefix]
    private static bool TrackExtraTurnTurnEnd(ICombatState combatState, CombatSide side)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Clear();
        return true;
    }
}