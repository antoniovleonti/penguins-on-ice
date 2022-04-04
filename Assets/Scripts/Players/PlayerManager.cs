using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager 
{
  public List<Player> Players;
  private HashSet<Gamepad> registeredGamepads = new HashSet<Gamepad>();
  public bool KBLeftRegistered = false;
  public bool KBRightRegistered = false;

  public PlayerManager()
  {
    // constructor
    Players = new List<Player>();
    registeredGamepads = new HashSet<Gamepad>();
  }

  public void RegisterNewGamepads()
  {
    foreach (Gamepad g in Gamepad.all)
    {
      if (!registeredGamepads.Contains(g))
      {
        // set up new player
        RegisterPlayer( "<Gamepad>/dpad/up",    "<Gamepad>/dpad/down",
                        "<Gamepad>/buttonEast", "<Gamepad>/buttonSouth",
                        "<Gamepad>/buttonNorth", g);
        
        // add gamepad to the list
        registeredGamepads.Add(g);
      }
    }
  }

  public void RegisterKBRight()
  {
    if (KBRightRegistered) return;
    RegisterPlayer( "<Keyboard>/I", "<Keyboard>/K", 
                    "<Keyboard>/J", "<Keyboard/L", 
                    "<Keyboard>/U");
    KBRightRegistered = true;
  }
  public void RegisterKBLeft()
  {
    if (KBLeftRegistered) return;
    RegisterPlayer( "<Keyboard>/W", "<Keyboard>/S", 
                    "<Keyboard>/A", "<Keyboard/D", 
                    "<Keyboard>/Q");
    KBLeftRegistered = true;
  }

  private void RegisterPlayer(string tup,
                              string tdown,
                              string treset,
                              string placeBid,
                              string concede,
                              InputDevice device = null)
  {
    var input = new InputActionMap();

    input.AddAction("TickerUp", binding: tup);
    input.AddAction("TickerDown", binding: tdown);
    input.AddAction("TickerReset", binding: treset);

    input.AddAction("PlaceBid", binding: placeBid);
    input.AddAction("Continue", binding: concede);

    input.Enable();
    
    if (device != null) input.devices = new InputDevice[] { device }; 

    string name = "Player " + (Players.Count + 1);
    Players.Add(new Player(name, 0, 0, 0, input));
  }
}