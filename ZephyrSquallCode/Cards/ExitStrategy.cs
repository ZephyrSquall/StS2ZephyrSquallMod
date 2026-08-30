using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class ExitStrategy() : ZephyrSquallCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ExitStrategyPower>(10)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (Owner.Creature.GetPower<ExitStrategyPower>() is ExitStrategyPower existingExitStrategyPower)
        {
            existingExitStrategyPower.SetRemainingHpLoss(DynamicVars["ExitStrategyPower"].IntValue);
        }
        else
        {
            ExitStrategyPower newExitStrategyPower = (ExitStrategyPower)ModelDb.Power<ExitStrategyPower>().ToMutable();
            // Set remainingHpLoss before applying the power to prevent a visual glitch where the power amount briefly
            // displays as its default value immediately after it is applied.
            newExitStrategyPower.SetRemainingHpLoss(DynamicVars["ExitStrategyPower"].IntValue);
            await PowerCmd.Apply(choiceContext, newExitStrategyPower, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["ExitStrategyPower"].UpgradeValueBy(-3);
}