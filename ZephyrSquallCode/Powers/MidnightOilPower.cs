using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class MidnightOilPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlayerResetEnergy(Player player)
    {
        return player != Owner.Player || Owner.Player?.PlayerCombatState?.TurnNumber == 1;
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player == Owner.Player ? Math.Max(0, count - 5 + Owner.Player.PlayerCombatState.Energy) : count;
    }
}