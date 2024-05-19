using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Building : AnimatedSprite2D
{
	private AnimatedSprite2D sprite;
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
	private int health = 100;
	public int Health
	{
		get => health;
		set
		{
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

	private List<RoofTopGuy> getAllRTGs()
	{
		return FindChildren("rtg*").Select(x => x as RoofTopGuy).ToList();
	}

	private void enableNextRTG()
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
		GetNode<CpuParticles2D>("burning").Hide();
		enableNextRTG();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

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
		GetNode<CpuParticles2D>("burning").Show();
		healthBar.Hide();
		sprite.Modulate = new Color(1, 1, 1);
		var anims = sprite.GetNode<AnimationPlayer>("anims");
		anims.Play("die");
		await ToSignal(anims, "animation_finished");
		var timer = GetTree().CreateTimer(2);
		await ToSignal(timer, "timeout");
		GetNode<CpuParticles2D>("burning").Hide();
		QueueFree();
	}

	private async void hitFlash()
	{
		isFlashing = true;
		var flashTime = .3f;
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
}
