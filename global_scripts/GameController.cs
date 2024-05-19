using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameController : Node2D
{
	public List<Path2D> SpawnerRails = new List<Path2D>();
	private Timer spawnRotationTimer;

	public override void _Ready()
	{
		Audio.Play(this, "bgm.wav");

		SpawnerRails = FindChildren("spawner_rail").Select((x) => x as Path2D).ToList();
		spawnRotationTimer = GetNode<Timer>("spawn_rotation_timer");
		spawnRotationTimer.Timeout += randomizeSpawnerPositions;
	}

	public override void _Process(double delta)
	{
	}

	private List<PathFollow2D> getAllSpawners()
	{
		List<PathFollow2D> spawners = new List<PathFollow2D>();
		SpawnerRails.ForEach((x) => spawners.AddRange(x.FindChildren("spawner").Select((x) => x as PathFollow2D)));
		return spawners;
	}

	private void randomizeSpawnerPositions()
	{
		var spawners = getAllSpawners();
		spawners.ForEach((x) =>
		{
			x.ProgressRatio = (float)GD.RandRange(0.0f, 1.0f);
		});
	}

	private List<PathFollow2D> getSpawners(int spawnerIdx)
	{
		List<PathFollow2D> spawners = new List<PathFollow2D>();
		spawners.AddRange(SpawnerRails[0].FindChildren("spawner").Select((x) => x as PathFollow2D));
		return spawners;
	}
}
