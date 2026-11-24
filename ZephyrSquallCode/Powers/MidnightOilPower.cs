using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class MidnightOilPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override object InitInternalData() => new Data();

    public override bool ShouldPlayerResetEnergy(Player player)
    {
        return player != Owner.Player || Owner.Player?.PlayerCombatState?.TurnNumber == 1;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (Owner.Player != null && participants.Contains(Owner))
        {
            if (Owner.Player.PlayerCombatState == null) throw new InvalidOperationException("Owner.Player.PlayerCombatState is null");
            GetInternalData<Data>().leftoverEnergy = Owner.Player.PlayerCombatState.Energy;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player.Creature == Owner)
        {
            decimal newCount = count + Amount * GetInternalData<Data>().leftoverEnergy;
            GetInternalData<Data>().leftoverEnergy = 0;
            return newCount;
        }

        return count;
    }

    private class Data
    {
        public int leftoverEnergy = 0;
    }
}