using MegaCrit.Sts2.Core.Entities.Players;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IOnBecomeWellRead
{
    Task OnBecomeWellRead(Player player);
}