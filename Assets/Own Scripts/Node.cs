using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    int[,] board;
    bool isWhite, isLeaf = false;
    List<Node> children = new List<Node>();
    int depth, score;
    List<int[,]> newBoardStates = new List<int[,]>();
    string nodeName;

    public Node(int[,] board, bool isWhite, int depth, string nodeName) {
        //Debug.Log(nodeName);
        this.depth = depth;
        this.isWhite = isWhite;
        this.board = board;
        this.nodeName = nodeName;
        SetChildren();
        CheckLeaf();
    }

    public void PrintChildren() {
        Debug.Log(nodeName + ": " + CalcScore());
        foreach (Node n in children) {
            n.PrintChildren();
        }
    }

    int CalcScore() {
        if (isLeaf) {
            if (isWhite) {
                score = 1;
            } else {
                score = -1;
            }
        } else {
            int sum = 0;
            foreach (Node n in children) {
                sum += n.CalcScore();
            }
            score = sum;
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
            int count = 1;
            foreach (int[,] newBoard in newBoardStates) {
                string childName = nodeName + "->c" + count;
                Node child = new Node(newBoard, !isWhite, depth + 1, childName);
                children.Add(child);
                count++;
            }
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
                }
            }
            // Diagonal Right
            if (x != GameDetails.BoardSizeX - 1 && y != GameDetails.BoardSizeY - 1) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x + 1, y + 1] == -1) {
                    newBoard[x, y] = 0;
                    newBoard[x + 1, y + 1] = 1;
                    newBoardStates.Add(newBoard);
                }
            }
            // Forward
            if (y != GameDetails.BoardSizeY - 1) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x, y + 1] == 0) {
                    newBoard[x, y] = 0;
                    newBoard[x, y + 1] = 1;
                    newBoardStates.Add(newBoard);
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
                }
            }
            // Diagonal Right
            if (x != GameDetails.BoardSizeX - 1 && y != 0) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x + 1, y - 1] == 1) {
                    newBoard[x, y] = 0;
                    newBoard[x + 1, y - 1] = -1;
                    newBoardStates.Add(newBoard);
                }
            }
            // Forward
            if (y != 0) {
                int[,] newBoard = (int[,]) board.Clone();
                if (board[x, y - 1] == 0) {
                    newBoard[x, y] = 0;
                    newBoard[x, y - 1] = -1;
                    newBoardStates.Add(newBoard);
                }
            }
        }
    }
}
