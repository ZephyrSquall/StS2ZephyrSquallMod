using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Maelstrom() : ZephyrSquallCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MaelstromPower>(1M)];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<MaelstromPower>(choiceContext, Owner.Creature, DynamicVars["MaelstromPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["MaelstromPower"].UpgradeValueBy(1M);
}