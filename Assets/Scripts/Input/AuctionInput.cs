using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AuctionInput : MonoBehaviour
{
    Auctioneer auctioneer;
    PlayerManager pm;
    AudioSource music;
    
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
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

        List<int> notConceded = pm.PollForConcessions();
        // if all players have conceded
        if ((pm.PlayerCount > 0 && notConceded.Count == 0) ||
            // or if all but the player with the best bid have conceded
            (   pm.PlayerCount > 1 &&
                notConceded.Count == 1 && 
                notConceded[0] == auctioneer.GetLeadingPlayer()))
        {
            auctioneer.RemainingSeconds = 0f;
        }
    }
}
