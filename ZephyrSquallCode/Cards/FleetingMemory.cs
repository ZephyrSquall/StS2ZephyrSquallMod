using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class FleetingMemory() : ZephyrSquallCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    private bool HasPlayedBookThisTurn() =>
        CombatManager.Instance.History.CardPlaysFinished.Any(e =>
            e.HappenedThisTurn(CombatState) && e.CardPlay.Player == Owner && e.CardPlay.Card is Book);

    protected override bool ShouldGlowGoldInternal => HasPlayedBookThisTurn();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromCard<Book>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation locationForCardPlay = base.GetResultLocationForCardPlay();
        if (!HasPlayedBookThisTurn()) locationForCardPlay.pileType = PileType.Exhaust;
        return locationForCardPlay;
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4);
}