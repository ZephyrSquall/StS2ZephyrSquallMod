using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class LaserFocusPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _unhandledSkips = 0;

    public bool ShouldSkipIndividualDraw(Player player)
    {
        if (_unhandledSkips < Amount)
        {
            _unhandledSkips++;
            return true;
        }

        return false;
    }

    public async Task DecrementUnhandledSkips()
    {
        for (int i = 0; i < _unhandledSkips; i++)
        {
            Flash();
            await PowerCmd.Decrement(this);
        }

        _unhandledSkips = 0;
    }
}