using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Hooks;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;


public class OverthinkIt() : ZephyrSquallCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self), IOnOverflow
{
    private bool _isPlaying;
    private CardPlay? _currentPlay;
    
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4M, ValueProp.Move), new CardsVar(6)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Overflow()];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        _isPlaying = true;
        _currentPlay = play;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        _currentPlay = null;
        _isPlaying = false;
    }

    public async Task OnOverflow(PlayerChoiceContext choiceContext, Player player, bool fromHandDraw)
    {
        if (_isPlaying && _currentPlay != null && player == Owner)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, _currentPlay);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(2M);
}