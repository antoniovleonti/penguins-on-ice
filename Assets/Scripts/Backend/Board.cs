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

    public bool MakeMove(int startRow, int startCol, int dRow, int dCol)
    {
        // validate input
        if (!CoordIsInBounds(startRow, startCol))
            throw new IndexOutOfRangeException("(startRow, startCol) not a valid coordinate.");

        if (!(Math.Abs(dRow) + Math.Abs(dCol) == 1))
            throw new ArgumentException("Either dRow XOR dCol must be in {-1,1}; other must == 0.");

        if (!(Penguins[startRow,startCol] > 0))
            throw new ArgumentException("(startRow, startCol) must point to a penguin.");

        // start moving penguin
        int active_penguin = Penguins[startRow,startCol];
        int new_row = startRow, new_col = startCol;
        // step penguin until next obstacle or penguin is found
        while ( CoordIsInBounds(new_row + dRow * 2, new_col + dCol * 2) &&
                Penguins[new_row + dRow * 2,new_col + dCol * 2] == 0 &&
                Obstacles[new_row + dRow,new_col + dCol] == 0
        ){
            new_row += dRow * 2;
            new_col += dCol * 2;
            
            if (Targets[new_row,new_col] == active_penguin)
            {
                break;
            }
        }
        // actually make the move if the penguin moved anywhere
        if (startRow != new_row || startCol != new_col)
        {
            Penguins[startRow,startCol] = 0;
            Penguins[new_row,new_col] = active_penguin;
        }
        // return answers question "was this a win?"
        return Targets[new_row,new_col] == active_penguin;
    }
}
