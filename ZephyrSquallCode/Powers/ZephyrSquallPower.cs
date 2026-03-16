using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using ZephyrSquall.ZephyrSquallCode.Extensions;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public abstract class ZephyrSquallPower : CustomPowerModel
{
    //Loads from ZephyrSquall/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}