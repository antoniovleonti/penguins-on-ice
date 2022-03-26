using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using PriorityQueues;

public class RoundManager 
{
    public int PlayerCount;
    public Board BoardState;
    public int[] PlayerBids;
    public bool[] PlayerHasShownBid;
    public BinaryHeap<int,int> BidQ = 
        new BinaryHeap<int,int>(PriorityQueueType.Minimum);
    public TimeSpan ElapsedTime
    {
        get { return stopwatch.Elapsed; }
    }
    private Stopwatch stopwatch = new Stopwatch();

    public RoundManager(Board board, int playerCount)
    {
        BoardState = board;
        PlayerCount = playerCount;
        PlayerBids = new int[PlayerCount];
        PlayerHasShownBid = new bool[PlayerCount];
    }

    public bool TryPlayerBid(int player, int bid)
    {
        // make sure it even makes sense to make the bid
        if (bid >= PlayerBids[player]) return false;
        // then make the bid
        PlayerBids[player] = bid;
        BidQ.Enqueue(player, bid);
        return true; // signal it worked
    }
}
