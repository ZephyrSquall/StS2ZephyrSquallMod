using System.Text;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using ZephyrSquall.ZephyrSquallCode.CardPiles;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Book() : CustomCardModel(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    public List<CardModel> RecordedCards { get; private set; } = [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("CardTitles")];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => RecordedCards.Select(card => HoverTipFactory.FromCard(card));
    
    public static async Task<Book?> CreateInHand(
        Player owner,
        List<CardModel> recordedCards,
        CombatState combatState)
    {
        Book book = combatState.CreateCard<Book>(owner);
        book.RecordedCards = recordedCards;
        await CardPileCmd.AddGeneratedCardToCombat(book, PileType.Hand, true);
        RecordPile.TargetBook = book;
        await CardPileCmd.Add(recordedCards, RecordPile.Record);

        book.UpdateCardTitles();
        
        return book;
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        RecordPile.TargetBook = this;
        await CardPileCmd.Add(RecordedCards, PileType.Hand);
        RecordedCards.Clear();
        UpdateCardTitles();
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);

    private void UpdateCardTitles()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < RecordedCards.Count; i++)
        {
            if (i > 0)
            {
                if (i == RecordedCards.Count - 1)
                {
                    sb.Append(RecordedCards.Count == 2 ? " and " : ", and ");
                }
                else
                {
                    sb.Append(", ");
                }
            }
            sb.Append($"[gold]{RecordedCards[i].Title}[/gold]");
        }
        ((StringVar) DynamicVars["CardTitles"]).StringValue = sb.ToString();
    }
}