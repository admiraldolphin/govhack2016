using UnityEngine;
using System.Collections;

public class MusicControl : MonoBehaviour {

    [SerializeField]
    private AudioClip menuMusic;
    [SerializeField]
    private AudioClip gameMusic;

    AudioSource audio {
        get {
            return GetComponent<AudioSource>();
        }
    }

    private void PlayClip(AudioClip clip) {
        if (audio.clip != clip) {
            audio.Stop();
            audio.clip = clip;
            audio.Play();
        }
    }

    public void PlayInGameMusic() {
        PlayClip(gameMusic);
    }

    public void PlayMenuMusic() {
        PlayClip(menuMusic);
    }

}
