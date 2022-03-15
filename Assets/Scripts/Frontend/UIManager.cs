using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // you can edit these arrays from the unity editor
    // put textures in here that correspond to the values
    // we expect to see. ex. first obstacleTexArr element
    // should be the background ice texture.
    public Texture2D[] tileTexs; // put obstacle textures here
    public Texture2D PenguinBGTex; // put penguin here
    public Texture2D PenguinFGTex; // put penguin here
    public Texture2D TargetTex; // targets here
    public Material LineMat;

    public Color[] IdColors;
    public float WallWidth = 0.1f;

    // these are generated programmatically from the TexArrs
    private Sprite[] tileSprites; 
    private Sprite penguinFGSprite;
    private Sprite penguinBGSprite;
    private Sprite targetSprite;

    private float tileScale; // size of sprite in world-space
    private float penguinBGScale; // size of sprite in world-space
    private float penguinFGScale; // size of sprite in world-space
    private float targetScale; // size of sprite in world-space

    private BlitzRunManager blitz;

    private GameObject tiles; // parent object to all board display objects
    private GameObject penguins;
    private GameObject walls;
    private GameObject targets;

    private Grid grid; // gameobject addon to make grid calculations easier
    private Camera cam;

    void Awake()
    {
        // stuff that other components want to access
        // create board gameobject to be a parent to all tiles
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

        // add a board grid which will do grid position calculations for us
        grid = gameObject.AddComponent(typeof(Grid)) as Grid;
    }

    // Start is called before the first frame update
    void Start()
    {
        blitz = gameObject.GetComponent<BackendManager>().Blitz;
        // create sprites from provided texture(s)
        int nTextures = tileTexs.GetLength(0);
        tileSprites = new Sprite[nTextures];
        for (int i = 0; i < nTextures; i++)
        {
            Texture2D tex = tileTexs[i];
            tileSprites[i] = Sprite.Create(
                tex, new Rect(0.0f, 0.0f, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f), 100.0f ); 
        }

        //IdColors = new Color[]{Color.red, Color.blue, Color.green, Color.yellow};
        penguinFGSprite = Sprite.Create(
            PenguinFGTex, new Rect(0.0f, 0.0f, PenguinFGTex.width, PenguinFGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        penguinBGSprite = Sprite.Create(
            PenguinBGTex, new Rect(0.0f, 0.0f, PenguinBGTex.width, PenguinBGTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        targetSprite = Sprite.Create(
            TargetTex, new Rect(0.0f, 0.0f, TargetTex.width, TargetTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        tileScale = tileSprites[0].bounds.extents.x;
        penguinFGScale = penguinFGSprite.bounds.extents.x;
        penguinBGScale = penguinBGSprite.bounds.extents.x;
        targetScale = targetSprite.bounds.extents.x;

        //captures camera, sets position+size for blitz mode
        cam = Camera.main;
        cam.transform.SetParent(gameObject.transform);
        cam.transform.localPosition = new Vector3(8, -7, -10);
        cam.orthographicSize = 9;

        Redraw();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Redraw()
    {
        eraseBoard();
        drawBackground();
        drawWalls();
        drawPenguins();
        drawTargets();
    }

    void drawBackground()
    {
        // add background tiles (odd indices in board.obstacles)
        for (int i = 0; i < blitz.RowCells; i++)
        {
            for (int j = 0; j < blitz.ColumnCells; j++)
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
                renderer.sprite = tileSprites[blitz.Obstacles[I,J]];
            }
        }
    }

    void drawWalls()
    {
        // constant corner positions relative to cell
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
                    drawLine(i,j, topLeft, topRight, WallWidth);
                //check for vertical wall to the left of the cell
                if(blitz.Obstacles[I,J-1] == 1) 
                    drawLine(i,j, topLeft, bottomLeft, WallWidth);

                //exception case (End row, i = rowcells -1)
                //check for horizontal wall below cell
                if( i == (blitz.RowCells - 1) && blitz.Obstacles[I+1,J] == 1)
                    drawLine(i,j, bottomLeft, bottomRight, WallWidth);

                //exception case (End Column, j = columncells -1)
                //check for vertical wall to the right of the cell
                if( j == (blitz.ColumnCells - 1) && blitz.Obstacles[I,J+1] == 1)
                    drawLine(i,j, topRight, bottomRight, WallWidth);
            }
        }
    }

    void drawLine(int i, int j, Vector3 localPosA, Vector3 localPosB, float width)
    {
        GameObject line = new GameObject("line @ ("+i+", "+j+")");

        // set up gameobject in hierarchy 
        var cell = new Vector3Int(j, -i, 1);
        line.transform.SetParent(tiles.transform);
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

    void drawPenguins()
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
                penguin.transform.SetParent(tiles.transform);

                var cell = new Vector3Int(j, -i, -1);
                penguin.transform.localPosition = grid.CellToLocal(cell) + new Vector3(0.5f, 0.5f, 0);

                // background is the silhouette of the penguin -- changes color
                GameObject bg = new GameObject("bg");
                bg.transform.SetParent(penguin.transform);
                bg.transform.localPosition = new Vector3(0,0,-1);
                bg.transform.localScale = bg.transform.localScale / (penguinBGScale * 2);

                SpriteRenderer bgRenderer = (SpriteRenderer)bg.AddComponent<SpriteRenderer>();
                bgRenderer.sprite = penguinBGSprite;
                bgRenderer.color = IdColors[blitz.Penguins[I,J]-1];

                // foreground contains penguin details -- does not change color
                GameObject fg = new GameObject("fg");
                fg.transform.SetParent(penguin.transform);
                fg.transform.localPosition = new Vector3(0,0,-2);
                fg.transform.localScale = fg.transform.localScale / (penguinFGScale * 2);

                SpriteRenderer fgRenderer = (SpriteRenderer)fg.AddComponent<SpriteRenderer>();
                fgRenderer.sprite = penguinFGSprite;
            }
        }
    }

    void drawTargets()
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
                tmp.transform.SetParent(targets.transform);

                var cell = grid.CellToLocal(new Vector3Int(j, -i, -1));
                tmp.transform.localPosition = cell + new Vector3(0.5f, 0.5f, 0);
                tmp.transform.localScale = tmp.transform.localScale / (targetScale * 2) * 0.9f;

                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>();
                renderer.sprite = targetSprite;
                renderer.color = IdColors[blitz.Targets[I,J] - 1];
            }
        }
    }

    void eraseBoard()
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
