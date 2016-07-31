using System;
using UnityEngine;
using InControl;


public class Player : MonoBehaviour
{
    public PlayerActions Actions { get; set; }

    public float speed = 10.0f;

    private ScoreElement scoreElement;

    private int _score = 0;

    private AudioLowPassFilter filter;

    public int score {
        get {
            return _score;
        }
        set {

            if (_score > value) {
                GetComponent<Animator>().SetTrigger("Shake Fist");
            }

            _score = value;
            scoreElement.score = value;
        }
    }

    void Start() {
        if (Actions == null) {
            Actions = PlayerActions.CreateWithKeyboardBindings();
        }

        scoreElement = FindObjectOfType<ScoreManager>().CreateScoreElement();

        scoreElement.teamImage.sprite = GetComponent<PlayerAppearance>().teamSprite;

        filter = GetComponent<AudioLowPassFilter>();

    }

    

    void OnDisable ()
    {
        if (Actions != null) {
            Actions.Destroy ();
        }
    }

    public float minFilterFreq = 50;
    public float maxFilterFreq = 7500;
    public float movementFilterThreshold = 100;

    void Update ()
    {
        var movement = Actions.Movement.Vector * speed * Time.deltaTime;;

        var body = GetComponent<Rigidbody2D>();

        body.AddForce(movement);

        var currentSpeed = body.velocity.magnitude;

        var normalisedSpeed = currentSpeed / movementFilterThreshold;

        var freq = Mathf.Lerp(minFilterFreq, maxFilterFreq, normalisedSpeed);

        filter.cutoffFrequency = freq;




        //transform.Translate(movement);

    }

}
