﻿using System;
using UnityEngine;
using System.Collections.Generic;
using InControl;


// This example iterates on the basic multiplayer example by using action sets with
// bindings to support both joystick and keyboard players. It would be a good idea
// to understand the basic multiplayer example first before looking a this one.
//
public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;

    const int maxPlayers = 4;

    List<Vector3> playerPositions = new List<Vector3> () {
        new Vector3 (-1, 1, 0),
        new Vector3 (1, 1, 0),
        new Vector3 (-1, -1, 0),
        new Vector3 (1, -1, 0),
    };

    List<Player> players = new List<Player> (maxPlayers);

    PlayerActions keyboardListener;
    PlayerActions joystickListener;


    void OnEnable ()
    {
        InputManager.OnDeviceDetached += OnDeviceDetached;
        keyboardListener = PlayerActions.CreateWithKeyboardBindings ();
        joystickListener = PlayerActions.CreateWithJoystickBindings ();
    }


    void OnDisable ()
    {
        InputManager.OnDeviceDetached -= OnDeviceDetached;
        joystickListener.Destroy ();
        keyboardListener.Destroy ();
    }

    public bool playersCanJoin = false;


    void Update ()
    {

        if (playersCanJoin == false)
            return;

        if (JoinButtonWasPressedOnListener (joystickListener)) {
            var inputDevice = InputManager.ActiveDevice;

            if (ThereIsNoPlayerUsingJoystick (inputDevice)) {
                CreatePlayer (inputDevice);
            }
        }

        if (JoinButtonWasPressedOnListener (keyboardListener)) {
            if (ThereIsNoPlayerUsingKeyboard ()) {
                CreatePlayer (null);
            }
        }
    }


    bool JoinButtonWasPressedOnListener (PlayerActions actions)
    {
        return actions.Join.WasPressed;
    }


    Player FindPlayerUsingJoystick (InputDevice inputDevice)
    {
        var playerCount = players.Count;
        for (int i = 0; i < playerCount; i++) {
            var player = players [i];
            if (player.Actions.Device == inputDevice) {
                return player;
            }
        }

        return null;
    }


    bool ThereIsNoPlayerUsingJoystick (InputDevice inputDevice)
    {
        return FindPlayerUsingJoystick (inputDevice) == null;
    }


    Player FindPlayerUsingKeyboard ()
    {
        var playerCount = players.Count;
        for (int i = 0; i < playerCount; i++) {
            var player = players [i];
            if (player.Actions == keyboardListener) {
                return player;
            }
        }

        return null;
    }


    bool ThereIsNoPlayerUsingKeyboard ()
    {
        return FindPlayerUsingKeyboard () == null;
    }


    void OnDeviceDetached (InputDevice inputDevice)
    {
        var player = FindPlayerUsingJoystick (inputDevice);
        if (player != null) {
            RemovePlayer (player);
        }
    }


    Player CreatePlayer (InputDevice inputDevice)
    {
        if (players.Count < maxPlayers) {
            // Pop a position off the list. We'll add it back if the player is removed.
            var playerPosition = playerPositions [0];
            playerPositions.RemoveAt (0);

            var gameObject = (GameObject)Instantiate (playerPrefab, playerPosition, Quaternion.identity);
            var player = gameObject.GetComponent<Player> ();

            if (inputDevice == null) {
                // We could create a new instance, but might as well reuse the one we have
                // and it lets us easily find the keyboard player.
                player.Actions = keyboardListener;
            } else {
                // Create a new instance and specifically set it to listen to the
                // given input device (joystick).
                var actions = PlayerActions.CreateWithJoystickBindings ();
                actions.Device = inputDevice;

                player.Actions = actions;
            }

            players.Add (player);

            return player;
        }

        return null;
    }

    public void RemoveAllPlayers() {
        foreach (var player in players) {
            playerPositions.Insert (0, player.transform.position);
            player.Actions = null;
            Destroy (player.gameObject);
        }

        players.Clear();
    }

    public void RemovePlayer (Player player)
    {
        playerPositions.Insert (0, player.transform.position);
        players.Remove (player);
        player.Actions = null;
        Destroy (player.gameObject);
    }

}
