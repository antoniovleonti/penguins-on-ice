using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBoardSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int tries = 0;
        Debug.Log("test");
        ProceduralBoard pb = null;
        // while (pb == null)
        //{
            //try { pb = new ProceduralBoard(4); }
            //catch (System.Exception) { tries++; }
        //}
        //Debug.Log("failed attempts: " + tries);
        pb = new ProceduralBoard(4); 
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
}
