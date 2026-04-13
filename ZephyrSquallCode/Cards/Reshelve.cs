using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Reshelve() : ZephyrSquallCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(0)];
    
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cardsOriginallyInDrawPile = PileType.Draw.GetPile(Owner).Cards.ToList();
        var cardsOriginallyInDiscardPile = PileType.Discard.GetPile(Owner).Cards.ToList();
        
        // Without a very brief wait, the visual for card models moving between card piles glitches out and shows the
        // card coming from the top-left corner of the screen for a frame before snapping back to its intended position.
        await Cmd.Wait(0.05f);
        
        await CardPileCmd.Add(cardsOriginallyInDrawPile, PileType.Discard);
        // The "Shuffle" is performed by putting each card into the Draw Pile in a random position.
        await CardPileCmd.Add(cardsOriginallyInDiscardPile, PileType.Draw, CardPilePosition.Random);
        
        // Handle hooks that are normally called during the process of shuffling the deck.
        var initialDrawPileOrder = PileType.Draw.GetPile(Owner).Cards.ToList();
        Hook.ModifyShuffleOrder(Owner.Creature.CombatState, Owner, initialDrawPileOrder, false);
        await CardPileCmd.Add(initialDrawPileOrder, PileType.Draw, CardPilePosition.Bottom, null, true);
        
        await Hook.AfterShuffle(Owner.Creature.CombatState, choiceContext, Owner);
        
        if (IsUpgraded)
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1M);
}