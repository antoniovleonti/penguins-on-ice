using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private PBoardViewer boardPlayer;
    private Vector3Int selection;
    private Grid grid;
    private BoardRenderer ui;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        boardPlayer = gameObject.GetComponent<BoardManager>().BoardViewer;
        grid = gameObject.GetComponent<Grid>();
        ui = gameObject.GetComponent<BoardRenderer>();
        cam = Camera.main; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var world = cam.ScreenToWorldPoint(Input.mousePosition);
            var local = grid.WorldToLocal(world);
            selection = grid.LocalToCell(local);
        }
        if (Input.GetMouseButtonUp(0))
        {
            // first grab the location of the mouse
            var world = cam.ScreenToWorldPoint(Input.mousePosition);
            var local = grid.WorldToLocal(world);
            var end = grid.LocalToCell(local);

            // get the coords of the first click
            int startI = Board.CellToCoord(-1 * selection.y);
            int startJ = Board.CellToCoord(selection.x);
            // get the coords of the second click
            int endI = Board.CellToCoord(-1 * end.y);
            int endJ = Board.CellToCoord(end.x);
            // calculate the direction
            int dy = Math.Sign(endI - startI);
            int dx = Math.Sign(endJ - startJ);

            Debug.Log((startI, startJ, dy,dx));
            
            // try to make the move
            bool hitTarget = false;
            try { hitTarget = boardPlayer.MakeMove(startI, startJ, dy, dx); } 
            catch { }
            
            if (hitTarget)  // if they got to the target with the active penguin
            {
                if (!boardPlayer.GetNextBoard()) // if the boardPlayer session is over 
                {
                    Debug.Log("done!");
                }
            }
            ui.Redraw();
            //Debug.Log(grid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)));
        }
    }
}