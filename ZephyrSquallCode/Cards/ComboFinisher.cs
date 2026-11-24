using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class ComboFinisher() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [ZephyrCardTags.Combo];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HitCountVar(1), new CalculationBaseVar(6), new ExtraDamageVar(6),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, target) =>
        {
            if (target == null) return 0;
            if (card.CombatState == null) throw new InvalidOperationException("card.CombatState is null");
            return ZephyrQueries.TimesHitSpecificEnemyThisTurn(card.CombatState, card.Owner.Creature, target);
        })

    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .WithHitCount(DynamicVars.HitCount.IntValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
}