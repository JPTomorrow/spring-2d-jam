using Godot;
using System;

public partial class MainMenu : Control
{

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	private void onPlayPressed()
	{
		GameState.TreesKilled = 0;
		Spawner.TreesSpawned = 0;
		GetTree().ChangeSceneToFile("res://scenes/main_world/main.tscn");
	}

	private void onExitPressed()
	{
		GetTree().Quit();
	}
}
