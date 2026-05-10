using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Cards;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Relics;

public class ResearchPaper : ZephyrSquallRelic, IOnRecord
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        IsMutable ? ZephyrHoverTips.Record(Owner) : ZephyrHoverTips.Record(),
        HoverTipFactory.FromCard<Book>()
    ];

    public Task OnRecord(IEnumerable<CardModel> cards, AbstractModel source)
    {
        var cardsUpgraded = 0;
        foreach (CardModel card in cards)
        {
            if (card.Owner == Owner && card.IsUpgradable)
            {
                CardCmd.Upgrade(card);
                cardsUpgraded++;
            }
        }

        if (cardsUpgraded > 0)
            Flash();
        return Task.CompletedTask;
    }
}