using BaseLib.Extensions;
using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Cards;
using ZephyrSquall.ZephyrSquallCode.Powers;
using ZephyrSquall.ZephyrSquallCode.Relics;

namespace ZephyrSquall.ZephyrSquallCode.Utilities;

public static class ZephyrQueries
{
    public static bool IsWellRead(Player player)
    {
        return PileType.Hand.GetPile(player).Cards.Count >= MaxHandSizePatch.GetMaxHandSize(player) -
            player.Creature.GetPowerAmount<LightReadingPower>();
    }

    public static int TimesDealtAttackDamageThisTurn(ICombatState combatState, Creature dealer)
    {
        return CombatManager.Instance.History.Entries.OfType<CreatureAttackedEntry>()
            .Sum(e => e.HappenedThisTurn(combatState) && e.Actor == dealer
                ? e.DamageResults.Count(d => d.Props.IsPoweredAttack_() && d.TotalDamage > 0)
                : 0);
    }

    public static int TimesDealtAttackDamageToSpecificEnemyThisTurn(ICombatState combatState, Creature dealer,
        Creature target)
    {
        return CombatManager.Instance.History.Entries.OfType<CreatureAttackedEntry>()
            .Sum(e => e.HappenedThisTurn(combatState) && e.Actor == dealer
                ? e.DamageResults.Count(d => d.Props.IsPoweredAttack_() && d.TotalDamage > 0 && d.Receiver == target)
                : 0);
    }

    public static int AttacksPlayedThisTurn(ICombatState combatState, Creature creature)
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Count((Func<CardPlayStartedEntry, bool>)(e =>
                e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.Owner.Creature == creature &&
                e.HappenedThisTurn(combatState)));
    }

    public static bool CanBeRecorded(CardModel card)
    {
        return card is not Book && (card.Owner.GetRelic<CursedTome>() != null || (card.Type != CardType.Status &&
            card.Type != CardType.Curse && card.Type != CardType.Quest));
    }
}