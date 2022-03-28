using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class ProofManager : MonoBehaviour
{
    BinaryHeap<(int,int),int> bidQ;
    bool[] hasTried;
    Board board;
    BoardRenderer bRenderer;
    ProofInput input;
    RoundManager manager;
    int currentBid;
    int currentPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        bRenderer = gameObject.GetComponent<BoardRenderer>();
        manager = gameObject.GetComponent<RoundManager>();
        input = gameObject.AddComponent<ProofInput>();
        hasTried = new bool[64];
    }

    void Start()
    {
        nextBid();
    }
    void OnDestroy()
    {
        input.enabled = false;
    }
    public void Init(Board board_, BinaryHeap<(int,int),int> bidQ_)
    {
        board = board_;
        bidQ = bidQ_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void nextBid()
    {
        board = board.GetFirstBoardState();
        bRenderer.Redraw(board);
        bool first = true;
        while (first || hasTried[currentPlayer])
        {
            if (bidQ.Count == 0) 
            {
                manager.FinishProofs(-1);
                break;
            }

            (currentPlayer, currentBid) = bidQ.Dequeue();
            first = false;
        }
        Debug.Log("Current bid: player " + currentPlayer + " for " + currentBid);
    }
    public void TryMove(int startY, int startX, int dY, int dX)
    {
        if (!board.IsValidMove(startY,startX,dY,dX)) 
        {
            return;
        }
        // calculate move dest and animate it
        int y,x; (y,x) = board.CalculateMove(startY,startX, dY,dX);
        // TODO: bRenderer.AnimateMove()

        bool didWin = board.MakeMove(startY,startX,dY,dX);
        if (didWin) 
        {
            if (board.MoveCount == currentBid)
                manager.FinishProofs(currentPlayer);
            // winning logic: probably just letting manager know the winner
            else nextBid();
        }
        else if (board.MoveCount >= currentBid)
        {
            nextBid();
        }
        else bRenderer.Redraw(board);
    }
}
