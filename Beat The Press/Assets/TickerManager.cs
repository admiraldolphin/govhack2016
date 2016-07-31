using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TickerManager : MonoBehaviour {

    public TickerElement tickerPrefab;

    public RectTransform tickerContainer;

    public List<TickerElement> tickerElements = new List<TickerElement>();

    public InputField inputField;

    void OnGUI() {
        GUI.Label (new Rect (10, 10, 100, 40), string.Format ("{0} items", tickerElements.Count));
    }

    void Update() {
        // Sweep for nulls in the tickerElements list
        tickerElements.RemoveAll(i => i == null);

    }

    public bool CanAddNewItem() {

        if (tickerElements.Count == 0)
            return true;

        var mostRecentItem = tickerElements[0];

        var mostRecentWidth = mostRecentItem.rectTransform.rect.size.x;
        var barWidth = tickerContainer.rect.size.x;

        var mostRecentXPosition = mostRecentItem.rectTransform.anchoredPosition.x;

        if (mostRecentXPosition < barWidth - mostRecentWidth) {
            // We can add a new item 
            return true;
        } else {
            return false;
        }

    }

    public TickerElement AddNewItem(string text) {
        
        var newElement = Instantiate(tickerPrefab);

        newElement.rectTransform.SetParent(tickerContainer.transform, false);

        newElement.text = text;

        var position = new Vector2(tickerContainer.rect.size.x, 0);

        newElement.rectTransform.anchoredPosition = position;

        tickerElements.Add(newElement);

        // sort the list based on descending X position, just to be sure
        tickerElements.Sort( delegate(TickerElement a, TickerElement b) {
            var aX = a.rectTransform.anchoredPosition.x;
            var bX = b.rectTransform.anchoredPosition.x;
            return bX.CompareTo(aX);
        });

        return newElement;

    }

}
