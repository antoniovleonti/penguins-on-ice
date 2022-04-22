using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class Auctioneer : MonoBehaviour
{
    public int PlayerCount;
    public int[] CurrentBids;
    BinaryHeap<(int,int),int> bidQ;
    public float RemainingSeconds = 60f;
    public RoundManager manager;
    AuctionInput input;
    PlayerManager pm;
    AuctionUIRenderer ui;

    void Awake()
    {
        bidQ = new BinaryHeap<(int,int),int>(PriorityQueueType.Minimum);
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<RoundManager>();

        ui = gameObject.GetComponent<AuctionUIRenderer>(); 
        pm = gameObject.GetComponent<PlayerManager>();
        input = gameObject.GetComponent<AuctionInput>();
        input.enabled = true;

        ui.RefreshTime(RemainingSeconds);

        BroadcastMessage("StartAuction");
    }
    void OnDestroy()
    {
        // if we aren't auctioning, we don't want auction-related input
        input.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bidQ.Count > 0)
        {
            // a bid has been made; start the countdown.
            RemainingSeconds -= Time.deltaTime;
            RemainingSeconds = RemainingSeconds < 0 ? 0 : RemainingSeconds;
            ui.RefreshTime(RemainingSeconds);
        }
        if (RemainingSeconds <= 0) 
        {
            //manager.StartProofs(bidQ);
            // this is more reliable
            BroadcastMessage("EndAuction", bidQ);
            Destroy(this);
        }
    }

    public void AddPlayerBid(int player, int bid)
    {
        // update bid
        bidQ.Enqueue((player,bid),bid); // add player to queue
    }

    public int GetLeadingPlayer ()
    {
        if (bidQ.Count == 0) return -1;
        int player,bid; 
        (player,bid) = bidQ.Peek;
        return player;
    }
}
