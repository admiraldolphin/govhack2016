using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public void NewGame() {
        SceneManager.LoadScene("SelectPlayers");
    }
}
