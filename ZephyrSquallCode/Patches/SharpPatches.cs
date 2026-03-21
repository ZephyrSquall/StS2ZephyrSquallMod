using System.Reflection;
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

[HarmonyPatch]
public class SharpDescriptionPatch
{
    static MethodBase TargetMethod()
    {
        MethodInfo method = typeof(CardModel).GetMethod("GetDescriptionForPile", BindingFlags.NonPublic | BindingFlags.Instance);
        return method;
    }
    
    [HarmonyTranspiler]
     static IEnumerable<CodeInstruction> AddSharpDescription(IEnumerable<CodeInstruction> instructions)
     // static IEnumerable<CodeInstruction> AddSharpDescription(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
     {
         var codeMatcher = new CodeMatcher(instructions);
         // LocalBuilder source = generator.DeclareLocal(typeof(List<string>));

         // Add "Sharp" text immediately before where added "Replay" text would go if the card also had Replay.
         codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
             .ThrowIfInvalid("Could not find it!!!!!!!!")
             // Load the "source" local variable onto the stack, which is index 5 in the raw IL.
             // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
             // to load that again.
             .InsertAndAdvance(CodeInstruction.LoadLocal(5))
             .InsertAndAdvance(CodeInstruction.Call(() => AddSharp(default, default)));
         

         return codeMatcher.Instructions();
     }
     
    public static CardModel AddSharp(CardModel cardModel, List<string> source)
    {
        var x = source[0];
        // This is just to cause a visible effect in-game to prove this helper method was called.
        SharpTracker.SharpAmount[cardModel] += 1;
        return cardModel;
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