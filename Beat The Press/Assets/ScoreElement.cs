using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreElement : MonoBehaviour {

    public Text scoreLabel;
    public Image teamImage;

    public int score {
        set {
            scoreLabel.text = value.ToString();
        }
    }
}
