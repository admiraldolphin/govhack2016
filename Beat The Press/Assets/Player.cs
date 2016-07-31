using System;
using UnityEngine;
using InControl;


public class Player : MonoBehaviour
{
    public PlayerActions Actions { get; set; }

    public float speed = 10.0f;

    private ScoreElement scoreElement;

    private int _score = 0;

    public int score {
        get {
            return _score;
        }
        set {
            _score = value;
            scoreElement.score = value;
        }
    }

    void Start() {
        if (Actions == null) {
            Actions = PlayerActions.CreateWithKeyboardBindings();
        }

        scoreElement = FindObjectOfType<ScoreManager>().CreateScoreElement();

    }

    

    void OnDisable ()
    {
        if (Actions != null) {
            Actions.Destroy ();
        }
    }

    void Update ()
    {
        var movement = Actions.Movement.Vector * speed * Time.deltaTime;;

        var body = GetComponent<Rigidbody2D>();

        body.AddForce(movement);

        //transform.Translate(movement);

    }

}
