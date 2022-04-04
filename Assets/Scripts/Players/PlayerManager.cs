using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
  public List<Player> Players = new List<Player>();
  private HashSet<Gamepad> registeredGamepads = new HashSet<Gamepad>();
  bool kbLeftRegistered = false;
  bool kbRightRegistered = false;
  float lastLPress = 0; // time since last time l was pressed
  float lastDPress = 0; // time since last time l was pressed

  void Awake()
  {

  }

  void Update()
  {
    RegisterNewGamepads();

    lastLPress += Time.deltaTime;
    lastDPress += Time.deltaTime;

    if (Keyboard.current.lKey.wasPressedThisFrame) 
    {
      if (lastLPress <= 0.25f) RegisterKBRight();
      lastLPress = 0;
    }
    if (Keyboard.current.dKey.wasPressedThisFrame)
    {
      if (lastDPress <= 0.25f) RegisterKBLeft();
      lastDPress = 0;
    }

    foreach (var p in Players) Debug.Log(p.Name);
  }

  bool UpdateTickers ()
  {
    bool wasUpdated = false;
    for (int i = 0; i < PlayerCount; i++)
    {
    }
    return wasUpdated;
  }

  public void RegisterNewGamepads()
  {
    foreach (Gamepad g in Gamepad.all)
    {
      if (!registeredGamepads.Contains(g))
      {
        // set up new player
        RegisterPlayer( 
          "<Gamepad>/dpad/up", "<Gamepad>/dpad/down",
          "<Gamepad>/buttonEast", "<Gamepad>/buttonSouth", 
          "<Gamepad>/buttonNorth", g);
        
        // add gamepad to the list
        registeredGamepads.Add(g);
      }
    }
  }

  public void RegisterKBRight()
  {
    if (kbRightRegistered) return;
    RegisterPlayer( 
      "<Keyboard>/I", "<Keyboard>/K", 
      "<Keyboard>/J", "<Keyboard/L", 
      "<Keyboard>/U");
    kbRightRegistered = true;
  }
  public void RegisterKBLeft()
  {
    if (kbLeftRegistered) return;
    RegisterPlayer( 
      "<Keyboard>/W", "<Keyboard>/S", 
      "<Keyboard>/A", "<Keyboard/D", 
      "<Keyboard>/Q");
    kbLeftRegistered = true;
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