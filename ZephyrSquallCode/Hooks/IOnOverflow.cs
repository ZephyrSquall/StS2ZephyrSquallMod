using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IOnOverflow
{
    Task OnOverflow(PlayerChoiceContext choiceContext, Player player, bool fromHandDraw);
}