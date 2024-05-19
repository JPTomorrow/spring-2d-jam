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
		GetTree().ChangeSceneToFile("res://scenes/main_world/main.tscn");
	}

	private void onExitPressed()
	{
		GetTree().Quit();
	}
}
