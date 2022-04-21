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
        trackers[player].TickerValue = info.TickerValue;
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
            s = info.Concedes ? "(Ready)" : "(Invalid)";
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

        var newTracker = GameObject.Instantiate(TrackerPrefab, uiParent.transform);
        trackers.Add(new PlayerTracker(newTracker, PopupTextPrefab));
        var world = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,0f,0f));
        var local = uiParent.transform.InverseTransformPoint(world);
        var pos_ = new Vector2(leftx + (count+1) * (trackerWidth + trackerGap), 40f);
        pos_.x = local.x;
        trackers[count].Pos = pos_;

        StartCoroutine(RepositionTrackers(leftx));

    }
    IEnumerator RepositionTrackers (float leftx)
    {
        // reposition existing trackers
        float totalTime = 0.15f;
        float elapsed = 0f;
        int count = trackers.Count;
        Vector2[] endPos = new Vector2[count];
        Vector2[] startPos = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            startPos[i] = trackers[i].Pos;
            endPos[i] = new Vector2(leftx + i * (trackerWidth + trackerGap), 40f);
        }
        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            elapsed = elapsed > 1 ? 1 : elapsed;
            var t = elapsed / totalTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            for (int i = 0; i < count; i++)
            {
                trackers[i].Pos = Vector2.Lerp(startPos[i],endPos[i],t);
                yield return null;
            }
        }
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
    static System.Random rand = new System.Random();
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
                floatingText.GetComponent<TextPopup>().DisplayText =  "+1!";
            }
            wins = value;
            getChildTextField("WINS").text = value.ToString(); 
        }
    }
    int tickerValue;
    public int TickerValue
    {
        get { return tickerValue; }
        set { 
            if (value != 0 && value != tickerValue)
            {
                var tf = getChildTextField("TICKER").gameObject.transform;
                Vector3 offset = Vector3.up * (value > tickerValue ? 1f : -1f);
                float maxLROffset = 0.5f;
                offset += Vector3.right * (float)(rand.NextDouble()*maxLROffset - maxLROffset/2);
                GameObject floatingText = 
                    GameObject.Instantiate( popup, 
                                            tf.position + offset, 
                                            Quaternion.identity, 
                                            tf);
                floatingText.GetComponent<TMP_Text>().color = Color.black;
                var popupComp = floatingText.GetComponent<TextPopup>();
                popupComp.DisplayText = value > tickerValue ? "+" : "-";
                popupComp.Direction = value > tickerValue ? Vector3.up : Vector3.down;
            }
            tickerValue = value;
            getChildTextField("TICKER").text = value.ToString(); 
        }
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
                floatingText.GetComponent<TMP_Text>().color = Color.yellow;
                floatingText.GetComponent<TextPopup>().DisplayText = value.ToString() + "!";
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