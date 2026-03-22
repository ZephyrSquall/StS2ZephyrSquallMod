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
public class HonedDescriptionPatch
{
    static MethodBase TargetMethod()
    {
        MethodInfo method = typeof(CardModel).GetMethod("GetDescriptionForPile", BindingFlags.NonPublic | BindingFlags.Instance);
        return method;
    }
    
    [HarmonyTranspiler]
     static IEnumerable<CodeInstruction> AddHonedDescription(IEnumerable<CodeInstruction> instructions)
     {
         var codeMatcher = new CodeMatcher(instructions);

         // Add "Honed" text immediately before where added "Replay" text would go if the card also had Replay.
         codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
             .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
             // Load the "source" local variable onto the stack, which is index 5 in the raw IL.
             // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
             // to load that again.
             .InsertAndAdvance(CodeInstruction.LoadLocal(5))
             .InsertAndAdvance(CodeInstruction.Call(() => AddHonedDescriptionHelper(default, default)))
             // Store the "source" local variable.
             .InsertAndAdvance(CodeInstruction.StoreLocal(5))
             // Load the cardModel onto the stack again to restore its original state before this patch.
             .InsertAndAdvance(CodeInstruction.LoadArgument(0));
         
         return codeMatcher.Instructions();
     }
     
    public static List<string> AddHonedDescriptionHelper(CardModel cardModel, List<string> source)
    {
        var honed = HonedTracker.HonedAmount[cardModel];
        if (honed > 0)
        {
            LocString locString = new LocString("static_hover_tips", "HONED.extraText");
            locString.Add("Honed", honed);
            source.Add(locString.GetFormattedText());  
        }
        return source;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public class HonedHoverTipPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> AddHonedHoverTip(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Add "Honed" hover tip immediately before "Replay" hover tip.
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
            .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
            // Load the "list" local variable onto the stack, which is index 0 in the raw IL.
            // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
            // to load that again.
            .InsertAndAdvance(CodeInstruction.LoadLocal(0))
            .InsertAndAdvance(CodeInstruction.Call(() => AddHonedHoverTipHelper(default, default)))
            // Store the "list" local variable.
            .InsertAndAdvance(CodeInstruction.StoreLocal(0))
            // Load the cardModel onto the stack again to restore its original state before this patch.
            .InsertAndAdvance(CodeInstruction.LoadArgument(0));
         
        return codeMatcher.Instructions();
    }
    
    public static List<IHoverTip> AddHonedHoverTipHelper(CardModel cardModel, List<IHoverTip> list)
    {
        var honed = HonedTracker.HonedAmount[cardModel];
        if (honed > 0)
        {
            LocString description = new LocString("static_hover_tips", "HONED_DYNAMIC.description");
            description.Add("Honed", honed);
            list.Add(new HoverTip(new LocString("static_hover_tips", "HONED_DYNAMIC.title"),
                description));
        }

        return list;
    }
}

[HarmonyPatch(typeof(Hook), "ModifyDamageInternal")]
public class HonedDamagePatch
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
            damage += HonedTracker.HonedAmount[cardSource];
        }

        return true;
    }
}

public class HonedTracker
{
    public static readonly SpireField<CardModel, int> HonedAmount = new(() => 0);
}