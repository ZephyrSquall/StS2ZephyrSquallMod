using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class PaperCut() : ZephyrSquallCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PaperCutPower>(7),
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        IsMutable ? ZephyrHoverTips.Record(Owner) : ZephyrHoverTips.Record(),
        HoverTipFactory.FromCard<Book>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<PaperCutPower>(choiceContext, Owner.Creature, DynamicVars["PaperCutPower"].IntValue, Owner.Creature, this);
        await RecordCmd.Record(choiceContext, Owner, DynamicVars.Cards.IntValue, CombatState, this);
    }

    protected override void OnUpgrade() => DynamicVars["PaperCutPower"].UpgradeValueBy(3);
}