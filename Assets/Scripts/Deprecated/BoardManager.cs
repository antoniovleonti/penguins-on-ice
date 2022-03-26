using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public PBoardViewer BoardViewer;
    private BoardRenderer bRenderer;

    // initialize things that other modules will use HERE vvvv
    void Awake()
    {
        // set up board
        BoardViewer = new PBoardViewer(6, 2, 3);
        BoardViewer.GetNextBoard();
    }
    // Start is called before the first frame update
    void Start()
    {
        // grab bRenderer
        bRenderer = gameObject.GetComponent<BoardRenderer>();
        bRenderer.Redraw(BoardViewer.CurrentBoard);
    }

    public void TryMove(int startY, int startX, int dY, int dX)
    {
        var board = BoardViewer.CurrentBoard;
        if (!board.IsValidMove(startY,startX,dY,dX)) 
        {
            Debug.Log("Invalid move");
            return;
        }
        // calculate move dest and animate it
        bool didWin = board.MakeMove(startY,startX,dY,dX);
        if (didWin && !BoardViewer.GetNextBoard())
        {
            bRenderer.EraseBoard();
        }
        // cant use "board" anymore since it's outdated
        else bRenderer.Redraw(BoardViewer.CurrentBoard);
    }
}
