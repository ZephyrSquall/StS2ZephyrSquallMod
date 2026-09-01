using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class HeadwindPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer == Owner && target == Applier && props.IsPoweredAttack() ? -Amount : 0;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            // This power specifies that it halves the current Amount. Slay the Spire always rounds down, which means
            // the current Amount should be rounded down after being halved. Therefore, the Amount to be removed needs
            // to be rounded up. (Rounding like this is essential to make sure Amount eventually reaches 0 and the power
            // is removed if the player stops applying Headwind.)
            var headwindOffset = -Math.Ceiling(Amount / 2M);
            await PowerCmd.ModifyAmount(choiceContext, this, headwindOffset, null, null);
        }
    }
}