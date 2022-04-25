using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public PBoardViewer BoardViewer;
    private HowToPlayInput input;
    private HTPBoardRenderer bRenderer;
    public Board BoardState
    {
        get{return BoardViewer.CurrentBoard;}
    }

    // initialize things that other modules will use HERE vvvv
    void Awake()
    {
        // set up board
        BoardViewer = new PBoardViewer(6, 2, 4);
        BoardViewer.GetNextBoard();
    }
    // Start is called before the first frame update
    void Start()
    {
        // grab bRenderer
        bRenderer = gameObject.GetComponent<HTPBoardRenderer>();
        bRenderer.Redraw(BoardViewer.CurrentBoard);
        input = gameObject.GetComponent<HowToPlayInput>();
    }

    public IEnumerator TryMove(int startY, int startX, int dY, int dX)
    {
        if (!BoardState.IsValidMove(startY,startX,dY,dX)) 
        {
            yield break;
        }

        // calculate move dest and play it in the backend
        int endY,endX; (endY,endX) = BoardState.CalculateMove(startY,startX, dY,dX);
        bool didWin = BoardState.MakeMove(startY,startX,dY,dX);;

        // animate the move and WAIT until it's done.
        input.enabled = false;
        yield return StartCoroutine(
            bRenderer.AnimThenRedraw(startY,endY,startX,endX,BoardState));
        input.enabled = true;

        if (didWin) 
        {   
            if (!BoardViewer.GetNextBoard())
            {
                BoardViewer = new PBoardViewer(6, 2, 4);
                BoardViewer.GetNextBoard();
            }
            bRenderer.Redraw(BoardState);
        }
    }

    public IEnumerator TryMoveQuick(int startY, int startX, int dY, int dX)
    {
        if (!BoardState.IsValidMove(startY,startX,dY,dX)) 
        {
            yield break;
        }

        // calculate move dest and play it
        int endY,endX; (endY,endX) = BoardState.CalculateMove(startY,startX, dY,dX);
        bool didWin = BoardState.MakeMove(startY,startX,dY,dX);

        if (didWin)
        {
            input.enabled = false;
            yield return StartCoroutine(
                bRenderer.AnimThenRedraw(startY,endY,startX,endX,BoardState));
            input.enabled = true;
        }
        else bRenderer.Redraw(BoardState);

        if (didWin) 
        {   
            if (!BoardViewer.GetNextBoard())
            {
                BoardViewer = new PBoardViewer(6, 2, 4);
                BoardViewer.GetNextBoard();
            }
            bRenderer.Redraw(BoardState);
        }
    }
}
