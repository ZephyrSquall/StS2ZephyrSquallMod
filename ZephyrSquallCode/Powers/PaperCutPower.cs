using MegaCrit.Sts2.Core.Entities.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class PaperCutPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}