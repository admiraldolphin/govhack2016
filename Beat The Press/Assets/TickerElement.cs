using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TickerElement : MonoBehaviour {

    public float speed;

    public string expectedImageName;

    [SerializeField]
    private Image borderImage;

    public Color color {
        get {
            return borderImage.color;
        }
        set {
            borderImage.color = value;
        }
    }

    public string text {
        get {
            return GetComponentInChildren<Text>().text;
        }
        set {
            GetComponentInChildren<Text>().text = value;
        }
    }

    private RectTransform _rectTransform;
    public RectTransform rectTransform {
        get {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private BoxCollider2D collider;

	// Update is called once per frame
	void Update () {

        // gotta do this in Update because the canvas hasn't given us a size in Start yet
        var size = rectTransform.rect.size;

        collider = GetComponent<BoxCollider2D>();

        collider.offset = new Vector2(size.x / 2.0f, 0);
        collider.size = size;

        // Move the object
        var offset = speed * Time.deltaTime;

        rectTransform.anchoredPosition -= new Vector2(offset, 0);

        // Are we off-screen? remove if that's the case

        if (rectTransform.anchoredPosition.x < -rectTransform.rect.size.x) {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D c) {

        var answer = c.gameObject.GetComponent<AnswerImage>();

        if (answer != null) {
            if (answer.id != expectedImageName) {
                answer.WasIncorrect();
            } else {
                Destroy(gameObject);
                answer.WasCorrect();
            }
        }
        
    }
}
