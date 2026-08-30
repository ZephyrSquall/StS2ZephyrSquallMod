using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Commands;
using ZephyrSquall.ZephyrSquallCode.Patches;
using ZephyrSquall.ZephyrSquallCode.Utilities;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

public class Acuate() : ZephyrSquallCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move), new IntVar("Honed", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ZephyrHoverTips.Honed()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        LocString prompt = new("card_selection", "TO_ADD_HONED");
        prompt.Add("Honed", DynamicVars["Honed"].IntValue);
        var prefs = new CardSelectorPrefs(prompt, 1);
        var selectedCard = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
        if (selectedCard != null)
        {
            await HonedCmd.AddHonedToSpecific(selectedCard, DynamicVars["Honed"].IntValue, this);
            await HonedCmd.AddHonedToSpecific(selectedCard, CardModifierTracker.HonedAmount[selectedCard], this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars["Honed"].UpgradeValueBy(1);
    }
}