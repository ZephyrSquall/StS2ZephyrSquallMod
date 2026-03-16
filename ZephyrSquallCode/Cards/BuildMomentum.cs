using System.Collections.ObjectModel;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ZephyrSquall.ZephyrSquallCode.Character;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(ZephyrSquallCardPool))]
public class BuildMomentum() : ZephyrSquallCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            return (IEnumerable<DynamicVar>) new ReadOnlyCollection<DynamicVar>(new DynamicVar[4]
            {
                (DynamicVar) new DamageVar(4M, ValueProp.Move),
                (DynamicVar) new CalculationBaseVar(0M),
                (DynamicVar) new CalculationExtraVar(1M),
                // Implement fetching the round number a different way without using a method explicitly marked as debug-only?
                (DynamicVar) new CalculatedVar("CalculatedHits").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => (Decimal) CombatManager.Instance.DebugOnlyGetState().RoundNumber))
            });
        }
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        BuildMomentum card = this;
        ArgumentNullException.ThrowIfNull((object?) cardPlay.Target, "cardPlay.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).WithHitCount((int) ((CalculatedVar) card.DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target)).FromCard((CardModel) card).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(2M);
}