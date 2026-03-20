using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Patches;


[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetDescriptionForPile))]
public class SharpDescriptionPatch
{
    [HarmonyPostfix]
    static void AddSharpDescription(ref string __result, CardModel __instance)
    {
        if (SharpTracker.SharpAmount[__instance] > 0)
        {
            LocString sharpDesc = new LocString("static_hover_tips", "REPLAY.extraText");
            sharpDesc.Add("Sharp", (decimal)SharpTracker.SharpAmount[__instance]);
            __result += sharpDesc;
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public class SharpHoverTipPatch
{
    [HarmonyPostfix]
    static void AddSharpHoverTip(ref IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        if (SharpTracker.SharpAmount[__instance] > 0)
        {
            LocString description = new LocString("static_hover_tips", "SHARP_DYNAMIC.description");
            description.Add("Sharp", (decimal)SharpTracker.SharpAmount[__instance]);
            __result = __result.Append(new HoverTip(new LocString("static_hover_tips", "SHARP_DYNAMIC.title"),
                description));
        }
    }
}

[HarmonyPatch(typeof(Hook), "ModifyDamageInternal")]
public class SharpDamagePatch
{
    [HarmonyPrefix]
    static bool IncreaseDamage(
        IRunState runState,
        CombatState? combatState,
        Creature? target,
        Creature? dealer,
        ref Decimal damage,
        ValueProp props,
        CardModel? cardSource,
        ModifyDamageHookType modifyDamageHookType,
        List<AbstractModel> modifiers)
    {
        if (cardSource != null)
        {
            damage += SharpTracker.SharpAmount[cardSource];
        }

        return true;
    }
}

public class SharpTracker
{
    public static readonly SpireField<CardModel, int> SharpAmount = new(() => 0);
}