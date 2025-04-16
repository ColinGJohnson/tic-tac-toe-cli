using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public enum TileContent : byte
    {
        Empty = 0,
        PlayerOne = 1,
        PlayerTwo = 2
    }

    public class TicTacToeBoard
    {
        public readonly TileContent[,] BoardRepr;
        private const int BoardSize = 3;


        public TicTacToeBoard()
        {
            BoardRepr = new TileContent[BoardSize, BoardSize];
        }

        public TicTacToeBoard(TileContent[,] board)
        {
            this.BoardRepr = (TileContent[,])board.Clone();
        }

        public TicTacToeBoard ChangeTile((int row, int column) position, TileContent content)
        {
            var modifiedBoard = (TileContent[,])BoardRepr.Clone();
            modifiedBoard[position.row, position.column] = content;
            return new TicTacToeBoard(modifiedBoard);
        }

        public string ToAscii(char playerOneChar, char playerTwoChar)
        {
            char getChar((int row, int column) position)
            {
                return BoardRepr[position.row, position.column] switch
                {
                    TileContent.PlayerOne => playerOneChar,
                    TileContent.PlayerTwo => playerTwoChar,
                    _ => ' ',
                };
            }

            var builder = new StringBuilder()
                .AppendLine($"┌───┬───┬───┐")
                .AppendLine($"│ {getChar((0, 0))} │ {getChar((0, 1))} │ {getChar((0, 2))} │")
                .AppendLine($"├───┼───┼───┤")
                .AppendLine($"│ {getChar((1, 0))} │ {getChar((1, 1))} │ {getChar((1, 2))} │")
                .AppendLine($"├───┼───┼───┤")
                .AppendLine($"│ {getChar((2, 0))} │ {getChar((2, 1))} │ {getChar((2, 2))} │")
                .AppendLine($"└───┴───┴───┘");
            return builder.ToString();
        }

        public List<(int row, int column)> GetEmptyTiles()
        {
            var emptyTiles = new List<(int row, int column)>();

            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    if (BoardRepr[i, j] == TileContent.Empty)
                    {
                        emptyTiles.Add((i, j));
                    }
                }
            }

            return emptyTiles;
        }

        /// <summary>
        ///     Checks if a given player has won on this board.
        /// </summary>
        /// <param name="player">The player to check a win for.</param>
        /// <returns>True if the player has won on this board.</returns>
        public bool CheckWin(TileContent player)
        {
            if (player == TileContent.Empty)
            {
                throw new ArgumentException(string.Format("{0} is not a valid player", player));
            }

            // horizontal or vertical wins
            for (var i = 0; i < BoardSize; i++)
            {
                var horizontalWin = true;
                var verticalWin = true;

                for (var j = 0; j < BoardSize; j++)
                {
                    if (BoardRepr[i, j] != player)
                    {
                        horizontalWin = false;
                    }
                    if (BoardRepr[j, i] != player)
                    {
                        verticalWin = false;
                    }
                }

                if (horizontalWin || verticalWin)
                {
                    return true;
                }
            }

            // diagonal
            var rightDiagonalWin = true;
            var leftDiagonalWin = true;
            for (var i = 0; i < BoardSize; i++)
            {
                if (BoardRepr[i, i] != player)
                {
                    rightDiagonalWin = false;
                }
                if (BoardRepr[i, BoardSize - (i + 1)] != player)
                {
                    leftDiagonalWin = false;
                }
            }
            return rightDiagonalWin || leftDiagonalWin;
        }
    }
}
