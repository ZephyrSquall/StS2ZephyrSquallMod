using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using ZephyrSquall.ZephyrSquallCode.Powers;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class StormBrewing() : ZephyrSquallCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    private int _currentTailwind = 1;
    private int _increasedTailwind;
    
    [SavedProperty]
    public int CurrentTailwind
    {
        get => _currentTailwind;
        set
        {
            _currentTailwind = value;
            DynamicVars["TailwindPower"].BaseValue = _currentTailwind;
        }
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<TailwindPower>("TailwindPower", CurrentTailwind),
        new ("Increase", 1),
        new ("Limit", 9)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TailwindPower>()];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get => [CardKeyword.Exhaust];
    }
    
    [SavedProperty]
    public int IncreasedTailwind
    {
        get => _increasedTailwind;
        set
        {
            AssertMutable();
            _increasedTailwind = value;
        }
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<TailwindPower>(choiceContext, Owner.Creature, DynamicVars["TailwindPower"].BaseValue, Owner.Creature, this);
        int extraTailwind = DynamicVars["Increase"].IntValue;
        if (IsUpgraded || CurrentTailwind + extraTailwind <= DynamicVars["Limit"].IntValue)
        {
            BuffFromPlay(extraTailwind);
            if (!(DeckVersion is StormBrewing deckVersion))
                return;
            deckVersion.BuffFromPlay(extraTailwind);
        }
    }

    protected override void AfterDowngraded() => UpdateTailwind();
    
    private void BuffFromPlay(int extraTailwind)
    {
        IncreasedTailwind += extraTailwind;
        UpdateTailwind();
    }

    private void UpdateTailwind() => CurrentTailwind = 1 + IncreasedTailwind;
}