using System;
using InControl;


public class PlayerActions : PlayerActionSet
{
    public PlayerAction Join;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerTwoAxisAction Rotate;
    public PlayerTwoAxisAction Movement;

    public PlayerActions ()
    {
        Left = CreatePlayerAction ("Left");
        Right = CreatePlayerAction ("Right");
        Up = CreatePlayerAction ("Up");
        Down = CreatePlayerAction ("Down");

        Join = CreatePlayerAction("Join");
        Movement = CreateTwoAxisPlayerAction (Left, Right, Down, Up);


    }


    public static PlayerActions CreateWithKeyboardBindings ()
    {
        var actions = new PlayerActions ();

        actions.Up.AddDefaultBinding (Key.UpArrow);
        actions.Down.AddDefaultBinding (Key.DownArrow);
        actions.Left.AddDefaultBinding (Key.LeftArrow);
        actions.Right.AddDefaultBinding (Key.RightArrow);

        actions.Join.AddDefaultBinding (Key.Space);

        return actions;
    }

}
