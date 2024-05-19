using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FlameField : Area2D
{
	private List<tree> burnList = new List<tree>();
	private Timer burnTickTimer;
	private Timer lifeTimeTimer;
	public override void _Ready()
	{
		burnTickTimer = GetNode<Timer>("burn_tick");
		lifeTimeTimer = GetNode<Timer>("lifetime_timer");
		burnTickTimer.Timeout += () =>
		{
			foreach (tree tree in burnList)
			{
				tree.Health -= 20;
			}
		};
		lifeTimeTimer.Timeout += async () =>
		{
			var flames = FindChildren("flame*").Select((x) => x as AnimatedSprite2D).ToList();
			foreach (var flame in flames)
			{
				var tt = GetTree().CreateTimer(GD.RandRange(0.1f, 0.5f));
				await ToSignal(tt, "timeout");
				flame.QueueFree();
			}
			QueueFree();
		};
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node body)
	{
		if (body is tree)
		{
			burnList.Add((tree)body);
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body is tree && burnList.Contains((tree)body))
		{
			burnList.Remove((tree)body);
		}
	}
}
