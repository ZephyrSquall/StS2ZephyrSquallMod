using MegaCrit.Sts2.Core.Commands.Builders;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IModifyAttackHitCountMultiplicative
{
    int ModifyAttackHitCountMultiplicative(AttackCommand attackCommand, int hitCount);
}