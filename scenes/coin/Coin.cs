using Godot;
using System;

public partial class Coin : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ApplyImpulse(new Vector2(GD.RandRange(-40, 40), 100));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
