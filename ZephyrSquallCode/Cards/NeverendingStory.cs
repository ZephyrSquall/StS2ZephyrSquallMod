using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class NeverendingStory() : ZephyrSquallCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4), new IntVar("RecordCards", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsMutable ? ZephyrHoverTips.Record(Owner) : ZephyrHoverTips.Record(), HoverTipFactory.FromCard<Book>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [ZephyrKeywords.Narrate];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        if (CombatState == null) throw new InvalidOperationException("CombatState is null");
        List<CardModel> cardsToRecord = [this];
        CardSelectorPrefs prefs = new(RecordCmd.RecordSelectionPrompt, DynamicVars["RecordCards"].IntValue);
        cardsToRecord.AddRange(await CardSelectCmd.FromHand(choiceContext, Owner, prefs, ZephyrQueries.CanBeRecorded,
            this));
        await Book.CreateInHand(Owner, cardsToRecord, CombatState);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}