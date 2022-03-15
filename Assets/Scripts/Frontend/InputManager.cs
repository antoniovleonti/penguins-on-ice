using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private BlitzRunManager blitz;
    private Vector3Int selection;
    private Grid grid;
    private UIManager ui;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        blitz = gameObject.GetComponent<BackendManager>().Blitz;
        grid = gameObject.GetComponent<Grid>();
        ui = gameObject.GetComponent<UIManager>();
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
            try { hitTarget = blitz.MakeMove(startI, startJ, dy, dx); } 
            catch { }
            
            if (hitTarget)  // if they got to the target with the active penguin
            {
                if (!blitz.NextBoard()) // if the blitz session is over 
                {
                    Debug.Log("done!");
                }
            }
            ui.Redraw();
            //Debug.Log(grid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)));
        }
    }
}