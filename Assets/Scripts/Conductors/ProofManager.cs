using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class ProofManager : MonoBehaviour
{
    BinaryHeap<(int,int),int> bidQ;
    public Board BoardState;
    BoardRenderer bRenderer;
    AuctionUIRenderer ui;
    ProofInput input;
    PlayerManager pm;
    RoundManager manager;
    int currentBid;
    int currentPlayer = -1;
    float timeLeft = 30f;

    // Start is called before the first frame update
    void Awake()
    {
        bRenderer = gameObject.GetComponent<BoardRenderer>();
        manager = gameObject.GetComponent<RoundManager>();
        input = gameObject.GetComponent<ProofInput>();
        pm = gameObject.GetComponent<PlayerManager>();
        ui = gameObject.GetComponent<AuctionUIRenderer>();
    }

    void Start()
    {
        input.enabled = true;
        BroadcastMessage("StartProofs");
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
        timeLeft -= Time.deltaTime;
        timeLeft = timeLeft < 0 ? 0 : timeLeft;
        ui.RefreshTime(timeLeft);

        if (timeLeft <= 0)
        {
            nextBid();
        }
        if (timeLeft < 29.5f && pm.Players[currentPlayer].PollForConcession())
        {
            ui.RefreshPlayer(currentPlayer, pm.Players[currentPlayer]);
            nextBid();
        }    
    }
    void nextBid()
    {
        // reset the last player's player count to zero 
        if (currentPlayer != -1)
        {
            pm.Players[currentPlayer].HasTried = true;
            pm.Players[currentPlayer].IsActive = false;
            pm.Players[currentPlayer].TickerValue = 0;
            ui.RefreshPlayer(currentPlayer,pm.Players[currentPlayer]);
        }

        BoardState = BoardState.GetFirstBoardState();
        bRenderer.Redraw(BoardState);
        bool first = true;
        while (first || pm.Players[currentPlayer].HasTried)
        {
            if (bidQ.Count == 0) 
            {
                BroadcastMessage("EndProofs", -1);
                Destroy(this);
                return;
            }

            (currentPlayer, currentBid) = bidQ.Dequeue();
            first = false;
        }
        timeLeft = 30f;
        ui.RefreshTime(timeLeft);

        pm.Players[currentPlayer].IsActive = true;
        ui.RefreshPlayer(currentPlayer, pm.Players[currentPlayer]);
    }
    public IEnumerator TryMove(int startY, int startX, int dY, int dX)
    {
        if (!BoardState.IsValidMove(startY,startX,dY,dX)) 
        {
            yield break;
        }

        // calculate move dest and play it in the backend
        int endY,endX; (endY,endX) = BoardState.CalculateMove(startY,startX, dY,dX);
        bool didWin = BoardState.MakeMove(startY,startX,dY,dX);
        // update player tracker
        pm.Players[currentPlayer].TickerValue = BoardState.MoveCount;
        ui.RefreshPlayer(currentPlayer,pm.Players[currentPlayer]);

        // animate the move and WAIT until it's done.
        input.enabled = false;
        yield return StartCoroutine(
            bRenderer.AnimThenRedraw(startY,endY,startX,endX,BoardState));
        input.enabled = true;

        
        if (didWin) 
        {   // player actually won
            if (BoardState.MoveCount == currentBid)
            {
                BroadcastMessage("EndProofs", currentPlayer);
                Destroy(this);
            }
            // player's solution was too fast
            else nextBid();
        }
        // player took too many moves
        else if (BoardState.MoveCount >= currentBid)
        {
            nextBid();
        }
    }
    public IEnumerator TryMoveQuick(int startY, int startX, int dY, int dX)
    {
        if (!BoardState.IsValidMove(startY,startX,dY,dX)) 
        {
            yield break;
        }

        // calculate move dest and play it
        int endY,endX; (endY,endX) = BoardState.CalculateMove(startY,startX, dY,dX);
        bool didWin = BoardState.MakeMove(startY,startX,dY,dX);
        // update player ticker
        pm.Players[currentPlayer].TickerValue = BoardState.MoveCount;
        ui.RefreshPlayer(currentPlayer,pm.Players[currentPlayer]);

        // if this move ended the proof, animate it; otherwise 
        // skip to the next boardstate immediately
        if (didWin || BoardState.MoveCount >= currentBid)
        {
            input.enabled = false;
            yield return StartCoroutine(
                bRenderer.AnimThenRedraw(startY,endY,startX,endX,BoardState));
            input.enabled = true;
        }
        else bRenderer.Redraw(BoardState);

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
