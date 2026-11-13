using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch]
public class ShareExtraTurnPatch
{
    private static readonly MethodInfo SwitchFromPlayerToEnemySideMethod =
        AccessTools.Method(typeof(CombatManager), "SwitchFromPlayerToEnemySide");

    private static MethodBase TargetMethod()
    {
        return AccessTools.AsyncMoveNext(SwitchFromPlayerToEnemySideMethod);
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ShareExtraTurn(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Match immediately before this.SwitchSides(turnState). This is after the lists of players taking an extra turn
        // are populated, but before anything is done with them, thus it's the perfect time to check and insert more
        // players into the lists if required. This method actually maintains two individual lists of players taking an
        // extra turn, turnstate.PlayersTakingExtraTurn and valueTupleList, so we must make sure to update both.
        codeMatcher.MatchStartForward(CodeMatch.Calls(AccessTools.Method(typeof(CombatManager), "SwitchSides")))
            .ThrowIfInvalid("Could not find call to this.SwitchSides")
            // At this point, the turnState field is already on top of the IL stack, so there is no need to load it
            // again. We only need to manually load the valueTupleList local variable onto the stack, which is index 2.
            .InsertAndAdvance(CodeInstruction.LoadLocal(2))
            // Note that ShareExtraTurnHelper returns the turnState, placing it back on top of the IL stack and
            // hence restoring it to the state it would have been in before this patch.
            .InsertAndAdvance(CodeInstruction.Call(() => ShareExtraTurnHelper(default, default)));

        return codeMatcher.Instructions();
    }

    public static CombatTurnState ShareExtraTurnHelper(CombatTurnState turnState,
        List<(Player, AbstractModel)> valueTupleList)
    {
        // Check all the players currently taking an extra turn, and get the first FairWindsPower found on one of them,
        // or null if none of them had that power.
        PowerModel? fairWindsPower = turnState.PlayersTakingExtraTurn.Select(p => p.Creature.GetPower<FairWindsPower>())
            .FirstOrDefault();
        // If this is not null, then a player taking an extra turn has the Fair Winds power, so add everyone who isn't
        // already taking an extra turn to the list of players taking an extra turn.
        if (fairWindsPower != null)
        {
            List<Player> playersNotAlreadyTakingExtraTurn =
                turnState.State.Players.Except(turnState.PlayersTakingExtraTurn).ToList();
            turnState.PlayersTakingExtraTurn.AddRange(playersNotAlreadyTakingExtraTurn);
            valueTupleList.AddRange(playersNotAlreadyTakingExtraTurn.Select(p => (p, (AbstractModel)fairWindsPower)));
        }

        return turnState;
    }
}