using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Cards;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Potions;

public sealed class InkBottle : ZephyrSquallPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TailwindPower>(10M)];
    
    public override IEnumerable<IHoverTip> ExtraHoverTips => [
        ZephyrHoverTips.Record(),
        HoverTipFactory.FromCard<Book>()
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await RecordCmd.RecordAny(choiceContext, Owner, Owner.Creature.CombatState, this);
    }
}