using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Character;
using ZephyrSquall.ZephyrSquallCode.Patches;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(ZephyrSquallCardPool))]
public class Whet() : ZephyrSquallCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Sharp", 2M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [new HoverTip(new LocString("static_hover_tips", "SHARP_STATIC.title"), (new LocString("static_hover_tips", "SHARP_STATIC.description")))];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, 1);
        CardModel? selectedCard = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (Func<CardModel, bool>) (c => c.Type == CardType.Attack), (AbstractModel) this)).FirstOrDefault<CardModel>();
        if (selectedCard != null)
        {
            SharpTracker.SharpAmount[selectedCard] += DynamicVars["Sharp"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Sharp"].UpgradeValueBy(1M);
}