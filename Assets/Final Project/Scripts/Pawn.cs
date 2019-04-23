using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessFigure {
    public override bool[,] PossibleMove() {
        bool[,] r = new bool[3, 3];
        ChessFigure c, c2;

        if (isWhite) {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 2) {
                c = BoardManager.Instance.ChessFigurePositions[CurrentX - 1, CurrentY + 1];
                if (c != null && !c.isWhite) {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }
            // Diagonal Right
            if (CurrentX != 2 && CurrentY != 2) {
                c = BoardManager.Instance.ChessFigurePositions[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite) {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }
            // Forward
            if (CurrentY != 2) {
                c = BoardManager.Instance.ChessFigurePositions[CurrentX, CurrentY + 1];
                if (c == null) {
                    r[CurrentX, CurrentY + 1] = true;
                }
            }
        } else {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 0) {
                c = BoardManager.Instance.ChessFigurePositions[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite) {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }
            // Diagonal Right
            if (CurrentX != 2 && CurrentY != 0) {
                c = BoardManager.Instance.ChessFigurePositions[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite) {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }
            // Forward
            if (CurrentY != 0) {
                c = BoardManager.Instance.ChessFigurePositions[CurrentX, CurrentY - 1];
                if (c == null) {
                    r[CurrentX, CurrentY - 1] = true;
                }
            }
        }
        return r;
    }
}