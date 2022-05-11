using System;

namespace Ex02
{
    public class Game
    {
        private readonly Player m_Player1;
        private readonly Player m_Player2;
        private readonly Board m_Board;
        private Player m_WinnerPlayer;

        public Game(Player i_Player1, Player i_Player2, Board i_Board)
        {
            this.m_Player1 = i_Player1;
            this.m_Player2 = i_Player2;
            this.m_Board = i_Board;
        }
        public void BeginGame()
        {
            Board.InitializeBoard(Player1, Player2);
            bool playerTurn = true;  //true is for player1, false is for player2
            Player currentPlayer;
            Player oppPlayer;
            string move = "";
            bool isAnotherEatPossible = false;
            //moveTile represents second eat (if not null)
            Tile moveTile = null;
            while (isGameActive(currentPlayer = playerTurn ? Player1 : Player2,
                       oppPlayer = playerTurn ? Player2 : Player1))
            {
                //Clean the board using DLL
                Ex02.ConsoleUtils.Screen.Clear();
                Board.PrintBoard();
                //check whether it is the first move of the round
                if (move != "")
                {
                    //move will be "" if computer wont have legal moves to make
                    string userNameOfPrevious = oppPlayer.Username;
                    char signOfPrevious = oppPlayer.Sign;
                    if (moveTile != null)
                    {
                        userNameOfPrevious = currentPlayer.Username;
                        signOfPrevious = currentPlayer.Sign;
                    }

                    Console.WriteLine($"{userNameOfPrevious}'s move was ({signOfPrevious}): {move}");
                }

                if (currentPlayer.isHuman)
                {
                    Console.Write($"{currentPlayer.Username}'s Turn ({currentPlayer.Sign}): ");
                    //Validate user's input until getting a valid input
                    while (!Board.ValidateInput(move = Console.ReadLine(), currentPlayer, moveTile) && move != "Q")
                    {
                        Console.WriteLine("Invalid move, try again");
                    }

                    if (move == "Q")
                    {
                        WinnerPlayer = oppPlayer;
                        break;
                    }
                }
                //if not a human, play a computer move
                else
                {
                    move = playComputerTurn(currentPlayer);
                    //if computer doesnt have legal moves, the user wins
                    if (move == "")
                    {
                        WinnerPlayer = oppPlayer;
                        break;
                    }
                }

                if (isAnotherEatPossible = Turn(move, currentPlayer, oppPlayer, ref moveTile))
                {
                    continue;
                }

                //Switch between turns
                playerTurn = !playerTurn;
                moveTile = null;
            }

            EndRound();

        }
        private string playComputerTurn(Player i_Computer)
        {
            //checks is there's an eat move, and if not check if theres a walk move
            if (!Board.IsThereEatingAlternative(i_Computer))
            {
                Board.IsThereWalkingAlternative(i_Computer);
            }

            return Board.MovementIndexString;
        }

        public void EndRound()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            Board.PrintBoard();
            int firstPlayerPucks = Player1.SimplePucks.Count + 4 * Player1.QueenPucks.Count;
            int secondPlayerPucks = Player2.SimplePucks.Count + 4 * Player2.QueenPucks.Count;
            int score = Math.Abs(firstPlayerPucks - secondPlayerPucks);
            if (WinnerPlayer != null)
            {
                Console.WriteLine($"\n{WinnerPlayer.Username} ({WinnerPlayer.Sign}) has won!");
                WinnerPlayer.Score += score;
            }
            else
            {
                Console.WriteLine("\nIt's a Tie!");
            }
        }


        public Player WinnerPlayer
        {
            get
            {
                return m_WinnerPlayer;
            }
            set
            {
                m_WinnerPlayer = value;
            }

        }

        private bool isGameActive(Player i_CurrentPlayer, Player i_OpponnetPlayer)
        {
            bool activeStatus = true;
            if (!hasLegalMoves(i_CurrentPlayer))
            {
                activeStatus = false;
                if (hasLegalMoves(i_OpponnetPlayer))
                {
                    WinnerPlayer = i_OpponnetPlayer;
                }
            }

            return activeStatus;
        }

