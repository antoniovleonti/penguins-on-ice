using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBoardViewer : Board
{
    private PBoardBuilder boardBuilder;
    private Stack<Board> previousBoards; // allows for undoing moves
    private Board currentBoard;
    public new int[,] Obstacles 
    {
        get { return currentBoard.Obstacles; }
    }
    public new int[,] Penguins
    {
        get { return currentBoard.Penguins; }
    }
    public new int[,] Targets 
    { 
        get { return currentBoard.Targets; } 
    }
    public new int Rows 
    { 
        get { return currentBoard.Rows; } 
    }
    public new int Columns 
    {
        get { return currentBoard.Columns; } 
    }
    public new int RowCells 
    { 
        get { return currentBoard.RowCells; } 
    }
    public new int ColumnCells 
    { 
        get { return currentBoard.ColumnCells; } 
    }
    private int targetIdx;
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
        if (targetIdx == boardBuilder.TargetCount) return false;
        int[,] currentTarget = new int[boardBuilder.Columns,boardBuilder.Rows];

        int i = boardBuilder.TargetCells[targetIdx,0], j = boardBuilder.TargetCells[targetIdx,1];
        int I = Board.CellToCoord(i), J = Board.CellToCoord(j);

        currentTarget[I,J] = boardBuilder.Targets[I,J];
        targetIdx++;

        currentBoard = new Board(boardBuilder.Obstacles, boardBuilder.Penguins, currentTarget);
        return true;
    }
    public new bool MakeMove(int startRow, int startCol, int dRow, int dCol)
    {
        previousBoards.Push(new Board(currentBoard));
        return currentBoard.MakeMove(startRow, startCol, dRow, dCol);
    }
}
