    using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Menu/PlayersScriptableObject")]
public class PlayersScriptableObject : ScriptableObject
{
    [SerializeField]
    private int _score;
    public int Score
    {
        get { return _score; }
        set { _score = value; }
    }
    
}
