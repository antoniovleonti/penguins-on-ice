using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcBoardWrapper : Board
{
    private ProceduralBoard sourceBoard;
    private Board activeBoard;
    public new int[,] Obstacles 
    {
        get { return activeBoard.Obstacles; }
    }
    public new int[,] Penguins
    {
        get { return activeBoard.Penguins; }
    }
    public new int[,] Targets { get { return activeBoard.Targets; } }
    public new int Rows { get { return activeBoard.Rows; } }
    public new int Columns { get { return activeBoard.Columns; } }
    public new int RowCells { get { return activeBoard.RowCells; } }
    public new int ColumnCells { get { return activeBoard.ColumnCells; } }
    private int[,] targetCells;
    private int targetIdx;
    private static System.Random rnd = new System.Random();

    public ProcBoardWrapper()
    {
        while (sourceBoard == null)
        {
            try { sourceBoard = new ProceduralBoard(6); }
            catch {}
        }
        targetCells = new int[16,2];
        Array.Copy(sourceBoard.TargetCells, targetCells, 2*sourceBoard.TargetCount);
        // we want to get the targets in a random order
        Shuffle(targetCells);

        NextBoard();
    }
    public bool NextBoard()
    {
        if (targetIdx == sourceBoard.TargetCount) return false;
        int[,] targetsMap = new int[33,33];

        int i = targetCells[targetIdx,0], j = targetCells[targetIdx,1];
        int I = Board.CellToCoord(i), J = Board.CellToCoord(j);

        targetsMap[I,J] = sourceBoard.Targets[I,J];
        targetIdx++;

        activeBoard = new Board(sourceBoard.Obstacles, sourceBoard.Penguins, targetsMap);
        return true;
    }
    public new bool MakeMove(int startRow, int startCol, int dRow, int dCol)
    {
        return activeBoard.MakeMove(startRow, startCol, dRow, dCol);
    }
    public static void Shuffle(int[,] arr)
    {
        int height = arr.GetUpperBound(0) + 1;
        int width = arr.GetUpperBound(1) + 1;

        for (int i = 0; i < width; ++i)
        {
            int randomRow = rnd.Next(i, height);
            for (int j = 0; j < width; ++j)
            {
                int tmp = arr[i, j];
                arr[i, j] = arr[randomRow, j];
                arr[randomRow, j] = tmp;
            }
        }
    }
}
