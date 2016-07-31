using UnityEngine;
using System.Collections;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;



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

    public List<int> colour;

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context) {
        choiceTextures = new Dictionary<int, Texture2D>();
    }

    public Dictionary<int, Texture2D> choiceTextures = new Dictionary<int, Texture2D>();

    public bool isFake {
        get {
            return string.IsNullOrEmpty(clue) == false && (choices == null || choices.Count == 0);
        }
    }

    public bool isUsable {
        get {
            return isFake ||  answerTexture != null;
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
            if (isFake) {
                return true;
            } else {
                return choiceTextures.Count == choices.Count;
            }

        }
    }

    public List<DownloadWithCallback> GetDownloaders(string baseURL) {

        if (isFake) {
            // return an empty list
            return new List<DownloadWithCallback>();           
        }

        var list = new List<DownloadWithCallback>();

        int i = 0;

        foreach (var item in choices) {
            var theItem = item;
            var loader = new DownloadWithCallback();
            var url = string.Format ("{0}{1}", baseURL, theItem);

            var thisCount = i;
            loader.www = new WWW(url);
            loader.callback = delegate {
                if (loader.www.error != null) {
                    Debug.LogErrorFormat("Error downloading {0}: {1}", theItem, loader.www.error);

                } else {
                    Debug.LogFormat("Downloaded {0}", theItem);
                    var tex = loader.www.texture;

                    choiceTextures[thisCount] = tex;
                }

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
            if (questions == null)
                return false;
            
            var usableQuestions = questions.FindAll(i => i.isUsable);
            return usableQuestions.Count > 0;
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

        var configFile = System.IO.Path.Combine(Application.persistentDataPath,"config.txt");

        if (System.IO.File.Exists(configFile)) {
            var text = System.IO.File.ReadAllLines(configFile);
            if (text.Length > 0) {
                this.URL = text[0];
            }
        } else {
            System.IO.File.WriteAllText(configFile, this.URL);
        }

        StartCoroutine(MainGameLoop());
    }



    private IEnumerator LoadData() {

        string fullURL = string.Format ("{0}/quiz?n={1}", URL, questionCount);

        this.dataLoadOperation = new WWW(fullURL);

        yield return dataLoadOperation;

        var loadedText = dataLoadOperation.text;

        questions = JsonConvert.DeserializeObject<List<Question>>(loadedText);

        if (questions == null) {
            Debug.LogErrorFormat("Error downloading questions: {0}", dataLoadOperation.error);
            questions = new List<Question>();
            yield break;
        }

        var allLoaders = new List<DownloadWithCallback>();

        foreach (var question in questions) {
            allLoaders.AddRange(question.GetDownloaders(URL));
        }

        yield return new WaitUntil( () => allLoaders.TrueForAll(i => i.www.isDone));

        foreach (var loader in allLoaders) {
            loader.callback();
        }

        Debug.Log("Loading complete");

    }


    public float imageSize = 2.5f;

    AnswerImage SpawnImage (GameObject[] spawnPoints, string imageName, Texture2D tex)
    {
        var name = string.Format ("Answer {0}", imageName);
        var obj = Instantiate (answerPrefab);
        obj.name = name;
        var pixelsPerUnit = tex.width / imageSize;
        var sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f), pixelsPerUnit);
        obj.GetComponent<SpriteRenderer> ().sprite = sprite;
        obj.id = imageName;
        obj.gameObject.AddComponent<BoxCollider2D> ();
        // Find a spawn point
        var spawn = spawnPoints [Random.Range (0, spawnPoints.Length)];
        obj.transform.position = spawn.transform.position;

        return obj;

    }

    public RectTransform menu;
    public RectTransform scoreboard;
    public RectTransform pressAnyKey;

    IEnumerator MainGameLoop() {

        while (true) {
            FindObjectOfType<MusicControl>().PlayMenuMusic();

            menu.gameObject.SetActive(true);
            scoreboard.gameObject.SetActive(false);
            pressAnyKey.gameObject.SetActive(false);

            yield return StartCoroutine(LoadData());


            Debug.Log("Waiting for download to complete");

            yield return new WaitUntil(delegate() {
                return this.state == State.Loaded;
            });

            Debug.Log("Download complete!");


            pressAnyKey.gameObject.SetActive(true);

            // wait for a button to be pressed
            yield return new WaitUntil(
                () => InControl.InputManager.CommandWasPressed ||
                InControl.InputManager.AnyKeyIsPressed
            );


            FindObjectOfType<MusicControl>().PlayInGameMusic();

            menu.gameObject.SetActive(false);

            FindObjectOfType<PlayerManager>().playersCanJoin = true;

            FindObjectOfType<TimeManager>().StartClock();

            activeQuestions = new List<QuestionSet>();


            while (UpdateGame()) {
                
                // wait for the next frame
                yield return null;
            }

            FindObjectOfType<ScoreManager>().UpdateScoreboard();

            FindObjectOfType<PlayerManager>().RemoveAllPlayers();

            scoreboard.gameObject.SetActive(true);

            FindObjectOfType<PlayerManager>().playersCanJoin = false;

            FindObjectOfType<MusicControl>().PlayMenuMusic();

            // clear all answers
            foreach (var answer in FindObjectsOfType<AnswerImage>()) {
                Destroy(answer.gameObject);
            }

            // clear the ticker
            FindObjectOfType<TickerManager>().Clear();

            // clear the scorebar
            FindObjectOfType<ScoreManager>().ClearScoreBar();


            // wait for a button to be pressed
            yield return new WaitUntil(
                () => InControl.InputManager.CommandWasPressed ||
                InControl.InputManager.AnyKeyIsPressed
            );



            // and start the game again
        }



    }

    List<QuestionSet> activeQuestions = new List<QuestionSet>();


    private bool UpdateGame() {



        if (FindObjectOfType<TimeManager>().clockHasExpired) {
            return false;
        }

        // should we remove expired questions?
        foreach (var question in activeQuestions) {
            if (question.tickerElement == null) {
                foreach (var answer in question.images) {  
                    if (answer != null)
                        Destroy(answer.gameObject);
                }
            }
        }

        activeQuestions.RemoveAll(q => q.tickerElement == null);

        // can we add a new question?
        if (ticker.CanAddNewItem()) {

            var questionSet = new QuestionSet();

            // select a question
            var nextQuestion = questions[Random.Range(0, questions.Count)];

            if (nextQuestion.answerTexture == null) {
                return true;
            }

            questionSet.question = nextQuestion;

            questionSet.images = new List<AnswerImage>();

            questionSet.tickerElement = ticker.AddNewItem(nextQuestion.clue);
            questionSet.tickerElement.expectedImageName = nextQuestion.answer;

            var questionColor = new Color32(
                (byte)nextQuestion.colour[0], 
                (byte)nextQuestion.colour[1], 
                (byte)nextQuestion.colour[2], 
                255
            );

            questionSet.tickerElement.color = questionColor;

            var spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");


            // spawn the answer
            questionSet.images.Add(SpawnImage(spawnPoints, nextQuestion.answer,nextQuestion.answerTexture));

            var availableChoices = new List<string>(nextQuestion.choices);
            availableChoices.Remove(nextQuestion.answer);

            int answersPerQuestion = 2;

            for (var i = 1; i < answersPerQuestion; i++) {
                var wrongImageName = availableChoices[Random.Range(0, availableChoices.Count)];

                var wrongImageKey = nextQuestion.choices.FindIndex(item => item == wrongImageName);

                if (nextQuestion.choiceTextures.ContainsKey(wrongImageKey) == false) {
                    continue;
                }

                var wrongImageTex = nextQuestion.choiceTextures[wrongImageKey];

                if (wrongImageTex == null) {
                    continue;
                }

                questionSet.images.Add(SpawnImage (spawnPoints, wrongImageName, wrongImageTex));
            }

            activeQuestions.Add(questionSet);

        }

        return true;
    }

    public AnswerImage answerPrefab;

    public TickerManager ticker;

    public struct QuestionSet {
        public Question question;
        public TickerElement tickerElement;
        public List<AnswerImage> images;
    }


	
}
