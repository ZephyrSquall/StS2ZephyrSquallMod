using MegaCrit.Sts2.Core.Models;

namespace ZephyrSquall.ZephyrSquallCode.Hooks;

public interface IOnRecord
{ 
    Task OnRecord(IEnumerable<CardModel> cards, AbstractModel source);
}