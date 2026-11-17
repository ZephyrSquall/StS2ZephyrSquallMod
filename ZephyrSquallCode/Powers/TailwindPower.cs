using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class TailwindPower : ZephyrSquallPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return Amount >= 10 && Owner.Player == player;
    }

    public override async Task AfterTakingExtraTurn(Player player)
    {
        if (Owner.Player == player)
        {
            Flash();
            Color color = new Color("FFFFFF80");
            double duration = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 1.3 : 2.0;
            NCombatRoom? instance = NCombatRoom.Instance;
            if (instance != null)
                instance.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, duration));
            SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, -10, null, null);
        }
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power != this) return Task.CompletedTask;

        if (Amount >= 10)
            StartPulsing();
        else
            StopPulsing();

        return Task.CompletedTask;
    }
}