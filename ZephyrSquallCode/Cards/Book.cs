using System.Text;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.CardPiles;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Book() : ZephyrSquallCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    private bool _needsToCloneRecordedCards;

    public List<CardModel> RecordedCards { get; private set; } = [];

    private bool HasPaperCut => IsInCombat && Owner.HasPower<PaperCutPower>();

    public override CardType Type => HasPaperCut ? CardType.Attack : CardType.Skill;

    public override TargetType TargetType => HasPaperCut ? TargetType.AnyEnemy : TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CardTitles"), new DamageVar(0, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    public static async Task<Book?> CreateInHand(Player owner, IEnumerable<CardModel> recordedCards,
        ICombatState combatState)
    {
        var book = combatState.CreateCard<Book>(owner);
        book._needsToCloneRecordedCards = false;
        book.RecordedCards = recordedCards.ToList();

        // If a Book with no Recorded cards would be created, don't create the Book.
        if (book.RecordedCards.Count == 0) return null;

        // Set the Record Pile's BookPosition to where the Book will end up after it is generated. This is tricky, as
        // for Recording to work properly, the Recorded cards must be moved to the Record Pile before the Book is added
        // to combat (otherwise, if the player has a full hand when they're Recording (possible with Ink Bottle), the
        // generated Book would go straight to the Discard Pile due to the hand having no room). But BookPosition must
        // be known at the time the Recorded cards are moved for the tween to work properly. Thus, we must predict where
        // in the Hand the Book will end up by calculating what the Hand size will be after the Book is added to it, and
        // getting the coordinates of where the last card in a Hand that big would be.
        var cardsInHandAfterRecord = PileType.Hand.GetPile(owner).Cards.Count - book.RecordedCards.Count + 1;
        ((RecordPile)RecordPile.Record.GetPile(owner)).BookPosition =
            HandPosHelper.GetPosition(cardsInHandAfterRecord, cardsInHandAfterRecord - 1) +
            new Vector2(NGame.Instance.GetViewportRect().Size.X / 2, NGame.Instance.GetViewportRect().Size.Y);

        await CardPileCmd.Add(recordedCards, RecordPile.Record);
        await CardPileCmd.AddGeneratedCardToCombat(book, PileType.Hand, owner);

        book.UpdateCardTitles();

        // Update the card's damage and visuals if the player already has the Paper Cut power.
        book.DynamicVars.Damage.BaseValue = book.Owner.Creature.GetPowerAmount<PaperCutPower>();
        book.ResetFrame();

        return book;
    }

    public void CheckRecordedCards()
    {
        // At the instant CardRemoveFinished is invoked (which is when this is called), the card getting moved has been
        // removed from the Record Pile but not yet added to its new pile and thus its Pile is null. So it is critical
        // to also check if the Pile is null first, otherwise the game will crash.
        RecordedCards.RemoveAll(c => c.Pile is null || c.Pile.Type != RecordPile.Record);
        UpdateCardTitles();
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power is PaperCutPower && power.Owner.Player == Owner)
        {
            DynamicVars.Damage.BaseValue = power.Amount;
            ResetFrame();
        }

        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (HasPaperCut && play.Target is not null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

        // Update the Record Pile's BookPosition so the cards that are about to be returned to the hand appear to come
        // from this Book.
        var nCardBook = NCard.FindOnTable(this);
        if (nCardBook is not null)
            ((RecordPile)RecordPile.Record.GetPile(Owner)).BookPosition = nCardBook.GlobalPosition;

        var returnToHand = new List<CardModel>();
        var autoPlay = new List<CardModel>();
        foreach (var card in RecordedCards)
            if (card.Keywords.Contains(ZephyrKeywords.Narrate))
                autoPlay.Add(card);
            else
                returnToHand.Add(card);

        await CardPileCmd.Add(returnToHand, PileType.Hand);
        foreach (var card in autoPlay) await CardCmd.AutoPlay(choiceContext, card, null);
    }

    // The following code is a bandaid fix for an issue that causes any card beyond the first Recorded card to appear as
    // a "Broken Card" in the play pile after it is autoplayed. As best as I can tell, I've narrowed down the true issue
    // to being that the CardPileCmd.Add method only checks to see if a card is in a hardcoded list of piles before
    // updating the visuals. This ends up not working here case as the Narrated card is autoplayed from the Record pile,
    // a custom pile outside the hardcoded list. However, the CardPileCmd.Add is an extremely huge and convoluted
    // method, so patching it properly is going to be incredibly difficult, and I've left it for another day. The
    // bandaid fix below doesn't stop the card from momentarily appearing as a "Broken Card", but at least causes it to
    // eventually switch back to its proper appearance so it's easy to tell the right card was autoplayed.
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card,
        bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        var nCardNarrate = NCard.FindOnTable(card);
        if (nCardNarrate is not null) nCardNarrate.UpdateVisuals(nCardNarrate.Model.Pile.Type, CardPreviewMode.Normal);
        return (pileType, position);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        _needsToCloneRecordedCards = true;
    }

    // When a Book is generated, check if it was created via any of its factory methods like CreateInHand. If not, then
    // the Book was copied, which means all of its Recorded cards need to be copied too.
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card == this && _needsToCloneRecordedCards)
        {
            List<CardModel> newRecordedCards = [];
            foreach (var oldRecordedCard in RecordedCards)
            {
                var cardPileAddResult =
                    await CardPileCmd.AddGeneratedCardToCombat(oldRecordedCard.CreateClone(), RecordPile.Record, Owner);
                newRecordedCards.Add(cardPileAddResult.cardAdded);
            }

            RecordedCards = newRecordedCards;
        }
    }

    private void UpdateCardTitles()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < RecordedCards.Count; i++)
        {
            if (i > 0)
            {
                if (i == RecordedCards.Count - 1)
                    sb.Append(RecordedCards.Count == 2 ? " and " : ", and ");
                else
                    sb.Append(", ");
            }

            sb.Append($"[gold]{RecordedCards[i].Title}[/gold]");
        }

        ((StringVar)DynamicVars["CardTitles"]).StringValue = sb.ToString();
    }

    // When the card changes from a Skill to an Attack mid-combat, resetting its frame is required, or else its visuals
    // won't change, and it will continue displaying as a Skill until it changes piles.
    private void ResetFrame()
    {
        var nCard = NCard.FindOnTable(this);
        if (nCard is not null)
        {
            // The setter for NCard.Model calls NCard's private Reload function, which is what we want. However, the
            // setter returns immediately if the Model is assigned the value it already has, hence it must be assigned
            // null first.
            nCard.Model = null;
            nCard.Model = this;
        }
    }
}