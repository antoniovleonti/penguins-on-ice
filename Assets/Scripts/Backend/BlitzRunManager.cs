using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzRunManager 
{
    private ProceduralBoard baseBoard;
    private Board activeBoard;
    public int[,] Obstacles 
    {
        get { return activeBoard.Obstacles; }
    }
    public int[,] Penguins
    {
        get { return activeBoard.Penguins; }
    }
    public int[,] Targets { get { return activeBoard.Targets; } }
    public int Rows { get { return activeBoard.Rows; } }
    public int Columns { get { return activeBoard.Columns; } }
    public int RowCells { get { return activeBoard.RowCells; } }
    public int ColumnCells { get { return activeBoard.ColumnCells; } }
    private int[,] targetCells;
    private int targetIdx;
    private static System.Random rnd = new System.Random();

    public BlitzRunManager()
    {
        while (baseBoard == null)
        {
            try { baseBoard = new ProceduralBoard(4); }
            catch {}
        }
        targetCells = new int[16,2];
        Array.Copy(baseBoard.TargetCells, targetCells, 2*baseBoard.TargetCount);
        Shuffle(targetCells);
    }
    public bool NextBoard()
    {
        if (targetIdx == baseBoard.TargetCount) return false;
        int[,] targetsMap = new int[33,33];

        int i = targetCells[targetIdx,0], j = targetCells[targetIdx,1];
        int I = Board.CellToCoord(i), J = Board.CellToCoord(j);

        targetsMap[I,J] = baseBoard.Targets[I,J];
        targetIdx++;

        activeBoard = new Board(baseBoard.Obstacles, baseBoard.Penguins, targetsMap);
        return true;
    }
    public bool MakeMove(int startRow, int startCol, int dRow, int dCol)
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
