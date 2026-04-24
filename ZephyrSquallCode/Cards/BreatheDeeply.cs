using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class BreatheDeeply() : ZephyrSquallCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<WindBlast>(IsUpgraded)];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (IsUpgraded)
            await PowerCmd.Apply<BreathingVeryDeeplyPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
        else
            await PowerCmd.Apply<BreathingDeeplyPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
    }
}