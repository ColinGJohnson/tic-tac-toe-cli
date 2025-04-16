using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    enum GameStatus
    {
        ONGOING,
        WIN, 
        LOSS,
        DRAW
    }

    enum BotAlgorithm
    {
        RANDOM = 0,
        MINIMAX = 1,
        MOST_WINS = 2,
        LEAST_LOSSES = 3,
        MOST_DRAWS = 4
    }

    class TicTacToeGame
    {
        public char playerOneChar { get; set; }
        public char playerTwoChar { get; set; }
        public TileContent currentPlayer { get; set; }

        public TicTacToeBoard currentBoard { get; set; }

        /// <summary>
        ///     The current status of the game, from the perspective of the human player.
        /// </summary>
        public GameStatus Status { get; set; } = GameStatus.ONGOING;

        private Dictionary<string, TicTacToeTreeNode> moveGraph;

        private TicTacToeTreeNode CurrentNode
        {
            get
            {
                return moveGraph[GetBoardStateKey(currentBoard.BoardRepr, currentPlayer)];
            }
        }

        public TicTacToeGame(TileContent startingPlayer)
        {
            currentPlayer = startingPlayer;

            // Exhaustively generate tictactoe games w/ win, loss and mixmax stats
            currentBoard = new TicTacToeBoard();
            moveGraph = GenerateMoves(currentBoard, currentPlayer);
        }

        public void UpdateGameStatus()
        {
             if (CurrentNode.IsLeaf())
            {
                if (CurrentNode.MiniMaxScore == 1)
                {
                    Status = GameStatus.LOSS;
                }
                else if (CurrentNode.MiniMaxScore == -1)
                {
                    Status = GameStatus.WIN;
                }
                else 
                {
                    Status = GameStatus.DRAW;
                }
            }
            else
            {
                Status = GameStatus.ONGOING;
            }
        }

        public void DoPlayerMove((int row, int column) move)
        {
            currentBoard = currentBoard.ChangeTile(move, TileContent.PlayerTwo);
            currentPlayer = TileContent.PlayerOne;
            UpdateGameStatus();
        }

        public void DoComputerMove(BotAlgorithm algorithm)
        {
            // choose randomly among the moves that offer the most wins
            var currentNode = moveGraph[GetBoardStateKey(currentBoard.BoardRepr, currentPlayer)];
            TicTacToeTreeNode moveSelection;
            var currentNodeChildren = new List<TicTacToeTreeNode>(currentNode.Children);

            switch (algorithm)
            {
                
                case BotAlgorithm.RANDOM:
                    var random = new Random();
                    moveSelection = currentNode.Children[random.Next(currentNode.Children.Count)];
                    break;
                case BotAlgorithm.MINIMAX:
                    static int compareNodesByMiniMax(TicTacToeTreeNode x, TicTacToeTreeNode y)
                    {
                        return x.MiniMaxScore.CompareTo(y.MiniMaxScore);
                    }
                    currentNodeChildren.Sort(compareNodesByMiniMax);
                    currentNodeChildren.Reverse();
                    moveSelection = currentNodeChildren[0];
                    break;
                case BotAlgorithm.MOST_WINS:
                    static int compareNodesByWins(TicTacToeTreeNode x, TicTacToeTreeNode y)
                    {
                        return x.SubtreeStats.winning.CompareTo(y.SubtreeStats.winning);
                    }
                    currentNodeChildren.Sort(compareNodesByWins);
                    currentNodeChildren.Reverse();
                    moveSelection = currentNodeChildren[0];
                    break;
                case BotAlgorithm.LEAST_LOSSES:
                    static int compareNodesByLosses(TicTacToeTreeNode x, TicTacToeTreeNode y)
                    {
                        return x.SubtreeStats.losing.CompareTo(y.SubtreeStats.losing);
                    }
                    currentNodeChildren.Sort(compareNodesByLosses);
                    moveSelection = currentNodeChildren[0];
                    break;
                case BotAlgorithm.MOST_DRAWS:
                    static int compareNodesByDraws(TicTacToeTreeNode x, TicTacToeTreeNode y)
                    {
                        return x.SubtreeStats.draw.CompareTo(y.SubtreeStats.draw);
                    }
                    currentNodeChildren.Sort(compareNodesByDraws);
                    currentNodeChildren.Reverse();
                    moveSelection = currentNodeChildren[0];
                    break;
                default:
                    throw new InvalidOperationException("Unimplemented move selection algorithm.");
            }

            currentBoard = moveSelection.Board;
            currentPlayer = TileContent.PlayerTwo;
            UpdateGameStatus();
        }

        /// <summary>
        ///     Memoized exhaustive search of tictactoe solutions. The computer is assumed to be player one.
        /// </summary>
        /// <param name="startingBoard"></param>
        /// <param name="startingPlayer"></param>
        /// <returns></returns>
        private static Dictionary<string, TicTacToeTreeNode> GenerateMoves(TicTacToeBoard startingBoard, TileContent startingPlayer)
        {
            var memoizedNodes = new Dictionary<string, TicTacToeTreeNode>();
            var moveStack = new Stack<TicTacToeTreeNode>();
            moveStack.Push(new TicTacToeTreeNode(startingBoard, startingPlayer));

            while (moveStack.Count > 0)
            {
                var currentNode = moveStack.Peek();

                // if the current node's board is a leaf (contains a win or a draw), record it
                if (currentNode.Board.CheckWin(TileContent.PlayerOne))
                {
                    currentNode.SubtreeStats = (winning: 1, losing: 0, draw: 0);
                    currentNode.MiniMaxScore = 1;
                    currentNode.Explored = true;
                }
                else if (currentNode.Board.CheckWin(TileContent.PlayerTwo))
                {
                    currentNode.SubtreeStats = (winning: 0, losing: 1, draw: 0);
                    currentNode.MiniMaxScore = -1;
                    currentNode.Explored = true;
                }
                else if (currentNode.Board.GetEmptyTiles().Count == 0)
                {
                    currentNode.SubtreeStats = (winning: 0, losing: 0, draw: 1);
                    currentNode.MiniMaxScore = 0;
                    currentNode.Explored = true;
                }

                if (currentNode.Explored)
                {
                    // tally move evaluation info from subtrees
                    if (!currentNode.IsLeaf())
                    {
                        // wins/losses/draws
                        (int winning, int losing, int draw) winStats = (0, 0, 0);
                        foreach (var node in currentNode.Children)
                        {
                            winStats.winning += node.SubtreeStats.winning;
                            winStats.losing += node.SubtreeStats.losing;
                            winStats.draw += node.SubtreeStats.draw;
                        }
                        currentNode.SubtreeStats = winStats;

                        // compute minimax score
                        var miniMaxEval = currentNode.CurrentTurn == TileContent.PlayerOne ? int.MinValue : int.MaxValue;
                        foreach (var childNode in currentNode.Children)
                        {
                            if (currentNode.CurrentTurn == TileContent.PlayerOne)
                            {
                                miniMaxEval = Math.Max(miniMaxEval, childNode.MiniMaxScore);
                            }
                            else
                            {
                                miniMaxEval = Math.Min(miniMaxEval, childNode.MiniMaxScore);
                            }
                        }
                        currentNode.MiniMaxScore = miniMaxEval;
                    }

                    //Console.WriteLine($"Movestack size: {moveStack.Count}, memoized boards: {memoizedNodes.Count}");
                    //Console.WriteLine($"Subtree Stats: {currentNode.SubtreeStats}");
                    //Console.WriteLine(currentNode.Board.ToAscii('X', 'O'));

                    // memoize this node so we don't explore it's subtrees again
                    memoizedNodes.Add(GetBoardStateKey(currentNode.Board.BoardRepr, currentNode.CurrentTurn), currentNode);

                    // pop this node off the stack, since there is nothing else to do with it (it is fully explored)
                    moveStack.Pop();
                }
                else
                {
                    foreach (var emptyPosition in currentNode.Board.GetEmptyTiles())
                    {
                        var nextBoard = currentNode.Board.ChangeTile(emptyPosition, currentNode.CurrentTurn);
                        var nextBoardKey = GetBoardStateKey(nextBoard.BoardRepr, currentNode.GetNextTurnPlayer());

                        // use a memoized TicTacToeTreeNode if one exists
                        if (memoizedNodes.ContainsKey(nextBoardKey))
                        {
                            currentNode.Children.Add(memoizedNodes[nextBoardKey]);
                        }
                        else
                        {
                            var nextNode = new TicTacToeTreeNode(nextBoard, currentNode.GetNextTurnPlayer());
                            currentNode.Children.Add(nextNode);
                            moveStack.Push(nextNode);
                        }
                    }
                    currentNode.Explored = true;
                }
            }

            return memoizedNodes;
        }

        static string GetBoardStateKey(TileContent[,] boardRepr, TileContent currentTurn)
        {
            var builder = new StringBuilder();
            foreach (var tile in boardRepr)
            {
                builder.Append(tile.GetHashCode());
            }
            builder.Append(currentTurn.GetHashCode());
            return builder.ToString();
        }
    }
}
