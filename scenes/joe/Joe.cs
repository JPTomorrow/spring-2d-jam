using Godot;
using System;

public partial class Joe : CharacterBody2D
{
	private PackedScene officePrefab;
	private Building officePlacementModel;
	public const float Speed = 200.0f;
	private AnimatedSprite2D sprite;
	public int health = 100;
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

	private float buildRange = 60f;
	private bool isBuildMode = false;
	private Vector2 placementResetPos = new Vector2(-20000, -20000);

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("sprite");

		officePrefab = ResourceLoader.Load<PackedScene>("res://scenes/hq/office.tscn");
		officePlacementModel = officePrefab.Instantiate<Building>();
		GetTree().Root.GetNode("main").CallDeferred("add_child", officePlacementModel); ;
		officePlacementModel.Position = placementResetPos;
		officePlacementModel.Disabled = true;
		officePlacementModel.Hide();
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			sprite.Play("move");
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
			sprite.FlipH = velocity.X < 0f;

			if (isBuildMode)
				projectBuilding(direction);
		}
		else
		{
			sprite.Play("idle");
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("interact"))
			isBuildMode = true;

		if (Input.IsActionJustReleased("interact"))
		{
			officePlacementModel.Hide();
			officePlacementModel.Position = placementResetPos;
			isBuildMode = false;
		}
	}

	private void projectBuilding(Vector2 direction)
	{
		if (officePlacementModel.Visible == false)
			officePlacementModel.Show();
		var r = buildRange;
		if (direction.Y >= 0f)
			r *= 2f;
		else
			r *= 0.5f;
		officePlacementModel.GlobalPosition = GlobalPosition + direction * r;
	}

	public async void Die()
	{
		sprite.GetNode<AnimationPlayer>("anims").Play("die");
		var timer = GetTree().CreateTimer(3f);
		await ToSignal(timer, "timeout");
		// load main menu
	}
}
