using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Runs;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

public class ExtraTurnTracker
{
    public static List<Player> PlayersTakingExtraTurn = [];
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterTakingExtraTurn))]
internal class ExtraTurnStartPatch
{
    [HarmonyPrefix]
    private static bool TrackExtraTurnExtraTurnStart(ICombatState combatState, Player player)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Add(player);
        return true;
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

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterTurnEnd))]
internal class TurnEndPatch
{
    [HarmonyPrefix]
    private static bool TrackExtraTurnTurnEnd(ICombatState combatState, CombatSide side)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Clear();
        return true;
    }
}