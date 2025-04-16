using System;
using System.Collections.Generic;
using System.Threading;

namespace TicTacToe
{
    class Program
    {
        private static void Main()
        {
            // player chooses which bot they want to play against
            string[] algorithmOptions = {
                "Random",
                "Minimax",
                "Most win paths",
                "Least loss paths",
                "Most draw paths"
            };
            var botAlgorithm = GetPlayerInput("How should the computer play?", algorithmOptions, true);

            // player chooses X or O's (first or second)
            char playerOneChar;
            char playerTwoChar;
            TileContent startingPlayer;
            string[] symbolOptions = { "X", "O" };
            if (GetPlayerInput("Would you like to play as X's or O's?", symbolOptions, true) == 0)
            {
                playerOneChar = 'O';
                playerTwoChar = 'X';
                startingPlayer = TileContent.PlayerTwo;
            }
            else
            {
                playerOneChar = 'X';
                playerTwoChar = 'O';
                startingPlayer = TileContent.PlayerOne;
            }

            // keep playing games until the player wants to quit
            var keepPlaying = true;
            var gameCount = 0;
            var winCount = 0;
            var drawCount = 0;
            while (keepPlaying)
            {
                var status = PlayGame((BotAlgorithm)botAlgorithm, playerOneChar, playerTwoChar, startingPlayer);
                gameCount++;
                if (status == GameStatus.WIN)
                {
                    winCount++;
                } 
                else if (status == GameStatus.DRAW)
                {
                    drawCount++;
                }

                var winStats = $"You have played {gameCount} games against the \"{Enum.GetName((BotAlgorithm)botAlgorithm)}\" bot with {winCount} wins and {drawCount} draws.";
                string[] gameEndOptions = { "Play again", "Quit" };
                keepPlaying = GetPlayerInput($"{winStats}\nDo you want to play another game?", gameEndOptions) == 0;
            }
        }

        static GameStatus PlayGame(BotAlgorithm algorithm, char playerOneChar, char playerTwoChar, TileContent startingPlayer)
        {
            var game = new TicTacToeGame(startingPlayer) {
                playerOneChar = playerOneChar,
                playerTwoChar = playerTwoChar
            };

            while (game.Status == GameStatus.ONGOING)
            {

                // computer move
                if (game.currentPlayer == TileContent.PlayerOne)
                {
                    Console.Clear();
                    Console.WriteLine("Computer Move:");
                    Console.WriteLine(game.currentBoard.ToAscii(playerOneChar, playerTwoChar));
                    Thread.Sleep(1000);

                    game.DoComputerMove(algorithm);

                    Console.Clear();
                    Console.WriteLine($"Computer Move:");
                    Console.WriteLine(game.currentBoard.ToAscii(playerOneChar, playerTwoChar));
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Your Move:");
                    Console.WriteLine(game.currentBoard.ToAscii(playerOneChar, playerTwoChar));

                    // let the player select a move
                    var emptyTiles = game.currentBoard.GetEmptyTiles();
                    var moveOptions = emptyTiles.ConvertAll(x => x.ToString());
                    var selection = GetPlayerInput("Choose a move from the following. (row, column)", moveOptions);
                    var selectedMove = emptyTiles[selection];

                    game.DoPlayerMove(selectedMove);
                }
            }

            Console.Clear();
            string gameEndMessage;
            switch (game.Status)
            {
                case GameStatus.WIN:
                    gameEndMessage = "You Win!";
                    break;
                case GameStatus.LOSS:
                    gameEndMessage = "You Lost.";
                    break;
                case GameStatus.DRAW:
                    gameEndMessage = "It's a draw.";
                    break;
                case GameStatus.ONGOING:
                    throw new InvalidOperationException("Game still ongoing.");
                default:
                    throw new InvalidOperationException("Unknown game status.");
            }
            Console.WriteLine(gameEndMessage);
            Console.WriteLine(game.currentBoard.ToAscii(playerOneChar, playerTwoChar));
            return game.Status;
        }

        static int GetPlayerInput(string message, IList<string> options, bool clear = false)
        {
            while (true)
            {
                if (clear)
                {
                    Console.Clear();
                }
                Console.WriteLine(message);
                for (var i = 0; i < options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {options[i]}");
                }
                Console.Write("Enter a number: ");
                var input = Console.ReadLine();
                var success = int.TryParse(input, out var inputNumber);
                if (success && inputNumber > 0 && inputNumber <= options.Count)
                {
                    return inputNumber - 1;
                }

                Console.WriteLine("Invalid option, select one of the following by typing a number and pressing enter.");
            }
        }

        static void PrintSplashAscii()
        {
            var ascii = System.IO.File.ReadAllText(@"");
            Console.WriteLine(ascii);
        }
    }
}
