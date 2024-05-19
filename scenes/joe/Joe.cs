using Godot;
using System;

public partial class Joe : CharacterBody2D
{
	private PackedScene officePrefab;
	private PackedScene molotovPrefab;
	private int molotovCount;
	public int MolotovCount
	{
		get => molotovCount;
		set
		{
			molotovCount = value;
			GetNode<RichTextLabel>("ui/Control/molotov_count").Text = molotovCount.ToString();
		}
	}
	private int coinCount;
	public int CoinCount
	{
		get => coinCount;
		set
		{
			coinCount = value;
			GetNode<RichTextLabel>("ui/Control/coin_count").Text = coinCount.ToString();
		}
	}
	private Building officePlacementModel;
	public const float Speed = 200.0f;
	private AnimatedSprite2D sprite;
	private HSlider healthBar;
	private bool isFlashing = false;
	public bool CanBuild = true;
	public int health = 100;
	public int Health
	{
		get => health;
		set
		{
			if (health <= 0)
				return;

			if (!isFlashing && value < health)
				hitFlash();

			if (!healthBar.Visible)
				healthBar.Show();
			health = value;
			healthBar.Value = value;

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
		GetNode<AnimationPlayer>("ui/Control/instructions/anims").Play("show");
		molotovPrefab = GD.Load<PackedScene>("res://scenes/molotov/molotov.tscn");
		sprite = GetNode<AnimatedSprite2D>("sprite");

		healthBar = GetNode<HSlider>("ui/Control/health_bar");
		officePrefab = ResourceLoader.Load<PackedScene>("res://scenes/hq/office.tscn");
		officePlacementModel = officePrefab.Instantiate<Building>();
		GetTree().Root.GetNode("main").CallDeferred("add_child", officePlacementModel); ;
		officePlacementModel.Position = placementResetPos;
		officePlacementModel.Disabled = true;
		officePlacementModel.Hide();
		MolotovCount = 3;
		CoinCount = 50;
	}


	public override void _PhysicsProcess(double delta)
	{
		if (Health <= 0)
			return;

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

		if (Input.IsActionJustPressed("fire"))
		{
			throwMolotov(direction);
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("interact"))
			isBuildMode = true;

		if (Input.IsActionJustReleased("interact"))
		{
			officePlacementModel.Hide();

			if (!CanBuild)
			{
				var err = GetNode<RichTextLabel>("ui/Control/input_error");
				err.Text = "[center]Cannot Build Here![/center]";
				err.GetNode<AnimationPlayer>("anims").Play("show");
			}

			if (CoinCount < 50)
			{
				var err = GetNode<RichTextLabel>("ui/Control/input_error");
				err.Text = "[center]Not Enough Coins![/center]";
				err.GetNode<AnimationPlayer>("anims").Play("show");
			}
			if (CanBuild && CoinCount >= 50)
			{
				var placed = officePrefab.Instantiate<Building>();
				GetTree().Root.GetNode("main/offices").AddChild(placed, true);
				placed.GlobalPosition = officePlacementModel.GlobalPosition;
				placed.EnableNextRTG();
				CoinCount -= 50;
			}

			officePlacementModel.Position = placementResetPos;
			isBuildMode = false;
		}

		if (Input.IsActionJustPressed("show_help"))
		{
			GetNode<AnimationPlayer>("ui/Control/instructions/anims").Play("show");
		}
	}

	private void throwMolotov(Vector2 direction)
	{
		if (MolotovCount <= 0)
			return;
		var molotov = molotovPrefab.Instantiate<Molotov>();
		molotov.GlobalPosition = GlobalPosition;
		molotov.LinearVelocity = direction * 250f;
		GetTree().Root.GetNode<Node2D>("main").AddChild(molotov);
		MolotovCount--;
	}

	private void projectBuilding(Vector2 direction)
	{
		if (officePlacementModel.Visible == false)
			officePlacementModel.Show();
		var r = buildRange;
		if (direction.Y >= 0f)
			r *= 3.0f;
		else
			r *= 0.5f;
		officePlacementModel.GlobalPosition = GlobalPosition + direction * r;
	}

	public async void Die()
	{
		Audio.Play(GetTree().Root, "died.wav");
		sprite.GetNode<AnimationPlayer>("anims").Play("die");
		var tt = GetNode<AnimationPlayer>("ui/Control/game_over/anims");
		tt.Play("show");
		await ToSignal(tt, "animation_finished");
		// load main menu
		GetTree().ChangeSceneToFile("res://scenes/main_menu/main_menu.tscn");
	}
	private async void hitFlash()
	{
		isFlashing = true;
		var flashTime = .1f;
		sprite.Modulate = new Color(1, 0, 0);
		var timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		sprite.Modulate = new Color(1, 1, 1);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		sprite.Modulate = new Color(1, 0, 0);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		sprite.Modulate = new Color(1, 1, 1);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		sprite.Modulate = new Color(1, 0, 0);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		sprite.Modulate = new Color(1, 1, 1);
		isFlashing = false;
	}

	private void onCoinPickupEntered(Node body)
	{
		if (body is Coin)
		{
			CoinCount++;
			Audio.Play(this, "checkpoint.wav");
			body.QueueFree();
		}
		if (body is Molotov)
		{
			MolotovCount++;
			Audio.Play(this, "high_score.wav");
			body.QueueFree();
		}
	}
}


