using MegaCrit.Sts2.Core.Combat;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IAfterExtraTurnStart
{
    Task AfterExtraTurnStart(ICombatState combatState);
}