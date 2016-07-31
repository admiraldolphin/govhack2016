using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

    public RectTransform scoreBar;
    public ScoreElement scorePrefab;

    public ScoreElement CreateScoreElement() {
        var element = Instantiate(scorePrefab);

        element.score = 0;

        element.transform.SetParent(scoreBar, false);

        return element;
    }

    public void PlayerWasCorrect (Player player)
    {
        if (player != null)
            player.score += 10;
    }

    public void PlayerWasIncorrect (Player player)
    {
        if (player != null)
            player.score -= 5;
    }
}

