using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using ZephyrSquall.ZephyrSquallCode.Patches;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Feint() : ZephyrSquallCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("HonedThreshold", 3), new CalculationBaseVar(1), new CalculationExtraVar(1),
        new CalculatedVar("CalculatedVulnerable").WithMultiplier((card, _) =>
            Math.Floor(CardModifierTracker.HonedAmount[card] / card.DynamicVars["HonedThreshold"].BaseValue))
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(), ZephyrHoverTips.Honed()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target,
            ((CalculatedVar)DynamicVars["CalculatedVulnerable"]).Calculate(play.Target), Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(1);
}