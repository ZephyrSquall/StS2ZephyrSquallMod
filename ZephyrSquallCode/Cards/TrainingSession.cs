using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class TrainingSession() : ZephyrSquallCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Train>(IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null) throw new InvalidOperationException("CombatState is null");
        foreach (Creature creature in CombatState.GetTeammatesOf(Owner.Creature)
                     .Where(c => c.IsAlive && c.IsPlayer))
        {
            if (creature.Player == null) throw new InvalidOperationException("creature.Player is null");
            CardModel train = CombatState.CreateCard<Train>(creature.Player);
            if (IsUpgraded) CardCmd.Upgrade(train);
            await CardPileCmd.AddGeneratedCardToCombat(train, PileType.Hand, Owner);
        }
    }
}