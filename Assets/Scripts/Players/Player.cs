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
    public bool Concedes = false;
    public bool IsActive = false;
    public bool HasTried = false;
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

    public bool PollTicker ()
    {
        bool wasUpdated = false;
        if (Input["tickerUp"].triggered)
        {
            int bid = CurrentBid;
            TickerValue += bid==0 || TickerValue<bid-1 ? 1 : 0;
            wasUpdated = true;
        }
        if (Input["tickerDown"].triggered)
        {
            TickerValue -= TickerValue >= 1 ? 1 : 0;
            wasUpdated = true;
        }
        if (Input["tickerReset"].triggered)
        {
            TickerValue = 0;
            wasUpdated = true;
        }
        return wasUpdated;
    }

    public int PollForBid ()
    {
        int prev = TickerValue;
        if (Input["PlaceBid"].triggered && TickerValue > 0)
        {
            CurrentBid = TickerValue;
            TickerValue = 0;
            return prev;
        }
        return -1;
    }

    public bool PollForConcession ()
    {
        if (Input["Continue"].triggered)
        {
            Concedes = !Concedes;
        }
        return Concedes;
    }
}
