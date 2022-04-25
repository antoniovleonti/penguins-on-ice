using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreBoardManager : MonoBehaviour
{
    public Button mainMenuButton;
   
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        mainMenuButton = GameObject.Find("MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(mainMenuOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void mainMenuOnClick()
    {
        SceneManager.LoadScene(0);
    }
}
