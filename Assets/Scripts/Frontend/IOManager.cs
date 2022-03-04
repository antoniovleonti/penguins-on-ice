using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOManager : MonoBehaviour
{
    public Texture2D IceTex; // editor-set texture for ice
    private Sprite iceSprite; // sprite to be displayed for ice
    private float iceSpriteScale; // size of sprite in world-space

    private Board board; // backend board we are displaying
    private GameObject boardObject; // parent object to all board display objects
    private Grid boardGrid; // gameobject addon to make grid calculations easier

    // Start is called before the first frame update
    void Start()
    {
        // create a new board
        int[,] obstacles = {
            {1,1,1,1,1,},
            {1,0,0,0,1,},
            {1,0,0,0,1,},
            {1,0,0,0,1,},
            {1,1,1,1,1,},
        };
        int[,] penguins = {
            {0,0,0,0,0,},
            {0,1,0,0,0,},
            {0,0,0,0,0,},
            {0,0,0,0,0,},
            {0,0,0,0,0,},
        };
        int[,] targets = {
            {0,0,0,0,0,},
            {0,0,0,0,0,},
            {0,0,0,0,0,},
            {0,0,0,1,0,},
            {0,0,0,0,0,},
        };
        board = new Board(obstacles, penguins, targets);

        // create board gameobject to be a parent to all tiles
        boardObject = new GameObject("board");
        // set its parent to the io manager
        boardObject.transform.SetParent(gameObject.transform);
        // add a board grid which will do grid position calculations for us
        boardGrid = boardObject.AddComponent(typeof(Grid)) as Grid;

        // create sprite from provided texture
        iceSprite = Sprite.Create(
            IceTex, new Rect(0.0f, 0.0f, IceTex.width, IceTex.height), 
            new Vector2(0.5f, 0.5f), 100.0f ); 

        // resize grid to match size of sprites
        iceSpriteScale = iceSprite.bounds.extents.x;
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
                var adjustedIdx = new Vector3Int((i-1)/2, (j-1)/2, 1);
                // use grid component to calculate correct position of tile and move
                // the tile to that position
                tmp.transform.localPosition = 2 * boardGrid.CellToLocal(adjustedIdx);
                // use the iceSpriteScale to make the sprite length and width (1,1)
                tmp.transform.localScale = tmp.transform.localScale / iceSpriteScale;
                
                // now add a sprite renderer so we can see our game object
                SpriteRenderer renderer = (SpriteRenderer)tmp.AddComponent<SpriteRenderer>(); 
                renderer.sprite = iceSprite;
            }
        }
        // add walls (even indices in board.Obstacles -- on tile edges)

        // add penguins (odd indices in board.Penguins -- in front of tiles)

        // add targets (odd indices in board.Targets -- int front of tiles, behind penguins)
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
