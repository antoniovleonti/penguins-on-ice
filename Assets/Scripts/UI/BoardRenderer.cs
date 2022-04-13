using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardRenderer : MonoBehaviour
{
    // you can edit these arrays from the unity editor
    // put textures in here that correspond to the values
    // we expect to see. ex. first obstacleTexArr element
    // should be the background ice texture.
    public Texture2D[] tileTexs; // put obstacle textures here
    public Texture2D PenguinBGTex; // put penguin here
    public Texture2D PenguinFGTex; // put penguin here
    public Texture2D TargetBGTex; // targets here
    public Texture2D TargetFGTex; // targets here
    public Material LineMat;

    public Color[] IdColors;
    public float WallWidth = 0.1f;

    // these are generated programmatically from the TexArrs
    Sprite[] tileSprites; 
    Sprite penguinFGSprite;
    Sprite penguinBGSprite;
    Sprite targetBGSprite;
    Sprite targetFGSprite;

    float tileScale; // size of sprite in world-space
    float penguinBGScale; // size of sprite in world-space
    float penguinFGScale; // size of sprite in world-space
    float targetBGScale; // size of sprite in world-space
    float targetFGScale; // size of sprite in world-space

    GameObject tiles; // parent object to all board display objects
    GameObject penguins;
    GameObject walls;
    GameObject targets;

    GameObject[,] penguinsGObjs;

    private Grid grid; // gameobject addon to make grid calculations easier
    private Camera cam;

    // things that need to happen before the script can be used go HERE
    void Awake()
    {
        tiles = new GameObject("board");
        tiles.transform.SetParent(gameObject.transform);
        tiles.transform.localPosition = Vector3.zero;

        penguins = new GameObject("penguins");
        penguins.transform.SetParent(gameObject.transform);
        penguins.transform.localPosition = Vector3.zero;

        walls = new GameObject("walls");
        walls.transform.SetParent(gameObject.transform);
        walls.transform.localPosition = Vector3.zero;

        targets = new GameObject("targets");
        targets.transform.SetParent(gameObject.transform);
        targets.transform.localPosition = Vector3.zero;

        cam = Camera.main;
        cam.transform.SetParent(gameObject.transform);

        // add a grid which will do position calculations for us
        grid = gameObject.AddComponent(typeof(Grid)) as Grid;

        tileSprites = new Sprite[tileTexs.GetLength(0)];
        for (int i = 0; i < tileTexs.GetLength(0); i++)
        {
            Texture2D tex = tileTexs[i];
            tileSprites[i] = Sprite.Create(
                tex, new Rect(0.0f, 0.0f, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f), 100.0f ); 
        }

        //IdColors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow};
        penguinFGSprite = Sprite.Create(
            PenguinFGTex, 
            new Rect(0.0f, 0.0f, PenguinFGTex.width, PenguinFGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        penguinBGSprite = Sprite.Create(
            PenguinBGTex, 
            new Rect(0.0f, 0.0f, PenguinBGTex.width, PenguinBGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        targetFGSprite = Sprite.Create(
            TargetFGTex, 
            new Rect(0.0f, 0.0f, TargetFGTex.width, TargetFGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        targetBGSprite = Sprite.Create(
            TargetBGTex, 
            new Rect(0.0f, 0.0f, TargetBGTex.width, TargetBGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        tileScale = tileSprites[0].bounds.extents.x;
        penguinFGScale = penguinFGSprite.bounds.extents.x;
        penguinBGScale = penguinBGSprite.bounds.extents.x;
        targetFGScale = targetFGSprite.bounds.extents.x;
        targetBGScale = targetBGSprite.bounds.extents.x;
    }

    void Start()
    {
        //captures camera, sets position+size for board mode

    }

    public IEnumerator AnimThenRedraw ( int colStart,
                                        int colEnd,
                                        int rowStart,
                                        int rowEnd,
                                        Board board)
    {
        colStart = (colStart - 1) / 2;
        rowStart = (rowStart - 1) / 2;
        colEnd = (colEnd - 1) / 2;
        rowEnd = (rowEnd - 1) / 2;

        float animTime = 0.25f;
        float elapsed = 0f;

        var startCell = new Vector3Int(rowStart,-colStart,-5);
        var endCell = new Vector3Int(rowEnd,-colEnd,-5);
        Vector3 startCoord = grid.CellToLocal(startCell) + new Vector3(0.5f, 0.5f, 0);
        Vector3 endCoord = grid.CellToLocal(endCell) + new Vector3(0.5f, 0.5f, 0);

        while (elapsed < animTime)
        {
            var oldPos = penguinsGObjs[colStart,rowStart].transform.localPosition;
            // https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            float t = elapsed/animTime;
            t = t*t*t * (t * (6f*t - 15f) + 10f);
            var newPos = Vector3.Lerp(oldPos, endCoord, t);
            penguinsGObjs[colStart,rowStart].transform.localPosition = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Redraw(board);
    }

    public IEnumerator MouseDragFeedback (int click_i, int click_j, Board board)
    {
        if (penguinsGObjs[click_i,click_j] == null)
        {
            yield break;
        }
        Vector3 local = penguinsGObjs[click_i,click_j].transform.position;
        var offset = new Vector3(0.5f, 0.5f, 0);
        var startLocal = grid.CellToLocal(new Vector3Int(click_j,-click_i,1)) + offset;
        // copy gameobject
        var ghost = Instantiate(penguinsGObjs[click_i,click_j], penguins.transform);
        // make color gray
        var bg = ghost.transform.Find("bg");
        SpriteRenderer bgSR = bg.GetComponent<SpriteRenderer>();
        Color bgColor = bgSR.color;
        bgColor.a = 0.5f;
        bgSR.color = bgColor;
        
        var fg = ghost.transform.Find("fg");
        //fg.GetComponent<SpriteRenderer>().color.a = 0.5f;
        // make translucent
        while (!Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // convert mouse position to local position
            var mousePos = Mouse.current.position;
            var vec = new Vector3(  mousePos.x.ReadValue(), 
                                    mousePos.y.ReadValue(), 
                                    0f );
            var world = cam.ScreenToWorldPoint(vec);
            local = grid.WorldToLocal(world);
            // calculate ghost position from local mouse position
            var diff = startLocal - local;
            if (Math.Abs(diff.x) > Math.Abs(diff.y))
            {
                local.y = startLocal.y;
            }
            else
            {
                local.x = startLocal.x;
            }
            local.z = 0f;
        
            // approximate the current mouse drag into a move
            var startI = (click_i * 2 + 1);
            var startJ = click_j * 2 + 1;
            int dy = -Math.Sign(local.y - startLocal.y);
            int dx = Math.Sign(local.x - startLocal.x);

            if (board.IsValidMove(startI,startJ,dy,dx))
            {
                try
                {
                    // get the end location in world position
                    var dest = board.CalculateMove(startI, startJ, dy, dx);
                    int i,j; (i,j) = dest;
                    i = (i-1)/2;
                    j = (j-1)/2;
                    var endCell = new Vector3Int(j,-i,0);
                    var endCoord = grid.CellToLocal(endCell) + new Vector3(0.5f, 0.5f, 0);
                    // make sure the penguin does not go past the end location
                    var ghostDist = (startLocal-local).magnitude;
                    var endDist = (startLocal-endCoord).magnitude;
                    if (ghostDist > endDist) local = endCoord;
                } 
                catch {}
            }
            local.z = -2f;
            
            var cur = ghost.transform.position;
            ghost.transform.position = local;
            ghost.transform.position = Vector3.Lerp(cur,local, Time.deltaTime*10f);

            // move gameobject to appropriate location relative to mouse
            // should not extend past where it will actually go when released
            // interpolation?
            yield return null;
        }
        penguinsGObjs[click_i,click_j].transform.position = ghost.transform.position;
        // delete the new gameobject
        Destroy(ghost);
    }
    public void Redraw(Board board)
    {
        EraseBoard();
        drawTiles(board);
        drawWalls(board);
        drawPenguins(board);
        drawTargets(board);
        positionCam(board);
    }

    void positionCam(Board board)
    {
        var centerCell = new Vector3Int(board.ColumnCells/2, -board.RowCells/2,1);
        var centerLocal = grid.CellToLocal(centerCell);
        centerLocal.z = -10;
        
        cam.transform.localPosition = centerLocal;
        cam.orthographicSize = Math.Max(board.ColumnCells,board.RowCells) / 2 + 3;
    }

    void drawTiles(Board board)
    {
        // add background tiles (odd indices in board.obstacles)
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                // set up a game object for this tile
                GameObject tile = new GameObject("tile @ ("+i+", "+j+")");
                tile.transform.SetParent(tiles.transform);
                // position it
                var cell = grid.CellToLocal(new Vector3Int(j, -i, 1));
                tile.transform.localPosition = cell + new Vector3(0.5f, 0.5f, 0);
                tile.transform.localScale = tile.transform.localScale / (tileScale * 2);
                
                // now add a sprite renderer so we can see our game object
                SpriteRenderer renderer = (SpriteRenderer)tile.AddComponent<SpriteRenderer>(); 
                int I = Board.CellToCoord(i), J = Board.CellToCoord(j);
                renderer.sprite = tileSprites[board.Obstacles[I,J]];
            }
        }
    }

    void drawWalls(Board board)
    {
        // constant corner positions relative to cell
        var topLeft = new Vector3(-0.5f, 0.5f, 0f);
        var topRight = new Vector3(0.5f, 0.5f, 0f);
        var bottomLeft = new Vector3(-0.5f, -0.5f, 0f);
        var bottomRight = new Vector3(0.5f, -0.5f, 0f);

        // add walls (even indices in board.Obstacles -- on tile edges)
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                //refer to cell
                int I = Board.CellToCoord(i);
                int J = Board.CellToCoord(j);

                //check for horizontal wall above cell
                if(board.Obstacles[I-1,J] == 1) 
                    drawLine(i,j, topLeft, topRight, WallWidth);
                //check for vertical wall to the left of the cell
                if(board.Obstacles[I,J-1] == 1) 
                    drawLine(i,j, topLeft, bottomLeft, WallWidth);

                //exception case (End row, i = rowcells -1)
                //check for horizontal wall below cell
                if( i == (board.RowCells - 1) && board.Obstacles[I+1,J] == 1)
                    drawLine(i,j, bottomLeft, bottomRight, WallWidth);

                //exception case (End Column, j = columncells -1)
                //check for vertical wall to the right of the cell
                if( j == (board.ColumnCells - 1) && board.Obstacles[I,J+1] == 1)
                    drawLine(i,j, topRight, bottomRight, WallWidth);
            }
        }
    }

    void drawLine(int i, int j, Vector3 localPosA, Vector3 localPosB, float width)
    {
        GameObject line = new GameObject("line @ ("+i+", "+j+")");

        // set up gameobject in hierarchy 
        var cell = new Vector3Int(j, -i, 1);
        line.transform.SetParent(walls.transform);
        //line.transform.localScale = line.transform.localScale / 4;
        line.transform.localPosition = grid.CellToLocal(cell) + new Vector3(0.5f, 0.5f, -1);
        
        //create new Linerenderer, set texture and width
        LineRenderer renderer = (LineRenderer)line.AddComponent<LineRenderer>();
        renderer.useWorldSpace = false; // draw line relative to GO position
        renderer.material = LineMat;
        renderer.startColor = renderer.endColor = Color.black;
        renderer.startWidth = width;
        renderer.endWidth = width;

        // add these verts to renderer
        renderer.SetPositions(new Vector3[]{localPosA, localPosB});
    }

    void drawPenguins(Board board)
    {
        penguinsGObjs = new GameObject[board.ColumnCells,board.RowCells];
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                int I = Board.CellToCoord(i); // "real" positions on board
                int J = Board.CellToCoord(j);

                // only do anything if there's a penguin here
                if (board.Penguins[I,J] == 0) {continue;}
                // create the gameobject
                GameObject penguin = new GameObject("penguin @ ("+i+", "+j+")"); 
                penguin.transform.SetParent(penguins.transform);

                var cell = new Vector3Int(j, -i, -5);
                var lpos = grid.CellToLocal(cell) + new Vector3(0.5f, 0.5f, 0);
                penguin.transform.localPosition = lpos;

                // background is the silhouette of the penguin -- changes color
                GameObject bg = new GameObject("bg");
                bg.transform.SetParent(penguin.transform);
                bg.transform.localPosition = new Vector3(0,0,-1);
                bg.transform.localScale = bg.transform.localScale / (penguinBGScale * 2);

                SpriteRenderer bgRenderer = (SpriteRenderer)bg.AddComponent<SpriteRenderer>();
                bgRenderer.sprite = penguinBGSprite;
                bgRenderer.color = IdColors[board.Penguins[I,J]-1];

                // foreground contains penguin details -- does not change color
                GameObject fg = new GameObject("fg");
                fg.transform.SetParent(penguin.transform);
                fg.transform.localPosition = new Vector3(0,0,-2);
                fg.transform.localScale = fg.transform.localScale / (penguinFGScale * 2);

                SpriteRenderer fgRenderer = (SpriteRenderer)fg.AddComponent<SpriteRenderer>();
                fgRenderer.sprite = penguinFGSprite;

                // add it to the array so we can animate later
                penguinsGObjs[i,j] = penguin;
            }
        }
    }

    void drawTargets(Board board)
    {
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                int I = Board.CellToCoord(i); // "real" positions on board
                int J = Board.CellToCoord(j);

                // only do anything if there's a penguin here
                if (board.Targets[I,J] == 0) {continue;}

                GameObject target = new GameObject("target @ ("+i+", "+j+")"); // create the gameobject
                target.transform.SetParent(targets.transform);

                var cell = new Vector3Int(j, -i, -1);
                target.transform.localPosition = grid.CellToLocal(cell) + new Vector3(0.5f, 0.5f, 0);

                // background is the silhouette of the penguin -- changes color
                GameObject bg = new GameObject("bg");
                bg.transform.SetParent(target.transform);
                bg.transform.localPosition = new Vector3(0,0,-1);
                bg.transform.localScale = bg.transform.localScale / (targetBGScale * 2);

                SpriteRenderer bgRenderer = (SpriteRenderer)bg.AddComponent<SpriteRenderer>();
                bgRenderer.sprite = targetBGSprite;
                bgRenderer.color = IdColors[board.Targets[I,J]-1];

                // foreground contains penguin details -- does not change color
                GameObject fg = new GameObject("fg");
                fg.transform.SetParent(target.transform);
                fg.transform.localPosition = new Vector3(0,0,-2);
                fg.transform.localScale = fg.transform.localScale / (targetFGScale * 2);

                SpriteRenderer fgRenderer = (SpriteRenderer)fg.AddComponent<SpriteRenderer>();
                fgRenderer.sprite = targetFGSprite;
            }
        }
    }

    public void EraseBoard()
    {
        // delete all children of the board game object
        int n = tiles.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
            GameObject.Destroy(tiles.transform.GetChild(i).gameObject);
        // same for penguins
        n = penguins.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
            GameObject.Destroy(penguins.transform.GetChild(i).gameObject);
        // same for targets
        n = targets.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
            GameObject.Destroy(targets.transform.GetChild(i).gameObject);
        // finally same for walls
        n = walls.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
            GameObject.Destroy(walls.transform.GetChild(i).gameObject);
    }
}
