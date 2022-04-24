using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class RoundManager : MonoBehaviour
{
    Board board;
    ProofManager proofs;
    BoardRenderer bRenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        bRenderer = gameObject.GetComponent<BoardRenderer>();
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
