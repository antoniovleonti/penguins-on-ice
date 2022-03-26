using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class Auctioneer : MonoBehaviour
{

    public int PlayerCount;
    int[] CurrentBids;
    BinaryHeap<int,int> bidQ;
    float RemainingSeconds;
    RoundManager manager;


    // Start is called before the first frame update
    void Start()
    {
        bidQ = new BinaryHeap<int,int>(PriorityQueueType.Minimum);
        manager = gameObject.GetComponent<RoundManager>();
    }
    public void Init(int playerCount, float startingTime)
    {
        PlayerCount = playerCount;
        RemainingSeconds = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (bidQ.Count > 0)
        {
            // a bid has been made; start the countdown.
            RemainingSeconds -= Time.deltaTime;
        }
        if (RemainingSeconds <= 0) 
        {
            manager.StartProofs(bidQ);
        }
    }

    public void AddPlayerBid(int player, int bid)
    {
        if (bid < CurrentBids[player]) return;
        // update bid
        CurrentBids[player] = bid;
        bidQ.Enqueue(player,bid); // add player to queue
    }
}
