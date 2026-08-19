using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Patches;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Culmination() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Honed()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        // Add the value of all Honed on all cards, except Honed from this card itself as this is already added by the
        // Honed patch.
        return this == cardSource
            ? Owner.PlayerCombatState.AllCards.Sum(c => c == cardSource ? 0 : CardModifierTracker.HonedAmount[c])
            : 0;
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}