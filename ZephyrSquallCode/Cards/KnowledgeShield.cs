using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Patches;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class KnowledgeShield() : ZephyrSquallCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => ZephyrQueries.IsWellRead(Owner);
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7M, ValueProp.Move),
        new IntVar("Deft", 3M),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> extraHoverTips = [ZephyrHoverTips.WellRead()];
            if (CardModifierTracker.DeftAmount[this] == 0)
                extraHoverTips.Add(ZephyrHoverTips.Deft());
            return extraHoverTips;
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (WellReadTracker.WasWellReadAtStartOfCardPlay)
        {
            CardModifierTracker.DeftAmount[this] += DynamicVars["Deft"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
        DynamicVars["Deft"].UpgradeValueBy(1M);
    }
}