using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Plagiarize() : ZephyrSquallCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips = [HoverTipFactory.FromCard<Book>()];
            if (IsUpgraded)
                hoverTips.Add(ZephyrHoverTips.Record());
            return hoverTips;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (IsUpgraded)
            await RecordCmd.Record(choiceContext, Owner, 1, CombatState, this);
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        CardModel? book = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (Func<CardModel, bool>) (c => c is Book ), this)).FirstOrDefault();
        if (book != null)
            await CardPileCmd.AddGeneratedCardToCombat(book.CreateClone(), PileType.Hand, Owner);
    }
}