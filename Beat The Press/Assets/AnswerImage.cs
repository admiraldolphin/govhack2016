using UnityEngine;
using System.Collections;

public class AnswerImage : MonoBehaviour {

	public string id;

    public AudioClip correctClip;
    public AudioClip incorrectClip;

    public float defaultGravityScale = 0;
    public float regularGravityScale = 1;

    public Player _lastHoldingPlayer;
    public Player lastHoldingPlayer {
        get {
            return _lastHoldingPlayer;
        }
        set {
            _lastHoldingPlayer = value;

            GetComponent<Rigidbody2D>().gravityScale = regularGravityScale;
        }
    }



    public void WasCorrect() {

        GetComponent<AudioSource>().PlayOneShot(correctClip);

        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());

        Destroy(gameObject, 2.0f);

        FindObjectOfType<ScoreManager>().PlayerWasCorrect(lastHoldingPlayer);
    }

    public void WasIncorrect ()
    {
        GetComponent<AudioSource>().PlayOneShot(incorrectClip);

        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());

        Destroy(gameObject, 2.0f);
        FindObjectOfType<ScoreManager>().PlayerWasIncorrect(lastHoldingPlayer);
    }
}
