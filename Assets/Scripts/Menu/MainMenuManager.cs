using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton, howToPlayButton, exitButton;
    private AssetBundle myAssetBundle;

    
    // Start is called before the first frame update
    void Start()
    {
        playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        playButton.onClick.AddListener(playOnClick);

        howToPlayButton = GameObject.Find("HowToPlayButton").GetComponent<Button>();
        howToPlayButton.onClick.AddListener(howToPlayOnClick);

        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(exitOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playOnClick()
    {
        SceneManager.LoadScene(1);
    }

    public void howToPlayOnClick()
    {
        SceneManager.LoadScene(2);
    }

    public void exitOnClick()
    {
        //Must flip the lines when building
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }
}