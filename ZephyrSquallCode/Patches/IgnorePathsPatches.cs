using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using ZephyrSquall.ZephyrSquallCode.Cards;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.ShouldAllowFreeTravel))]
public class IgnorePathWithEyesInTheSkyPatch
{
    [HarmonyPostfix]
    private static bool ShouldAllowFreeTravelPostfix(bool __result, IRunState runState)
    {
        return EyesInTheSkyTracker.EyesInTheSkyAmount[runState] > 0 || __result;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
public class DecrementEyesInTheSkyAmountPatch
{
    [HarmonyPostfix]
    private static void AfterRoomEnteredPostfix(ref Task __result, IRunState runState, AbstractRoom room)
    {
        __result = AsyncWrapper(__result, runState, room);
    }

    private static async Task AsyncWrapper(Task originalTask, IRunState runState, AbstractRoom room)
    {
        if (EyesInTheSkyTracker.EyesInTheSkyAmount[runState] > 0 && runState.CurrentRoomCount == 1)
            EyesInTheSkyTracker.EyesInTheSkyAmount[runState]--;

        // If this is a combat room, convert all of RunState's EyesInTheSkyAmount into the EyesInTheSky power, to help
        // give the player more visibility of how many times they can ignore paths as the RunState's amount is invisible
        // to the player. For multiplayer, ensure this only goes to one player to prevent multiplying the number of
        // remaining rooms; prioritize picking a player with Eyes in the Sky in their deck.
        if (EyesInTheSkyTracker.EyesInTheSkyAmount[runState] > 0 && room is CombatRoom)
        {
            Player? playerToReceiveEyesInTheSkyPower;
            List<Player> playersWithEyesInTheSky =
                runState.Players.Where(p => p.Deck.Cards.Any(c => c is EyesInTheSky)).ToList();
            if (playersWithEyesInTheSky.Count > 0)
                playerToReceiveEyesInTheSkyPower = runState.Rng.CombatTargets.NextItem(playersWithEyesInTheSky);
            else
                playerToReceiveEyesInTheSkyPower = runState.Rng.CombatTargets.NextItem(runState.Players);

            await PowerCmd.Apply<EyesInTheSkyPower>(new ThrowingPlayerChoiceContext(),
                playerToReceiveEyesInTheSkyPower.Creature, EyesInTheSkyTracker.EyesInTheSkyAmount[runState], null,
                null);
            EyesInTheSkyTracker.EyesInTheSkyAmount[runState] = 0;
        }

        await originalTask;
    }
}

public class EyesInTheSkyTracker
{
    public static readonly SavedSpireField<IRunState, int> EyesInTheSkyAmount =
        new(() => 1, "ZephyrSquallEyesInTheSkyAmount");
}