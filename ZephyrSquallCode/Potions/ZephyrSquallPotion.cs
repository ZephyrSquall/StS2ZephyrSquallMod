using BaseLib.Abstracts;
using BaseLib.Utils;
using ZephyrSquall.ZephyrSquallCode.Character;

namespace ZephyrSquall.ZephyrSquallCode.Potions;

[Pool(typeof(ZephyrSquallPotionPool))]
public abstract class ZephyrSquallPotion : CustomPotionModel;