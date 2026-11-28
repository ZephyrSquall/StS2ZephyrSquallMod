using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using ZephyrSquall.ZephyrSquallCode.Relics;

namespace ZephyrSquall.ZephyrSquallCode.Utilities;

public static class ZephyrHoverTips
{
    public static IHoverTip Honed()
    {
        return new HoverTip(new LocString("static_hover_tips", "ZEPHYRSQUALL-HONED_STATIC.title"),
            new LocString("static_hover_tips", "ZEPHYRSQUALL-HONED_STATIC.description"));
    }

    public static IHoverTip Overflow()
    {
        return new HoverTip(new LocString("static_hover_tips", "ZEPHYRSQUALL-OVERFLOW.title"),
            new LocString("static_hover_tips", "ZEPHYRSQUALL-OVERFLOW.description"));
    }

    public static IHoverTip Record()
    {
        return new HoverTip(new LocString("static_hover_tips", "ZEPHYRSQUALL-RECORD.title"),
            new LocString("static_hover_tips", "ZEPHYRSQUALL-RECORD.description"));
    }

    public static IHoverTip Record(Player? player)
    {
        return player?.GetRelic<CursedTome>() == null ? Record() : RecordWithCursedTome();
    }

    public static IHoverTip RecordWithCursedTome()
    {
        return new HoverTip(new LocString("static_hover_tips", "ZEPHYRSQUALL-RECORD_WITH_CURSED_TOME.title"),
            new LocString("static_hover_tips", "ZEPHYRSQUALL-RECORD_WITH_CURSED_TOME.description"));
    }

    public static IHoverTip WellRead()
    {
        return new HoverTip(new LocString("static_hover_tips", "ZEPHYRSQUALL-WELL_READ.title"),
            new LocString("static_hover_tips", "ZEPHYRSQUALL-WELL_READ.description"));
    }
}