using System;

public class BoardState 
{
    public int[,] obstacles {get;}
    public int[,] penguins {get;}
    public int[,] targets {get;}
    public int move_count {get; set;}

    public BoardState(int[,] _obstacles, int[,] _penguins, int[,] _targets, int _move_count)
    {
        // copy obstacles array
        obstacles = new int[_obstacles.GetLength(0), _obstacles.GetLength(1)];
        Array.Copy(_obstacles, obstacles, _obstacles.Length);
        // copy penguins array
        // it is assumed that penguins is the same size as obstacles
        penguins = new int[_penguins.GetLength(0), _penguins.GetLength(1)];
        Array.Copy(_penguins, penguins, _penguins.Length);

        targets = new int[_targets.GetLength(0), _targets.GetLength(1)];
        Array.Copy(_targets, targets, _targets.Length);

        move_count = _move_count;
    }

    public bool is_in_bounds(int row, int col)
    {
        return row < obstacles.GetLength(0) && col < obstacles.GetLength(1);
    }

    public bool make_move(int start_row, int start_col, int d_row, int d_col)
    {
        // validate input
        if (!is_in_bounds(start_row, start_col))
            throw new IndexOutOfRangeException("(start_row, start_col) not a valid coordinate.");

        if (!(Math.Abs(d_row) + Math.Abs(d_col) == 1))
            throw new ArgumentException("Either d_row XOR d_col must be in {-1,1}; other must == 0.");

        if (!(penguins[start_row,start_col] > 0))
            throw new ArgumentException("(start_row, start_col) must point to a penguin.");

        // start moving penguin
        int active_penguin = penguins[start_row,start_col];
        int new_row = start_row, new_col = start_col;
        // step penguin until next obstacle or penguin is found
        while ( is_in_bounds(new_row + d_row * 2, new_col + d_col * 2) &&
                penguins[new_row + d_row * 2,new_col + d_col * 2] == 0 &&
                obstacles[new_row + d_row,new_col + d_col] == 0
        ){
            new_row += d_row * 2;
            new_col += d_col * 2;
            
            if (targets[new_row,new_col] == active_penguin)
            {
                break;
            }
        }
        // actually make the move if the penguin moved anywhere
        if (start_row != new_row || start_col != new_col)
        {
            penguins[start_row,start_col] = 0;
            penguins[new_row,new_col] = active_penguin;
            move_count++;
        }
        // return answers question "was this a win?"
        return targets[new_row,new_col] == active_penguin;
    }
}
