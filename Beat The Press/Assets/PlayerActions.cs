using System;
using InControl;


public class PlayerActions : PlayerActionSet
{
    public PlayerAction Join;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Grab;

    public PlayerAction GrabberLeft;
    public PlayerAction GrabberRight;
    public PlayerAction GrabberUp;
    public PlayerAction GrabberDown;

    public PlayerTwoAxisAction GrabberRotation;
    public PlayerTwoAxisAction Movement;

    public PlayerActions ()
    {
        Left = CreatePlayerAction ("Left");
        Right = CreatePlayerAction ("Right");
        Up = CreatePlayerAction ("Up");
        Down = CreatePlayerAction ("Down");

        Grab = CreatePlayerAction("Grab");

        GrabberLeft = CreatePlayerAction("Grabber Left");
        GrabberRight = CreatePlayerAction("Grabber Right");
        GrabberUp = CreatePlayerAction("Grabber Up");
        GrabberDown = CreatePlayerAction("Grabber Down");


        Join = CreatePlayerAction("Join");
        Movement = CreateTwoAxisPlayerAction (Left, Right, Down, Up);
        GrabberRotation = CreateTwoAxisPlayerAction (GrabberLeft, GrabberRight, GrabberDown, GrabberUp);

    }


    public static PlayerActions CreateWithKeyboardBindings ()
    {
        var actions = new PlayerActions ();

        actions.Up.AddDefaultBinding (Key.UpArrow);
        actions.Down.AddDefaultBinding (Key.DownArrow);
        actions.Left.AddDefaultBinding (Key.LeftArrow);
        actions.Right.AddDefaultBinding (Key.RightArrow);

        actions.Grab.AddDefaultBinding(Key.G);

        actions.Join.AddDefaultBinding (Key.Space);

        actions.GrabberLeft.AddDefaultBinding (Key.A);
        actions.GrabberRight.AddDefaultBinding (Key.D);
        actions.GrabberUp.AddDefaultBinding (Key.W);
        actions.GrabberDown.AddDefaultBinding (Key.S);

        return actions;
    }

}
