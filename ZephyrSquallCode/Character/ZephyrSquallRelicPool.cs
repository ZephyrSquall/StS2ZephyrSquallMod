using BaseLib.Abstracts;
using Godot;

namespace ZephyrSquall.ZephyrSquallCode.Character;

public class ZephyrSquallRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => ZephyrSquall.CharacterId;
    public override Color LabOutlineColor => ZephyrSquall.Color;
}