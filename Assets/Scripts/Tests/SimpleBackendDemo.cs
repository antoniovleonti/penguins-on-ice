using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBackendDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // this function will just play a very simple game using the boardstate class
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
        Board bs = new Board(obstacles, penguins, targets);       
        Debug.Log(bs.make_move(1,1, 1,0)); // move down
        Debug.Log(bs.make_move(3,1, 0,1)); // move right
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
