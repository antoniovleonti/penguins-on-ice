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

        string s = "obstacles\n";
        for (int i = 0; i < pb.Rows; i++)
        { 
            for (int j = 0; j < pb.Columns; j++)
            {
                s += pb.Obstacles[i,j];
            }
            s += "\n";
        }
        Debug.Log(s);
        s = "targets\n";
        for (int i = 0; i < pb.Rows; i++)
        { 
            for (int j = 0; j < pb.Columns; j++)
            {
                s += pb.Targets[i,j];
            }
            s += "\n";
        }
        Debug.Log(s);
        s = "penguins\n";
        for (int i = 0; i < pb.Rows; i++)
        { 
            for (int j = 0; j < pb.Columns; j++)
            {
                s += pb.Penguins[i,j];
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    // Update is called once per frame
    void Update()
    {
        // not necessary to call these every frame in practice,
        // but it can be useful for testing and experimenting
        EraseBoard();
        DrawBoard();
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
                tmp.transform.localPosition = 2 * boardGrid.CellToLocal(adjustedIdx);
                // use the iceSpriteScale to make the sprite length and width (1,1)
                tmp.transform.localScale = tmp.transform.localScale / spriteScale;
                
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

                    //1,1   -1,3    1,3
                    //1,3    1,3    3,3
                    //3,1   -1,1    1,1
                    //3,3    1,1    3,1
                    //Debug.Log("i:"+i+"\tj:"+j);
                    //Debug.Log("I:"+I+"\tJ:"+J);
                    //Debug.Log("("+(J-2)+","+(board.Columns - (I+1))+")");
                    //Debug.Log("("+J+","+(board.Columns - (I+1))+")");

                    //assign first and second index to draw line
                    var firstIdx = new Vector3Int(J-2, -1 *I +2, 1);
                    var secondIdx = new Vector3Int(J, -1 *I +2, 1);
                    renderer.SetPosition(0, firstIdx);
                    renderer.SetPosition(1, secondIdx);
                }
                //check for vertical wall to the left of the cell
                if(board.Obstacles[I,J-1] == 1)
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
                    
                    //1,1   -1,1   -1,3
                    //1,3    1,1    1,3
                    //3,1   -1,-1  -1,1
                    //3,3    1,-1   1,1
                    //assign first and second index to draw line
                    var firstIdx = new Vector3Int(J-2, -1 * I +2, 1);
                    var secondIdx = new Vector3Int(J-2, -1 * I , 1);
                    renderer.SetPosition(0, firstIdx);
                    renderer.SetPosition(1, secondIdx);
                }

                //exception case (End row, i = rowcells -1)
                //check for horizontal wall below cell
                if( i == (board.RowCells - 1))
                {
                    if(board.Obstacles[I+1,J] == 1)
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
                        var firstIdx = new Vector3Int((J-2), -1 * I, 1);
                        var secondIdx = new Vector3Int(J, -1 * I, 1);
                        renderer.SetPosition(0, firstIdx);
                        renderer.SetPosition(1, secondIdx);
                    }
                }

                //exception case (End Column, j = columncells -1)
                //check for vertical wall to the right of the cell
                if( j == (board.ColumnCells - 1))
                {
                    if(board.Obstacles[I,J+1] == 1)
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
                        
                        //1,3  3,1    3,3
                        //3,3  3,-1   3,1
                        //assign first and second index to draw line
                        //Debug.Log("i:"+i+"\tj:"+j);
                        //Debug.Log("I:"+I+"\tJ:"+J);
                        var firstIdx = new Vector3Int(J, -1 * I+2, 1);
                        var secondIdx = new Vector3Int(J, -1 * I, 1);
                        renderer.SetPosition(0, firstIdx);
                        renderer.SetPosition(1, secondIdx);
                    }
                }
            }
        }
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
                Debug.Log("I:"+I+"\tJ:"+J);
                GameObject tmp = new GameObject("" + i + " " + j);
                tmp.transform.SetParent(boardObject.transform);

                var adjustedIdx = new Vector3Int(j, -i, -1);
                tmp.transform.localPosition = 2 * boardGrid.CellToLocal(adjustedIdx);
                tmp.transform.localScale = tmp.transform.localScale / spriteScale;
                tmp.transform.localScale = tmp.transform.localScale * 0.9f;

                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>();
                renderer.sprite = penguinSpriteArr[board.Penguins[I,J] - 1];
            }
        }
        
        // for (int i = 1; i < board.Rows; i += 2)
        // {
        //     for (int j = 1; j < board.Columns; j += 2)
        //     {
        //         if(board.Penguins[i,j] == 0){continue;}
        //         // set up a game object for this tile
        //         GameObject tmp = new GameObject("" + i + " " + j);
        //         tmp.transform.SetParent(boardObject.transform);
        //         // must adjust indices to account for the fact that background
        //         // tiles only exist on odd-numbered indices in array
        //         var adjustedIdx = new Vector3Int((j-1)/2, (i-1)/2, -1);
        //         // use grid component to calculate correct position of tile and move
        //         // the tile to that position
        //         tmp.transform.localPosition = 2 * boardGrid.CellToLocal(adjustedIdx);
        //         // use the SpriteScale to make the sprite length and width (1,1)
        //         tmp.transform.localScale = tmp.transform.localScale / spriteScale;
        //         tmp.transform.localScale = tmp.transform.localScale * 0.9f;

        //         // now add a sprite renderer so we can see our game object
        //         SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>(); 
        //         // pick the right sprite based on the number in the obstacles array
        //         renderer.sprite = penguinSpriteArr[board.Penguins[i,j] - 1];
        //     }
        // }
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
                Debug.Log("I:"+I+"\tJ:"+J);
                GameObject tmp = new GameObject("" + i + " " + j);
                tmp.transform.SetParent(boardObject.transform);

                var adjustedIdx = new Vector3Int(j, -i, -1);
                tmp.transform.localPosition = 2 * boardGrid.CellToLocal(adjustedIdx);
                tmp.transform.localScale = tmp.transform.localScale / spriteScale;
                tmp.transform.localScale = tmp.transform.localScale * 0.9f;

                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>();
                renderer.sprite = targetSpriteArr[board.Targets[I,J] - 1];
            }
        }
        
        // for (int i = 1; i < board.Rows; i += 2)
        // {
        //     for (int j = 1; j < board.Columns; j += 2)
        //     {
        //         if(board.Targets[i,j] == 0){continue;}
        //         // set up a game object for this tile
        //         GameObject tmp = new GameObject("" + i + " " + j);
        //         tmp.transform.SetParent(boardObject.transform);
        //         // must adjust indices to account for the fact that background
        //         // tiles only exist on odd-numbered indices in array
        //         var adjustedIdx = new Vector3Int((j-1)/2, (i-1)/2, -1);
        //         // use grid component to calculate correct position of tile and move
        //         // the tile to that position
        //         tmp.transform.localPosition = 2 * boardGrid.CellToLocal(adjustedIdx);
        //         // use the SpriteScale to make the sprite length and width (1,1)
        //         tmp.transform.localScale = tmp.transform.localScale / spriteScale;
        //         tmp.transform.localScale = tmp.transform.localScale * 0.9f;
        //         // now add a sprite renderer so we can see our game object
        //         SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>(); 
        //         // pick the right sprite based on the number in the obstacles array
        //         renderer.sprite = targetSpriteArr[board.Targets[i,j] - 1];
        //     }
        // }
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
