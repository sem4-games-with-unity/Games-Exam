using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttonhandler : MonoBehaviour {
    public void LoadSingleplayer() {
        GameDetails.GameMode = GameDetails.Mode.Singleplayer;
        SceneManager.LoadScene("Board Select");
    }

    public void LoadMultiplayer() {
        GameDetails.GameMode = GameDetails.Mode.Multiplayer;
        SceneManager.LoadScene("Board Select");
    }

    public void SetBoard(string board) {
        switch (board) {
            case "3x3":
                GameDetails.BoardSizeX = 3;
                GameDetails.BoardSizeY = 3;
                break;
            case "4x3":
                GameDetails.BoardSizeX = 4;
                GameDetails.BoardSizeY = 3;
                break;
            case "4x4":
                GameDetails.BoardSizeX = 4;
                GameDetails.BoardSizeY = 4;
                break;
            case "5x3":
                GameDetails.BoardSizeX = 5;
                GameDetails.BoardSizeY = 3;
                break;
            case "5x4":
                GameDetails.BoardSizeX = 5;
                GameDetails.BoardSizeY = 4;
                break;
            case "5x5":
                GameDetails.BoardSizeX = 5;
                GameDetails.BoardSizeY = 5;
                break;
            default:
                break;
        }
        SceneManager.LoadScene(board);
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
