using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public int[,] board;
    public bool isWhite;
    bool isLeaf = false;
    public List<Node> children = new List<Node>();
    int depth;
    float score;
    List<int[,]> newBoardStates = new List<int[,]>();
    public string nodeName;

    public Node(int[,] board, bool isWhite, int depth, string nodeName) {
        //Debug.Log(nodeName);
        this.depth = depth;
        this.isWhite = isWhite;
        this.board = board;
        this.nodeName = nodeName;
        SetChildren();
        //Debug.Log(children.Count);
        CheckLeaf();
    }

    public void PrintBoard() {
        string strBoard = "";
        for (int y = GameDetails.BoardSizeY - 1; y >= 0; y--) {
            string row = "";
            for (int x = 0; x < GameDetails.BoardSizeX; x++) {
                row += board[x, y] + " ";
            }
            strBoard += row + "\n";
        }
        Debug.Log(strBoard);
    }

    public float CalcScore() {
        if (isLeaf) {
            if (!isWhite) {
                score = 100f / depth;
            } else {
                score = -100f / depth;
            }
        } else {
            float sum = 0;
            foreach (Node n in children) {
                sum += n.CalcScore();
            }
            score = sum / children.Count;
        }
        return score;
    }

    void CheckLeaf() {
        if (newBoardStates.Count == 0) {
            isLeaf = true;
        }
    }

    bool CheckEndGame() {
        bool gameOver = false;
        for (int x = 0; x < GameDetails.BoardSizeX; x++) {
            if (board[x, 0] == -1) {
                gameOver = true;
            }
            if (board[x, GameDetails.BoardSizeY - 1] == 1) {
                gameOver = true;
            }
        }
        return gameOver;
    }

    void SetChildren() {
        if (!CheckEndGame()) {
            FindNewBoardStates();
            //int count = 1;
            //foreach (int[,] newBoard in newBoardStates) {
            //    string childName = nodeName + "->c" + count;
            //    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
            //    children.Add(child);
            //    count++;
            //}
        }
    }

    void FindNewBoardStates() {
        for (int x = 0; x < GameDetails.BoardSizeX; x++) {
            for (int y = 0; y < GameDetails.BoardSizeY; y++) {
                if (isWhite) {
                    if (board[x, y] == 1) {
                        MakeNewBoardState(x, y);
                    }
                } else {
                    if (board[x, y] == -1) {
                        MakeNewBoardState(x, y);
                    }
                }
            }
        }
    }

    void MakeNewBoardState(int x, int y) {
        if (isWhite) {
            // Diagonal Left
            if (x != 0 && y != GameDetails.BoardSizeY - 1) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x - 1, y + 1] == -1) {
                    newBoard[x, y] = 0;
                    newBoard[x - 1, y + 1] = 1;
                    newBoardStates.Add(newBoard);
                    string childName = "(" + x + ", " + y + ")->(" + (x - 1) + "," + (y + 1) + ")";
                    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                    children.Add(child);
                }
            }
            // Diagonal Right
            if (x != GameDetails.BoardSizeX - 1 && y != GameDetails.BoardSizeY - 1) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x + 1, y + 1] == -1) {
                    newBoard[x, y] = 0;
                    newBoard[x + 1, y + 1] = 1;
                    newBoardStates.Add(newBoard);
                    string childName = "(" + x + ", " + y + ")->(" + (x + 1) + "," + (y + 1) + ")";
                    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                    children.Add(child);
                }
            }
            // Forward
            if (y != GameDetails.BoardSizeY - 1) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x, y + 1] == 0) {
                    newBoard[x, y] = 0;
                    newBoard[x, y + 1] = 1;
                    newBoardStates.Add(newBoard);
                    string childName = "(" + x + ", " + y + ")->(" + x + "," + (y + 1) + ")";
                    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                    children.Add(child);
                }
            }
        } else {
            // Diagonal Left
            if (x != 0 && y != 0) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x - 1, y - 1] == 1) {
                    newBoard[x, y] = 0;
                    newBoard[x - 1, y - 1] = -1;
                    newBoardStates.Add(newBoard);
                    string childName = "(" + x + ", " + y + ")->(" + (x - 1) + "," + (y - 1) + ")";
                    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                    children.Add(child);
                }
            }
            // Diagonal Right
            if (x != GameDetails.BoardSizeX - 1 && y != 0) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x + 1, y - 1] == 1) {
                    newBoard[x, y] = 0;
                    newBoard[x + 1, y - 1] = -1;
                    newBoardStates.Add(newBoard);
                    string childName = "(" + x + ", " + y + ")->(" + (x + 1) + "," + (y - 1) + ")";
                    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                    children.Add(child);
                }
            }
            // Forward
            if (y != 0) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x, y - 1] == 0) {
                    newBoard[x, y] = 0;
                    newBoard[x, y - 1] = -1;
                    newBoardStates.Add(newBoard);
                    string childName = "(" + x + ", " + y + ")->(" + x + "," + (y - 1) + ")";
                    Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                    children.Add(child);
                }
            }
        }
    }
}
