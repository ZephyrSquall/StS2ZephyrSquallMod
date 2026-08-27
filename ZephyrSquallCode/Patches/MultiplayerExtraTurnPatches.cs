using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch]
public class ShareExtraTurnPatch
{
    private static readonly Type _innerAsyncClass = AccessTools.FirstInner(typeof(CombatManager),
        t => t.Name.Contains("<SwitchFromPlayerToEnemySide>d__"));

    // In our code, we need a list of all players in the game so we can insert everyone into the list of players taking
    // an extra turn if needed. Unfortunately, from within CombatManager, this list of players exists within the
    // _turnState field, which is a private CombatTurnState. This means this patch can't reference a CombatTurnState
    // directly, hence we need to do a bunch of Harmony reflection shenanigans to get around the private access
    // modifier.
    private static readonly FieldInfo _turnStateField = AccessTools.Field(typeof(CombatManager), "_turnState");

    // The method I want to patch, "SwitchFromPlayerToEnemySide", is async. In IL, async methods are implemented with a
    // private inner class (in this case named "<SwitchFromPlayerToEnemySide>d__120") inside its "MoveNext" method.
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(_innerAsyncClass, "MoveNext");
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ShareExtraTurn(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // This matches the line `list = turnState.PlayersTakingExtraTurn.ToList<Player>();` which is immediately after
        // the original list of players is determined and stored in turnState.PlayersTakingExtraTurn. We insert our
        // helper function directly before this line to modify turnState.PlayersTakingExtraTurn so that it contains
        // all players if necessary. Note that in the IL, turnState.PlayersTakingExtraTurn (which is a List<Player>) is
        // on top of the stack at this point, making it easy to intercept.
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(List<Player>).ToList()))
            .ThrowIfInvalid("Could not find call to List.ToList")
            // Load the CombatManager instance onto the IL stack.
            .InsertAndAdvance(CodeInstruction.LoadLocal(1))
            // This function consumes both the turnState.PlayersTakingExtraTurn that was already on the IL stack and the
            // CombatManager instance we placed on the stack. This function returns the modified
            // turnState.PlayersTakingExtraTurn value, which gets placed on top of the stack, thus making sure it still
            // has the List<Player> value it expects for the rest of the SwitchFromPlayerToEnemySide function.
            .SetInstruction(CodeInstruction.Call(() => ShareExtraTurnHelper(default, default)));

        return codeMatcher.Instructions();
    }

    public static List<Player> ShareExtraTurnHelper(List<Player> playersTakingExtraTurn, CombatManager combatManager)
    {
        // If anyone who's taking an extra turn has the Fair Winds power, replace the current list of players taking an
        // extra turn with a new list that includes every player.
        if (playersTakingExtraTurn.Any(p => p.Creature.HasPower<FairWindsPower>()))
        {
            // Get the private CombatTurnState _turnState field as a plain object pointer.
            object turnStateObj = _turnStateField?.GetValue(combatManager);
            // Get the public CombatState class inside _turnState via its compiler backing field.
            FieldInfo stateBackingField = AccessTools.Field(turnStateObj.GetType(), "<State>k__BackingField");
            CombatState state = stateBackingField?.GetValue(turnStateObj) as CombatState;

            // To ensure that the _playersTakingExtraTurn field is modified, we must specifically use methods that
            // modify the list in-place.
            playersTakingExtraTurn.Clear();
            playersTakingExtraTurn.AddRange(state.Players);
        }

        return playersTakingExtraTurn;
    }
}