using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class ProofManager : MonoBehaviour
{
    BinaryHeap<(int,int),int> bidQ;
    bool[] hasTried;
    public Board BoardState;
    BoardRenderer bRenderer;
    AuctionUIRenderer ui;
    ProofInput input;
    PlayerManager pm;
    RoundManager manager;
    int currentBid;
    int currentPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        bRenderer = gameObject.GetComponent<BoardRenderer>();
        manager = gameObject.GetComponent<RoundManager>();
        input = gameObject.GetComponent<ProofInput>();
        pm = gameObject.GetComponent<PlayerManager>();
        ui = gameObject.GetComponent<AuctionUIRenderer>();
        hasTried = new bool[64];
    }

    void Start()
    {
        input.enabled = true;
        nextBid();
    }
    void OnDestroy()
    {
        input.enabled = false;
    }
    public void Init(Board BoardState_, BinaryHeap<(int,int),int> bidQ_)
    {
        BoardState = BoardState_;
        bidQ = bidQ_;
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.Players[currentPlayer].PollForConcession())
        {
            pm.Players[currentPlayer].Status = "(Gave up)";
            ui.RefreshPlayer(currentPlayer, pm.Players[currentPlayer]);
            nextBid();
        }    
    }
    void nextBid()
    {
        BoardState = BoardState.GetFirstBoardState();
        bRenderer.Redraw(BoardState);
        bool first = true;
        while (first || hasTried[currentPlayer])
        {
            if (bidQ.Count == 0) 
            {
                BroadcastMessage("EndProofs", -1);
                Destroy(this);
                break;
            }

            (currentPlayer, currentBid) = bidQ.Dequeue();
            first = false;
        }
        hasTried[currentPlayer] = true;
        Debug.Log("Current bid: player " + currentPlayer + " for " + currentBid);
    }
    public void TryMove(int startY, int startX, int dY, int dX)
    {
        if (!BoardState.IsValidMove(startY,startX,dY,dX)) 
        {
            return;
        }
        StartCoroutine(MakeMove(startY,startX,dY,dX));
    }
    IEnumerator MakeMove (int startY, int startX, int dY, int dX)
    {
        // calculate move dest and animate it
        int endY,endX; (endY,endX) = BoardState.CalculateMove(startY,startX, dY,dX);
        // TODO: bRenderer.AnimateMove()

        bool didWin = BoardState.MakeMove(startY,startX,dY,dX);
        // animate the move and WAIT until it's done.
        input.enabled = false;
        yield return StartCoroutine(
            bRenderer.AnimThenRedraw(startY,endY,startX,endX,BoardState));
        input.enabled = true;

        if (didWin) 
        {
            if (BoardState.MoveCount == currentBid)
            {
                BroadcastMessage("EndProofs", currentPlayer);
                Destroy(this);
            }
            else nextBid();
        }
        else if (BoardState.MoveCount >= currentBid)
        {
            nextBid();
        }
    }
}
