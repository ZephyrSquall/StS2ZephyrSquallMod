using System.Text;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.UI;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.CardPiles;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Book() : CustomCardModel(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    public List<CardModel> RecordedCards { get; private set; } = [];
    
    private bool HasPaperCut => IsInCombat && Owner.HasPower<PaperCutPower>();

    public override CardType Type => HasPaperCut ? CardType.Attack : CardType.Skill;
    
    public override TargetType TargetType => HasPaperCut ? TargetType.AnyEnemy : TargetType.Self;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("CardTitles"),
        new DamageVar(0, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    public static async Task<Book?> CreateInHand(
        Player owner,
        IEnumerable<CardModel> recordedCards,
        ICombatState combatState)
    {
        Book book = combatState.CreateCard<Book>(owner);
        book.RecordedCards = recordedCards.ToList();
        await CardPileCmd.AddGeneratedCardToCombat(book, PileType.Hand, owner);
        RecordPile.TargetBook = book;
        await CardPileCmd.Add(recordedCards, RecordPile.Record);

        book.UpdateCardTitles();
        
        // Update the card's damage and visuals if the player already has the Paper Cut power.
        book.DynamicVars.Damage.BaseValue = book.Owner.Creature.GetPowerAmount<PaperCutPower>();
        book.ResetFrame();
        
        return book;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is PaperCutPower && power.Owner.Player == Owner)
        {
            DynamicVars.Damage.BaseValue = power.Amount;
            ResetFrame();
        }
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HasPaperCut && play.Target is not null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        
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

    // When the card changes from a Skill to an Attack mid-combat, resetting its frame is required, or else its visuals
    // won't change, and it will continue displaying as a Skill until it changes piles.
    private void ResetFrame()
    {
        var nCard = NCard.FindOnTable(this);
        if (nCard is not null)
        {
            // The setter for NCard.Model calls NCard's private Reload function, which is what we want. However, the
            // setter won't be called if the Model is assigned the value it already has, hence it must be assigned null
            // first.
            nCard.Model = null;
            nCard.Model = this;
        }
    }
}