using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ZephyrSquall.ZephyrSquallCode.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class BreathingDeeplyPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<WindBlast>()];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner.Player == player)
        {
            List<WindBlast> windBlastList = new List<WindBlast>();
            for (int index = 0; index < Amount; ++index)
                windBlastList.Add(combatState.CreateCard<WindBlast>(Owner.Player));
            await CardPileCmd.AddGeneratedCardsToCombat(windBlastList, PileType.Hand, Owner.Player);
            await PowerCmd.Remove(this);
        }
    }
}