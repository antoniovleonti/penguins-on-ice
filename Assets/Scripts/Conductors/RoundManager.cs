using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class RoundManager : MonoBehaviour
{
    Board board;
    Auctioneer auctioneer;
    ProofManager proofs;
    BoardRenderer bRenderer;
    FlowManager manager;
    
    // Start is called before the first frame update
    void Awake()
    {
        bRenderer = gameObject.GetComponent<BoardRenderer>();
        manager = gameObject.GetComponent<FlowManager>();

        StartAuction();
    }
    void Start()
    {
    }
    public void Init(Board board_)
    {
        board = board_;
        bRenderer.Redraw(board);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartAuction()
    {
        auctioneer = gameObject.AddComponent<Auctioneer>();
    }
    public void EndAuction(BinaryHeap<(int,int),int> bidQ)
    {
        proofs = gameObject.AddComponent<ProofManager>();
        proofs.Init(board, bidQ);
    }
    public void EndRound()
    {
        Destroy(this);
    }
}
