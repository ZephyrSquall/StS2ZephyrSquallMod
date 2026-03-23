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
public class CardModifierDescriptionPatch
{
    static MethodBase TargetMethod()
    {
        MethodInfo method = typeof(CardModel).GetMethod("GetDescriptionForPile", BindingFlags.NonPublic | BindingFlags.Instance);
        return method;
    }
    
    [HarmonyTranspiler]
     static IEnumerable<CodeInstruction> AddCardModifierDescriptions(IEnumerable<CodeInstruction> instructions)
     {
         var codeMatcher = new CodeMatcher(instructions);

         // Add modifier text immediately before where added "Replay" text would go if the card also had Replay.
         codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
             .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
             // Load the "source" local variable onto the stack, which is index 5 in the raw IL.
             // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
             // to load that again.
             .InsertAndAdvance(CodeInstruction.LoadLocal(5))
             .InsertAndAdvance(CodeInstruction.Call(() => AddCardModifierDescriptionsHelper(default, default)))
             // Store the top of the stack into the "source" local variable.
             .InsertAndAdvance(CodeInstruction.StoreLocal(5))
             // Load the cardModel onto the stack again to restore its original state before this patch.
             .InsertAndAdvance(CodeInstruction.LoadArgument(0));
         
         return codeMatcher.Instructions();
     }
     
    public static List<string> AddCardModifierDescriptionsHelper(CardModel cardModel, List<string> source)
    {
        var deft = CardModifierTracker.DeftAmount[cardModel];
        if (deft > 0)
        {
            LocString locString = new LocString("static_hover_tips", "DEFT.extraText");
            locString.Add("Deft", deft);
            source.Add(locString.GetFormattedText());  
        }
        
        var honed = CardModifierTracker.HonedAmount[cardModel];
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
public class CardModifierHoverTipPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> AddCardModifierHoverTips(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Add modifier hover tip immediately before "Replay" hover tip.
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
            .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
            // Load the "list" local variable onto the stack, which is index 0 in the raw IL.
            // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
            // to load that again.
            .InsertAndAdvance(CodeInstruction.LoadLocal(0))
            .InsertAndAdvance(CodeInstruction.Call(() => AddCardModifierHoverTipsHelper(default, default)))
            // Store the top of the stack into the "list" local variable.
            .InsertAndAdvance(CodeInstruction.StoreLocal(0))
            // Load the cardModel onto the stack again to restore its original state before this patch.
            .InsertAndAdvance(CodeInstruction.LoadArgument(0));
         
        return codeMatcher.Instructions();
    }
    
    public static List<IHoverTip> AddCardModifierHoverTipsHelper(CardModel cardModel, List<IHoverTip> list)
    {
        var deft = CardModifierTracker.DeftAmount[cardModel];
        if (deft > 0)
        {
            LocString description = new LocString("static_hover_tips", "DEFT_DYNAMIC.description");
            description.Add("Deft", deft);
            list.Add(new HoverTip(new LocString("static_hover_tips", "DEFT_DYNAMIC.title"),
                description));
        }
        
        var honed = CardModifierTracker.HonedAmount[cardModel];
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

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyBlock))]
public class DeftBlockPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> IncreaseBlock(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Jumps to immediately before the foreach loop that calls ModifyBlockMultiplicative on each hook listener (this
        // is immediately before the final call to IterateHookListeners in the IL)
        codeMatcher.End().MatchStartBackwards(CodeMatch.Calls(() => default(CombatState).IterateHookListeners()))
            .ThrowIfInvalid("Could not find call to IterateHookListeners")
            // Load current block amount "num1" (local index 1) and card source (argument index 4)
            .InsertAndAdvance(CodeInstruction.LoadArgument(4))
            .InsertAndAdvance(CodeInstruction.LoadLocal(1))
            .InsertAndAdvance(CodeInstruction.Call(() => IncreaseBlockHelper(default, default)))
            // Store the top of the stack into the "num1" local variable.
            .InsertAndAdvance(CodeInstruction.StoreLocal(1));
        
        return codeMatcher.Instructions();
    }

    public static Decimal IncreaseBlockHelper(CardModel? cardSource, Decimal block)
    {
        if (cardSource != null)
        {
            block += CardModifierTracker.DeftAmount[cardSource];
        }

        return block;
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
            damage += CardModifierTracker.HonedAmount[cardSource];
        }

        return true;
    }
}

public class CardModifierTracker
{
    public static readonly SpireField<CardModel, int> DeftAmount = new(() => 0);
    public static readonly SpireField<CardModel, int> HonedAmount = new(() => 0);
}