using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuctionUIRenderer : MonoBehaviour
{
    public GameObject TrackerPrefab;
    public GameObject ClockPrefab;
    public GameObject PhaseDisplayPrefab;
    public GameObject PopupTextPrefab;
    GameObject uiParent;
    List<PlayerTracker> trackers;
    ClockDisplay clock;
    PhaseDisplay phaseDisplay;
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
        InitPhaseDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartAuction ()
    {
        RefreshPhase("Bidding phase");
    }
    void StartProofs ()
    {
        RefreshPhase("Proofs phase");
    }
    public void RefreshTime(float seconds)
    {
        clock.SetTime(seconds);
    }

    public void RefreshPhase(string phase)
    {
        phaseDisplay.Text = phase;
    }
    public void RefreshWins(int[] wins)
    {
        for (int i = 0; i < trackers.Count; i++)
            trackers[i].Wins = wins[i];
    }



    public void RefreshPlayer(int player, Player info)
    {
        trackers[player].Wins = info.Wins;
        trackers[player].TickerValue = info.TickerValue.ToString();
        trackers[player].CurrentBid = info.CurrentBid;
        trackers[player].Name = info.Name;

        Color c;
        string s;
        if (info.IsActive) 
        {
            c = Color.green;
            s = "(Showing)";
        }
        else if (info.Concedes || info.HasTried) 
        {
            c = Color.gray;
            s = info.Concedes ? "(Concedes)" : "(Invalid)";
        }
        else 
        {
            c = Color.white;
            s = "";
        }

        trackers[player].NamePlateColor = c;
        trackers[player].Status = s;
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

    public void InitPhaseDisplay () 
    {
        var go = GameObject.Instantiate(PhaseDisplayPrefab, uiParent.transform);
        // anchor is set up in the prefab already, just need to position
        var xform = go.GetComponent<RectTransform>();
        xform.anchoredPosition = new Vector2(20f, -20f);
        //
        phaseDisplay = new PhaseDisplay(go);
    }

    public void AddTracker ()
    {
        var count = trackers.Count;
        float leftx = -((count) * trackerWidth + count * trackerGap) / 2;
        
        // reposition existing trackers
        for (int i = 0; i < count; i++)
        {
            var pos = new Vector2(leftx + i * (trackerWidth + trackerGap), 30f);
            trackers[i].Pos = pos;
        }
        // make a new one for player p
        var newTracker = GameObject.Instantiate(TrackerPrefab, uiParent.transform);
        trackers.Add(new PlayerTracker(newTracker, PopupTextPrefab));

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
    GameObject popup;
    public PlayerTracker(GameObject go_, GameObject popupTextPrefab)
    {
        go = go_;
        popup = popupTextPrefab;
    }
    public string Name
    {
        get { return getChildTextField("NAME").text; }
        set { getChildTextField("NAME").text = value; }
    }
    public string Status
    {
        get {return getChildTextField("STATUS").text; }
        set { getChildTextField("STATUS").text = value; }
    }
    int wins;
    public int Wins
    {
        get { return wins; }
        set { 
            if (value != wins)
            {
                var tf = getChildTextField("WINS").gameObject.transform;
                GameObject floatingText = 
                    GameObject.Instantiate(popup, tf.position + Vector3.up*1f, Quaternion.identity, tf);
                floatingText.GetComponent<TMP_Text>().color = Color.blue;
                floatingText.GetComponent<TextPopup>().displayText =  "+1!";
            }
            wins = value;
            getChildTextField("WINS").text = value.ToString(); 
        }
    }
    public string TickerValue
    {
        get { return getChildTextField("TICKER").text; }
        set { getChildTextField("TICKER").text = value; }
    }
    int currentBid;
    public int CurrentBid
    {
        get { return currentBid; }
        set { 
            if (value != 0 && value != currentBid)
            {
                var tf = getChildTextField("BID").gameObject.transform;
                GameObject floatingText = 
                    GameObject.Instantiate(popup, tf.position + Vector3.up*1f, Quaternion.identity, tf);
                floatingText.GetComponent<TMP_Text>().color = Color.green;
                floatingText.GetComponent<TextPopup>().displayText = value.ToString() + "!";
            }
            currentBid = value;
            getChildTextField("BID").text = value.ToString(); 
        }
    }
    public Vector2 Pos
    {
        get { return go.GetComponent<RectTransform>().anchoredPosition; }
        set { go.GetComponent<RectTransform>().anchoredPosition = value; }
    }
    public Color NamePlateColor
    {
        get {
            var plateGO = go.transform.Find("NAMEPLATE").gameObject;
            return plateGO.GetComponent<Image>().color;
        }
        set {
            var plateGO = go.transform.Find("NAMEPLATE").gameObject;
            plateGO.GetComponent<Image>().color = value;
        }
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

class PhaseDisplay
{
    GameObject go;
    public string Text
    {
        get { return getChildTextField("PHASE").text; }
        set { getChildTextField("PHASE").text = value; }
    }
    public PhaseDisplay (GameObject go)
    {
        this.go = go;
    }
    TextMeshProUGUI getChildTextField(string childName)
    {
        GameObject child = go.transform.Find(childName).gameObject;
        return child.GetComponent<TextMeshProUGUI>();
    }
    
}