using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDetails : MonoBehaviour {
    public enum Team {
        White,
        Black
    };

    public enum Mode {
        Singleplayer,
        Multiplayer
    };

    private static Team winner, loser;
    private static string winCondition;
    private static int boardSizeX = 4, boardSizeY = 4;
    private static Mode mode = Mode.Singleplayer;

    public static Mode GameMode {
        get => mode;
        set => mode = value;
    }

    public static Team Winner {
        get => winner;
        set => winner = value;
    }

    public static Team Loser {
        get => loser;
        set => loser = value;
    }

    public static string WinCondition {
        get => winCondition;
        set => winCondition = value;
    }

    public static int BoardSizeX {
        get => boardSizeX;
        set => boardSizeX = value;
    }

    public static int BoardSizeY {
        get => boardSizeY;
        set => boardSizeY = value;
    }
}
