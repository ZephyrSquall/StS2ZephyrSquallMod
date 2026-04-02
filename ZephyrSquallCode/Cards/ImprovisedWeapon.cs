using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class ImprovisedWeapon() : ZephyrSquallCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private int _currentDamage = 30;
    private int _decreasedDamage;
    
    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new ("Decrease", 2)
    ];
    
    [SavedProperty]
    public int DecreasedDamage
    {
        get => _decreasedDamage;
        set
        {
            AssertMutable();
            _decreasedDamage = value;
        }
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        int lostDamage = DynamicVars["Decrease"].IntValue;
        DebuffFromPlay(lostDamage);
        if (!(DeckVersion is ImprovisedWeapon deckVersion))
            return;
        deckVersion.DebuffFromPlay(lostDamage);
    }

    protected override void OnUpgrade() => DynamicVars["Decrease"].UpgradeValueBy(-1M);
    
    protected override void AfterDowngraded() => UpdateDamage();

    private void DebuffFromPlay(int lostDamage)
    {
        DecreasedDamage += lostDamage;
        UpdateDamage();
    }

    private void UpdateDamage() => CurrentDamage = Math.Max(30 - DecreasedDamage, 0);
}