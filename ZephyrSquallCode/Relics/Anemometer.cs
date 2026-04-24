using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Relics;

public class Anemometer : ZephyrSquallRelic
{
    private int _tailwindStored;
    
    [SavedProperty]
    public int TailwindStored
    {
        get => _tailwindStored;
        set
        {
            _tailwindStored = value;
            InvokeDisplayAmountChanged();
        }
    }
    
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override bool ShowCounter => !CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => TailwindStored;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TailwindPower>()];
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom && TailwindStored > 0)
        {
            Flash();
            await PowerCmd.Apply<TailwindPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, TailwindStored, Owner.Creature, null);
            TailwindStored = 0;
        }
    }
    
    public override Task BeforeCombatStart()
    {
        // Without calling this here, the counter will not be updated after combat starts and therefore remain visible.
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    
    public override Task AfterCombatEnd(CombatRoom combatRoom)
    {
        TailwindStored = Owner.Creature.GetPowerAmount<TailwindPower>();
        return Task.CompletedTask;
    }
}