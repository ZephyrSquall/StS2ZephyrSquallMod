using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
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
        if (Amount >= 10 && Owner.Player == player) {
            this.Flash();
            await PowerCmd.ModifyAmount(this, -10M, (Creature?)null, (CardModel?)null);
        }
    }
}