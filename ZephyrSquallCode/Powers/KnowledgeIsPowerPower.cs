using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class KnowledgeIsPowerPower : ZephyrSquallPower, IOnOverflow
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Overflow()];
    
    public async Task OnOverflow(PlayerChoiceContext choiceContext, Player player, bool fromHandDraw)
    {
        if (player == Owner.Player)
        {
            IReadOnlyList<Creature> hittableEnemies = CombatState.HittableEnemies;
            if (hittableEnemies.Count == 0)
                return;
            Creature target = Owner.Player.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
            Flash();
            await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, Owner);
        }

    }
}