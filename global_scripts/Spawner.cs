using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : PathFollow2D
{
	private PackedScene treePrefab;
	private Timer spawnTimer;
	private float speed = 1f;
	private List<tree> spawnedTrees = new List<tree>();

	public static int spawnLimit = 10;

	public override void _Ready()
	{
		speed = (float)GD.RandRange(1f, 3f);
		treePrefab = GD.Load<PackedScene>("res://scenes/tree/tree.tscn");
		spawnTimer = new Timer();
		spawnTimer.Timeout += spawn;
		spawnTimer.WaitTime = GD.RandRange(1.0f, 3.0f);
		spawnTimer.OneShot = false;
		spawnTimer.Autostart = true;
		AddChild(spawnTimer);
	}

	private void spawn()
	{
		var enemy = treePrefab.Instantiate<tree>();
		enemy.GlobalPosition = GlobalPosition;
		GetTree().Root.GetNode("main").AddChild(enemy);
	}

	public override void _Process(double delta)
	{
		// Move the PathFollow2D along the path.
		ProgressRatio += speed;
	}
}
