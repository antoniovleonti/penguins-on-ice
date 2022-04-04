using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player 
{
    public string Name;
    public int Wins;
    public int TickerValue;
    public int CurrentBid;
    public InputActionMap Input;
    public Player(  string name,
                    int wins,
                    int tickerValue,
                    int currentBid,
                    InputActionMap input)
    {
        Name = name;
        Wins = wins;
        TickerValue = tickerValue;
        CurrentBid = currentBid;
        Input = input;
    }

    public bool UpdateTicker()
    {
        if (Input["tickerUp"].triggered)
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
}
