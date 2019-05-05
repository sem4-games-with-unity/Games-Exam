using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttonhandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadSingleplayer() {
        SceneManager.LoadScene("3x3 AI");
    }

    public void LoadMultiplayer() {
        SceneManager.LoadScene("3x3");
    }

    public void LoadHowToPlay() {
        SceneManager.LoadScene("How to play");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("Main Menu");
    }
}
