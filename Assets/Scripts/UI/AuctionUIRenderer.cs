using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuctionUIRenderer : MonoBehaviour
{
    public GameObject TrackerPrefab;
    public GameObject ClockPrefab;
    GameObject uiParent;
    List<PlayerTracker> trackers;
    ClockDisplay clock;
    // Start is called before the first frame update
    void Awake()
    {
        uiParent = new GameObject("ui canvas", typeof(Canvas), typeof(CanvasScaler));
        uiParent.transform.SetParent(gameObject.transform);

        var canvas = uiParent.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        var scaler = uiParent.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.matchWidthOrHeight = 0.5f;

        trackers = new List<PlayerTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RefreshTime(float seconds)
    {
        clock.SetTime(seconds);
    }
    public void RefreshNames(string[] names)
    {
        for (int i = 0; i < trackers.Count; i++)
            trackers[i].Name = names[i];
    }
    public void RefreshWins(int[] wins)
    {
        for (int i = 0; i < trackers.Count; i++)
            trackers[i].Wins = wins[i].ToString();
    }

    public void RefreshTickers(int[] tickers)
    {
        for (int i = 0; i < trackers.Count; i++)
        {
            string s = tickers[i]==0 ? "*" : tickers[i].ToString();
            trackers[i].Ticker = s;
        }
    }

    public void RefreshBids(int[] bids)
    {
        for (int i = 0; i < trackers.Count; i++)
        {
            string s = bids[i]==0 ? "*" : bids[i].ToString();
            trackers[i].Bid = s;
        }
    }

    public void Init(int playerCount) 
    {
        eraseUI(); // start clean
        float gap = 20f;
        var prefabXform = TrackerPrefab.GetComponent<RectTransform>();
        float trackerWidth = prefabXform.rect.width * prefabXform.transform.localScale.x;
        float leftx = - (playerCount * trackerWidth/2 + (playerCount-1) * gap) / 2;
        for (int i = 0; i < playerCount; i++)
        {
            // add a ui element to track this player
            var tracker = GameObject.Instantiate(TrackerPrefab, uiParent.transform);
            var xform = tracker.GetComponent<RectTransform>();
            // anchor is set up in the prefab already, just need to position
            var pos = new Vector2(leftx + i * (trackerWidth + gap), 30f);
            xform.anchoredPosition = pos;

            trackers.Add(new PlayerTracker(tracker));
            trackers[i].Name = "Player " + (i+1);
        }
        // set up the clock
        var clockGO = GameObject.Instantiate(ClockPrefab, uiParent.transform);
        var clockXform = clockGO.GetComponent<RectTransform>();
        // anchor is set up in the prefab already, just need to position
        var clockPos = new Vector2(0, -20f);
        clockXform.anchoredPosition = clockPos;
        //
        clock = new ClockDisplay(clockGO);
        
    }

    private void eraseUI()
    {
        // delete all children of the parent game object
        int n = uiParent.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject.Destroy(uiParent.transform.GetChild(i).gameObject);
        }
        trackers.Clear();
    }
}

class PlayerTracker
{
    GameObject go;
    public PlayerTracker(GameObject go_)
    {
        go = go_;
    }
    public string Name
    {
        get { return getChildTextField("NAME").text; }
        set { getChildTextField("NAME").text = value; }
    }
    public string Wins
    {
        get { return getChildTextField("WINS").text; }
        set { getChildTextField("WINS").text = value; }
    }
    public string Ticker
    {
        get { return getChildTextField("TICKER").text; }
        set { getChildTextField("TICKER").text = value; }
    }
    public string Bid
    {
        get { return getChildTextField("BID").text; }
        set { getChildTextField("BID").text = value; }
    }
    private TextMeshProUGUI getChildTextField(string childName)
    {

        GameObject child = go.transform.Find(childName).gameObject;
        return child.GetComponent<TextMeshProUGUI>();
    }
}

class ClockDisplay
{
    GameObject go;
    public ClockDisplay(GameObject go_)
    {
        go = go_;
    }
    public void SetTime (float s)
    {
        var str = string.Format("{0:f}", s);
        getChildTextField("TIME").text = str;
    }
    private TextMeshProUGUI getChildTextField(string childName)
    {

        GameObject child = go.transform.Find(childName).gameObject;
        return child.GetComponent<TextMeshProUGUI>();
    }
}