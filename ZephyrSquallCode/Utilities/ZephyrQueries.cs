using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Cards;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Relics;

namespace ZephyrSquall.ZephyrSquallCode.Utilities;

public static class ZephyrQueries
{
    public static bool IsWellRead(Player player) =>
        PileType.Hand.GetPile(player).Cards.Count >= MaxHandSizePatch.GetMaxHandSize(player, CardPile.MaxCardsInHand) -
        player.Creature.GetPowerAmount<LightReadingPower>();

    public static int TimesHitAnyoneThisTurn(ICombatState combatState, Creature dealer) =>
        CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
            .Count(e => e.HappenedThisTurn(combatState) && e.Dealer == dealer && e.Result.Props.IsPoweredAttack());

    public static int TimesHitSpecificEnemyThisTurn(ICombatState combatState, Creature dealer, Creature target) =>
        CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
            .Count(e => e.HappenedThisTurn(combatState) && e.Receiver == target && e.Dealer == dealer &&
                        e.Result.Props.IsPoweredAttack());

    public static int AttacksPlayedThisTurn(ICombatState combatState, Creature creature) =>
        CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Count((Func<CardPlayStartedEntry, bool>)(e => e.HappenedThisTurn(combatState) &&
                                                           e.CardPlay.Card.Type == CardType.Attack &&
                                                           e.CardPlay.Card.Owner.Creature == creature));

    public static bool CanBeRecorded(CardModel card) =>
        card is not Book && (card.Owner.GetRelic<CursedTome>() != null || (card.Type != CardType.Status &&
                                                                           card.Type != CardType.Curse &&
                                                                           card.Type != CardType.Quest));
}