using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreboardEntry : MonoBehaviour {

    [SerializeField]
    private Text text;

    [SerializeField]
    private Image image;

    public Sprite teamSprite {
        set {
            image.sprite = value;
        }
    }

    public int score {
        set {
            text.text = value.ToString();
        }
    }

	
}
