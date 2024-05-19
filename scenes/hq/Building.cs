using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Building : AnimatedSprite2D
{
	private Timer itemSpawnTimer;
	private bool disabled = false;
	public bool Disabled
	{
		get => disabled;
		set
		{
			disabled = value;
			if (disabled)
			{
				Modulate = new Color(1, 1, 1, .5f);
				GetNode<StaticBody2D>("collision").Hide();
			}
			else
			{
				Modulate = new Color(1, 1, 1, 1);
				GetNode<StaticBody2D>("collision").Show();
			}
		}
	}

	private HSlider healthBar;
	private bool isFlashing = false;
	private bool canBeUpgraded = false;
	private int health = 100;

	public int Health
	{
		get => health;
		set
		{
			if (Disabled || health <= 0)
				return;

			if (!disabled && !isFlashing && value < health)
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

	private List<RoofTopGuy> getAllRTGs()
	{
		return FindChildren("rtg*").Select(x => x as RoofTopGuy).ToList();
	}

	public void EnableNextRTG()
	{
		var rtgs = getAllRTGs();
		foreach (var rtg in rtgs)
		{
			if (rtg.Disabled)
			{
				rtg.Disabled = false;
				return;
			}
		}

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		itemSpawnTimer = GetNode<Timer>("item_spawn_timer");
		itemSpawnTimer.Start();
		itemSpawnTimer.Timeout += onItemSpawn;
		healthBar = GetNode<HSlider>("text/health_bar");
		GetNode<CpuParticles2D>("burning").Hide();
		if (Name == "hq")
			EnableNextRTG();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var joe = GetTree().Root.GetNode<Joe>("main/joe");
		if (Input.IsActionJustPressed("upgrade") && joe.CoinCount < 30)
		{
			var err = joe.GetNode<RichTextLabel>("ui/Control/input_error");
			err.GetNode<AnimationPlayer>("anims").Play("show");
			err.Text = "[center]Cannot afford rooftop koreans! need 30 coins![/center]";
		}
		if (Input.IsActionJustPressed("upgrade") && canBeUpgraded && joe.CoinCount >= 30)
		{
			EnableNextRTG();
			Audio.Play(this, "life_up.wav");
			joe.CoinCount -= 30;
			canBeUpgraded = false;
		}

	}

	private async void onItemSpawn()
	{
		if (Name == "hq")
		{
			//spawn coin
			var count = 1;
			count += GetTree().Root.GetNode("main/offices").GetChildCount();
			for (var i = 0; i < count; i++)
			{
				var coin = GD.Load<PackedScene>("res://scenes/coin/coin.tscn").Instantiate<Coin>();
				coin.GlobalPosition = GlobalPosition;
				var tt = GetTree().CreateTimer(.5f);
				await ToSignal(tt, "timeout");
				GetTree().Root.GetNode("main").AddChild(coin, true);
			}
		}
		if (Name.ToString().Contains("building"))
		{
			if (Disabled)
				return;
			//spawn molotov
			var molotov = GD.Load<PackedScene>("res://scenes/molotov/molotov.tscn").Instantiate<Molotov>();
			molotov.GlobalPosition = Position;
			molotov.Disabled = true;
			molotov.ApplyImpulse(new Vector2((float)GD.RandRange(-20f, 20f), 100));
			GetTree().Root.GetNode("main").AddChild(molotov, true);
		}
	}

	private void onNoBuildEntered(Node body)
	{
		if (disabled)
		{
			GetTree().Root.GetNode<Joe>("main/joe").CanBuild = false;
			Modulate = new Color(1, 0, 0);
		}
	}

	private void onNoBuildExited(Node body)
	{
		if (disabled)
		{
			GetTree().Root.GetNode<Joe>("main/joe").CanBuild = true;
			Modulate = new Color(1, 1, 1);
		}
	}

	public async void Die()
	{
		Audio.Play(GetTree().Root, "life_down.wav");
		Audio.Play(GetTree().Root, "fire.wav");
		getAllRTGs().ForEach(x => x.Disabled = true);
		GetNode<CpuParticles2D>("burning").Show();
		healthBar.Hide();
		Modulate = new Color(1, 1, 1);
		var anims = GetNode<AnimationPlayer>("anims");
		anims.Play("death");
		GetNode<CpuParticles2D>("burning").Show();
		await ToSignal(anims, "animation_finished");
		var timer = GetTree().CreateTimer(2);
		await ToSignal(timer, "timeout");
		GetNode<CpuParticles2D>("burning").Hide();
		var tt = GetTree().Root.GetNode<AnimationPlayer>("main/joe/ui/Control/game_over/anims");
		tt.Play("show");
		await ToSignal(tt, "animation_finished");
		if (Name == "hq")
			GetTree().ChangeSceneToFile("res://scenes/main_menu/main_menu.tscn");
		else
			QueueFree();
	}

	private async void hitFlash()
	{
		isFlashing = true;
		var flashTime = .3f;
		Modulate = new Color(1, 0, 0);
		var timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		Modulate = new Color(1, 1, 1);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		Modulate = new Color(1, 0, 0);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		Modulate = new Color(1, 1, 1);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		Modulate = new Color(1, 0, 0);
		timer = GetTree().CreateTimer(flashTime);
		await ToSignal(timer, "timeout");
		Modulate = new Color(1, 1, 1);
		isFlashing = false;
	}

	private void onUpgradeEntered(Node body)
	{
		if (body is Joe)
		{
			if (((Joe)body).CoinCount >= 30)
			{
				canBeUpgraded = true;
			}
		}
	}
	private void onUpgradeExited(Node body)
	{
		if (body is Joe)
		{
			canBeUpgraded = false;
		}
	}


}
