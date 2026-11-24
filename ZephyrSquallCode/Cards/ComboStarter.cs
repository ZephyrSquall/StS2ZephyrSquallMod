using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class ComboStarter() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [ZephyrCardTags.Combo];

    // This should add an additional hit if no Attacks have started to be played this turn excluding this card, and this
    // card has never finished playing this turn.
    private bool ShouldAddAdditionalHit =>
        CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Count((Func<CardPlayStartedEntry, bool>)(e => e.HappenedThisTurn(CombatState) &&
                                                           e.CardPlay.Card.Type == CardType.Attack &&
                                                           e.CardPlay.Card.Owner == Owner && e.CardPlay.Card != this)) <
        1 && !CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>()
            .Any(e => e.HappenedThisTurn(CombatState) && e.CardPlay.Card == this);

    protected override bool ShouldGlowGoldInternal => ShouldAddAdditionalHit;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move), new HitCountVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.HitCount.IntValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        if (attack.ModelSource == this && ShouldAddAdditionalHit) return hitCount + 1;
        return hitCount;
    }

    protected override void OnUpgrade() => DynamicVars.HitCount.UpgradeValueBy(1);
}