        private bool hasLegalMoves(Player i_Player)
        {
            bool isThereAlternative = false;
            foreach (Tile puck in i_Player.SimplePucks)
            {
                isThereAlternative = checkAltenativeRoute(puck, i_Player);
            }

            foreach (Tile puck in i_Player.QueenPucks)
            {
                isThereAlternative = checkAltenativeRoute(puck, i_Player);
            }

            return isThereAlternative;
        }

        private bool checkAltenativeRoute(Tile puck, Player i_Player)
        {
            return Board.IsThereEatingAlternative(i_Player)
                   || Board.IsThereWalkingAlternative(i_Player);
        }

        public Player Player1
        {
            get
            {
                return m_Player1;
            }
        }

        public Player Player2
        {
            get
            {
                return m_Player2;
            }
        }

        public Board Board
        {
            get
            {
                return m_Board;
            }
        }


        public bool Turn(string i_PlayerMovment, Player i_CurrentPlayer, Player i_OpponentPlayer, ref Tile io_MoveTile)
        {
            bool anotherTurn = false;
            const char rowsIndicator = 'a';
            const char columnIndicator = 'A';
            //Convert characters to index
            int originColumn = i_PlayerMovment[0] - columnIndicator;
            int originRow = i_PlayerMovment[1] - rowsIndicator;
            int destinationColumn = i_PlayerMovment[3] - columnIndicator;
            int destinationRow = i_PlayerMovment[4] - rowsIndicator;
            io_MoveTile = Board.BoardMatrix[destinationRow, destinationColumn];
            /*
             * Change pointer from origin to pointer of destination
             *      remove from player list the origin and append destination tile
             *      change sign of new tile to match the player
             *      change the sign of the old tile to '_'
             */
            Tile originPuck = Board.BoardMatrix[originRow, originColumn];
            if (destinationRow == 0 && originPuck.Puck == 'X' || destinationRow == Board.BoardSize - 1 && originPuck.Puck == 'O')
            {
                i_CurrentPlayer.SimplePucks.Remove(originPuck);
                i_CurrentPlayer.QueenPucks.Add(io_MoveTile);
                io_MoveTile.Puck = i_CurrentPlayer.QueenSign;
            }
            else if (i_CurrentPlayer.SimplePucks.Remove(originPuck))
            {
                i_CurrentPlayer.SimplePucks.Add(io_MoveTile);
                io_MoveTile.Puck = i_CurrentPlayer.Sign;
            }
            else if (i_CurrentPlayer.QueenPucks.Remove(originPuck))
            {
                i_CurrentPlayer.QueenPucks.Add(io_MoveTile);
                io_MoveTile.Puck = i_CurrentPlayer.QueenSign;
            }

            originPuck.Puck = ' ';
            int distance = destinationRow - originRow;
            int colOrientation = Math.Sign(destinationColumn - originColumn);
            int rowOrientation = Math.Sign(distance);
            //Upon eating:
            if (Math.Abs(distance) > 1)
            {
                //get the tile that is about to be eaten and remove it from players' lists
                Tile eaten = Board.BoardMatrix[destinationRow - rowOrientation, destinationColumn - colOrientation];
                if (!i_OpponentPlayer.QueenPucks.Remove(eaten))
                {
                    i_OpponentPlayer.SimplePucks.Remove(eaten);
                }

                eaten.Puck = ' ';
                int[,] directions =
                {
                    {-1, -1}, //up and left
                    {-1, 1}, //up and right
                    {1, 1}, //down and right
                    {1, -1} // down and left
                };
                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    anotherTurn = Board.FoundEatInPath(destinationRow, destinationColumn, directions[i, 0], directions[i, 1], i_CurrentPlayer);
                    if (anotherTurn)
                    {
                        break;
                    }
                }

            }

            return anotherTurn;
        }

    }
}
