using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class EyesInTheSky() : ZephyrSquallCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EyesInTheSkyPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<EyesInTheSkyPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<EyesInTheSkyPower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}