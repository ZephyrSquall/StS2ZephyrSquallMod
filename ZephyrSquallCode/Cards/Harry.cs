using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Harry() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => CanGainBlock;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("HitRequirement", 3), new BlockVar(8, ValueProp.Move), new DamageVar(4, ValueProp.Move),
        new HitCountVar(2), new CalculationBaseVar(0), new CalculationExtraVar(1),
        new CalculatedVar("TimesHit").WithMultiplier((card, _) =>
        {
            if (card.CombatState == null) throw new InvalidOperationException("card.CombatState is null");
            return ZephyrQueries.TimesHitAnyoneThisTurn(card.CombatState, card.Owner.Creature);
        })

    ];

    private bool CanGainBlock
    {
        get
        {
            if (CombatState == null) throw new InvalidOperationException("CombatState is null");
            return ZephyrQueries.TimesHitAnyoneThisTurn(CombatState, Owner.Creature) >= DynamicVars["HitRequirement"].IntValue;
        }
    }
        

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        if (CanGainBlock) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.HitCount.IntValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}