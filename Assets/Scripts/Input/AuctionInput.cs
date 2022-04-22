using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AuctionInput : MonoBehaviour
{
    Auctioneer auctioneer;
    AuctionUIRenderer ui;
    PlayerManager pm;
    
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        ui = gameObject.GetComponent<AuctionUIRenderer>();
        pm = gameObject.GetComponent<PlayerManager>();
        // this is what we will inform of player actions
        auctioneer = gameObject.GetComponent<Auctioneer>();
    }
    void OnEnable ()
    {
        auctioneer = gameObject.GetComponent<Auctioneer>();
    }

    // Update is called once per frame
    void Update()
    {
        pm.PollTickers();
        pm.PollForBids();

        var notConceded = pm.PollForConcessions();
        if ((pm.PlayerCount > 0 && notConceded.Count == 0) ||
            (   notConceded.Count == 1 && 
                notConceded[0] == auctioneer.GetLeadingPlayer()))
        {
            auctioneer.RemainingSeconds = 0f;
        }
    }
}
