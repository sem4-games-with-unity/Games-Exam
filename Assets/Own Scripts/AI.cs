using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    System.Random r = new System.Random();

    public ChessFigure SelectChessFigure() {
        List<GameObject> activeFigures = BoardManager.Instance.GetAllActiveFigures();
        List<GameObject> blackFigures = new List<GameObject>();
        foreach (GameObject go in activeFigures) {
            if (!go.GetComponent<ChessFigure>().isWhite) {
                blackFigures.Add(go);
            }
        }
        GameObject gameObject = blackFigures[r.Next(blackFigures.Count)];
        return gameObject.GetComponent<ChessFigure>();
    }

    public Vector2 MakeMove(ChessFigure figure) {
        bool[,] possibleMoves = figure.PossibleMove();

        List<Vector2> possibleMovements = new List<Vector2>();

        for (int i = 0; i < GameDetails.BoardSizeX; i++) {
            for (int j = 0; j < GameDetails.BoardSizeY; j++) {
                if (possibleMoves[i, j]) {
                    possibleMovements.Add(new Vector2(i, j));
                }
            }
        }

        if (possibleMovements.Count > 0)
            return possibleMovements[r.Next(possibleMovements.Count)];
        else
            return new Vector2(-1, -1);
    }
}
