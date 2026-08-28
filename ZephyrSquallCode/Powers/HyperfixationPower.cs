using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class HyperfixationPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Amount represents number of cards to Upgrade, so only the number of cards to be Downgraded needs to be tracked
    // separately.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("DowngradeCards", 0)];

    public override Task AfterCombatEnd(CombatRoom room)
    {
        List<CardModel> cardsToPreview = [];

        var upgradedCards = Owner.Player.Deck.Cards.Where(c => c.IsUpgraded).ToList();
        for (var i = 0; i < DynamicVars["DowngradeCards"].BaseValue && upgradedCards.Count > 0; i++)
        {
            var card = Owner.Player.RunState.Rng.Niche.NextItem(upgradedCards);
            upgradedCards.Remove(card);
            CardCmd.Downgrade(card);
            cardsToPreview.Add(card);
        }

        // Exclude cards currently in cardsToPreview List so we don't Upgrade a card that was just Downgraded.
        var upgradableCards =
            Owner.Player.Deck.Cards.Where(c => c.IsUpgradable && !cardsToPreview.Contains(c)).ToList();
        for (var i = 0; i < Amount && upgradableCards.Count > 0; i++)
        {
            var card = Owner.Player.RunState.Rng.Niche.NextItem(upgradableCards);
            upgradableCards.Remove(card);
            CardCmd.Upgrade(card, CardPreviewStyle.None);
            cardsToPreview.Add(card);
        }

        CardCmd.Preview(cardsToPreview, 2.0f);
        return Task.CompletedTask;
    }

    public void AddDowngradeCards(int downgradeCards)
    {
        DynamicVars["DowngradeCards"].BaseValue += downgradeCards;
    }
}