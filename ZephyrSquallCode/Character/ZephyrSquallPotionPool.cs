using BaseLib.Abstracts;
using Godot;

namespace ZephyrSquall.ZephyrSquallCode.Character;

public class ZephyrSquallPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => ZephyrSquall.CharacterId;
    public override Color LabOutlineColor => ZephyrSquall.Color;
}