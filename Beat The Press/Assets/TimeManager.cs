using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;



class TimeManager : MonoBehaviour
{
    public Text label;

    private bool clockStarted = true;

    public int time;

    private float timeRemaining;

    public bool clockHasExpired {
        get {
            return clockStarted == true && timeRemaining <= 0;
        }
    }

    public string FormatSeconds(int seconds) {
        return string.Format("{0}:{1:D2}", seconds / 60, seconds % 60);
    }

    void Awake() {
        timeRemaining = time;
    }


    public void StartClock ()
    {
        clockStarted = true;

        timeRemaining = time;
    }

    void Update() {

        if (timeRemaining <= 0) {
            return;
        }

        if (clockStarted) {
            timeRemaining -= Time.deltaTime;
            label.text = FormatSeconds((int)timeRemaining);
        } else {
            label.text = "--:--";
        }

    }
}





