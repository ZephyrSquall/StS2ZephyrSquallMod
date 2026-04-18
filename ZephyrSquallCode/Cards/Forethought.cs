using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Character;
using ZephyrSquall.ZephyrSquallCode.Patches;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(ZephyrSquallCardPool))]
public class Forethought() : ZephyrSquallCard(2,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7M, ValueProp.Move), new IntVar("Deft", 4M)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Deft()];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        CardModel? selectedCard = (await CardSelectCmd.FromSimpleGrid(choiceContext,
            PileType.Draw.GetPile(Owner).Cards.Where(c => c.GainsBlock).OrderBy(c => c.Rarity).ThenBy(c => c.Id).ToList(), Owner, prefs)).FirstOrDefault();
        if (selectedCard != null)
        {
            CardModifierTracker.DeftAmount[selectedCard] += DynamicVars["Deft"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
        DynamicVars["Deft"].UpgradeValueBy(1M);
    }
}