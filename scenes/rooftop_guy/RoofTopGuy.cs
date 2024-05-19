using Godot;
using System;
using System.Collections.Generic;

public partial class RoofTopGuy : AnimatedSprite2D
{
	public List<tree> targets = new List<tree>();
	private PackedScene bulletPrefab;
	private AnimatedSprite2D gun;
	private Node2D muzzlePosition;
	private Timer fireCooldown;
	private tree currentTarget = null;
	private bool disabled = true;
	public bool Disabled
	{
		get => disabled;
		set
		{
			disabled = value;
			if (disabled)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}
	}

	public override void _Ready()
	{
		bulletPrefab = GD.Load<PackedScene>("res://scenes/bullet/bullet.tscn");
		gun = GetNode<AnimatedSprite2D>("gun");
		gun.AnimationFinished += () => gun.Play("idle");
		muzzlePosition = GetNode<Node2D>("gun/muzzle_pos");
		fireCooldown = GetNode<Timer>("fire_cooldown");
		fireCooldown.Timeout += onFireCooldownTimeout;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (!Disabled && targets.Count > 0 && currentTarget != null)
		{
			if (currentTarget.Health <= 0)
			{
				targets.Remove(currentTarget);
				if (targets.Count > 0)
				{
					currentTarget = targets[0];
				}
				else
				{
					currentTarget = null;
				}
			}
			else
			{

				// rotate gun towards target
				Vector2 targetPos = currentTarget.GlobalPosition;
				gun.LookAt(targetPos);
				if (gun.RotationDegrees < -90 || gun.RotationDegrees > 90)
				{
					gun.FlipV = true;
				}
				else
				{
					gun.FlipV = false;
				}
			}
		}
	}

	private void onFireCooldownTimeout()
	{
		if (!Disabled && currentTarget != null && currentTarget.Health > 0)
		{
			gun.Play("fire");
			Audio.Play(this, "shoot.wav");
			Vector2 targetPos = currentTarget.GlobalPosition;
			Bullet bullet = (Bullet)bulletPrefab.Instantiate();
			bullet.GlobalPosition = muzzlePosition.GlobalPosition;
			bullet.FireAtTarget(targetPos);
			GetTree().Root.GetNode("main").AddChild(bullet);
		}
	}

	private void onTargetRangeEntered(Node2D body)
	{
		if (body is tree)
		{
			targets.Add((tree)body);
			if (currentTarget == null)
			{
				currentTarget = targets[0];
			}
		}
	}

	private void onTargetRangeExited(Node2D body)
	{
		if (body is tree && targets.Contains((tree)body))
		{
			targets.Remove((tree)body);
		}
	}
}
