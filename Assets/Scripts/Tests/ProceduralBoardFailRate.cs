using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBoardFailRate : MonoBehaviour
{
    public int NumberOfTrials = 100000;
    // Start is called before the first frame update
    void Start()
    {
        int fails = 0;
        for (int k = 0; k < 100000; k++)
        {
            try { var bs = new PBoardBuilder(); }
            catch (System.Exception) { fails += 1; }
            // string s = "";
            //for (int i = 0; i < bs.Rows; i++)
            // { 
                // for (int j = 0; j < bs.Columns; j++)
                // {
                    // s += bs.Obstacles[i,j];
                //}
                // s += "\n";
            //}
            // Debug.Log(s);
        }
        Debug.Log(fails);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
