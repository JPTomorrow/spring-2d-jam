using Godot;
using System;

public partial class Molotov : RigidBody2D
{
	private static PackedScene flameFieldPrefab;
	private Timer cookTimer;

	public bool Disabled = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		flameFieldPrefab = GD.Load<PackedScene>("res://scenes/flame_field/flame_field.tscn");
		cookTimer = GetNode<Timer>("cook_timer");
		cookTimer.Timeout += () =>
		{
			explode();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Disabled && !cookTimer.IsStopped())
		{
			cookTimer.Stop();
			SetCollisionLayerValue(6, true);
			SetCollisionMaskValue(6, true);
			LinearDamp = 6f;
		}
	}

	private void explode()
	{
		var flameField = flameFieldPrefab.Instantiate<FlameField>();
		flameField.GlobalPosition = Position;
		GetTree().Root.GetNode<Node2D>("main").AddChild(flameField);
		Audio.Play(GetTree().Root.GetNode<Node2D>("main"), "glass_break.wav");
		Audio.Play(GetTree().Root.GetNode<Node2D>("main"), "fire.wav");
		QueueFree();
	}
}
