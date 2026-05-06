using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IOnAddDeft
{
    Task OnAddDeft(CardModel card, int amount, AbstractModel source);
}