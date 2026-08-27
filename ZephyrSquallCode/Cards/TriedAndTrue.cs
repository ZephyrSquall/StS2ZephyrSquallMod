using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Patches;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class TriedAndTrue() : ZephyrSquallCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(5), new IntVar("Honed", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Honed()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await HonedCmd.AddHoned(choiceContext, Owner, DynamicVars["Honed"].IntValue, 1, this, PileType.Draw);
        await CardCmd.Discard(choiceContext,
            (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)).Where(c =>
                CardModifierTracker.HonedAmount[c] == 0));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["Honed"].UpgradeValueBy(1);
    }
}