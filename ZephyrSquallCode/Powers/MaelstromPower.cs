using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class MaelstromPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterDamageGiven(
        PlayerChoiceContext context,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack_() && result.TotalDamage > 0)
        {
            Flash();
            await Cmd.Wait(0.2f);
            await CreatureCmd.Damage(context, CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
        }
    }
}