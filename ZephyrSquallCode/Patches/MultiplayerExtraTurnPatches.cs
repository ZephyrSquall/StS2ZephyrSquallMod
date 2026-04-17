using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch]
public class ShareExtraTurnPatch
{
    static Type _innerAsyncClass = AccessTools.FirstInner(typeof(CombatManager), t => t.Name.Contains("<SwitchFromPlayerToEnemySide>d__"));
    
    // The method I want to patch, "SwitchFromPlayerToEnemySide", is async. In IL, async methods are implemented with a
    // private inner class (in this case named "<SwitchFromPlayerToEnemySide>d__120") inside its "MoveNext" method.
    static MethodBase TargetMethod()
    {
        return AccessTools.Method(_innerAsyncClass, "MoveNext");
    }
    
    [HarmonyTranspiler]
     static IEnumerable<CodeInstruction> ShareExtraTurn(IEnumerable<CodeInstruction> instructions)
     {
         var codeMatcher = new CodeMatcher(instructions);
         
         codeMatcher.MatchEndForward(CodeMatch.Calls(() => default(List<Player>).Clear()))
             .ThrowIfInvalid("Could not find call to List.Clear")
             .Advance(-1);
         // Get a reference to the _playersTakingExtraTurn field.
         var playersTakingExtraTurnField  = codeMatcher.Operand;
         if (playersTakingExtraTurnField == null)
         {
             Log.Error("playersTakingExtraTurn is NULL");
         }

         codeMatcher.Advance(3);
         // Get a reference to the _state field
         var stateField = codeMatcher.Operand;
         if (stateField == null)
         {
             Log.Error("state is NULL");
         }

         codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(List<Player>).ToList<Player>()))
             .ThrowIfInvalid("Could not find call to List.ToList")
             .InsertAndAdvance(CodeInstruction.LoadLocal(1))
             .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, stateField))
             .InsertAndAdvance(CodeInstruction.Call(() => ShareExtraTurnHelper(default, default)));
         
         return codeMatcher.Instructions();
     }
    
    
    public static List<Player> ShareExtraTurnHelper(List<Player> playersTakingExtraTurn, CombatState state)
    {
        // If anyone who's taking an extra turn has the Fair Winds power, replace the current list of players taking an
        // extra turn with a new list that includes every player.
        if (playersTakingExtraTurn.Any(p => p.Creature.HasPower<FairWindsPower>()))
        {
            // To ensure that the _playersTakingExtraTurn field is modified, we must specifically use methods that
            // modify the list in-place.
            playersTakingExtraTurn.Clear();
            playersTakingExtraTurn.AddRange(state.Players);
        }
        
        return playersTakingExtraTurn;
    }
}
