using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Character;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(ZephyrSquallCardPool))]
public class BurstOfSpeed() : ZephyrSquallCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    private bool _isExtraTurn;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TailwindPower>("TailwindPower", 4M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TailwindPower>()];

    protected override bool IsPlayable => !_isExtraTurn;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<TailwindPower>(Owner.Creature, DynamicVars["TailwindPower"].BaseValue, Owner.Creature, this);
    }

    public override Task AfterTakingExtraTurn(Player player)
    {
        _isExtraTurn = true;
        return Task.CompletedTask;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        _isExtraTurn = false;
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => DynamicVars["TailwindPower"].UpgradeValueBy(2M);
}