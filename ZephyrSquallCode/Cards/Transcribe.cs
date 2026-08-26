using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Transcribe() : ZephyrSquallCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsMutable ? ZephyrHoverTips.Record(Owner) : ZephyrHoverTips.Record(), HoverTipFactory.FromCard<Book>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        Book? book = await RecordCmd.RecordUpTo(choiceContext, Owner, DynamicVars.Cards.IntValue, CombatState, this);
        if (book != null)
            await CardPileCmd.AddGeneratedCardToCombat(book.CreateCloneForPlayer(play.Target.Player), PileType.Hand,
                Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}