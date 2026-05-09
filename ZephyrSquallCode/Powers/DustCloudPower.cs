using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class DustCloudPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    
    protected override object InitInternalData() => new Data();
    
    private class Data
    {
        public CardModel? SelectedCard;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Card")];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Deft()];
    
    public void SetSelectedCard(CardModel card)
    {
        GetInternalData<Data>().SelectedCard = card;
        ((StringVar) DynamicVars["Card"]).StringValue = card.Title;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        var selectedCard = GetInternalData<Data>().SelectedCard;
        if (selectedCard != null && dealer == Owner && props.IsPoweredAttack() && result.TotalDamage > 0)
        {
            Flash();
            await ModifierCmd.AddDeftToSpecific(selectedCard, Amount, this);
        }
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
            await PowerCmd.Remove(this);
    }
}