using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

public class ExtraTurnTracker
{
    public static bool IsExtraTurn;
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterTakingExtraTurn))]
class ExtraTurnStartPatch
{
    [HarmonyPrefix]
    static bool TrackExtraTurnStart(CombatState combatState, Player player)
    {
        ExtraTurnTracker.IsExtraTurn = true;
        return true;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterTurnEnd))]
class ExtraTurnEndPatch
{
    [HarmonyPrefix]
    static bool TrackExtraTurnEnd(CombatState combatState, CombatSide side)
    {
        ExtraTurnTracker.IsExtraTurn = false;
        return true;
    }
}