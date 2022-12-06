using System;

namespace CheckersGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the checkers game!");
            Console.WriteLine("The board is numbered 1-8 on the x-axis and A-H on the y-axis.");
            Console.WriteLine("You are the black pieces (B) and the AI is the white pieces (W).");
            Console.WriteLine("To make a move, enter the coordinates of the piece you want to move and the coordinates of the destination square (e.g. A1B2).");
            Console.WriteLine("You can only move diagonally and you must capture the opponent's pieces by jumping over them if possible.");
            Console.WriteLine("Let's begin!\n");

            // initialize the game board
            string[,] board = new string[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        if (i < 3)
                        {
                            board[i, j] = "B";
                        }
                        else if (i > 4)
                        {
                            board[i, j] = "W";
                        }
                        else
                        {
                            board[i, j] = ".";
                        }
                    }
                    else
                    {
                        board[i, j] = " ";
                    }
                }
            }

            // initialize the player and AI colors
            string playerColor = "B";
            string aiColor = "W";

            // keep playing until the game is over
            while (true)
            {
                // display the game board
                Console.WriteLine("  1 2 3 4 5 6 7 8");
                Console.WriteLine(" +-+-+-+-+-+-+-+-+");
                for (int i = 0; i < 8; i++)
                {
                    Console.Write((char)('A' + i) + "|");
                    for (int j = 0; j < 8; j++)
                    {
                        Console.Write(board[i, j] + "|");
                    }
                    Console.WriteLine();
                    Console.WriteLine(" +-+-+-+-+-+-+-+-+");
                }

                // check if the game is over
                if (IsGameOver(board, playerColor))
                {
                    Console.WriteLine("You won!");
                    break;
                }
                if (IsGameOver(board, aiColor))
                {
                    Console.WriteLine("You lost!");
                    break;
                }

                // prompt the player for a move
                Console.Write("Enter a move (e.g. A1B2): ");
                string input = Console.ReadLine();
                int fromX = input[0] - 'A';
                int fromY = input[1] - '1';
                int toX = input[2] - 'A';
                int toY = input[3] - '1';

                // check if the move is valid
                if (!IsValidMove(board, playerColor, fromX, fromY, toX, toY))
                {
                    Console.WriteLine("Invalid move. Try again.");
                    continue;
                }
                                           
                Console.Clear();

                // make the move on the board
                board[toX, toY] = board[fromX, fromY];
                board[fromX, fromY] = ".";

                // check if the player can make another move by jumping over an opponent's piece
                if (CanJump(board, playerColor, toX, toY))
                {
                    Console.WriteLine("You can make another move by jumping over an opponent's piece.");
                    continue;
                }
                // let the AI make a move
                Tuple<int, int, int, int> aiMove = GetAIMove(board, aiColor);
                fromX = aiMove.Item1;
                fromY = aiMove.Item2;
                toX = aiMove.Item3;
                toY = aiMove.Item4;
                Console.WriteLine($"AI moves {(char)('A' + fromX)}{fromY + 1} to {(char)('A' + toX)}{toY + 1}");

                // make the AI move on the board
                board[toX, toY] = board[fromX, fromY];
                board[fromX, fromY] = ".";

                // check if the AI can make another move by jumping over an opponent's piece
                if (CanJump(board, aiColor, toX, toY))
                {
                    Console.WriteLine("AI can make another move by jumping over your piece.");
                }
            }
        }

        // helper method that checks if the game is over for the given player
        static bool IsGameOver(string[,] board, string playerColor)
        {
            // check if the player has any remaining pieces
            bool hasPieces = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == playerColor)
                    {
                        hasPieces = true;
                        break;
                    }
                }
            }
            if (!hasPieces)
            {
                return true;
            }

            // check if the player has any valid moves
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == playerColor)
                    {
                        if (IsValidMove(board, playerColor, i, j, i + 1, j + 1) ||
                            IsValidMove(board, playerColor, i, j, i - 1, j + 1) ||
                            IsValidMove(board, playerColor, i, j, i + 1, j - 1) ||
                            IsValidMove(board, playerColor, i, j, i - 1, j - 1))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        // helper method that checks if the given move is valid for the given player
        static bool IsValidMove(string[,] board, string playerColor, int fromX, int fromY, int toX, int toY)
        {
            // check if the move is within the bounds of the board
            if (fromX < 0 || fromX > 7 || fromY < 0 || fromY > 7 ||
                toX < 0 || toX > 7 || toY < 0 || toY > 7)
            {
                return false;
            }

            // check if the player's piece is at the given "from" position
            if (board[fromX, fromY] != playerColor && board[fromX, fromY] != playerColor.ToLower())
            {
                return false;
            }

            // check if the "to" position is empty
            if (board[toX, toY] != ".")
            {
                return false;
            }

            // check if the move is a diagonal move of size 1
            if (Math.Abs(fromX - toX) == 1 && Math.Abs(fromY - toY) == 1)
            {
                return true;
            }

            // check if the move is a diagonal move of size 2 (a jump)
            if (Math.Abs(fromX - toX) == 2 && Math.Abs(fromY - toY) == 2)
            {
                // find the x and y position of the piece that was jumped over
                int jumpedX = (fromX + toX) / 2;
                int jumpedY = (fromY + toY) / 2;

                // check if there is an opponent's piece at the jumped position
                if (board[jumpedX, jumpedY] != playerColor && board[jumpedX, jumpedY] != ".")
                {
                    return true;
                }
            }

            // the move is invalid
            return false;
        }
        // helper method that checks if the given player can make another move by jumping over an opponent's piece
        static bool CanJump(string[,] board, string playerColor, int fromX, int fromY)
        {
            // check if the player's piece at the given coordinates is a king
            bool isKing = playerColor == "B" && fromX == 0 || playerColor == "W" && fromX == 7;

            // check if the player can jump to the top-left
            if (fromX > 1 && fromY > 1 && board[fromX - 1, fromY - 1] != playerColor && board[fromX - 2, fromY - 2] == ".")
            {
                return true;
            }
            // check if the player can jump to the top-right
            if (fromX > 1 && fromY < 6 && board[fromX - 1, fromY + 1] != playerColor && board[fromX - 2, fromY + 2] == ".")
            {
                return true;
            }
            // check if the player can jump to the bottom-left
            if (fromX < 6 && fromY > 1 && board[fromX + 1, fromY - 1] != playerColor && board[fromX + 2, fromY - 2] == ".")
            {
                return true;
            }
            // check if the player can jump to the bottom-right
            if (fromX < 6 && fromY < 6 && board[fromX + 1, fromY + 1] != playerColor && board[fromX + 2, fromY + 2] == ".")
            {
                return true;
            }
            // if the player is a king, also check the diagonal moves in the opposite direction
            if (isKing)
            {
                // check if the player can jump to the bottom-left
                if (fromX > 1 && fromY > 1 && board[fromX - 1, fromY - 1] != playerColor && board[fromX - 2, fromY - 2] == ".")
                {
                    return true;
                }
                // check if the player can jump to the bottom-right
                if (fromX > 1 && fromY < 6 && board[fromX - 1, fromY + 1] != playerColor && board[fromX - 2, fromY + 2] == ".")
                {
                    return true;
                }
                // check if the player can jump to the top-left
                if (fromX < 6 && fromY > 1 && board[fromX + 1, fromY - 1] != playerColor && board[fromX + 2, fromY - 2] == ".")
                {
                    return true;
                }
                // check if the player can jump to the top-right
                if (fromX < 6 && fromY < 6 && board[fromX + 1, fromY + 1] != playerColor && board[fromX + 2, fromY + 2] == ".")
                {
                    return true;
                }
            }
            // if the player cannot make another move by jumping over an opponent's piece, return false
            return false;
        }
        static Tuple<int, int, int, int> GetAIMove(string[,] board, string aiColor)
        {
            // find all the valid moves for the AI
            List<Tuple<int, int, int, int>> moves = new List<Tuple<int, int, int, int>>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == aiColor || board[i, j] == aiColor.ToLower())
                    {
                        // check all the possible destinations for the piece at (i, j)
                        for (int k = 0; k < 8; k++)
                        {
                            for (int l = 0; l < 8; l++)
                            {
                                if (IsValidMove(board, aiColor, i, j, k, l))
                                {
                                    moves.Add(new Tuple<int, int, int, int>(i, j, k, l));
                                }
                            }
                        }
                    }
                }
            }

            // prioritize moves that involve jumping over opponent's pieces
            foreach (Tuple<int, int, int, int> move in moves)
            {
                if (CanJump(board, aiColor, move.Item3, move.Item4))
                {
                    return move;
                }
            }

            // if there are no jumping moves, choose a random valid move
            Random rnd = new Random();
            return moves[rnd.Next(moves.Count)];
        }
    }
}