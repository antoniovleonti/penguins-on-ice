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
}
