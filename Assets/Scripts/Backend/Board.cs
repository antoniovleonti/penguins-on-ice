using System;
using System.Collections.Generic;
using UnityEngine;

public class Board 
{
    public int[,] Obstacles;
    public int[,] Penguins;
    public int[,] Targets; 
    public int RowCells;
    public int Rows; 
    public int ColumnCells;
    public int Columns;
    public int MoveCount = 0;
    private Stack<Board> previousMoves = new Stack<Board>();

    public Board() { }

    // copy constructor
    public Board(Board b) : this (b.Obstacles, b.Penguins, b.Targets) 
    { 
        MoveCount = b.MoveCount;
        previousMoves = b.previousMoves;
    }

    public Board(int[,] _obstacles, int[,] _penguins, int[,] _targets)
    {
        Rows = _obstacles.GetLength(0);
        Columns = _obstacles.GetLength(1);
        RowCells = (Rows-1) / 2;
        ColumnCells = (Columns-1) / 2;
        // copy obstacles array
        Obstacles = new int[Rows, Columns];
        Array.Copy(_obstacles, Obstacles, Rows * Columns);
        // copy penguins array
        // it is assumed that penguins is the same size as obstacles
        Penguins = new int[Rows, Columns];
        Array.Copy(_penguins, Penguins, Rows * Columns);

        Targets = new int[Rows, Columns];
        Array.Copy(_targets, Targets, Rows * Columns);
    }

    public static int CellToCoord(int cell)
    {
        return cell * 2 + 1;
    }

    public bool CoordIsInBounds(int row, int col)
    {
        return 0 <= row && row < Rows && 
            0 <= col && col < Columns;
    }

    public bool CellIsInBounds(int row, int col)
    {
        return 0 <= row && row < RowCells &&
            0 <= col && col < ColumnCells;
    }
    // calculate destination of move
    public (int,int)? CalculateMove(int startRow, int startCol, int dRow, int dCol)
    {
        // validate input
        if (!CoordIsInBounds(startRow, startCol)) return null;
        if (!(Math.Abs(dRow) + Math.Abs(dCol) == 1)) return null;
        if (!(Penguins[startRow,startCol] > 0)) return null;

        // start moving penguin
        int activePenguin = Penguins[startRow,startCol];
        int newRow = startRow, newCol = startCol;
        // step penguin until next obstacle or penguin is found
        while ( CoordIsInBounds(newRow + dRow * 2, newCol + dCol * 2) &&
                Penguins[newRow + dRow * 2,newCol + dCol * 2] == 0 &&
                Obstacles[newRow + dRow,newCol + dCol] == 0
        ){
            newRow += dRow * 2;
            newCol += dCol * 2;
            
            if (Targets[newRow,newCol] == activePenguin)
                break;
        }
        // simply return the new col and row
        return (newCol, newRow);
    }
    public bool MakeMove(int startRow, int startCol, int dRow, int dCol)
    {
        (int,int)? tmp = CalculateMove(startRow,startCol,dRow,dCol);
        if (tmp == null) return false;
        int newCol, newRow; (newCol,newRow) = tmp ?? (-1,-1);
        
        // return answers question "was this a win?"
        int activePenguin = Penguins[startRow,startCol];
        return Targets[newRow,newCol] == activePenguin;
    }
    public Board? GetLastBoardState()
    {
        if (MoveCount == 0) return null;
        return previousMoves.Pop();
    }
    public Board GetFirstBoardState()
    {
        Board ans = null;
        while (previousMoves.Count > 0) ans = previousMoves.Pop();
        return ans ?? this;
    }
}

public class MoveNotValidException : Exception
{
    public MoveNotValidException() { }

    public MoveNotValidException(string message)
        : base(message)
    {
    }

    public MoveNotValidException(string message, Exception inner)
        : base(message, inner)
    {
    }
}