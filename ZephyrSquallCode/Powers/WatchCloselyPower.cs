using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class WatchCloselyPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult _,
        ValueProp props,
        Creature? dealer,
        CardModel? __)
    {
        if (target == Owner && props.IsPoweredAttack())
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side)
            await PowerCmd.Remove(this);
    }
}