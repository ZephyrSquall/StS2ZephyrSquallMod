using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Harry() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => CanGainBlock;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new DamageVar(4M, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("TimesDealtAttackDamage").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => ((Harry) card).TimesDealtAttackDamage))
    ];

    private bool CanGainBlock => TimesDealtAttackDamage >= 3;

    private int TimesDealtAttackDamage =>
        CombatManager.Instance.History.Entries.OfType<CreatureAttackedEntry>()
            .Sum(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner.Creature
                ? e.DamageResults.Count(d => d.Props.IsPoweredAttack_() && d.TotalDamage > 0)
                : 0);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CanGainBlock)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(2)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
        DynamicVars.Damage.UpgradeValueBy(1M);
    }
}