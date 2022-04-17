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
    public int MoveCount 
    {
        get { return previousMoves.Count; }
    }
    private Stack<Board> previousMoves = new Stack<Board>();

    public Board() { }

    // copy constructor
    public Board(Board b) : this (b.Obstacles, b.Penguins, b.Targets) 
    { 
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

    public bool CoordIsInBounds(int col, int row)
    {
        return 0 <= row && row < Rows && 
            0 <= col && col < Columns;
    }

    public bool CellIsInBounds(int col, int row)
    {
        return 0 <= row && row < RowCells &&
            0 <= col && col < ColumnCells;
    }
    public bool IsValidMove(int startCol, int startRow, int dCol, int dRow)
    {
        if (CoordIsInBounds(startCol, startRow) && 
            Math.Abs(dRow) + Math.Abs(dCol) == 1 && 
            Penguins[startCol,startRow] > 0)
        {
            return true;
        }
        else return false;
    }
    
    // calculate destination of move
    public (int,int) CalculateMove(int startCol, int startRow, int dCol, int dRow)
    {
        // validate input
        if (!IsValidMove(startCol,startRow,dCol,dRow))
        {
            throw new Exception("Cannot calculate destination of invalid move!");
        }

        // start moving penguin
        int activePenguin = Penguins[startCol,startRow];
        int newRow = startRow, newCol = startCol;
        // step penguin until next obstacle or penguin is found
        bool CanMoveOneMoreSquare(int curY, int curX)
        {
            return (
                CoordIsInBounds(curY + dCol * 2, curX + dRow * 2) &&
                Penguins[curY + dCol * 2, curX + dRow * 2] == 0 &&
                Obstacles[curY + dCol, curX + dRow] == 0
            );
        }
        while (CanMoveOneMoreSquare(newCol,newRow))
        {
            newCol += dCol * 2;
            newRow += dRow * 2;
            
            if (Targets[newCol,newRow] == activePenguin)
                break;
        }
        // simply return the new col and row
        return (newCol, newRow);
    }
    public bool MakeMove(int startCol, int startRow, int dCol, int dRow)
    {
        (int,int) tmp = CalculateMove(startCol,startRow,dCol,dRow);
        int newCol, newRow; (newCol,newRow) = tmp; 

        if (startCol == newCol && startRow == newRow)
        {
            // The penguin didn't move. Do nothing.
            // we'll assume they haven't won yet since they're still playing.
            return false; 
        }
        previousMoves.Push(new Board(this));

        int activePenguin = Penguins[startCol,startRow];
        Penguins[startCol,startRow] = 0;
        Penguins[newCol,newRow] = activePenguin;
        
        // return answers question "was this a win?"
        return Targets[newCol,newRow] == activePenguin;
    }

    // board history functions
    public Board PopLastBoardState()
    {
        if (MoveCount == 0) return null;
        return previousMoves.Peek();
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