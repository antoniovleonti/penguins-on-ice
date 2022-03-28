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

    void Awake() // called when script is loaded
    {
        ui = gameObject.GetComponent<AuctionUIRenderer>();
        boards = new PBoardViewer();
        PlayerScores = new int[64];
    }
    void Start() // right before first frame update
    {
        ui.Init(Gamepad.all.Count);
        StartNextRound(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNextRound(int winner)
    {
        if (winner != -1) // -1 == no winner
        {
            PlayerScores[winner]++;
            ui.RefreshWins(PlayerScores);
        }
        Destroy(round); // this round manager has done its job.

        if (!boards.GetNextBoard()) 
        {
            // end of game logic here
            // TODO: go to scoreboard
            return;
        }
        // if we successfully got another board, start the next round
        round = gameObject.AddComponent<RoundManager>();
        round.Init(boards.CurrentBoard);
    }
}
