using Godot;
using System;

public partial class tree : CharacterBody2D
{
	private AnimatedSprite2D sprite;
	public const float Speed = 50.0f;
	private int health = 100;
	public int Health
	{
		get => health;
		set
		{
			health = value;
			if (health <= 0)
			{
				Die();
			}

		}
	}

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Health <= 0)
			return;

		Vector2 velocity = Velocity;

		var p = GetTree().Root.GetNode<CharacterBody2D>("main/joe");
		var hq = GetTree().Root.GetNode<AnimatedSprite2D>("main/hq");
		Vector2 direction;

		if (GlobalPosition.DistanceTo(p.GlobalPosition) < GlobalPosition.DistanceTo(hq.GlobalPosition))
			direction = (p.GlobalPosition - GlobalPosition).Normalized();
		else
			direction = (hq.GlobalPosition - GlobalPosition).Normalized();

		velocity.X = direction.X * Speed;
		velocity.Y = direction.Y * Speed;

		Velocity = velocity;
		MoveAndSlide();

		if (Input.IsKeyPressed(Key.G))
		{
			Health = 0;

		}
	}

	public async void Die()
	{
		var anims = sprite.GetNode<AnimationPlayer>("anims");
		anims.Play("die");
		await ToSignal(anims, "animation_finished");
		var timer = GetTree().CreateTimer(2);
		await ToSignal(timer, "timeout");
		QueueFree();
	}
}
