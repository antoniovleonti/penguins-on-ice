using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapParser 
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    //Input: String mapFilePath: location of map file, may be relative or absolute
    //Output: BoardState boardMap: BoardState object reflecting the specified map file
    static Board ReadMapFile(string MapFilePath)
    {
        string[] fileLines = System.IO.File.ReadAllLines(MapFilePath);
        int boardSize = int.Parse(fileLines[0]);
        int stringLength = fileLines.Length;

        int[,] obstacles = new int[boardSize, boardSize];
        int[,] penguins = new int[boardSize, boardSize];
        int[,] targets = new int[boardSize, boardSize];
        Board BoardMap = new Board(obstacles, penguins, targets);

        //i starts at 1 to skip first line containing boardSize
        for (int i = 1; i < stringLength; i++)
        {
            string temp = fileLines[i];
            for (int j = 0; j < boardSize; j++)
            {
                //i must be decremented to refer to correct board position
                checkChar(temp[j], i-1, j, BoardMap);
            }
        }
        return BoardMap;
    }

    /*Input: char x: character read from the file to be evaluated. Will be used to update board
             int i: location of the referred char for first dimension on all arrays in board
             int j: location of the referred char for second dimension on all arrays in board
             BoardState board: BoardState object that will be updated based on the character
    Output: Nothing returned, but board is updated*/
    private static void checkChar(char x, int i, int j, Board board)
    {
        if (x == ' ')
        {
            board.Obstacles[i,j] = 0;
            board.Penguins[i,j] = 0;
            board.Targets[i,j] = 0;
        }
        else if (Char.IsNumber(x))
        {
            board.Obstacles[i,j] = x - '0';
        }
        else if (Char.IsLower(x))
        {
            board.Penguins[i,j] = x;
        }
        else if (Char.IsUpper(x))
        {
            board.Targets[i,j] = x;
        } else
        {
            //throw new FormatException.FormatException("FORMAT EXCEPTION: Map File contains a character ('"+x+"') that is not recognized");
            Debug.Log("FORMAT EXCEPTION: Map File contains a character ('"+x+"') that is not recognized");
        }
        
        switch (x) {
            case '0':
                board.Obstacles[i,j] = 0;
                board.Penguins[i,j] = 0;
                board.Targets[i,j] = 0;
                break;
            case '1':
                board.Obstacles[i,j] = 1;
                break;
            case 'a':
                board.Penguins[i,j] = 1;
                break;
            case 'A':
                board.Targets[i,j] = 1;
                break;
            default:
                break;
        }
    }
}
