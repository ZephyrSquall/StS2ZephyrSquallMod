using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class BreathingVeryDeeplyPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<WindBlast>(true)];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (Owner.Player == player)
        {
            List<WindBlast> windBlastList = new List<WindBlast>();
            for (int index = 0; index < Amount; ++index)
                windBlastList.Add(combatState.CreateCard<WindBlast>(Owner.Player));
            foreach (WindBlast card in windBlastList)
                CardCmd.Upgrade(card);
            await CardPileCmd.AddGeneratedCardsToCombat(windBlastList, PileType.Hand, true);
            await PowerCmd.Remove(this);
        }
    }
}