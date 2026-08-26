using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Forum() : ZephyrSquallCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0), new CalculationExtraVar(1), new CalculatedVar("MostCards").WithMultiplier(
            (Func<CardModel, Creature, decimal>)((card, _) => card.CombatState.GetTeammatesOf(card.Owner.Creature)
                .Where(c => c != null && c.IsAlive && c.IsPlayer)
                .Max(c =>
                {
                    // If Forum's owner is the player with the most cards in Hand, the display amount becomes
                    // misleading, because Forum is in the Hand and counts itself when displaying how many cards are in
                    // the Hand with the most, but Forum is in the play pile and doesn't count itself when it is
                    // actually played and drawing cards. To account for this, check if Forum is in the owner's Hand,
                    // and if so, subtract 1 from its owner's Hand size.
                    var cardsInHand = c.Player.PlayerCombatState.Hand.Cards.Count;
                    if (card.Pile.Type == PileType.Hand && c.Player == card.Owner) cardsInHand -= 1;
                    return cardsInHand;
                })))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var mostCards = ((CalculatedVar)DynamicVars["MostCards"]).Calculate(play.Target);
        foreach (Creature creature in
                 CombatState.GetTeammatesOf(Owner.Creature).Where(c => c != null && c.IsAlive && c.IsPlayer))
            await CardPileCmd.DrawWithoutBlockingOnOtherPlayers(choiceContext,
                mostCards - creature.Player.PlayerCombatState.Hand.Cards.Count, creature.Player, this);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(1);
}