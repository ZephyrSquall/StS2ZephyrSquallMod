using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;


public class FlowState() : ZephyrSquallCard(3,
        CardType.Attack, CardRarity.Rare,
        TargetType.AllEnemies), IOnBecomeWellRead
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(17M, ValueProp.Move)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.WellRead()];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    public async Task OnBecomeWellRead(Player player)
    {
        await HandleAutoplay(player);
    }

    // In case the player became Well Read during the enemy turn, do an extra check as soon as possible upon the player
    // turn starting.
    public override async Task AfterEnergyReset(Player player)
    {
        await HandleAutoplay(player);
    }

    private async Task HandleAutoplay(Player player)
    {
        if (player == Owner && Pile.Type == PileType.Hand && Owner.Creature.CombatState.CurrentSide == CombatSide.Player)
        {
            var choiceContext = new BlockingPlayerChoiceContext();
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5M);
}