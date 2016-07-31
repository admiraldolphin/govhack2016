using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{

    public RectTransform scoreBar;
    public ScoreElement scorePrefab;

    public ScoreboardEntry scoreboardEntryPrefab;
    public RectTransform scoreboard;

    public void UpdateScoreboard() {

        foreach (Transform child in scoreboard) {
            Destroy(child.gameObject);
        }

        var allPlayers = new List<Player>(FindObjectsOfType<Player>());

        allPlayers.Sort(delegate(Player x, Player y) {
            return y.score.CompareTo(x.score);
        });

        foreach (var player in allPlayers) {
            var entry = Instantiate(scoreboardEntryPrefab);
            entry.score = player.score;
            entry.teamSprite = player.GetComponent<PlayerAppearance>().teamSprite;

            entry.transform.SetParent(scoreboard, false);
        }

    }

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

    public void ClearScoreBar ()
    {
        foreach (Transform t in scoreBar) {
            Destroy(t.gameObject);
        }
    }
}

