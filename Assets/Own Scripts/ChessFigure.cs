﻿using UnityEngine;

public abstract class ChessFigure : MonoBehaviour {
    public int CurrentX {
        get; set;
    }
    public int CurrentY {
        get; set;
    }
    public bool isWhite;

    public void SetPosition(int x, int y) {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove() {
        return new bool[GameDetails.BoardSizeX, GameDetails.BoardSizeY];
    }
}