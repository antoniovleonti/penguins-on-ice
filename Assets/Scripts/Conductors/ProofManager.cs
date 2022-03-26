using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueues;

public class ProofManager : MonoBehaviour
{
    BinaryHeap<int,int> bidQ;
    Board board;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(Board board_, BinaryHeap<int,int> bidQ_)
    {
        board = board_;
        bidQ = bidQ_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
