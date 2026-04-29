using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Relics;

public class SpireEncyclopedia : ZephyrSquallRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Analysis>()];
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player == Owner && combatState.RoundNumber == 1)
        {
            List<CardModel> cards = new List<CardModel>();
            for (int index = 0; index < DynamicVars.Cards.IntValue; ++index)
                cards.Add( Owner.Creature.CombatState.CreateCard<Analysis>(Owner));
            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
        }
    }
}