using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch(typeof(CardPileCmd), MethodType.Async, new Type[] {
    typeof(PlayerChoiceContext),
    typeof(Decimal),
    typeof(Player),
    typeof(bool)
})]
[HarmonyPatch(nameof(CardPileCmd.Draw))]
class SkipIndividualDrawsPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> SkipDraws(ILGenerator generator, IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Insert label at the "++i" part of the for loop.
        // Note: The last "stloc.s V_8" instruction is closer to the point where I want to insert the label, so it seems
        // that would be a smarter instruction to match on. But in all my testing, if I matched on "stloc.s V_8"
        // instead, the patch would always produce an invalid program. I am unsure why. The only idea I have is the bool
        // return value from the ShouldSkipIndividualDraw check gives this method an extra local variable that offsets
        // everything, so "V_8" operands are actually somewhere else. In any case, matching on the Add instruction
        // instead has proven far more reliable.
        codeMatcher.End().MatchStartBackwards(new CodeMatch(OpCodes.Add))
            .ThrowIfInvalid("Could not find add instruction")
            .Advance(-6);
        Label skipIndividualDrawLabel = generator.DefineLabel();
        codeMatcher.Labels.Add(skipIndividualDrawLabel);
        
        // Get player field
        var method = AccessTools.Method(typeof(CardPileCmd), "CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot");
        codeMatcher.Start().MatchStartForward(CodeMatch.Calls(method))
            .ThrowIfInvalid("Could not find call to CardPileCmd.CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot")
            .Advance(-1);
        var playerField  = codeMatcher.Operand;
        if (playerField == null)
        {
            Log.Error("player is NULL");
        };

        // Check if the current card draw should be skipped immediately at the start of the for loop.
        codeMatcher.Start().MatchStartForward(CodeMatch.Calls(() => CardPileCmd.ShuffleIfNecessary(default, default)))
            .ThrowIfInvalid("Could not find call to List.Add")
            .Advance(-4)
            .InsertAndAdvance(CodeInstruction.LoadArgument(0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, playerField))
            .InsertAndAdvance(CodeInstruction.Call(() => ShouldSkipIndividualDraw(default)))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brtrue, skipIndividualDrawLabel));
            
        return codeMatcher.Instructions();
    }

    private static bool ShouldSkipIndividualDraw(Player player)
    {
        var willSkip = false;
        var laserFocusPower = player.Creature.GetPower<LaserFocusPower>();
        if (laserFocusPower != null)
        {
            willSkip = laserFocusPower.ShouldSkipIndividualDraw(player);
        }
        
        return willSkip;
    }
}