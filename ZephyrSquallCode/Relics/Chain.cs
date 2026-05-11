using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Relics;

public class Chain : ZephyrSquallRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        if (attack.Attacker == Owner.Creature && attack.ModelSource is CardModel cardSource &&
            cardSource.Tags.Contains(ZephyrCardTags.Combo))
        {
            Flash();
            return hitCount + 1;
        }

        return hitCount;
    }
}