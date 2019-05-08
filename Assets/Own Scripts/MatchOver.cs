using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class MatchOver : MonoBehaviour {
    public GameObject resultText;
    public GameObject winConditionText;
    // Start is called before the first frame update
    void Start() {
        resultText.GetComponent<Text>().text = GameDetails.Winner + " team won!";
        if (GameDetails.WinCondition == "had no available moves")
            winConditionText.GetComponent<Text>().text = GameDetails.Loser + " " + GameDetails.WinCondition;
        if (GameDetails.WinCondition == "reached the final line")
            winConditionText.GetComponent<Text>().text = GameDetails.Winner + " " + GameDetails.WinCondition;
    }
}
