using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
  public int PlayerCount
  {
    get { return Players.Count; }
  }
  public List<Player> Players = new List<Player>();

  HashSet<Gamepad> registeredGamepads = new HashSet<Gamepad>();
  AuctionUIRenderer ui;

  bool kbLeftRegistered = false;
  bool kbRightRegistered = false;
  float lastLPress = 0; // time since last time l was pressed
  float lastDPress = 0; // time since last time l was pressed

  void Awake()
  {
    ui = gameObject.GetComponent<AuctionUIRenderer>();
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
  }

  void EndRound ()
  {
    for (int i = 0; i < PlayerCount; i++)
    {
      Players[i].Concedes = false;
      Players[i].IsActive = false;
      Players[i].HasTried = false;
      Players[i].TickerValue = 0;
      Players[i].CurrentBid = 0;
      // update ui
      ui.RefreshPlayer(i, Players[i]);
    }
  }
  
  void EndAuction ()
  {
    // reset the concessions
    for (int i = 0; i < PlayerCount; i++)
    {
      Players[i].Concedes = false;
      Players[i].TickerValue = 0;
      ui.RefreshPlayer(i, Players[i]);
    }
  }

  public void PollTickers ()
  {
    for (int i = 0; i < PlayerCount; i++)
    {
      if (Players[i].PollTicker())
      {
        ui.RefreshPlayer(i, Players[i]);
      }
    }
  }

  public void PollForBids ()
  {
    var auc = gameObject.GetComponent<Auctioneer>();
    for (int i = 0; i < PlayerCount; i++)
    {
      var bid = Players[i].PollForBid();
      if (bid != -1)
      {
        auc.AddPlayerBid(i, bid);
        ui.RefreshPlayer(i, Players[i]);
      }
    }
  }

  public bool PollForConcessions ()
  {
    bool allConceded = PlayerCount > 0;

    for (int i = 0; i < PlayerCount; i++)
    {
      bool didConcede = Players[i].PollForConcession();
      allConceded = allConceded && didConcede;
      ui.RefreshPlayer(i, Players[i]);
    }

    return allConceded;
  }

  public bool PollPlayerForConcession (int player)
  {
    return false;
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

    string name = "Player " + (PlayerCount + 1);
    var p = new Player(name, 0, 0, 0, input);
    Players.Add(p);
    ui.AddTracker();
    ui.RefreshPlayer(PlayerCount-1, p);
  }
}