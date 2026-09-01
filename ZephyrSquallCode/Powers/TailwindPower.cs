using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class TailwindPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return Amount >= 10 && Owner.Player == player;
    }

    public override async Task AfterTakingExtraTurn(Player player)
    {
        if (Amount >= 10 && Owner.Player == player)
        {
            Flash();
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, -10, null, null);
        }
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power != this) return Task.CompletedTask;

        if (Amount >= 10)
            StartPulsing();
        else
            StopPulsing();

        return Task.CompletedTask;
    }
}