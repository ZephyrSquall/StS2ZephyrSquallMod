using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class TomeSmack() : ZephyrSquallCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7M, ValueProp.Move)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "RECORD.title"), new LocString("static_hover_tips", "RECORD.description")),
        HoverTipFactory.FromCard<Book>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (Func<CardModel, bool>) (c => c.Type != CardType.Status && c.Type != CardType.Curse && c is not Book), this);
        List<CardModel> recordedCards = selectedCards.ToList();
        await Book.CreateInHand(Owner, recordedCards, CombatState);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3M);
}