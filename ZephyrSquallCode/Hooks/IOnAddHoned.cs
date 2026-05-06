using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IOnAddHoned
{
    Task OnAddHoned(CardModel card, int amount, AbstractModel source);
}