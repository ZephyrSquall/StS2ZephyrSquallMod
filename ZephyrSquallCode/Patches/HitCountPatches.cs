using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Hooks;
using ZephyrSquall.ZephyrSquallCode.Hooks;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyAttackHitCount))]
internal class ModifyAttackHitCountMultiplicativePatch
{
    [HarmonyPostfix]
    private static void ModifyAttackHitCountPostfix(ref Decimal __result, ICombatState combatState,
        AttackCommand attackCommand, int originalHitCount)
    {
        int hitCount = (int)__result;
        int modifiedHitCount = ZephyrHooks.ModifyAttackHitCountMultiplicative(combatState, attackCommand, hitCount);
        __result = modifiedHitCount;
    }
}