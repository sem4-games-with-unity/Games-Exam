using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class BoardManager : MonoBehaviour {
    public static BoardManager Instance {
        get; set;
    }
    private bool[,] allowedMoves {
        get; set;
    }
    public ChessFigure[,] ChessFigurePositions {
        get; set;
    }
    private ChessFigure selectedFigure;
    private const float TILE_SIZE = 1.0f, TILE_OFFSET = 0.5f;
    int selectionX = -1, selectionY = -1;
    public List<GameObject> chessFigures;
    private List<GameObject> activeFigures = new List<GameObject>();
    public bool isWhiteTurn = true;
    bool hasLegalMoves = true;
    bool useAI;
    public AI ai;
    private bool endgame = false;

    // Start is called before the first frame update
    void Start() {
        useAI = GameDetails.GameMode == GameDetails.Mode.Singleplayer;
        Instance = this;
        ChessFigurePositions = new ChessFigure[GameDetails.BoardSizeX, GameDetails.BoardSizeY];
        SpawnAllChessFigures();
    }

    // Update is called once per frame
    void Update() {
        DrawChessBoard();
        UpdateSelection();
        hasLegalMoves = CheckLegalMoves();
        if (!hasLegalMoves) {
            endgame = true;
            StartCoroutine(EndGame(true));
        } else {
            if (!endgame) {
                if (useAI) {
                    switch (isWhiteTurn) {
                        case true:
                            if (Input.GetMouseButtonDown(0)) {
                                if (selectionX >= 0 && selectionY >= 0) {
                                    if (selectedFigure == null)
                                        SelectChessFigure(selectionX, selectionY);
                                    else
                                        MoveChessFigure(selectionX, selectionY);
                                }
                            }
                            break;
                        case false:
                            Vector2 aiMove = new Vector2();
                            do {
                                selectedFigure = ai.SelectChessFigure();
                                allowedMoves = selectedFigure.PossibleMove();
                                aiMove = ai.MakeMove(selectedFigure);
                            } while (aiMove.x < 0 && aiMove.y < 0);
                            Debug.Log("Moving (" + selectedFigure.CurrentX + "," + selectedFigure.CurrentY + ") to (" + aiMove.x + "," + aiMove.y + ")");
                            MoveChessFigure((int) Math.Round(aiMove.x), (int) Math.Round(aiMove.y));
                            break;
                        default:
                            break;
                    }
                } else {
                    if (Input.GetMouseButtonDown(0)) {
                        if (selectionX >= 0 && selectionY >= 0) {
                            if (selectedFigure == null)
                                SelectChessFigure(selectionX, selectionY);
                            else
                                MoveChessFigure(selectionX, selectionY);
                        }
                    }
                }
            }
        }
    }

    private void SelectChessFigure(int x, int y) {
        if (ChessFigurePositions[x, y] == null)
            return;
        if (ChessFigurePositions[x, y].isWhite != isWhiteTurn)
            return;

        bool hasAtLeastOneMove = false;
        allowedMoves = ChessFigurePositions[x, y].PossibleMove();

        for (int i = 0; i < GameDetails.BoardSizeX; i++) {
            for (int j = 0; j < GameDetails.BoardSizeY; j++) {
                if (allowedMoves[i, j]) {
                    hasAtLeastOneMove = true;
                    i = GameDetails.BoardSizeX - 1;
                    break;
                }
            }
        }

        if (!hasAtLeastOneMove)
            return;

        selectedFigure = ChessFigurePositions[x, y];
        BoardHighlighting.Instance.HighlightAllowedMoves(allowedMoves);
    }

    private bool CheckLegalMoves() {
        bool hasLegalMoves = false;
        for (int i = 0; i < activeFigures.Count; i++) {
            if (hasLegalMoves) {
                break;
            }
            if (activeFigures[i].GetComponent<ChessFigure>().isWhite == isWhiteTurn) {
                bool[,] PossibleMoves = activeFigures[i].GetComponent<ChessFigure>().PossibleMove();
                bool tmp = false;
                foreach (bool b in PossibleMoves) {
                    if (!tmp) {
                        tmp = b;
                    }
                }
                if (!hasLegalMoves) {
                    hasLegalMoves = tmp;
                }
            }
        }
        return hasLegalMoves;
    }

    private void MoveChessFigure(int x, int y) {
        if (allowedMoves[x, y]) {
            ChessFigure c = ChessFigurePositions[x, y];
            if (c != null && c.isWhite != isWhiteTurn) {
                activeFigures.Remove(c.gameObject);
                Destroy(c.gameObject);
            }

            ChessFigurePositions[selectedFigure.CurrentX, selectedFigure.CurrentY] = null;
            selectedFigure.transform.DOMove(GetTileCenter(x, y), 0.5f);
            //selectedFigure.transform.position = GetTileCenter(x, y);
            selectedFigure.SetPosition(x, y);
            ChessFigurePositions[x, y] = selectedFigure;
            if (y == 0 || y == GameDetails.BoardSizeY - 1) {
                endgame = true;
                StartCoroutine(EndGame(false));
            } else {
                isWhiteTurn = !isWhiteTurn;
            }
        }

        BoardHighlighting.Instance.HideHighlights();
        selectedFigure = null;
    }

    private void DrawChessBoard() {
        Vector3 widthLine = Vector3.right * GameDetails.BoardSizeX;
        Vector3 heightLine = Vector3.forward * GameDetails.BoardSizeY;

        for (int i = 0; i <= GameDetails.BoardSizeX; i++) {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= GameDetails.BoardSizeY; j++) {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }
        if (selectionX >= 0 && selectionY >= 0) {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * (selectionX + 1),
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX);
        }
    }

    private void UpdateSelection() {
        if (!Camera.main)
            return;

        RaycastHit hit;
        float raycastDistance = 100.0f;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastDistance, LayerMask.GetMask("ChessPlane"))) {
            selectionX = (int) hit.point.x;
            selectionY = (int) hit.point.z;
        } else {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnChessFigure(int index, int x, int y) {
        GameObject go = Instantiate(chessFigures[index], GetTileCenter(x, y), chessFigures[index].transform.rotation) as GameObject;
        go.transform.SetParent(transform);
        ChessFigurePositions[x, y] = go.GetComponent<ChessFigure>();
        ChessFigurePositions[x, y].SetPosition(x, y);
        activeFigures.Add(go);
    }

    private Vector3 GetTileCenter(int x, int y) {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void SpawnAllChessFigures() {
        for (int i = 0; i < GameDetails.BoardSizeX; i++) {
            // White
            SpawnChessFigure(0, i, 0);
            // Black
            SpawnChessFigure(1, i, GameDetails.BoardSizeY - 1);
        }
    }

    IEnumerator EndGame(bool negate) {
        yield return new WaitForSeconds(0.5f);
        if (negate) {
            isWhiteTurn = !isWhiteTurn;
            GameDetails.WinCondition = "had no available moves";
        } else {
            GameDetails.WinCondition = "reached the final line";
        }

        if (isWhiteTurn) {
            GameDetails.Winner = GameDetails.Team.White;
            GameDetails.Loser = GameDetails.Team.Black;
        } else {
            GameDetails.Winner = GameDetails.Team.Black;
            GameDetails.Loser = GameDetails.Team.White;
        }

        foreach (GameObject go in activeFigures)
            Destroy(go);

        SceneManager.LoadScene("Result");
        yield return null;
    }

    public List<GameObject> GetAllActiveFigures() {
        return activeFigures;
    }
}
