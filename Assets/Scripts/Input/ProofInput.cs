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

    void EndProofs ()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2Int clickCell = GetCurrentMouseCell();

            StartCoroutine(listenForMouseRelease(clickCell));
            StartCoroutine(
                bRenderer.MouseDragFeedback(clickCell, manager.BoardState));
        }
    }
    IEnumerator listenForMouseRelease (Vector2Int clickCell)
    {
        while (!Mouse.current.leftButton.wasReleasedThisFrame)
        {
            yield return null; // do nothing this frame
        }
        Vector3Int gridClickCell = new Vector3Int(clickCell.x,-clickCell.y,0);
        Vector3 startPos = grid.GetCellCenterLocal(gridClickCell);

        Vector2 mousePos = GetCurrentMousePos();
        Vector2 mouseProjPos = OrthoProjectionFromOrigin(mousePos,startPos);
        Vector2Int mouseProjCell = (Vector2Int)grid.LocalToCell(mouseProjPos);
        mouseProjCell.y *= -1;

        Vector2Int d = PointsToDirection(startPos,mouseProjPos);
        d.y *= -1;

        if (clickCell == mouseProjCell)
        {
            // player has released on the same square they clicked
            StartCoroutine(listenToArrowKeys(clickCell));
        }
        else
        {
            // try to make the move
            Vector2Int clickCoord = 
                new Vector2Int( Board.CellToCoord(clickCell.x),
                                Board.CellToCoord(clickCell.y));
            StartCoroutine(manager.TryMove(clickCoord.y, clickCoord.x, d.y, d.x));
        }
    }

    IEnumerator listenToArrowKeys (Vector2Int activePenguin)
    {
        bool down, up, left, right;
        do
        {
            yield return null;

            if (Mouse.current.leftButton.wasPressedThisFrame)
                yield break;

            down = Keyboard.current.downArrowKey.wasPressedThisFrame;
            up = Keyboard.current.upArrowKey.wasPressedThisFrame;
            left = Keyboard.current.leftArrowKey.wasPressedThisFrame;
            right = Keyboard.current.rightArrowKey.wasPressedThisFrame;
        }
        while (!(down || up || left || right));

        // else
        var d = new Vector2Int();

        d.y += (down?1:0) + (up?-1:0);
        d.x += (left?-1:0) + (right?1:0);

        Vector2Int coord = 
            new Vector2Int( Board.CellToCoord(activePenguin.x),
                            Board.CellToCoord(activePenguin.y));

        int yCoord,xCoord;
        (yCoord,xCoord) = 
            manager.BoardState.CalculateMove(coord.y, coord.x,
                                             d.y, d.x);

        var newActivePenguin = new Vector2Int(xCoord,yCoord);
        newActivePenguin.x = (newActivePenguin.x-1)/2;
        newActivePenguin.y = (newActivePenguin.y-1)/2;

        StartCoroutine(manager.TryMoveQuick(coord.y,coord.x,d.y,d.x));

        StartCoroutine(listenToArrowKeys(newActivePenguin));
    }

    public Vector2 OrthoProjectionFromOrigin(Vector2 point, Vector2 source)
    {
        // calculate ghost position from mouseProjPos mouse position
        var d = point - source;
        if (Math.Abs(d.x) > Math.Abs(d.y)) 
            point.y = source.y;
        else 
            point.x = source.x;

        return point;
    }

    public Vector2Int PointsToDirection(Vector2 start, Vector2 end)
    {
        var d = end - start;
        return new Vector2Int(Math.Sign(d.x), Math.Sign(d.y));
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