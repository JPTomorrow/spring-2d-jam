using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public partial class tree : CharacterBody2D
{
	private AnimatedSprite2D sprite;
	public const float Speed = 30.0f;
	private HSlider healthBar;
	private bool isFlashing = false;
	private bool isAttacking = false;
	private List<Node> targetsInAttackRadius = new List<Node>();
	private int health = 100;
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

	public override void _Ready()
	{

		player = GetTree().Root.GetNode<CharacterBody2D>("main/joe");
		hq = GetTree().Root.GetNode<AnimatedSprite2D>("main/hq");
		sprite = GetNode<AnimatedSprite2D>("sprite");
		healthBar = GetNode<HSlider>("health_bar");
		healthBar.Hide();
	}


	private CharacterBody2D player;
	private AnimatedSprite2D hq;
	public override void _PhysicsProcess(double delta)
	{
		if (isAttacking || Health <= 0 || !IsInstanceValid(player) || !IsInstanceValid(hq))
			return;

		Vector2 velocity = Velocity;

		Vector2 direction;

		if (GlobalPosition.DistanceTo(player.GlobalPosition) < GlobalPosition.DistanceTo(hq.GlobalPosition))
			direction = (player.GlobalPosition - GlobalPosition).Normalized();
		else
			direction = (hq.GlobalPosition - GlobalPosition).Normalized();

		velocity.X = direction.X * Speed;
		velocity.Y = direction.Y * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}

	public async void Attack()
	{
		if (Health <= 0)
			return;
		var anims = sprite.GetNode<AnimationPlayer>("anims");
		anims.Play("attack");
		await ToSignal(anims, "animation_finished");
		Audio.Play(this, "stomp.wav");
		var dust = sprite.GetNode<AnimatedSprite2D>("attack_smoke");
		dust.Play();
		if (targetsInAttackRadius.Any())
		{
			foreach (var target in targetsInAttackRadius)
			{
				if (target is Joe)
				{
					((Joe)target).Health -= 10;
				}
				else if (target is StaticBody2D)
				{
					((Building)target.GetParent()).Health -= 10;
				}
			}

		}
		await ToSignal(dust, "animation_finished");
		if (Health > 0 && isAttacking)
			Attack();

	}

	public async void Die()
	{
		healthBar.Hide();
		sprite.Modulate = new Color(1, 1, 1);
		SetCollisionLayerValue(1, false);
		SetCollisionLayerValue(2, false);
		GameState.TreesKilled++;
		Spawner.TreesSpawned--;
		var anims = sprite.GetNode<AnimationPlayer>("anims");
		anims.Play("die");
		await ToSignal(anims, "animation_finished");
		var timer = GetTree().CreateTimer(2);
		await ToSignal(timer, "timeout");
		QueueFree();
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

	private void onAtkRangeEntered(Node body)
	{
		if (!isAttacking && (body is Joe || body is StaticBody2D) && !body.Name.ToString().Contains("border_blocker"))
		{
			GD.Print("attacking");
			targetsInAttackRadius.Add(body);
			// attack
			isAttacking = true;
			Attack();
		}
	}
	private void onAtkRangeExited(Node body)
	{

		if (targetsInAttackRadius.Contains(body))
		{
			GD.Print("not attacking");

			targetsInAttackRadius.Remove(body);
		}
		if (!targetsInAttackRadius.Any())
		{
			isAttacking = false;
		}
	}
}
