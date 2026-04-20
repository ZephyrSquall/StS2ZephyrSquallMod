using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IOnBecomeWellRead
{
    Task OnBecomeWellRead(PlayerChoiceContext choiceContext, Player player);
}