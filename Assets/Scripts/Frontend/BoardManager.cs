using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public ProcBoardWrapper PBoard;
    public int?[] PlayersLowestBids;
    public int[] PlayersPoints;
    public int PlayerCount;
    public int Phase; // 0, 1, 2... for pre-bid (bid with no timer), after the first bid, proof, end screen

    // initialize things that other modules will use HERE vvvv
    void Awake()
    {
        PBoard = new ProcBoardWrapper();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    void StartBidPhase()
    {
        // timer
        
    }
    void StartProofStage()
    {

    }
}
