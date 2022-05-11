using System;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class Board
    {
        private readonly int m_BoardSize;
        private readonly Tile[,] m_BoardMatrix;


        public Board(int i_BoardSize)
        {
            this.m_BoardSize = i_BoardSize;
            this.m_BoardMatrix = new Tile[BoardSize, BoardSize];
        }

        public int BoardSize
        {
            get
            {
                return m_BoardSize;
            }
        }

        //validateMove(move = Console.ReadLine(), currentPlayer)
        public bool ValidateInput(string i_MoveToMake, Player i_CurrentPlayer, Tile i_LastMoveTile)
        {
            //[Af>Bf] => [A,f,B,f] A<= X <=F 
            bool isValid = false;
            if (i_MoveToMake.Length == 5 && i_MoveToMake[2] == '>')
            {
                const char rowsIndicator = 'a';
                const char columnIndicator = 'A';
                StringBuilder moveToMake = new StringBuilder(i_MoveToMake);
                moveToMake.Remove(2, 3);
                //Convert characters to index
                int originColumn = i_MoveToMake[0] - columnIndicator;
                int originRow = i_MoveToMake[1] - rowsIndicator;
                int destinationColumn = i_MoveToMake[3] - columnIndicator;
                int destinationRow = i_MoveToMake[4] - rowsIndicator;
                if (isInBound(originRow, originColumn) &&
                    isInBound(destinationRow, destinationColumn))
                {
                    int orientation = i_CurrentPlayer.Orientaion;
                    /*
                     * First move and orientation is good
                     * Second move.
                     * Queen move. (!SimplePuck)
                     */
                    //Get current puck
                    Tile currentPlace = BoardMatrix[originRow, originColumn];
                    //if player was identified in the specif tile
                    bool isASimplePuck = i_CurrentPlayer.SimplePucks.Contains(currentPlace);
                    bool isOrientationOkay = orientation == Math.Sign(destinationRow - originRow);
                    //if i_LastMoveTile is null meaning its the first eat, otherwise check if input is the same as last turn
                    //Check whether the tile belongs to the player
                    if (doesPuckBelongToPlayer(i_CurrentPlayer, currentPlace))
                    {
                        if ((i_LastMoveTile == null && isOrientationOkay) || i_LastMoveTile == BoardMatrix[originRow, originColumn] || !isASimplePuck)
                        {
                            int distanceOfMovment = destinationRow - originRow;
                            //If (not) a simple puck making step bigger than 2
                            if (!(isASimplePuck && Math.Abs(distanceOfMovment) > 2) && distanceOfMovment != 0)
                            {
                                //Checks if movement is valid
                                isValid = movementValidation(originRow, destinationRow, originColumn,
                                    destinationColumn, i_CurrentPlayer);
                                if (isValid)
                                {
                                    //if it is valid check for alternative ways (to eat)
                                    //Walk:
                                    if (Math.Abs(distanceOfMovment) == 1)
                                    {
                                        isValid = !IsThereEatingAlternative(i_CurrentPlayer);
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return isValid;

        }

        public bool IsThereWalkingAlternative(Player i_CurrentPlayer)
        {
            Random r = new Random();
            bool doesAlternativeExists = false;
            int orientation = i_CurrentPlayer.Orientaion;
            foreach (Tile puck in i_CurrentPlayer.SimplePucks.OrderBy(queen => r.Next()))
            {
                int destinationRow = puck.RowNumber + orientation;
                int destinationColumnPlus = puck.ColumnNumber + 1;
                int destinationColumnMinus = puck.ColumnNumber - 1;

                bool checkValidOfPlusMovment = isInBound(destinationRow, destinationColumnPlus) &&
                                                     movementValidation(puck.RowNumber, destinationRow,
                                                         puck.ColumnNumber,
                                                         destinationColumnPlus, i_CurrentPlayer);
                bool checkValidOfMinusMovment = isInBound(destinationRow, destinationColumnMinus) &&
                                                      movementValidation(puck.RowNumber, puck.RowNumber + orientation,
                                                          puck.ColumnNumber,
                                                          destinationColumnMinus, i_CurrentPlayer);

                doesAlternativeExists = checkValidOfMinusMovment || checkValidOfPlusMovment;
                if (doesAlternativeExists)
                {
                    if (checkValidOfPlusMovment)
                    {
                        MovementIndexString = indexToCharArray(puck.ColumnNumber, puck.RowNumber, destinationColumnPlus, destinationRow);
                    }
                    else
                    {
                        MovementIndexString = indexToCharArray(puck.ColumnNumber, puck.RowNumber, destinationColumnMinus, destinationRow);
                    }
                    break;
                }
            }

            if (!doesAlternativeExists)
            {
                foreach (Tile queen in i_CurrentPlayer.QueenPucks.OrderBy(queen => r.Next()))
                {
                    //Directions for a queen to go on a diagonal
                    int[,] directions =
                    {
                        {-1, -1}, //up and left
                        {-1, 1}, //up and right
                        {1, 1}, //down and right
                        {1, -1} // down and left
                    };
                    for (int i = 0; i < directions.GetLength(0); i++)
                    {
                        int destinationRow = queen.RowNumber + directions[i, 0];
                        int destinationCol = queen.ColumnNumber + directions[i, 1];
                        //Console.WriteLine(table[i, 0] + " " + table[i, 1]); //4
                        doesAlternativeExists = (isInBound(destinationRow, destinationCol) && movementValidation(
                            queen.RowNumber, destinationRow, queen.ColumnNumber,
                            destinationCol, i_CurrentPlayer));
                        if (doesAlternativeExists)
                        {
                            MovementIndexString = indexToCharArray(queen.ColumnNumber, queen.RowNumber,
                                destinationCol, destinationRow);
                            break;
                        }
                        else
                        {
                            MovementIndexString = "";
                        }

                    }

                    if (doesAlternativeExists)
                    {
                        break;
                    }
                }
            }

            return doesAlternativeExists;

        }

        public bool IsThereEatingAlternative(Player i_CurrentPlayer)
        {
            //iterates over every tile
            Random r = new Random();
            string outEatingIndex;
            bool doesAlternativeExists = false;
            int orientation = i_CurrentPlayer.Orientaion;
            foreach (Tile puck in i_CurrentPlayer.SimplePucks.OrderBy(queen => r.Next()))
            {
                int destinationRow = puck.RowNumber + (2 * orientation);
                int destinationColumnPlus = puck.ColumnNumber + 2;
                int destinationColumnMinus = puck.ColumnNumber - 2;
                bool checkValidOfPlusEatingMovment = isInBound(destinationRow, destinationColumnPlus) && movementValidation(puck.RowNumber, destinationRow, puck.ColumnNumber,
                    destinationColumnPlus, i_CurrentPlayer);
                bool checkValidOfMinusEatingMovment = isInBound(destinationRow, destinationColumnMinus) && movementValidation(puck.RowNumber, puck.RowNumber + (2 * orientation), puck.ColumnNumber,
                    destinationColumnMinus, i_CurrentPlayer);
                doesAlternativeExists = checkValidOfMinusEatingMovment || checkValidOfPlusEatingMovment;
                if (doesAlternativeExists)
                {
                    if (checkValidOfPlusEatingMovment)
                    {
                        MovementIndexString = indexToCharArray(puck.ColumnNumber, puck.RowNumber, destinationColumnPlus, destinationRow);
                    }
                    else
                    {
                        MovementIndexString = indexToCharArray(puck.ColumnNumber, puck.RowNumber, destinationColumnMinus, destinationRow);
                    }

                    break;
                }
                else
                {
                    MovementIndexString = "";
                }
            }

            if (!doesAlternativeExists)
            {
                foreach (Tile queen in i_CurrentPlayer.QueenPucks.OrderBy(queen => r.Next()))
                {
                    //Directions for a queen to go on a diagonal
                    int[,] directions =
                    {
                        {-1, -1}, //up and left
                        {-1, 1}, //up and right
                        {1, 1}, //down and right
                        {1, -1} // down and left
                    };
                    for (int i = 0; i < directions.GetLength(0); i++)
                    {
                        int rowNumber = queen.RowNumber + directions[i, 0];
                        int colNumber = queen.ColumnNumber + directions[i, 1];
                        //Console.WriteLine(table[i, 0] + " " + table[i, 1]); //4
                        while (isInBound(rowNumber, colNumber))
                        {
                            if (BoardMatrix[rowNumber, colNumber].Puck != ' ')
                            {
                                doesAlternativeExists = FoundEatInPath(rowNumber - directions[i, 0], colNumber - directions[i, 1], directions[i, 0], directions[i, 1], i_CurrentPlayer);
                                break;
                            }

                            //Console.WriteLine("queen while");

                            rowNumber += directions[i, 0];
                            colNumber += directions[i, 1];
                        }

                        if (doesAlternativeExists)
                        {
                            MovementIndexString = indexToCharArray(queen.ColumnNumber, queen.RowNumber,
                                colNumber, rowNumber);
                            break;
                        }
                        else
                        {
                            MovementIndexString = "";
                        }

                    }

                    if (doesAlternativeExists)
                    {
                        break;
                    }
                }

            }

            return doesAlternativeExists;
        }

        public string MovementIndexString { get; private set; }

        private string indexToCharArray(int i_OriginColumn, int i_OriginRow, int i_DestinationColumn, int i_DestinationRow)
        {
            const char rowsIndicator = 'a';
            const char columnIndicator = 'A';
            return "" + (char)(i_OriginColumn + columnIndicator) +
                (char)(i_OriginRow + rowsIndicator)
                + ">" + (char)(i_DestinationColumn + columnIndicator) +
                (char)(i_DestinationRow + rowsIndicator);
        }

        public bool FoundEatInPath(int i_OriginRow, int i_OriginColumn, int i_RowOrientation, int i_ColOrientation, Player i_Player)
        {
            bool eatFound = false;
            int destinationRow = i_OriginRow;
            int destinationCol = i_OriginColumn;
            do
            {
                //i_OriginRow += iRowOrientaion
                destinationRow += i_RowOrientation; //5
                destinationCol += i_ColOrientation; //2
                Tile oneStepPuck = isInBound(destinationRow, destinationCol) ? BoardMatrix[destinationRow, destinationCol] : null;
                if (oneStepPuck != null && oneStepPuck.Puck == ' ')
                {
                    int targetRowBound = destinationRow - i_RowOrientation;
                    int targetColBound = destinationCol - i_ColOrientation;
                    Tile nextPuckToEat = isInBound(targetRowBound, targetColBound) ? BoardMatrix[targetRowBound, targetColBound] : null;
                    if (nextPuckToEat != null && nextPuckToEat.Puck != ' ' && !doesPuckBelongToPlayer(i_Player, nextPuckToEat))
                    {
                        eatFound = true;
                    }
                    break;
                }

                //Console.WriteLine("DO WHILE LINE 190");

            } while (isInBound(destinationRow, destinationCol)//TODO MAYBE CHANGE ORIGIN TO DESTINATION IN CONDITION WHILE
                     && !doesPuckBelongToPlayer(i_Player, BoardMatrix[destinationRow, destinationCol]));

            return eatFound;

        }

        private bool isInBound(int i_RowNumber, int i_ColNumber)
        {
            return !(i_RowNumber < 0 || i_ColNumber < 0 || i_RowNumber >= BoardSize || i_ColNumber >= BoardSize);
        }

        private bool movementValidation(int i_OriginRow, int i_DestinationRow,
            int i_OriginColumn, int i_DestinationColumn, Player i_CurrentPlayer)
        {
            bool isValid = false;
            int directionRow = i_DestinationRow - i_OriginRow;
            int orientationRow = Math.Sign(directionRow);
            directionRow = Math.Abs(directionRow); //5-2 => 3()
            int directionCol = i_DestinationColumn - i_OriginColumn;
            int orientationCol = Math.Sign(directionCol);
            directionCol = Math.Abs(directionCol);
            if (directionRow == directionCol)
            {
                isValid = true;
                for (int i = 1; i <= directionRow; i++)
                {
                    //i stops 1 step before destination to check if theres an enemy
                    if (i == directionRow - 1)
                    {
                        //TODO MIGHT CAUSE OUT OF BOUNDS
                        Tile target = BoardMatrix[i_DestinationRow - orientationRow,
                            i_DestinationColumn - orientationCol];

                        if (doesPuckBelongToPlayer(i_CurrentPlayer, target) || target.Puck == ' ')
                        {
                            isValid = false;
                            break;
                        }
                    }
                    else if (BoardMatrix[i_OriginRow + i * orientationRow,
                                 i_OriginColumn + i * orientationCol].Puck != ' ')
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }


        private static bool doesPuckBelongToPlayer(Player i_Player, Tile location)
        {
            return i_Player.SimplePucks.Contains(location) || i_Player.QueenPucks.Contains(location);
        }
        public Tile[,] BoardMatrix
        {
            get
            {
                return m_BoardMatrix;
            }
        }

        public void InitializeBoard(Player i_Player1, Player i_Player2)
        {

            //formula for getting the amount of pucks per side
            int initialPucks = (BoardSize / 2 - 1) * (BoardSize / 2);
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    //Conditions for initing top board ('O')
                    if ((i < ((BoardSize / 2) - 1)) && (i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0))
                    {
                        BoardMatrix[i, j] = new Tile(i, j, 'O');
                        i_Player2.SimplePucks.Add(BoardMatrix[i, j]);
                        //Console.Write('O');
                    }
                    //Conditions for initing bottom board ('X')
                    else if (i > (BoardSize / 2) && (i % 2 == 1 && j % 2 == 0 || i % 2 == 0 && j % 2 == 1))
                    {
                        BoardMatrix[i, j] = new Tile(i, j, 'X');
                        i_Player1.SimplePucks.Add(BoardMatrix[i, j]);
                        //Console.Write('X');
                    }
                    else
                    {
                        BoardMatrix[i, j] = new Tile(i, j, ' ');
                        //Console.Write('_');
                    }

                }
                //Console.WriteLine();
            }
        }

        // Print the board for player with troops X or for player with troops O.
        public void PrintBoard()
        {
            StringBuilder lineOfEquals = new StringBuilder();
            lineOfEquals.Append('=', BoardSize * 4);
            Console.Write("   ");
            string capitalLetters = "";
            for (int k = 0; k < BoardSize; k++)
            {
                capitalLetters += (char)('A' + k) + "   ";
            }

            Console.WriteLine(capitalLetters);
            Console.WriteLine(" " + lineOfEquals);
            for (int i = 0; i < BoardSize; i++)
            {
                Console.Write((char)('a' + i));
                for (int j = 0; j < BoardSize; j++)
                {
                    Console.Write("| " + BoardMatrix[i, j].Puck + " ");
                    if (j == BoardSize - 1)
                    {
                        Console.Write('|');
                    }
                }

                Console.WriteLine("\n " + lineOfEquals);
            }
        }
    }
}
