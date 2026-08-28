using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Hyperfixation() : ZephyrSquallCard(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("UpgradeCards", 2), new IntVar("DowngradeCards", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        (await PowerCmd.Apply<HyperfixationPower>(choiceContext, Owner.Creature, DynamicVars["UpgradeCards"].IntValue,
            Owner.Creature, this)).AddDowngradeCards(DynamicVars["DowngradeCards"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["UpgradeCards"].UpgradeValueBy(1);
    }
}