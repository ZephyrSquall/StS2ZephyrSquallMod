using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class SiftThrough() : ZephyrSquallCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(6), new IntVar("KeptCards", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        List<CardModel> drawnCards =
            (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)).ToList();
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars["KeptCards"].IntValue);
        var keptCards = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, drawnCards.Contains, this)).ToList();
        drawnCards.RemoveAll(keptCards.Contains);
        await CardCmd.Discard(choiceContext, drawnCards);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}