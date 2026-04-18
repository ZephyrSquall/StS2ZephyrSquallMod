using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(StatusCardPool))]
public class LostInThought() : CustomCardModel(-1,
    CardType.Status, CardRarity.Status,
    TargetType.None)
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Retain];
}