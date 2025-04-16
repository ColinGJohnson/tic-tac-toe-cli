using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public class TicTacToeTreeNode
    {
        public bool Explored { get; set; }

        public TileContent CurrentTurn { get; }

        public int MiniMaxScore { get; set; }

        public (int winning, int losing, int draw) SubtreeStats { get; set; }

        public TicTacToeBoard Board { get; }

        public List<TicTacToeTreeNode> Children { get; }

        public TicTacToeTreeNode(TicTacToeBoard board, TileContent currentTurn)
        {
            Explored = false;
            Board = board;
            CurrentTurn = currentTurn;
            Children = new List<TicTacToeTreeNode>();
        }

        public TileContent GetNextTurnPlayer()
        {
            return CurrentTurn == TileContent.PlayerOne ? TileContent.PlayerTwo : TileContent.PlayerOne;
        }

        public bool IsLeaf()
        {
            if (!Explored)
            {
                throw new InvalidOperationException("Node is not explored");
            }

            return Children.Count == 0;
        }
    }
}
