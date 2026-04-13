using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Character;
using ZephyrSquall.ZephyrSquallCode.Commands;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(ZephyrSquallCardPool))]
public class Whet() : ZephyrSquallCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Honed", 2M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [new HoverTip(new LocString("static_hover_tips", "HONED_STATIC.title"), new LocString("static_hover_tips", "HONED_STATIC.description"))];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await ModifierCmd.AddHoned(choiceContext, Owner, DynamicVars["Honed"].IntValue, 1, this);
    }

    protected override void OnUpgrade() => DynamicVars["Honed"].UpgradeValueBy(1M);
}