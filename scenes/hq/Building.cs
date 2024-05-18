using Godot;
using System;

public partial class Building : AnimatedSprite2D
{
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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
