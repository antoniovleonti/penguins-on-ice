using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private Vector3Int selection;
    private BoardManager manager;
    private Grid grid;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        cam = Camera.main; 
        manager = gameObject.GetComponent<BoardManager>();
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

            // try to make the move
            manager.TryMove(startI, startJ, dy, dx);
        }
    }
}