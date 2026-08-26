using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using ZephyrSquall.ZephyrSquallCode.Commands;

namespace ZephyrSquall.ZephyrSquallCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Train() : ZephyrSquallCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Honed", 3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ModifierCmd.AddHoned(choiceContext, Owner, DynamicVars["Honed"].IntValue, 1, this);
    }

    protected override void OnUpgrade() => DynamicVars["Honed"].UpgradeValueBy(2);
}