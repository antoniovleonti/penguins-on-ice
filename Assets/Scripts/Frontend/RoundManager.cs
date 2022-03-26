using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class RoundManager : MonoBehaviour
{
    int playerCount;
    Board board;
    Auctioneer auctioneer;
    ProofManager proofs;
    BoardRenderer bRenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        bRenderer = gameObject.GetComponent<BoardRenderer>();
    }
    public void Init(Board board_, int playerCount_)
    {
        board = board_;
        playerCount = playerCount_;
        bRenderer.Redraw(board);
        StartAuction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartAuction()
    {
        auctioneer = gameObject.AddComponent<Auctioneer>();
        auctioneer.Init(playerCount, 90f);
    }
    public void StartProofs(BinaryHeap<int,int> bidQ)
    {
        // start by ending the auction
        if (auctioneer != null) Destroy(auctioneer);

        proofs = gameObject.AddComponent<ProofManager>();
        proofs.Init(board, bidQ);
    }
}
