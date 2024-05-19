using Godot;
using System;

public partial class Bullet : RigidBody2D
{

	public float Speed = 100f;
	private Timer despawnTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		despawnTimer = GetNode<Timer>("despawn_timer");
		despawnTimer.Timeout += onDespawnTimeout;
		BodyEntered += OnBodyEntered;
	}

	private void onDespawnTimeout()
	{
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (LinearVelocity.Length() < 80f)
		{
			QueueFree();
		}
	}

	public void FireAtTarget(Vector2 target)
	{
		LookAt(target);
		LinearVelocity = Position.DirectionTo(target) * Speed;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is tree)
		{
			(body as tree).Health -= 33;
			QueueFree();
		}
	}
}
