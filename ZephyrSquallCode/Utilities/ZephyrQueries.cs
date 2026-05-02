using BaseLib.Extensions;
using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Utilities;

public static class ZephyrQueries
{
    public static bool IsWellRead(Player player)
    {
        return PileType.Hand.GetPile(player).Cards.Count >= MaxHandSizePatch.GetMaxHandSize(player) - player.Creature.GetPowerAmount<LightReadingPower>();
    }
    
    public static int TimesDealtAttackDamageThisTurn(ICombatState combatState, Creature creature)
    {
        return CombatManager.Instance.History.Entries.OfType<CreatureAttackedEntry>()
            .Sum(e => e.HappenedThisTurn(combatState) && e.Actor == creature
                ? e.DamageResults.Count(d => d.Props.IsPoweredAttack_() && d.TotalDamage > 0)
                : 0);
    }
}