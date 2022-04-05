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
    float trackerGap = 20f;
    float trackerWidth;
    // Start is called before the first frame update
    void Awake()
    {
        // make a new gameobject, add canvas and canvas scaler components
        uiParent = new GameObject("ui canvas", typeof(Canvas), typeof(CanvasScaler));
        uiParent.transform.SetParent(gameObject.transform);

        var canvas = uiParent.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera; // snap to camera
        canvas.worldCamera = Camera.main; // ... the main camera

        var scaler = uiParent.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.matchWidthOrHeight = 0.5f; // width and height are weighted equally

        trackers = new List<PlayerTracker>();

        // precompute size of trackers
        var prefabXform = TrackerPrefab.GetComponent<RectTransform>();
        trackerWidth = prefabXform.rect.width * prefabXform.transform.localScale.x;

        // get the clock ready
        InitClock();
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

    public void RefreshTickerValues(int[] tickers)
    {
        for (int i = 0; i < trackers.Count; i++)
        {
            // you can't bid zero so replace 0 with another char
            string s = tickers[i]==0 ? "*" : tickers[i].ToString();
            trackers[i].TickerValue = s;
        }
    }

    public void RefreshCurrentBids(int[] bids)
    {
        for (int i = 0; i < trackers.Count; i++)
        {
            // zero means no bid
            string s = bids[i]==0 ? "*" : bids[i].ToString();
            trackers[i].CurrentBid = s;
        }
    }

    public void RefreshPlayer(int player, Player info)
    {
        trackers[player].Wins = info.Wins.ToString();
        trackers[player].TickerValue = info.TickerValue.ToString();
        trackers[player].CurrentBid = info.CurrentBid.ToString();
        trackers[player].Name = info.Name;
    }

    public void InitClock () 
    {
        var go = GameObject.Instantiate(ClockPrefab, uiParent.transform);
        var xform = go.GetComponent<RectTransform>();
        // anchor is set up in the prefab already, just need to position
        xform.anchoredPosition = new Vector2(0, -20f);
        //
        clock = new ClockDisplay(go);
    }

    public void AddTracker ()
    {
        var count = trackers.Count;
        float leftx = -((count) * trackerWidth + count * trackerGap) / 2;
        Debug.Log(leftx);
        
        // reposition existing trackers
        for (int i = 0; i < count; i++)
        {
            var pos = new Vector2(leftx + i * (trackerWidth + trackerGap), 30f);
            trackers[i].Pos = pos;
        }
        // make a new one for player p
        var newTracker = GameObject.Instantiate(TrackerPrefab, uiParent.transform);
        trackers.Add(new PlayerTracker(newTracker));

        var pos_ = new Vector2(leftx + count * (trackerWidth + trackerGap), 30f);
        trackers[count].Pos = pos_;
    }

    private void eraseUI ()
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
    public string TickerValue
    {
        get { return getChildTextField("TICKER").text; }
        set { getChildTextField("TICKER").text = value; }
    }
    public string CurrentBid
    {
        get { return getChildTextField("BID").text; }
        set { getChildTextField("BID").text = value; }
    }
    public Vector2 Pos
    {
        get { return go.GetComponent<RectTransform>().anchoredPosition; }
        set { go.GetComponent<RectTransform>().anchoredPosition = value; }
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