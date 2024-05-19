using Godot;
using System;

public partial class Audio : Node
{
	public override void _Ready()
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), -8f);
	}

	public override void _Process(double delta)
	{
	}

	public static void Play(Node parent, string fileName)
	{
		AudioStreamWav sound = GD.Load<AudioStreamWav>($"res://sfx/{fileName}");
		AudioStreamPlayer player = new AudioStreamPlayer();
		parent.AddChild(player);
		player.Stream = sound;
		player.Finished += () => player.QueueFree();

		player.Play();
	}
}
