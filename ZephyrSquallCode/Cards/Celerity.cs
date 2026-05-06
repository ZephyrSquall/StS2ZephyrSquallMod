using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Celerity() : ZephyrSquallCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Deft(), HoverTipFactory.Static(StaticHoverTip.Block)];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<CelerityPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}