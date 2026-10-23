using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using ZephyrSquall.ZephyrSquallCode.Patches;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class EyesInTheSkyPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        IRunState runState = Owner.Player.RunState;
        EyesInTheSkyTracker.EyesInTheSkyAmount[runState] += Amount;
        return Task.CompletedTask;
    }
}