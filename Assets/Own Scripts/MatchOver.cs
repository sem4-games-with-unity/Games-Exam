using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class MatchOver : MonoBehaviour
{
    public GameObject resultText;
    // Start is called before the first frame update
    void Start(){
        resultText.GetComponent<Text>().text = GameDetails.Winner + " team won!";
    }
}
