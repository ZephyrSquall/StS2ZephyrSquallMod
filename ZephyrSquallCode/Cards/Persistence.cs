using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Persistence() : ZephyrSquallCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Honed", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Honed()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<PersistencePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        await HonedCmd.AddHoned(choiceContext, Owner, DynamicVars["Honed"].IntValue, 1, this);
    }

    protected override void OnUpgrade() => DynamicVars["Honed"].UpgradeValueBy(4);
}