using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class OneWithTheWindPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TailwindPower>()];

    public override decimal ModifyPowerAmountGiven(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        // Technically, this increases Tailwind when applied by the owner, not when the owner gains it. This is
        // necessary so cards can recognize when the owner has this power and update their displayed numbers
        // accordingly, as cards don't know who their target is while in hand. Zephyr has no cards that apply Tailwind
        // to another player, so it would take an extreme edge case for this to matter. (Only situation I can think of
        // is if another card copies buffs that a player already has,  which I believe no card in the base game or
        // Zephyr's card pool can do.)
        return (power is TailwindPower) && giver == Owner ? amount + Amount : amount;
    }

    public override Task AfterModifyingPowerAmountGiven(PowerModel power)
    {
        Flash();
        return Task.CompletedTask;
    }
}