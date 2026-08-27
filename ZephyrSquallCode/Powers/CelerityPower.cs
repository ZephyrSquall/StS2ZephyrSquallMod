using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class CelerityPower : ZephyrSquallPower, IOnAddHoned
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ZephyrHoverTips.Honed(), HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public async Task OnAddHoned(CardModel card, int honedAmount, AbstractModel source)
    {
        if (card.Owner == Owner.Player && card.Type == CardType.Skill)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount * honedAmount, ValueProp.Unpowered, null);
        }
    }
}