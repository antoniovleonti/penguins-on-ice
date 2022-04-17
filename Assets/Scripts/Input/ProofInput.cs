using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProofInput : MonoBehaviour
{
    ProofManager manager;
    Grid grid;
    Camera cam;
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
            Vector2Int clickCell = GetCurrentMouseCell();

            StartCoroutine( listenForMouseRelease(clickCell));
            StartCoroutine(
                bRenderer.MouseDragFeedback(clickCell, manager.BoardState));
        }
    }
    IEnumerator listenForMouseRelease (Vector2Int clickCell)
    {
        Debug.Log(clickCell);
        while (!Mouse.current.leftButton.wasReleasedThisFrame)
        {
            yield return null; // do nothing this frame
        }
        // we now know that the mouse was released this frame.
        Vector2Int releaseCell = GetCurrentMouseCell();

        // snap dragged direction to one of four orthogonal directions
        var diff = releaseCell - clickCell;
        if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            releaseCell.y = clickCell.y;
        }
        else
        {
            releaseCell.x = clickCell.x;
        }

        Vector2Int clickCoord = new Vector2Int( Board.CellToCoord(clickCell.x),
                                                Board.CellToCoord(clickCell.y));
        // calculate the direction the player dragged in
        diff = releaseCell - clickCell; // needs to be updated
        int dx = Math.Sign(diff.x);
        int dy = Math.Sign(diff.y);

        Debug.Log((clickCoord.y,clickCoord.x,dy,dx));

        // try to make the move
        manager.TryMove(clickCoord.y, clickCoord.x, dy, dx);
    }

    public Vector2Int GetCurrentMouseCell ()
    {
        var local = GetCurrentMousePos();
        var cell3 = grid.LocalToCell(local);
        cell3.y *= -1;

        return (Vector2Int)cell3;
    }
    public Vector2 GetCurrentMousePos ()
    {
        var mousePos = Mouse.current.position;
        var vec = new Vector3(  mousePos.x.ReadValue(), 
                                mousePos.y.ReadValue(), 
                                0 );
        var world = cam.ScreenToWorldPoint(vec);
        return grid.WorldToLocal(world);
    }
}