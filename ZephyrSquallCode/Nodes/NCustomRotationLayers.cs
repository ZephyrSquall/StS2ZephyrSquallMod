using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

public partial class NCustomRotationLayers : Control
{
	private TextureRect _layer2;
	private TextureRect _layer3;

	private NEnergyCounter? _parent;
	private Player _player;

	public override void _Ready()
	{
		_parent = GetParent<NEnergyCounter>();
		_player = _parent._player;
		_layer2 = GetNode<TextureRect>("Layer2");
		_layer3 = GetNode<TextureRect>("Layer3");
	}

	public override void _Process(double delta)
	{
		var speed = _player.PlayerCombatState is { Energy: 0 } ? 5f : 30f;
		_layer2.RotationDegrees -= (float)delta * speed * 1;
		_layer3.RotationDegrees -= (float)delta * speed * 2f;
	}
}
