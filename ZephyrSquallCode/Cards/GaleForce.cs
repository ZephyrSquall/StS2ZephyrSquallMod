using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Patches;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class GaleForce() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    // Like Bombardment, this uses Early to prevent this card playing multiple times if another auto-pre-play effect
    // Exhausts it.
    public override async Task AfterAutoPrePlayPhaseEnteredEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner && Pile?.Type == PileType.Exhaust &&
            ExtraTurnTracker.PlayersTakingExtraTurn.Contains(player))
            await CardCmd.AutoPlay(choiceContext, this, null);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}