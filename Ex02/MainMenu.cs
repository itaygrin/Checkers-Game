using System;

namespace Ex02
{
    public class MainMenu
    {
        public static void Main(string[] args)
        {
            mainMenu();
        }

        private static void mainMenu()
        {
            Console.WriteLine("Enter your name: (max 10 chars)");
            string userName = promptUserName();
            Player player1 = new Player(userName, 'X');
            Console.WriteLine("Select board size: (6/8/10)");
            int boardSize = promptBoardSize();
            Board board = new Board(boardSize);
            Console.WriteLine("Play against pc or player? (pc/player)");
            Player player2 = promptTwoPlayerOrPc();
            bool anotherMatch = true;
            //Clean players' lists and start a new round
            while (anotherMatch)
            {
                player1.QueenPucks.Clear();
                player1.SimplePucks.Clear();
                player2.QueenPucks.Clear();
                player2.SimplePucks.Clear();
                new Game(player1, player2, board).BeginGame();
                Console.WriteLine($"{player1.Username} has {player1.Score} points\n" +
                                  $"{player2.Username} has {player2.Score} points");
                anotherMatch = promptAnotherMatch();
            }
        }

        private static bool promptAnotherMatch()
        {
            bool isValidInput = false;
            bool anotherMatch = false;
            Console.WriteLine("Rematch? (y/n)");
            string choice = "";
            while (!isValidInput)
            {
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "y":
                        isValidInput = true;
                        anotherMatch = true;
                        break;
                    case "n":
                        isValidInput = true;
                        anotherMatch = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input, try again.");
                        break;
                }
            }

            return anotherMatch;


        }

        private static Player promptTwoPlayerOrPc()
        {
            Player player2 = null;
            string pcOrPlayer = Console.ReadLine().ToLower();
            bool isValidInput = false;
            while (!isValidInput)
            {
                switch (pcOrPlayer)
                {
                    case "player":
                        Console.WriteLine("Enter second players name:");
                        player2 = new Player(promptUserName(), 'O');
                        isValidInput = true;
                        break;
                    case "pc":
                        player2 = new Player("PC", 'O', false);
                        isValidInput = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input, try again.");
                        break;
                }
            }

            return player2;
        }

        private static int promptBoardSize()
        {
            int boardSize;
            while (!int.TryParse(Console.ReadLine(), out boardSize) ||
                   (boardSize != 6 && boardSize != 8 && boardSize != 10))
            {
                Console.WriteLine("Invalid input, try again");
            }

            return boardSize;
        }

        private static string promptUserName()
        {
            string userName;
            while ((userName = Console.ReadLine()) == null || userName.Length > 10 || userName.Contains(" "))
            {
                Console.WriteLine("Invalid input, try again");
            }

            return userName;
        }
    }
}
