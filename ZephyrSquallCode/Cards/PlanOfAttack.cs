using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Character;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(ZephyrSquallCardPool))]
public class PlanOfAttack() : ZephyrSquallCard(-1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("AdditionalDamage", 2M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
    
    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
         return Owner.Creature == dealer && props.IsPoweredAttack_() && Pile.Type == PileType.Hand ? DynamicVars["AdditionalDamage"].BaseValue : 0M;
    }
    
    protected override void OnUpgrade() => DynamicVars["AdditionalDamage"].UpgradeValueBy(1M);
}