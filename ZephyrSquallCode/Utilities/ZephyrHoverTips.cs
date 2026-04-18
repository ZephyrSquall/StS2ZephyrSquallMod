using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace ZephyrSquall.ZephyrSquallCode.Utilities;

public static class ZephyrHoverTips
{
    public static IHoverTip Deft()
    {
        return new HoverTip(new LocString("static_hover_tips", "DEFT_STATIC.title"),
            new LocString("static_hover_tips", "DEFT_STATIC.description"));
    }
    
    public static IHoverTip Honed()
    {
        return new HoverTip(new LocString("static_hover_tips", "HONED_STATIC.title"),
            new LocString("static_hover_tips", "HONED_STATIC.description"));
    }
    
    public static IHoverTip Record()
    {
        return new HoverTip(new LocString("static_hover_tips", "RECORD.title"),
            new LocString("static_hover_tips", "RECORD.description"));
    }
    
    public static IHoverTip WellRead()
    {
        return new HoverTip(new LocString("static_hover_tips", "WELL_READ.title"),
            new LocString("static_hover_tips", "WELL_READ.description"));
    }
}