using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Patches;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class ConnectTheDots : ZephyrSquallCard
{
    public ConnectTheDots() : base(1,
        CardType.Attack, CardRarity.Rare,
        TargetType.AnyEnemy)
    {
        CardModifierTracker.HonedAmount[this] = 4;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3M, ValueProp.Move)];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        var thisHoned = CardModifierTracker.HonedAmount[this];
        if (thisHoned > 0)
            await ModifierCmd.AddHoned(choiceContext, Owner, thisHoned, 1, this);
    }

    protected override void OnUpgrade() => CardModifierTracker.HonedAmount[this] += 2;
    
    protected override void AfterDowngraded() => CardModifierTracker.HonedAmount[this] -= 2;
}