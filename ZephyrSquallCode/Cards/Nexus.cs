using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Nexus() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private int _combatsRemaining = 3;

    [SavedProperty]
    public int CombatsRemaining
    {
        get => _combatsRemaining;
        set
        {
            _combatsRemaining = value;
            DynamicVars["CombatsRemaining"].BaseValue = _combatsRemaining;
        }
    }

    // This must be saved too, otherwise saving and quitting after spawning the card reward but before moving to the
    // next room will cause the extra Nexus to disappear.
    [SavedProperty] public bool ShouldAddNexusToCardRewards { get; set; } = false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move), new("CombatsRemaining", 3), new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedHits").WithMultiplier((Func<CardModel, Creature, Decimal>)((card, _) =>
            card.Owner.PlayerCombatState.AllCards.OfType<Nexus>().Count()))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target))
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);

    public override Task AfterCombatEnd(CombatRoom combatRoom)
    {
        if (Pile.Type == PileType.Deck && CombatsRemaining > 0)
        {
            CombatsRemaining--;
            if (CombatsRemaining == 0) ShouldAddNexusToCardRewards = true;
        }

        return Task.CompletedTask;
    }

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        ShouldAddNexusToCardRewards = false;
        return Task.CompletedTask;
    }

    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> rewardOptions,
        CardCreationOptions creationOptions)
    {
        if (ShouldAddNexusToCardRewards && Owner == player && creationOptions.Source == CardCreationSource.Encounter &&
            creationOptions.Flags.HasFlag(CardCreationFlags.IsCardReward) &&
            creationOptions.Flags.HasFlag(CardCreationFlags.IsFromCombat))
        {
            CardCreationResult cardCreationResult = new CardCreationResult(player.RunState.CreateCard<Nexus>(player));
            rewardOptions.Add(cardCreationResult);
            return true;
        }

        return false;
    }
}