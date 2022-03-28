using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AuctionInput : MonoBehaviour
{
    public int PlayerCount 
    {
        get { return gamepads.Length; }
    }
    Auctioneer auctioneer;
    AuctionUIRenderer ui;
    Gamepad[] gamepads; // gamepads ordered by the player they are assigned to
    int[] tickers;
    
    void Awake()
    {
        gamepads = new Gamepad[Gamepad.all.Count];
        // PREDICTED ISSUE: UNPLUGGING AND REPLUGGING IN CONTROLLERS WILL RESULT
        // IN CONTROLLERS BEING REASSIGNED TO DIFFERENT PLAYERS
        for (int i = 0; i < Gamepad.all.Count; i++)
            gamepads[i] = Gamepad.all[i];

        ui = gameObject.GetComponent<AuctionUIRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // this is what we will inform of player actions
        auctioneer = gameObject.GetComponent<Auctioneer>();
        tickers = new int[PlayerCount];
        
        ui.RefreshTickers(tickers);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTickers() || checkForBids())
        {
            ui.RefreshTickers(tickers);
        }
    }

    bool updateTickers ()
    {
        bool wasUpdated = false;
        for (int i = 0; i < PlayerCount; i++)
        {
            if (gamepads[i].dpad.up.wasPressedThisFrame)
            {
                int bid = auctioneer.CurrentBids[i];
                tickers[i] += bid==0 || tickers[i]<bid-1 ? 1 : 0;
                wasUpdated = true;
            }
            if (gamepads[i].dpad.down.wasPressedThisFrame)
            {
                tickers[i] -= tickers[i] > 1 ? 1 : 0;
                wasUpdated = true;
            }
            if (gamepads[i].buttonEast.wasPressedThisFrame)
            {
                tickers[i] = 0;
                wasUpdated = true;
            }
        }
        return wasUpdated;
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
