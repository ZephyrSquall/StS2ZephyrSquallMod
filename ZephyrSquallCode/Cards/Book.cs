using System.Text;
using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using ZephyrSquall.ZephyrSquallCode.CardPiles;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Book() : CustomCardModel(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self), ICustomUiModel
{
    public List<CardModel> RecordedCards { get; private set; } = [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("CardTitles")];
    
    public static async Task<Book?> CreateInHand(
        Player owner,
        IEnumerable<CardModel> recordedCards,
        CombatState combatState)
    {
        Book book = combatState.CreateCard<Book>(owner);
        book.RecordedCards = recordedCards.ToList();
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

    public void CreateCustomUi(Control toAdd)
    {
        // Eventually, this method will show a preview for all Recorded cards, but only while hover tips are displaying.
        // For now, I'm having trouble getting the card preview to display without breaking, so I'm simplifying to just
        // showing the first  Recorded card above the Book at all times.
        var firstRecordedCard = RecordedCards.FirstOrDefault();
        if (firstRecordedCard != null)
        {
            Log.Warn("Book has recorded cards!");
            var nCard = NCard.Create(firstRecordedCard);

            if (nCard is { } nonNullNCard)
            {
                toAdd.AddChild(nCard);
                Log.Warn("Found an NCard for Book's first recorded card!");
                nCard.Visibility = ModelVisibility.Visible;
                nCard.Position = new Vector2(0f, -500f);
                nCard.UpdateVisuals(nCard.Model.Pile.Type, CardPreviewMode.Normal);
                nCard._Ready();
                
                // It seems the card preview never enters a "Ready" state, which UpdateVisuals explicitly checks for and
                // will do no work if it's not ready. This line helps me check if this may be the case.
                if (!nCard.IsNodeReady())
                    Log.Warn("Node is NOT ready!");
            }
        }
    }
}