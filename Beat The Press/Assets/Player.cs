using System;
using UnityEngine;
using InControl;


public class Player : MonoBehaviour
{
    public PlayerActions Actions { get; set; }

    public float speed = 10.0f;

    void OnDisable ()
    {
        if (Actions != null) {
            Actions.Destroy ();
        }
    }

    void Update ()
    {
        var movement = Actions.Movement.Vector * speed * Time.deltaTime;;

        transform.Translate(movement);

    }

}
