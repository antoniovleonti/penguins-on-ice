using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowManager : MonoBehaviour
{
    // Start is called before the first frame update
    PBoardViewer boards;
    public int[] PlayerScores;
    RoundManager round = null;
    AuctionUIRenderer ui;
    void Start()
    {
        ui = gameObject.GetComponent<AuctionUIRenderer>();
        ui.Init(Gamepad.all.Count);
        boards = new PBoardViewer(5,1,2);
        PlayerScores = new int[64];
        StartNextRound(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNextRound(int winner)
    {
        if (winner != -1) 
        {
            PlayerScores[winner]++;
            Debug.Log("winner's score: "+PlayerScores[winner]);
            ui.RefreshWins(PlayerScores);
        }
        Destroy(round);

        if (!boards.GetNextBoard()) 
        {
            // end of game logic here
            Debug.Log("no more targets to show");
            return;
        }

        round = gameObject.AddComponent<RoundManager>();
        round.Init(boards.CurrentBoard);
    }
}
