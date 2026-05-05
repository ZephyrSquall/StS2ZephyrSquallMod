using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace ZephyrSquall.ZephyrSquallCode.Utilities;

public class ZephyrKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Narrate;
}