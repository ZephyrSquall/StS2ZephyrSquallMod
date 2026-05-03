using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
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
        codeMatcher.End().MatchStartBackwards(CodeMatch.StoresLocal("V_8"))
            .ThrowIfInvalid("Could not find stloc instruction for V_8")
            .Advance(-2);
        Label skipIndividualDrawLabel = generator.DefineLabel();
        codeMatcher.Labels.Add(skipIndividualDrawLabel);
        
        // Create new label for jump point after checking if a draw should be skipped, but don't add it yet as the
        // instruction to jump to has not yet been created.
        Label doNotSkipIndividualDrawLabel = generator.DefineLabel();
        
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
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse, doNotSkipIndividualDrawLabel))

            // These instructions are only accessed if ShouldSkipIndividualDraw returned true. These instructions are
            // used to align the IL execution stack with what it should look like just before the "++i" statement and
            // then jump to it.
            // At least, that's the intention, but finding out exactly what the differences are in the IL execution
            // stack between these two locations is proving to be extremely difficult, so I'm leaving this for another
            // day. The following instructions are just demonstrative and ultimately make no difference to the execution
            // stack. Hopefully in the future I find out exactly what needs to be added and/or removed from the
            // execution stack so the branch instruction can be uncommented.
            .InsertAndAdvance(CodeInstruction.LoadArgument(0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, playerField))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Pop));
            // .InsertAndAdvance(new CodeInstruction(OpCodes.Br, skipIndividualDrawLabel));
        
        
        // Insert the doNotSkipIndividualDrawLabel here, so the code knows where to pick up from if this draw isn't to
        // be skipped.
        codeMatcher.Labels.Add(doNotSkipIndividualDrawLabel);
            
        var codeInstructions = codeMatcher.Instructions();
        
        // A power cannot be decremented outside an async function, and the ShouldSkipIndividualDraw check cannot be
        // placed where it is while being async (to the best of my knowledge). So as a workaround, that check increments
        // a counter for each time a card is skipped, which then gets checked at this async method at the end of the
        // Draw method to decrement the power if needed.
        // Note that despite this async call supposedly being inserted at the end of the original method, my testing
        // shows that it is actually called inside the Draw method's for loop somehow, as this code runs once at the end
        // of each loop. I'm not sure why this happens, but it's convenient since it means the visual of the power
        // decreasing happens immediately instead of after all cards are drawn, which is what I wanted. Unfortunately,
        // this still happens after the card draw in the loop, so I still need the synchronous ShouldSkipIndividualDraw
        // method.
        var x = AsyncMethodCall.Create(generator, codeInstructions, original,
            AccessTools.Method(typeof(SkipIndividualDrawsPatch), nameof(DecrementLaserFocus)), afterState: original);
        return x;
    }

    private static bool ShouldSkipIndividualDraw(Player player)
    {
        // Will eventually turn this into a hook.
        var willSkip = false;
        var laserFocusPower = player.Creature.GetPower<LaserFocusPower>();
        if (laserFocusPower != null)
        {
            willSkip = laserFocusPower.ShouldSkipIndividualDraw(player);
        }

        // This log correctly identifies exactly when I want to skip a card or not. Everything works except the branch
        // opcode.
        Log.Warn($"Player is drawing a card, would I skip it if I was working properly? {willSkip}");
        return willSkip;
    }
    
    private static async Task DecrementLaserFocus(Player player)
     {
         var laserFocusPower = player.Creature.GetPower<LaserFocusPower>();
         if (laserFocusPower != null)
         {
             await laserFocusPower.DecrementUnhandledSkips();
         }
     }
}