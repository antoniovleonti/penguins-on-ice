using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProofInput : MonoBehaviour
{
    private Vector3Int selection;
    private ProofManager manager;
    private Grid grid;
    private Camera cam;
    BoardRenderer bRenderer;
    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        cam = Camera.main; 
    }
    void OnEnable()
    {
        // this is where the result of player input goes
        manager = gameObject.GetComponent<ProofManager>();
        bRenderer = gameObject.GetComponent<BoardRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var mousePos = Mouse.current.position;
            var vec = new Vector3(  mousePos.x.ReadValue(), 
                                    mousePos.y.ReadValue(), 
                                    0 );
            var world = cam.ScreenToWorldPoint(vec);
            var local = grid.WorldToLocal(world);
            selection = grid.LocalToCell(local);

            StartCoroutine(bRenderer.MouseDragFeedback(-selection.y, selection.x, manager.BoardState));
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // first grab the location of the mouse
            var mousePos = Mouse.current.position;
            var vec = new Vector3(  mousePos.x.ReadValue(), 
                                    mousePos.y.ReadValue(), 
                                    0 );
            var world = cam.ScreenToWorldPoint(vec);
            var local = grid.WorldToLocal(world);
            var end = grid.LocalToCell(local);

            // get the coords of the first click
            int startI = Board.CellToCoord(-1 * selection.y);
            int startJ = Board.CellToCoord(selection.x);
            // get the coords of the second click
            int endI = Board.CellToCoord(-1 * end.y);
            int endJ = Board.CellToCoord(end.x);

            //Improve Mouse Input
            int i = endI - startI;
            int j = endJ - startJ;

            if(Mathf.Abs(i) > Mathf.Abs(j))
            {
                endJ = startJ;
            }
            else
            {
                endI = startI;
            }

            // calculate the direction
            int dy = Math.Sign(endI - startI);
            int dx = Math.Sign(endJ - startJ);

            // try to make the move
            manager.TryMove(startI, startJ, dy, dx);
        }
    }
}