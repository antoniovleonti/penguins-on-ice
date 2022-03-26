using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private Vector3Int? click;

    GameObject PopupWindow;
    // Start is called before the first frame update
    void Start()
    {
        PopupWindow = GameObject.Find("PopupMessage");
        PopupWindow.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (click == null)
            {
                PopupWindow.SetActive(false);
            }
        }
    }
}
