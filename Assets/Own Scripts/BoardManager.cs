using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Threading;
using System.Linq;

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
    Node rootNode;
    Node curNode;
    private Thread myThread;
    readonly object lockingObject = new object();

    private void Awake() {
        Instance = this;
        ChessFigurePositions = new ChessFigure[GameDetails.BoardSizeX, GameDetails.BoardSizeY];
        SpawnAllChessFigures();
        useAI = GameDetails.GameMode == GameDetails.Mode.Singleplayer;
        if (useAI) {
            myThread = new Thread(MakeGameTree);
            myThread.Start();
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (useAI) {
            //MakeGameTree();
            lock (lockingObject) {
                curNode = rootNode;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        Debug.Log("Current board");
        PrintBoard();
        DrawChessBoard();
        UpdateSelection();
        hasLegalMoves = CheckLegalMoves();
        if (!hasLegalMoves) {
            endgame = true;
            StartCoroutine(EndGame(true));
        } else {
            if (!endgame) {
                if (useAI) {
                    lock (lockingObject) {
                        switch (isWhiteTurn) {
                            case true:
                                if (Input.GetMouseButtonDown(0)) {
                                    if (selectionX >= 0 && selectionY >= 0) {
                                        if (selectedFigure == null) {
                                            SelectChessFigure(selectionX, selectionY);
                                        } else {
                                            MoveChessFigure(selectionX, selectionY);
                                            SetCurNode();
                                        }
                                    }
                                }
                                break;
                            case false:
                                if (curNode != null) {
                                    Node best = null;
                                    float minScore = float.MaxValue;
                                    foreach (Node n in curNode.children) {
                                        float score = n.CalcScore();
                                        if (score < minScore) {
                                            best = n;
                                            minScore = score;
                                        }
                                    }
                                    Debug.Log("curNode board");
                                    curNode.PrintBoard();
                                    Debug.Log("best child board");
                                    best.PrintBoard();
                                    for (int x = 0; x < GameDetails.BoardSizeX; x++) {
                                        for (int y = 0; y < GameDetails.BoardSizeY; y++) {
                                            if (curNode.board[x, y] == -1 && best.board[x, y] == 0) {
                                                SelectChessFigure(x, y);
                                            }
                                        }
                                    }
                                    allowedMoves = selectedFigure.PossibleMove();
                                    for (int x = 0; x < GameDetails.BoardSizeX; x++) {
                                        for (int y = 0; y < GameDetails.BoardSizeY; y++) {
                                            if (curNode.board[x, y] != -1 && best.board[x, y] == -1) {
                                                MoveChessFigure(x, y);
                                                SetCurNode();
                                            }
                                        }
                                    }
                                } else {
                                    Vector2 aiMove = new Vector2();
                                    do {
                                        selectedFigure = ai.SelectChessFigure();
                                        allowedMoves = selectedFigure.PossibleMove();
                                        aiMove = ai.MakeMove(selectedFigure);
                                    } while (aiMove.x < 0 && aiMove.y < 0);
                                    MoveChessFigure((int) Math.Round(aiMove.x), (int) Math.Round(aiMove.y));
                                }
                                break;
                            default:
                                break;
                        }
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

    void PrintBoard() {
        string strBoard = "";
        for (int y = GameDetails.BoardSizeY - 1; y >= 0; y--) {
            string row = "";
            for (int x = 0; x < GameDetails.BoardSizeX; x++) {
                string val = "";
                ChessFigure c = ChessFigurePositions[x, y];
                if (c == null) {
                    val += 0;
                } else {
                    if (c.isWhite) {
                        val += 1;
                    } else {
                        val += -1;
                    }
                }
                row += val + " ";
            }
            strBoard += row + "\n";
        }
        Debug.Log(strBoard);
    }

    private void SetCurNode() {
        if (curNode != null) {
            Debug.Log("curNode name: " + curNode.nodeName);
            int[,] newBoard = new int[GameDetails.BoardSizeX, GameDetails.BoardSizeY];
            for (int x = 0; x < GameDetails.BoardSizeX; x++) {
                for (int y = 0; y < GameDetails.BoardSizeY; y++) {
                    if (ChessFigurePositions[x, y] == null) {
                        newBoard[x, y] = 0;
                    } else {
                        if (ChessFigurePositions[x, y].isWhite) {
                            newBoard[x, y] = 1;
                        } else {
                            newBoard[x, y] = -1;
                        }
                    }
                }
            }
            foreach (Node n in curNode.children) {
                if (ArraysEqual(newBoard, n)) {
                    break;
                }
            }
            Debug.Log("curNode name: " + curNode.nodeName);
        }
    }

    private bool ArraysEqual(int[,] newBoard, Node n) {
        bool found = true;
        for (int x = 0; x < GameDetails.BoardSizeX; x++) {
            for (int y = 0; y < GameDetails.BoardSizeY; y++) {
                if (n.board[x, y] != newBoard[x, y]) {
                    curNode = n;
                    found = false;
                    x = GameDetails.BoardSizeX;
                    break;
                }
            }
        }
        return found;
    }

    private void SelectChessFigure(int x, int y) {
        Debug.Log("Selecting Pawn");
        ChessFigure cf = ChessFigurePositions[x, y];
        Debug.Log(cf);
        if (cf == null)
            return;
        if (cf.isWhite != isWhiteTurn)
            return;
        bool hasAtLeastOneMove = false;
        allowedMoves = cf.PossibleMove();
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
        selectedFigure = cf;
        Debug.Log("Pawn Selected");
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
            Debug.Log("Move allowed");
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
            Debug.Log("Moved Pawn");
            if (y == 0 || y == GameDetails.BoardSizeY - 1) {
                endgame = true;
                StartCoroutine(EndGame(false));
            } else {
                isWhiteTurn = !isWhiteTurn;
            }
            selectedFigure = null;
        }
        BoardHighlighting.Instance.HideHighlights();
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

    void MakeGameTree() {
        lock (lockingObject) {
            if (rootNode == null) {
                if (GameDetails.BoardSizeX == 3 && GameDetails.BoardSizeY == 3) {
                    rootNode = new Node(new int[,] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } }, true, 0, "root");
                }
                //if (GameDetails.BoardSizeX == 4 && GameDetails.BoardSizeY == 3) {
                //    rootNode = new Node(new int[,] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } }, true, 0, "root");
                //}
                //if (GameDetails.BoardSizeX == 4 && GameDetails.BoardSizeY == 4) {
                //    rootNode = new Node(new int[,] { { 1, 0, 0, -1 }, { 1, 0, 0, -1 }, { 1, 0, 0, -1 }, { 1, 0, 0, -1 } }, true, 0, "root");
                //}
                //if (GameDetails.BoardSizeX == 5 && GameDetails.BoardSizeY == 3) {
                //    rootNode = new Node(new int[,] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } }, true, 0, "root");
                //}
                //if (GameDetails.BoardSizeX == 5 && GameDetails.BoardSizeY == 4) {
                //    rootNode = new Node(new int[,] { { 1, 0, 0, -1 }, { 1, 0, 0, -1 }, { 1, 0, 0, -1 }, { 1, 0, 0, -1 }, { 1, 0, 0, -1 } }, true, 0, "root");
                //}
                //if (GameDetails.BoardSizeX == 5 && GameDetails.BoardSizeY == 5) {
                //    rootNode = new Node(new int[,] { { 1, 0, 0, 0, -1 }, { 1, 0, 0, 0, -1 }, { 1, 0, 0, 0, -1 }, { 1, 0, 0, 0, -1 }, { 1, 0, 0, 0, -1 } }, true, 0, "root");
                //}
                //rootNode.PrintChildren();
                Debug.Log("Created Game Tree");
                //rootNode.PrintChildren();
            }
        }
    }
}
