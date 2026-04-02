using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class FairWindsPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterTakingExtraTurn(Player player)
    {
        if (Owner.Player == player)
        {
            Flash();
            await PowerCmd.Decrement(this);
        }
    }
}