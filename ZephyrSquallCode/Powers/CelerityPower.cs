using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class CelerityPower : ZephyrSquallPower, IOnAddDeft
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ZephyrHoverTips.Deft(), HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public async Task OnAddDeft(CardModel card, int honedAmount, AbstractModel source)
    {
        if (card.Owner == Owner.Player)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount * honedAmount, ValueProp.Unpowered, null);
        }
    }
}