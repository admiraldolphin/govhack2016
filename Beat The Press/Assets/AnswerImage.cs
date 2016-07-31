using UnityEngine;
using System.Collections;

public class AnswerImage : MonoBehaviour {

	public string id;

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
        Destroy(gameObject);
        FindObjectOfType<ScoreManager>().PlayerWasCorrect(lastHoldingPlayer);
    }

    public void WasIncorrect ()
    {
        Destroy(gameObject);
        FindObjectOfType<ScoreManager>().PlayerWasIncorrect(lastHoldingPlayer);
    }
}
