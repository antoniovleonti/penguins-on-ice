using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    // Start is called before the first frame update
    PBoardViewer boards;
    public int[] PlayerScores;
    PlayerManager pm;
    RoundManager round = null;
    AuctionUIRenderer ui;

    void Awake() // called when script is loaded
    {
        ui = gameObject.GetComponent<AuctionUIRenderer>();
        pm = gameObject.GetComponent<PlayerManager>();
        boards = new PBoardViewer();
        PlayerScores = new int[64];
    }
    void Start() // right before first frame update
    {
        //Forces a single round
        // for (int i = 0; i < 15; i++)
        // {
        //     boards.GetNextBoard();
        // }

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

            //iterate through each player, add entry by entry to saveData
            Scoreboards.ScoreBoardSaveData saveData = new Scoreboards.ScoreBoardSaveData();
            for (int i = 0; i < pm.PlayerCount; i++){
                Scoreboards.ScoreboardEntry entry = new Scoreboards.ScoreboardEntry();

                entry.entryName = $"Player {i+1}";
                Debug.Log($"I = {i}");
                entry.entryScore = pm.Players[i].Wins;
                
                bool scoreAdded = false;
                for (int j = 0; j < saveData.entryList.Count; j++)
                {
                    if(entry.entryScore > saveData.entryList[j].entryScore)
                    {
                        saveData.entryList.Insert(j, entry);
                        scoreAdded = true;
                        break;
                    }
                }

                if (!scoreAdded && saveData.entryList.Count < pm.PlayerCount)
                {
                    saveData.entryList.Add(entry);
                }
            }
            
            //Save saveData to file
            using(StreamWriter stream = new StreamWriter($"{Application.persistentDataPath}/scores.json"))
            {
                string json = JsonUtility.ToJson(saveData, true);
                stream.Write(json);
            }

            SceneManager.LoadScene(3);
            return;
        }
        // if we successfully got another board, start the next round
        round = gameObject.AddComponent<RoundManager>();
        round.Init(boards.CurrentBoard);
    }
    public void EndProofs(int winner)
    {
        if (winner != -1) // -1 == no winner
        {
            pm.Players[winner].Wins++;
            ui.RefreshWins(PlayerScores);
        }
        BroadcastMessage("EndRound");

        StartNextRound();
    }
}
