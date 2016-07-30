using UnityEngine;
using System.Collections;

using Newtonsoft.Json;
using System.Collections.Generic;

public struct DownloadWithCallback {

    public delegate void Callback();

    public WWW www;
    public Callback callback;
}

[JsonObject(MemberSerialization.Fields)]
public class Question {


    public string clue;
    public List<string> choices = new List<string>();
    public string answer;
    public string source;

    public Dictionary<int, Texture2D> choiceTextures = new Dictionary<int, Texture2D>();

    public bool isUsable {
        get {
            return answerTexture != null;
        }
    }

    public Texture2D answerTexture {
        get {
            var answerIndex = choices.FindIndex(i => i == answer);

            if (choiceTextures.ContainsKey(answerIndex)) {
                return choiceTextures[answerIndex];
            } else {
                return null;
            }
        }
    }

    public bool allTexturesLoaded {
        get {
            return choiceTextures.Count == choices.Count;
        }
    }

    public List<DownloadWithCallback> GetDownloaders(string baseURL) {

        var list = new List<DownloadWithCallback>();

        int i = 0;

        foreach (var item in choices) {
            var loader = new DownloadWithCallback();
            var url = string.Format ("{0}{1}", baseURL, item);
            loader.www = new WWW(url);
            loader.callback = delegate {
                var tex = loader.www.texture;

                choiceTextures[i] = tex;
            };

            list.Add(loader);

            i++;

        }

        return list;

    }

    

}

public class DataManager : MonoBehaviour {

    public string URL;

    private List<Question> questions = new List<Question>();

    public bool dataAvailable { 
        get {
            return questions != null && questions.Count > 0; 
        }
    }

    public int questionCount = 4;

    private enum State {
        NotLoaded,
        Loading,
        Loaded,
        Error
    }

    private WWW dataLoadOperation;

    private State state {
        get {
            if (dataAvailable == false) {
                if (dataLoadOperation != null) {
                    if (dataLoadOperation.error != null) {
                        return State.Error;
                    } else if (dataLoadOperation.isDone == false) {
                        return State.Loading;                    
                    } else {
                        return State.Error;
                    }
                } else {
                    return State.NotLoaded;
                }
            } else {
                return State.Loaded;
            }
        }
    }

    private string stateString {
        get {
            switch (state) {
            case State.Error:
                return string.Format ("{0} {1}", state, dataLoadOperation.error);
            default:
                return state.ToString();
            }
        }
    }

    void Start() {
        LoadData();
    }


    public void LoadData() {
        
        StartCoroutine(WaitForDownload());

    }

    private IEnumerator WaitForDownload() {

        string fullURL = string.Format ("{0}/quiz?n={1}", URL, questionCount);

        this.dataLoadOperation = new WWW(fullURL);

        yield return dataLoadOperation;

        var loadedText = dataLoadOperation.text;

        questions = JsonConvert.DeserializeObject<List<Question>>(loadedText);

        var allLoaders = new List<DownloadWithCallback>();

        foreach (var question in questions) {
            allLoaders.AddRange(question.GetDownloaders(URL));
        }

        yield return new WaitUntil( () => allLoaders.TrueForAll(i => i.www.isDone));

        foreach (var loader in allLoaders) {
            loader.callback();
        }


    }

    void OnGUI() {

        int completeCount = 0;

        if (questions != null) {
            foreach (var q in questions) {
                if (q.allTexturesLoaded) {
                    completeCount ++;
                }
            }
        }

        GUI.Label (new Rect (10, 50, 400, 50), string.Format ("Data manager: {0}\nQuestions: {1} ({2} complete)", stateString, questions.Count, completeCount));
    }
    
	
}
