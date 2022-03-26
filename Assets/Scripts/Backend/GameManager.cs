using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class GameManager 
{
    public PBoardViewer BoardViewer;
    public RoundManager CurrentRound;
    public int PlayerCount;
    public int[] PlayersPoints;

    public GameManager(int playerCount)
    {
        BoardViewer = new PBoardViewer(8,4,4);
        PlayerCount = playerCount;
    }
    public bool StartNextRound()
    {
        if (BoardViewer.GetNextBoard())
        {
            CurrentRound = new RoundManager(BoardViewer, PlayerCount);
            return true;
        }
        return false;
    }
}
