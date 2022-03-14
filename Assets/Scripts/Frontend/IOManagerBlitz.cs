using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOManagerBlitz : MonoBehaviour
{
    // you can edit these arrays from the unity editor
    // put textures in here that correspond to the values
    // we expect to see. ex. first obstacleTexArr element
    // should be the background ice texture.
    public Texture2D[] ObstacleTexArr; // put obstacle textures here
    public Texture2D PenguinBGTex; // put penguin here
    public Texture2D PenguinFGTex; // put penguin here
    public Texture2D TargetTex; // targets here
    public Color[] penguinColors;
    public float WallWidth = 0.1f;

    // these are generated programmatically from the TexArrs
    private Sprite[] obstacleSpriteArr; 
    private Sprite penguinFGSprite;
    private Sprite penguinBGSprite;
    private Sprite targetSprite;

    private float obstacleScale; // size of sprite in world-space
    private float penguinBGScale; // size of sprite in world-space
    private float penguinFGScale; // size of sprite in world-space
    private float targetScale; // size of sprite in world-space

    private BlitzRunManager blitz;
    private GameObject boardObject; // parent object to all board display objects
    private Grid boardGrid; // gameobject addon to make grid calculations easier

    private Camera cam;
    private Vector3Int? click1;
    private Vector3Int? click2;

    // Start is called before the first frame update
    void Start()
    {
        blitz = new BlitzRunManager();
        blitz.NextBoard();
        // create board gameobject to be a parent to all tiles
        boardObject = new GameObject("board");
        // set its parent to the io manager
        boardObject.transform.SetParent(gameObject.transform);
        // add a board grid which will do grid position calculations for us
        boardGrid = boardObject.AddComponent(typeof(Grid)) as Grid;

        // create sprites from provided texture(s)
        int nTextures = ObstacleTexArr.GetLength(0);
        obstacleSpriteArr = new Sprite[nTextures];
        for (int i = 0; i < nTextures; i++)
        {
            Texture2D tex = ObstacleTexArr[i];
            obstacleSpriteArr[i] = Sprite.Create(
                tex, new Rect(0.0f, 0.0f, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f), 100.0f ); 
        }

        //penguinColors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow};
        // ^ do the same for penguins and targets down here
        penguinFGSprite = Sprite.Create(
            PenguinFGTex, new Rect(0.0f, 0.0f, PenguinFGTex.width, PenguinFGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        // ^ do the same for penguins and targets down here
        penguinBGSprite = Sprite.Create(
            PenguinBGTex, new Rect(0.0f, 0.0f, PenguinBGTex.width, PenguinBGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        targetSprite = Sprite.Create(
            TargetTex, new Rect(0.0f, 0.0f, TargetTex.width, TargetTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        // resize grid to match size of sprites
        obstacleScale = obstacleSpriteArr[0].bounds.extents.x;
        penguinFGScale = penguinFGSprite.bounds.extents.x;
        penguinBGScale = penguinBGSprite.bounds.extents.x;
        targetScale = targetSprite.bounds.extents.x;

        
        //captures camera, sets position+size for blitz mode
        cam = Camera.main;
        cam.transform.localPosition = new Vector3(8, -7, -10);
        cam.orthographicSize = 9;

        DrawBoard();
    }

    // Update is called once per frame
    void Update()
    {
        // not necessary to call these every frame in practice,
        // but it can be useful for testing and experimenting
        if (Input.GetMouseButtonDown(0))
        {
            if (click1 == null)
            {
                click1 = boardGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
                //other updates after first click
            } else
            {
                Vector3Int startCell = click1 ?? new Vector3Int(0, 0, 0);
                click2 = boardGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
                Vector3Int endCell = click2 ?? new Vector3Int(0, 0, 0);
                click1 = null;
                click2 = null;
                // get the coords of the first click
                int startI = Board.CellToCoord(-1 * startCell.y);
                int startJ = Board.CellToCoord(startCell.x);
                // get the coords of the second click
                int endI = Board.CellToCoord(-1 * endCell.y);
                int endJ = Board.CellToCoord(endCell.x);
                // calculate the direction
                int dy = Math.Sign(endI - startI);
                int dx = Math.Sign(endJ - startJ);
                
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
                DrawBoard();
            }
            //Debug.Log(boardGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)));
        }
    }

    void DrawBoard()
    {
        EraseBoard();
        DrawBackground();
        DrawWalls();
        DrawPenguins();
        DrawTargets();
    }

    void DrawBackground()
    {
        // add background tiles (odd indices in board.obstacles)
        for (int i = 0; i < blitz.RowCells; i++)
        {
            for (int j = 0; j < blitz.ColumnCells; j++)
            {
                // set up a game object for this tile
                GameObject tile = new GameObject("tile @ ("+i+", "+j+")");
                tile.transform.SetParent(boardObject.transform);
                // position it
                var cell = boardGrid.CellToLocal(new Vector3Int(j, -i, 1));
                tile.transform.localPosition = cell + new Vector3(0.5f, 0.5f, 0);
                tile.transform.localScale = tile.transform.localScale / (obstacleScale * 2);
                
                // now add a sprite renderer so we can see our game object
                SpriteRenderer renderer = (SpriteRenderer)tile.AddComponent<SpriteRenderer>(); 
                int I = Board.CellToCoord(i), J = Board.CellToCoord(j);
                renderer.sprite = obstacleSpriteArr[blitz.Obstacles[I,J]];
            }
        }
    }

    void DrawWalls()
    {
        var topLeft = new Vector3(-0.5f, 0.5f, 0f);
        var topRight = new Vector3(0.5f, 0.5f, 0f);
        var bottomLeft = new Vector3(-0.5f, -0.5f, 0f);
        var bottomRight = new Vector3(0.5f, -0.5f, 0f);
        // add walls (even indices in board.Obstacles -- on tile edges)
        for (int i = 0; i < blitz.RowCells; i++)
        {
            for (int j = 0; j < blitz.ColumnCells; j++)
            {
                //refer to cell
                int I = Board.CellToCoord(i);
                int J = Board.CellToCoord(j);

                //check for horizontal wall above cell
                if(blitz.Obstacles[I-1,J] == 1)
                {
                    DrawLine(i,j, topLeft, topRight, WallWidth);
                }
                //check for vertical wall to the left of the cell
                if(blitz.Obstacles[I,J-1] == 1)
                {
                    DrawLine(i,j, topLeft, bottomLeft, WallWidth);
                }

                //exception case (End row, i = rowcells -1)
                //check for horizontal wall below cell
                if( i == (blitz.RowCells - 1) && blitz.Obstacles[I+1,J] == 1)
                {
                    DrawLine(i,j, bottomLeft, bottomRight, WallWidth);
                }

                //exception case (End Column, j = columncells -1)
                //check for vertical wall to the right of the cell
                if( j == (blitz.ColumnCells - 1) && blitz.Obstacles[I,J+1] == 1)
                {
                    DrawLine(i,j, topRight, bottomRight, WallWidth);
                }
            }
        }
    }

    void DrawLine(int i, int j, Vector3 localPosA, Vector3 localPosB, float width)
    {
        GameObject line = new GameObject("line @ ("+i+", "+j+")");

        // set up gameobject in hierarchy 
        var cell = new Vector3Int(j, -i, 1);
        line.transform.SetParent(boardObject.transform);
        //line.transform.localScale = line.transform.localScale / 4;
        line.transform.localPosition = boardGrid.CellToLocal(cell) + new Vector3(0.5f, 0.5f, -1);
        
        //create new Linerenderer, set texture and width
        LineRenderer renderer = (LineRenderer)line.AddComponent<LineRenderer>();
        renderer.useWorldSpace = false; // draw line relative to GO position
        renderer.material.SetTexture("_MainTex", (Texture)ObstacleTexArr[1]);
        renderer.startWidth = width;
        renderer.endWidth = width;

        // add these verts to renderer
        //renderer.SetPositions(new Vector3[]{localPosA, localPosB});
        renderer.SetPosition(0, localPosA);
        renderer.SetPosition(1, localPosB);
    }

    void DrawPenguins()
    {
        for (int i = 0; i < blitz.RowCells; i++)
        {
            for (int j = 0; j < blitz.ColumnCells; j++)
            {
                int I = Board.CellToCoord(i); // "real" positions on board
                int J = Board.CellToCoord(j);

                // only do anything if there's a penguin here
                if (blitz.Penguins[I,J] == 0) {continue;}

                GameObject penguin = new GameObject("penguin @ ("+i+", "+j+")"); // create the gameobject
                penguin.transform.SetParent(boardObject.transform);

                var cell = new Vector3Int(j, -i, -1);
                penguin.transform.localPosition = boardGrid.CellToLocal(cell) + new Vector3(0.5f, 0.5f, 0);
                //penguin.transform.localScale = penguin.transform.localScale * 0.9f; // make penguins a little smaller

                GameObject bg = new GameObject("bg");
                bg.transform.SetParent(penguin.transform);
                bg.transform.localPosition = new Vector3(0,0,-1);
                bg.transform.localScale = bg.transform.localScale / (penguinBGScale * 2);

                SpriteRenderer bgRenderer = (SpriteRenderer)bg.AddComponent<SpriteRenderer>();
                bgRenderer.sprite = penguinBGSprite;
                bgRenderer.color = penguinColors[blitz.Penguins[I,J]-1];

                GameObject fg = new GameObject("fg");
                fg.transform.SetParent(penguin.transform);
                fg.transform.localPosition = new Vector3(0,0,-2);
                fg.transform.localScale = fg.transform.localScale / (penguinFGScale * 2);

                SpriteRenderer fgRenderer = (SpriteRenderer)fg.AddComponent<SpriteRenderer>();
                fgRenderer.sprite = penguinFGSprite;
                // recolor penguin according to ID
            }
        }
    }

    void DrawTargets()
    {
        for (int i = 0; i < blitz.RowCells; i++)
        {
            for (int j = 0; j < blitz.ColumnCells; j++)
            {
                int I = Board.CellToCoord(i);
                int J = Board.CellToCoord(j);

                if (blitz.Targets[I,J] == 0) {continue;}
                //Debug.Log("I:"+I+"\tJ:"+J);
                GameObject tmp = new GameObject("" + i + " " + j);
                tmp.transform.SetParent(boardObject.transform);

                var adjustedIdx = new Vector3Int(j, -i, -1);
                tmp.transform.localPosition = boardGrid.CellToLocal(adjustedIdx) + new Vector3(0.5f, 0.5f, 0);
                tmp.transform.localScale = tmp.transform.localScale / (targetScale * 2) * 0.9f;

                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>();
                renderer.sprite = targetSprite;
                renderer.color = penguinColors[blitz.Targets[I,J] - 1];
            }
        }
    }

    void EraseBoard()
    {
        // delete all children of the board game object
        int n_children = boardObject.transform.childCount;
        for (int i = n_children - 1; i >= 0; i--)
        {
            GameObject.Destroy(boardObject.transform.GetChild(i).gameObject);
        }
    }
}
