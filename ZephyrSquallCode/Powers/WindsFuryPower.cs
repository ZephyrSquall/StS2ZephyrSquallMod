using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class WindsFuryPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        if (attack.Attacker == Owner && CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Count((Func<CardPlayStartedEntry, bool>) (e => e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.Owner.Creature == Owner && e.HappenedThisTurn(CombatState))) <= 1)
        {
            Flash();
            return hitCount + Amount;
        }
        return hitCount;
    }
}