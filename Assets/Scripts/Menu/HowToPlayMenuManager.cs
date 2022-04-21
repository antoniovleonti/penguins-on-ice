using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HowToPlayMenuManager : MonoBehaviour
{
    public Button mainMenuButton;
    private AssetBundle myAssetBundle;
    public GameObject popupTextPrefab;

    
    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton = GameObject.Find("MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(mainMenuOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void mainMenuOnClick()
    {
        //Vector3 spawn = new Vector3(0,0,1);
        // Vector3 spawn = mainMenuButton.transform.position + new Vector3(0, 10, 2);
        // Debug.Log("Spawn = "+ spawn.ToString("F3"));
        // var p = mainMenuButton.transform.parent;
        // GameObject floatingText = Instantiate(popupTextPrefab, spawn, Quaternion.identity, p);
        // floatingText.GetComponent<TMP_Text>().color = Color.red;
        // floatingText.GetComponent<TextPopup>().displayText = "+1";
        SceneManager.LoadScene(0);
    }
}