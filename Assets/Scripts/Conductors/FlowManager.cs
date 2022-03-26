using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    // Start is called before the first frame update
    PBoardViewer boards;
    public int PlayerCount;
    int[] playerScores;
    RoundManager round = null;
    void Start()
    {
        boards = new PBoardViewer();
        playerScores = new int[PlayerCount];
        StartNextRound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNextRound()
    {
        if (round != null) { Destroy(round); }

        boards.GetNextBoard();

        round = gameObject.AddComponent<RoundManager>();
        round.Init(boards.CurrentBoard, PlayerCount);
    }
}
