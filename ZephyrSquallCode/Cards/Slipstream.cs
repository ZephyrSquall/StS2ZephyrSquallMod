using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Slipstream() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move), new CalculationBaseVar(0), new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier((Func<CardModel, Creature, decimal>)((card, target) =>
            card.CombatState.GetTeammatesOf(card.Owner.Creature)
                .Where(c => c != null && c.IsPlayer && c.Player != card.Owner)
                .Sum(c => ZephyrQueries.TimesDealtAttackDamageToSpecificEnemyThisTurn(card.CombatState, c,
                    target))))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target))
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}