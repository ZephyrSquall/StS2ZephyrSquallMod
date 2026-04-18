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
class ExtraTurnStartPatch
{
    [HarmonyPrefix]
    static bool TrackExtraTurnExtraTurnStart(CombatState combatState, Player player)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Add(player);
        return true;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCombatStart))]
class CombatStartPatch
{
    [HarmonyPrefix]
    static bool TrackExtraTurnCombatStart(IRunState runState, CombatState? combatState)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Clear();
        return true;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterTurnEnd))]
class TurnEndPatch
{
    [HarmonyPrefix]
    static bool TrackExtraTurnTurnEnd(CombatState combatState, CombatSide side)
    {
        ExtraTurnTracker.PlayersTakingExtraTurn.Clear();
        return true;
    }
}