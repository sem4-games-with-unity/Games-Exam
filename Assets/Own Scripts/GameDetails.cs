using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDetails : MonoBehaviour
{
    public enum Team {
        White,
        Black
    };

    private static Team winner; 

    public static Team Winner {
        get => winner;
        set => winner = value;
    }
}
