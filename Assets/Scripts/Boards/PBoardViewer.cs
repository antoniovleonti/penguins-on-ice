using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBoardViewer 
{
    public Board CurrentBoard;
    private PBoardBuilder boardBuilder;
    private int currentTarget;
    private static System.Random rnd = new System.Random();

    public PBoardViewer () : this (8,4,4)
    {
        
    }
    public PBoardViewer (int quadSize, int quadLWalls, int penguinCount)
    {
        while (boardBuilder == null)
        {
            int failedAttempts = 0;
            try 
            {
                boardBuilder = new PBoardBuilder(quadSize, quadLWalls, penguinCount); 
            }
            catch 
            {
                failedAttempts++; 
                if (failedAttempts >= 32)
                {
                    Debug.Log("too many failed attempts");
                    break;
                }
            }
        }
    }
    public bool GetNextBoard()
    {
        if (currentTarget == boardBuilder.TargetCount) return false;
        int[,] currentTargetArr = new int[boardBuilder.Columns,boardBuilder.Rows];

        int i = boardBuilder.TargetCells[currentTarget,0]; 
        int j = boardBuilder.TargetCells[currentTarget,1];
        int I = Board.CellToCoord(i), J = Board.CellToCoord(j);

        currentTargetArr[I,J] = boardBuilder.Targets[I,J];
        currentTarget++;

        CurrentBoard = new Board(boardBuilder.Obstacles, boardBuilder.Penguins, currentTargetArr);
        return true;
    }
}
