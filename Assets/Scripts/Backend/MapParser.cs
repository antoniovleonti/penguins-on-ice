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
    static Board readMapFile(string mapFilePath) {
        string[] fileLines = System.IO.File.ReadAllLines(mapFilePath);
        int boardSize = int.Parse(fileLines[0]);
        int stringLength = fileLines.Length;

        int[,] obstacles = new int[boardSize, boardSize];
        int[,] penguins = new int[boardSize, boardSize];
        int[,] targets = new int[boardSize, boardSize];
        Board boardMap = new Board(obstacles, penguins, targets);

        //i starts at 1 to skip first line containing boardSize
        for (int i = 1; i < stringLength; i++) {
            string temp = fileLines[i];
            for (int j = 0; j < boardSize; j++){
                //i must be decremented to refer to correct board position
                checkChar(temp[j], i-1, j, boardMap);
            }
        }
        return boardMap;
    }

    /*Input: char x: character read from the file to be evaluated. Will be used to update board
             int i: location of the referred char for first dimension on all arrays in board
             int j: location of the referred char for second dimension on all arrays in board
             BoardState board: BoardState object that will be updated based on the character
    Output: Nothing returned, but board is updated*/
    static void checkChar(char x, int i, int j, Board board) {
        switch (x) {
            case '0':
                board.obstacles[i,j] = 0;
                board.penguins[i,j] = 0;
                board.targets[i,j] = 0;
                break;
            case '1':
                board.obstacles[i,j] = 1;
                break;
            case 'a':
                board.penguins[i,j] = 1;
                break;
            case 'A':
                board.targets[i,j] = 1;
                break;
            default:
                break;
        }
    }
}
