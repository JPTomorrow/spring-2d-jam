using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public partial class Spawner : PathFollow2D
{
	private PackedScene treePrefab;
	private float speed = 1f;
	public static int currentWaveLimit = 5;
	public static int TreesSpawned = 0;

	public override void _Ready()
	{
		speed = (float)GD.RandRange(1f, 3f);
		treePrefab = GD.Load<PackedScene>("res://scenes/tree/tree.tscn");

	}

	public void SpawnTree()
	{
		if (TreesSpawned >= currentWaveLimit)
			return;
		var enemy = treePrefab.Instantiate<tree>();
		enemy.GlobalPosition = GlobalPosition;
		var randScale = (float)GD.RandRange(0.4f, 1.0f);
		enemy.Scale = new Vector2(randScale, randScale);
		GetTree().Root.GetNode("main").AddChild(enemy);
		TreesSpawned++;
	}

	public override void _Process(double delta)
	{
		// Move the PathFollow2D along the path.
		ProgressRatio += speed;
		if (TreesSpawned < currentWaveLimit)
		{
			SpawnTree();
		}
	}
}
