using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Potions;

public sealed class TechniquePotion : ZephyrSquallPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Honed", 5)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Honed()];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await HonedCmd.AddHoned(choiceContext, Owner, DynamicVars["Honed"].IntValue, 1, this);
    }
}