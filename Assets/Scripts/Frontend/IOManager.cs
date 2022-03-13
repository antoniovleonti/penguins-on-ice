using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOManager : MonoBehaviour
{
    // you can edit these arrays from the unity editor
    // put textures in here that correspond to the values
    // we expect to see. ex. first obstacleTexArr element
    // should be the background ice texture.
    public Texture2D[] ObstacleTexArr; // put obstacle textures here
    public Texture2D[] PenguinTexArr; // put penguins here
    public Texture2D[] TargetTexArr; // targets here

    // these are generated programmatically from the TexArrs
    private Sprite[] obstacleSpriteArr; 
    private Sprite[] penguinSpriteArr;
    private Sprite[] targetSpriteArr;

    private float spriteScale; // size of sprite in world-space

    private Board board; // backend board we are displaying
    private GameObject boardObject; // parent object to all board display objects
    private Grid boardGrid; // gameobject addon to make grid calculations easier

    private Camera cam;
    private Vector3Int? click1;
    private Vector3Int? click2;

    // Start is called before the first frame update
    void Start()
    {
        // create a new board
        int[,] obstacles = {
            {1,1,1,1,1,1,1,},
            {1,0,1,0,0,0,1,},
            {1,0,0,0,0,0,1,},
            {1,0,1,0,1,0,1,},
            {1,0,0,1,0,0,1,},
            {1,0,0,0,0,0,1,},
            {1,1,1,1,1,1,1,},
        };
        int[,] penguins = {
            {0,0,0,0,0,0,0,},
            {0,1,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
        };
        int[,] targets = {
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,0,0,},
            {0,0,0,0,0,1,0,},
            {0,0,0,0,0,0,0,},
        };
        //board = new Board(obstacles, penguins, targets);
        ProceduralBoard pb = null;
        while (pb == null)
        {
            try { pb = new ProceduralBoard(4); }
            catch (System.Exception) {}
        }
        board = pb;

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
        // ^ do the same for penguins and targets down here
        nTextures = PenguinTexArr.GetLength(0);
        penguinSpriteArr = new Sprite[nTextures];
        for (int i = 0; i < nTextures; i++)
        {
            Texture2D tex = PenguinTexArr[i];
            penguinSpriteArr[i] = Sprite.Create(
                tex, new Rect(0.0f, 0.0f, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f), 100.0f ); 
        }

        nTextures = TargetTexArr.GetLength(0);
        targetSpriteArr = new Sprite[nTextures];
        for (int i = 0; i < nTextures; i++)
        {
            Texture2D tex = TargetTexArr[i];
            targetSpriteArr[i] = Sprite.Create(
                tex, new Rect(0.0f, 0.0f, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f), 100.0f ); 
        }
        // resize grid to match size of sprites
        spriteScale = obstacleSpriteArr[0].bounds.extents.x;
        //Debug.Log("Rows:"+board.Rows+"\tCols:"+board.Columns);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // not necessary to call these every frame in practice,
        // but it can be useful for testing and experimenting
        EraseBoard();
        DrawBoard();
        if (Input.GetMouseButtonDown(0))
        {
            if (click1 == null)
            {
                click1 = boardGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
                //other updates after first click
            } else
            {
                Vector3Int temp1 = click1?? new Vector3Int(0, 0, 0);
                click2 = boardGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
                Vector3Int temp2 = click2?? new Vector3Int(0, 0, 0);
                int I_click1 = Board.CellToCoord(-1 * temp1.y);
                int J_click1 = Board.CellToCoord(temp1.x);
                int I_click2 = Board.CellToCoord(-1 * temp2.y);
                int J_click2 = Board.CellToCoord(temp2.x);
                Debug.Log((I_click2, J_click2));
                int tempI = Math.Sign(I_click2 - I_click1);
                int tempJ = Math.Sign(J_click2 - J_click1);
                Debug.Log((I_click1, J_click1, tempI, tempJ));
                try
                {
                    board.make_move(I_click1, J_click1, tempI, tempJ);
                } catch{

                }
                click1 = null;
                click2 = null;
            }

            //Debug.Log(boardGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)));
        }
    }

    void DrawBoard()
    {
        DrawBackground();
        DrawWalls();
        DrawPenguins();
        DrawTargets();
    }

    void DrawBackground()
    {
        // add background tiles (odd indices in board.obstacles)
        for (int i = 1; i < board.Rows; i += 2)
        {
            for (int j = 1; j < board.Columns; j += 2)
            {
                // set up a game object for this tile
                GameObject tmp = new GameObject("" + i + " " + j);
                tmp.transform.SetParent(boardObject.transform);
                // must adjust indices to account for the fact that background
                // tiles only exist on odd-numbered indices in array
                var adjustedIdx = new Vector3Int((i-1)/2, -1 * (j-1)/2, 1);
                // use grid component to calculate correct position of tile and move
                // the tile to that position
                tmp.transform.localPosition = boardGrid.CellToLocal(adjustedIdx) + new Vector3(0.5f, 0.5f, 0);
                // use the iceSpriteScale to make the sprite length and width (1,1)
                tmp.transform.localScale = tmp.transform.localScale / (spriteScale * 2);
                
                // now add a sprite renderer so we can see our game object
                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>(); 
                // pick the right sprite based on the number in the obstacles array
                renderer.sprite = obstacleSpriteArr[board.Obstacles[i,j]];
            }
        }
    }

    void DrawWalls()
    {
        // add walls (even indices in board.Obstacles -- on tile edges)
        //horizontal walls (Even Row, Odd Column)
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                //refer to cell
                int I = Board.CellToCoord(i);
                int J = Board.CellToCoord(j);
                //check for horizontal wall above cell
                if(board.Obstacles[I-1,J] == 1)
                {
                    DrawAboveWall(i, j, I, J);
                }
                //check for vertical wall to the left of the cell
                if(board.Obstacles[I,J-1] == 1)
                {
                    DrawLeftWall(i, j, I, J);
                }

                //exception case (End row, i = rowcells -1)
                //check for horizontal wall below cell
                if( i == (board.RowCells - 1))
                {
                    if(board.Obstacles[I+1,J] == 1)
                    {
                        DrawBelowWall(i, j, I, J);
                    }
                }

                //exception case (End Column, j = columncells -1)
                //check for vertical wall to the right of the cell
                if( j == (board.ColumnCells - 1))
                {
                    if(board.Obstacles[I,J+1] == 1)
                    {
                        DrawRightWall(i, j, I, J);
                    }
                }
            }
        }
    }

    void DrawAboveWall(int i, int j, int I, int J)
    {
        GameObject tmp = new GameObject(""+ i + " " + j);


        tmp.transform.SetParent(boardObject.transform);
        tmp.transform.localScale = tmp.transform.localScale / (spriteScale * 2);

        //create new Linerenderer, set texture and width
        LineRenderer renderer = (LineRenderer)tmp.AddComponent<LineRenderer>();
        renderer.material.SetTexture("_MainTex", (Texture)ObstacleTexArr[0]);
        float width  = 0.1f;
        renderer.startWidth = width;
        renderer.endWidth = width;

        //1,1   -1,3    1,3
        //1,3    1,3    3,3
        //3,1   -1,1    1,1
        //3,3    1,1    3,1
        //Debug.Log("i:"+i+"\tj:"+j);
        //Debug.Log("I:"+I+"\tJ:"+J);
        //Debug.Log("("+(J-2)+","+(board.Columns - (I+1))+")");
        //Debug.Log("("+J+","+(board.Columns - (I+1))+")");

        //assign first and second index to draw line
        var centerOfCell = new Vector3(j + 0.5f, (-1 * i) + 0.5f, -2);
        var topLeftCorner = centerOfCell + new Vector3(-0.5f, 0.5f, 0);
        var topRightCorner = centerOfCell + new Vector3(0.5f, 0.5f, 0);
        renderer.SetPosition(0, topLeftCorner);
        renderer.SetPosition(1, topRightCorner);
    }
    void DrawLeftWall(int i, int j, int I, int J)
    {
        GameObject tmp = new GameObject(""+ i + " " + j);


        tmp.transform.SetParent(boardObject.transform);
        tmp.transform.localScale = tmp.transform.localScale / spriteScale;

        //create new Linerenderer, set texture and width
        LineRenderer renderer = (LineRenderer)tmp.AddComponent<LineRenderer>();
        renderer.material.SetTexture("_MainTex", (Texture)ObstacleTexArr[0]);
        float width  = 0.1f;
        renderer.startWidth = width;
        renderer.endWidth = width;
        
        //1,1   -1,1   -1,3
        //1,3    1,1    1,3
        //3,1   -1,-1  -1,1
        //3,3    1,-1   1,1
        //assign first and second index to draw line
        var centerOfCell = new Vector3(j + 0.5f, (-1 * i) + 0.5f, -2);
        var topLeftCorner = centerOfCell + new Vector3(-0.5f, 0.5f, 0);
        var botLeftCorner = centerOfCell + new Vector3(-0.5f, -0.5f, 0);
        renderer.SetPosition(0, topLeftCorner);
        renderer.SetPosition(1, botLeftCorner);
    }
    void DrawBelowWall(int i, int j, int I, int J)
    {
        //create new object for the wall, set parent to boardObject and scale to 1
        GameObject tmp = new GameObject(""+ i + " " + j);


        tmp.transform.SetParent(boardObject.transform);
        tmp.transform.localScale = tmp.transform.localScale / spriteScale;

        //create new Linerenderer, set texture and width
        LineRenderer renderer = (LineRenderer)tmp.AddComponent<LineRenderer>();
        renderer.material.SetTexture("_MainTex", (Texture)ObstacleTexArr[0]);
        float width  = 0.1f;
        renderer.startWidth = width;
        renderer.endWidth = width;
        
        //3,1  -1,-1    1,-1
        //3,3   1,-1    3,-1
        //assign first and second index to draw line
        var centerOfCell = new Vector3(j + 0.5f, (-1 * i) + 0.5f, -2);
        var botRightCorner = centerOfCell + new Vector3(0.5f, -0.5f, 0);
        var botLeftCorner = centerOfCell + new Vector3(-0.5f, -0.5f, 0);
        renderer.SetPosition(0, botRightCorner);
        renderer.SetPosition(1, botLeftCorner);
    }
    void DrawRightWall(int i, int j, int I, int J)
    {
        GameObject tmp = new GameObject(""+ i + " " + j);


        tmp.transform.SetParent(boardObject.transform);
        tmp.transform.localScale = tmp.transform.localScale / spriteScale;

        //create new Linerenderer, set texture and width
        LineRenderer renderer = (LineRenderer)tmp.AddComponent<LineRenderer>();
        renderer.material.SetTexture("_MainTex", (Texture)ObstacleTexArr[0]);
        float width  = 0.1f;
        renderer.startWidth = width;
        renderer.endWidth = width;
        
        //1,3  3,1    3,3
        //3,3  3,-1   3,1
        //assign first and second index to draw line
        //Debug.Log("i:"+i+"\tj:"+j);
        //Debug.Log("I:"+I+"\tJ:"+J);
        var centerOfCell = new Vector3(j + 0.5f, (-1 * i) + 0.5f, -2);
        var botRightCorner = centerOfCell + new Vector3(0.5f, -0.5f, 0);
        var topRightCorner = centerOfCell + new Vector3(0.5f, 0.5f, 0);
        renderer.SetPosition(0, topRightCorner);
        renderer.SetPosition(1, botRightCorner);
    }
    
    void DrawPenguins()
    {
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                int I = Board.CellToCoord(i);
                int J = Board.CellToCoord(j);


                if (board.Penguins[I,J] == 0) {continue;}
                //Debug.Log("I:"+I+"\tJ:"+J);
                GameObject tmp = new GameObject("" + i + " " + j);
                tmp.transform.SetParent(boardObject.transform);

                var adjustedIdx = new Vector3Int(j, -i, -1);
                tmp.transform.localPosition = boardGrid.CellToLocal(adjustedIdx) + new Vector3(0.5f, 0.5f, 0);
                tmp.transform.localScale = tmp.transform.localScale / (spriteScale * 2);
                tmp.transform.localScale = tmp.transform.localScale * 0.9f;

                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>();
                renderer.sprite = penguinSpriteArr[board.Penguins[I,J] - 1];
            }
        }
    }

    void DrawTargets()
    {
        for (int i = 0; i < board.RowCells; i++)
        {
            for (int j = 0; j < board.ColumnCells; j++)
            {
                int I = Board.CellToCoord(i);
                int J = Board.CellToCoord(j);

                if (board.Targets[I,J] == 0) {continue;}
                //Debug.Log("I:"+I+"\tJ:"+J);
                GameObject tmp = new GameObject("" + i + " " + j);
                tmp.transform.SetParent(boardObject.transform);

                var adjustedIdx = new Vector3Int(j, -i, -1);
                tmp.transform.localPosition = boardGrid.CellToLocal(adjustedIdx) + new Vector3(0.5f, 0.5f, 0);
                tmp.transform.localScale = tmp.transform.localScale / (spriteScale * 2);
                tmp.transform.localScale = tmp.transform.localScale * 0.9f;

                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>();
                renderer.sprite = targetSpriteArr[board.Targets[I,J] - 1];
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
