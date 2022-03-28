using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// this class shows how to use multiple gamepads
public class GamepadTest : MonoBehaviour
{
    int[] bids;
    Gamepad[] gamepads;
    // Start is called before the first frame update
    void Start()
    {
        gamepads = new Gamepad[Gamepad.all.Count];
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            gamepads[i] = Gamepad.all[i];
        }
        bids = new int[gamepads.Length];
    }

    // Update is called once per frame
    void Update()
    {
        bool wasTouched = false;
        for (int i = 0; i < gamepads.Length; i++)
        {
            if (gamepads[i].dpad.up.wasPressedThisFrame)
            {
                bids[i] += 1;
                wasTouched = true;
            }
            if (gamepads[i].dpad.down.wasPressedThisFrame)
            {
                bids[i] -= 1;
                wasTouched = true;
            }
        }
        if (wasTouched) Debug.Log(string.Join(" ", bids));
    }
}
