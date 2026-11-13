using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using ZephyrSquall.ZephyrSquallCode.Hooks;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class FairWindsPower : ZephyrSquallPower, IAfterExtraTurnStart
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterExtraTurnStart(ICombatState combatState)
    {
        Flash();
        await PowerCmd.Decrement(this);
    }
}