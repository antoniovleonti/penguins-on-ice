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
        
        ui.RefreshTickers(tickers);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.UpdateTickers() || checkForBids())
        {
            ui.RefreshTickers(tickers);
        }
    }


    bool checkForBids ()
    {
        bool bidFound = false;
        for (int i = 0; i < PlayerCount; i++)
        {
            if (gamepads[i].buttonSouth.wasPressedThisFrame)
            {
                if (tickers[i] > 0) auctioneer.AddPlayerBid(i, tickers[i]);
                tickers[i] = 0;
                bidFound = true;
            }
        }
        return bidFound;
    }
}
