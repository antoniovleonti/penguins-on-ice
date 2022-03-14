using System;
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

    public Board()
    {}

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

    public bool make_move(int start_row, int start_col, int d_row, int d_col)
    {
        // validate input
        if (!CoordIsInBounds(start_row, start_col))
            throw new IndexOutOfRangeException("(start_row, start_col) not a valid coordinate.");

        if (!(Math.Abs(d_row) + Math.Abs(d_col) == 1))
            throw new MoveNotValidException("Either d_row XOR d_col must be in {-1,1}; other must == 0.");

        if (!(Penguins[start_row,start_col] > 0))
            throw new ArgumentException("(start_row, start_col) must point to a penguin.");

        // start moving penguin
        int active_penguin = Penguins[start_row,start_col];
        int new_row = start_row, new_col = start_col;
        // step penguin until next obstacle or penguin is found
        while ( CoordIsInBounds(new_row + d_row * 2, new_col + d_col * 2) &&
                Penguins[new_row + d_row * 2,new_col + d_col * 2] == 0 &&
                Obstacles[new_row + d_row,new_col + d_col] == 0
        ){
            new_row += d_row * 2;
            new_col += d_col * 2;
            
            if (Targets[new_row,new_col] == active_penguin)
            {
                break;
            }
        }
        // actually make the move if the penguin moved anywhere
        if (start_row != new_row || start_col != new_col)
        {
            Penguins[start_row,start_col] = 0;
            Penguins[new_row,new_col] = active_penguin;
        }
        // return answers question "was this a win?"
        return Targets[new_row,new_col] == active_penguin;
    }
}

public class MoveNotValidException : Exception
{
    public MoveNotValidException()
    {
    }

    public MoveNotValidException(string message)
        : base(message)
    {
    }

    public MoveNotValidException(string message, Exception inner)
        : base(message, inner)
    {
    }
}