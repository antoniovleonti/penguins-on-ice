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
    }
}
