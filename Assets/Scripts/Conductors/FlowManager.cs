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
        StartNextRound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNextRound()
    {
        if (!boards.GetNextBoard()) 
        {
            // end of game logic here
            // TODO: go to scoreboard
            return;
        }
        // if we successfully got another board, start the next round
        round = gameObject.AddComponent<RoundManager>();
        Debug.Log(round);
        round.Init(boards.CurrentBoard);
    }
    public void EndProofs(int winner)
    {
        if (winner != -1) // -1 == no winner
        {
            PlayerScores[winner]++;
            ui.RefreshWins(PlayerScores);
        }
        BroadcastMessage("EndRound");

        StartNextRound();
    }
}