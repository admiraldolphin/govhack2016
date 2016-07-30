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


        GrabberLeft = CreatePlayerAction("Grabber Left");
        GrabberRight = CreatePlayerAction("Grabber Right");
        GrabberUp = CreatePlayerAction("Grabber Up");
        GrabberDown = CreatePlayerAction("Grabber Down");

        Grab = CreatePlayerAction("Grab");

        Join = CreatePlayerAction("Join");
        Movement = CreateTwoAxisPlayerAction (Left, Right, Down, Up);
        GrabberRotation = CreateTwoAxisPlayerAction (GrabberLeft, GrabberRight, GrabberDown, GrabberUp);

    }

    public static PlayerActions CreateWithJoystickBindings () {
        var actions = new PlayerActions();

        actions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
        actions.Down.AddDefaultBinding( InputControlType.LeftStickDown );
        actions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
        actions.Right.AddDefaultBinding( InputControlType.LeftStickRight );

        actions.Up.AddDefaultBinding( InputControlType.DPadUp );
        actions.Down.AddDefaultBinding( InputControlType.DPadDown );
        actions.Left.AddDefaultBinding( InputControlType.DPadLeft );
        actions.Right.AddDefaultBinding( InputControlType.DPadRight );

        actions.GrabberUp.AddDefaultBinding( InputControlType.RightStickUp );
        actions.GrabberDown.AddDefaultBinding( InputControlType.RightStickDown );
        actions.GrabberLeft.AddDefaultBinding( InputControlType.RightStickLeft );
        actions.GrabberRight.AddDefaultBinding( InputControlType.RightStickRight );

        actions.Grab.AddDefaultBinding(InputControlType.Action1);
        actions.Grab.AddDefaultBinding(InputControlType.Action2);
        actions.Grab.AddDefaultBinding(InputControlType.Action3);
        actions.Grab.AddDefaultBinding(InputControlType.Action4);
        actions.Grab.AddDefaultBinding(InputControlType.RightBumper);
        actions.Grab.AddDefaultBinding(InputControlType.RightTrigger);


        actions.Join.AddDefaultBinding (InputControlType.Start);
        actions.Join.AddDefaultBinding (InputControlType.Options);

        return actions;

    }

    public static PlayerActions CreateWithKeyboardBindings ()
    {
        var actions = new PlayerActions ();

        actions.Up.AddDefaultBinding (Key.UpArrow);
        actions.Down.AddDefaultBinding (Key.DownArrow);
        actions.Left.AddDefaultBinding (Key.LeftArrow);
        actions.Right.AddDefaultBinding (Key.RightArrow);

        actions.GrabberLeft.AddDefaultBinding (Key.A);
        actions.GrabberRight.AddDefaultBinding (Key.D);
        actions.GrabberUp.AddDefaultBinding (Key.W);
        actions.GrabberDown.AddDefaultBinding (Key.S);

        actions.Grab.AddDefaultBinding(Key.G);
        actions.Join.AddDefaultBinding (Key.Space);

        return actions;
    }

}
