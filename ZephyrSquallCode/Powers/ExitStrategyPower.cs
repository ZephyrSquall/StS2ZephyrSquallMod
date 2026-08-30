using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class ExitStrategyPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => DynamicVars["RemainingHpLoss"].IntValue;

    // Initially set RemainingHpLoss to a ridiculously high value so that when SetRemainingHpLoss is first called, this
    // cannot be the smaller value.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("RemainingHpLoss", 99999)];

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        return target == Owner ? Math.Min(amount, DynamicVars["RemainingHpLoss"].BaseValue) : amount;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner)
        {
            DynamicVars["RemainingHpLoss"].BaseValue -= result.UnblockedDamage;
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner.Side != side) await PowerCmd.Remove(this);
    }

    // Set RemainingHpLoss to the smaller of the new value and its current value (this prevents raising RemainingHpLoss
    // if a second Exit Strategy is played in the same turn).
    public void SetRemainingHpLoss(int remainingHpLoss)
    {
        DynamicVars["RemainingHpLoss"].BaseValue = Math.Min(DynamicVars["RemainingHpLoss"].BaseValue, remainingHpLoss);
        InvokeDisplayAmountChanged();
    }
}