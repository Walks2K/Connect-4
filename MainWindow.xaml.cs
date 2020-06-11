﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Connect_4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members
        /// <summary>
        /// Holds current state of cells on board
        /// </summary>
        private CellTypes[,] mCellTypes;

        /// <summary>
        /// True if it is player 1's turn
        /// </summary>
        private bool mPlayer1Turn;

        /// <summary>
        /// True if the game has ended
        /// </summary>
        private bool mGameEnded;

        /// <summary>
        /// Holds the information about which game is being played (PVP or PVE etc)
        /// </summary>
        private GameTypes mGameType = GameTypes.PlayerVersusPlayer;

        /// <summary>
        /// Random number generation
        /// </summary>
        private Random random = new Random();
        #endregion
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }

        #endregion

        /// <summary>
        /// Setup a new game
        /// </summary>
        private void NewGame()
        {
            // Create a new 2D array of free cells
            mCellTypes = new CellTypes[6, 7];

            // Ensure cells are init as free
            for (int i = 0; i < mCellTypes.GetLength(0); i++)
            {
                for (int j = 0; j < mCellTypes.GetLength(1); j++)
                {
                    mCellTypes[i, j] = CellTypes.Free;
                }
            }

            // Ensure player 1 has active turn
            mPlayer1Turn = true;

            // Iterate every button on the grid
            Container.Children.Cast<Button>().ToList().ForEach(button =>
            {
                // Change background, foreground and content to default values
                button.Content = string.Empty;
                button.Background = Brushes.White;
                button.Foreground = Brushes.Blue;
            });

            // Ensure game isn't finished
            mGameEnded = false;
        }

        /// <summary>
        /// Button click event from grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Start a new game if game has ended
            if (mGameEnded)
            {
                NewGame();
                return;
            }

            // Cast sender to button
            var button = (Button)sender;

            // Find button position in array
            //var row = Grid.GetRow(button);
            var col = Grid.GetColumn(button);

            if (!DropCoin(mCellTypes, mPlayer1Turn ? CellTypes.Red : CellTypes.Yellow, col))
                return;

            // Toggle turn
            mPlayer1Turn = !mPlayer1Turn;

            // Check for a winner
            var Winner = CheckForWinner(mCellTypes);

            if (Winner == CellTypes.Red)
            {
                mGameEnded = true;
                MessageBox.Show("Red has won!");
            }
            else if (Winner == CellTypes.Yellow)
            {
                mGameEnded = true;
                MessageBox.Show("Yellow has won!");
            }

            if (mGameType == GameTypes.PlayerVersusAI && !mGameEnded)
            {
                int bestCol = FindBestColumn(mCellTypes);

                if (bestCol >= 0)
                {
                    DropCoin(mCellTypes, CellTypes.Yellow, bestCol);

                    // Check for a winner
                    Winner = CheckForWinner(mCellTypes);

                    if (Winner == CellTypes.Red)
                    {
                        mGameEnded = true;
                        MessageBox.Show("Red has won!");
                    }
                    else if (Winner == CellTypes.Yellow)
                    {
                        mGameEnded = true;
                        MessageBox.Show("Yellow has won!");
                    }
                }

                // Toggle turn
                mPlayer1Turn = !mPlayer1Turn;
            }

        }

        private int FindBestColumn(CellTypes[,] board)
        {
            int bestVal = -10;
            List<int> bestCol = new List<int>{ };

            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (!DropCoin(board, CellTypes.Yellow, col))
                    continue;

                int moveVal = MiniMax(board, 2, true);

                RemoveTopCoin(board, col);

                if (moveVal > bestVal)
                {
                    bestCol.Clear();
                    bestCol.Add(col);
                    bestVal = moveVal;
                }
                else if (moveVal == bestVal)
                {
                    if (!bestCol.Contains(col))
                        bestCol.Add(col);
                }
            }

            return bestCol[random.Next(bestCol.Count)];
        }


        /// <summary>
        /// Drops coin into top slot on given column
        /// </summary>
        /// <param name="board"></param>
        /// <param name="player"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool DropCoin(CellTypes[,] board, CellTypes player, int col)
        {
            for (int row = mCellTypes.GetLength(0) - 1; row >= 0; row--)
            {
                if (board[row, col] != CellTypes.Free)
                    continue;

                var CellToFill = Container.Children.Cast<Button>().First(ButtonToMove => Grid.GetRow(ButtonToMove) == row && Grid.GetColumn(ButtonToMove) == col);

                // Set the cell value based on turn
                mCellTypes[row, col] = player;

                // Set the cell colour based on turn
                CellToFill.Background = player == CellTypes.Red ? Brushes.Red : Brushes.Yellow;

                return true;
            }

            return false;
        }
        /// <summary>
        /// Removes the top coin from any given column
        /// </summary>
        /// <param name="board"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool RemoveTopCoin(CellTypes[,] board, int col)
        {
            for (int row = 0; row < mCellTypes.GetLength(0); row++)
            {
                if (board[row, col] == CellTypes.Free)
                    continue;

                var CellToFill = Container.Children.Cast<Button>().First(ButtonToMove => Grid.GetRow(ButtonToMove) == row && Grid.GetColumn(ButtonToMove) == col);

                mCellTypes[row, col] = CellTypes.Free;

                // Set the cell colour based on turn
                CellToFill.Background = Brushes.White;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks a given board for a winner and returns the CellTypes if one is found
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private CellTypes CheckForWinner(CellTypes[,] board)
        {
            // Horizontal check
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1) - 3; j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i, j + 1] == player && board[i, j + 2] == player && board[i, j + 3] == player)
                        return player;
                }
            }

            // Vertical check
            for (int i = 0; i < board.GetLength(0) - 3; i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i + 1, j] == player && board[i + 2, j] == player && board[i + 3, j] == player)
                        return player;
                }
            }

            // Ascending diagonal check
            for (int i = 3; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1) - 3; j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i - 1, j + 1] == player && board[i - 2, j + 2] == player && board[i - 3, j + 3] == player)
                        return player;
                }
            }

            // Descending diagonal check
            for (int i = 3; i < board.GetLength(0); i++)
            {
                for (int j = 3; j < board.GetLength(1); j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i - 1, j - 1] == player && board[i - 2, j - 2] == player && board[i - 3, j - 3] == player)
                        return player;
                }
            }

            // Check for no winner and full board
            if (!board.Cast<CellTypes>().Any(result => result == CellTypes.Free))
            {
                // Game ended
                mGameEnded = true;

                // Turn all cells orange
                Container.Children.Cast<Button>().ToList().ForEach(button =>
                {
                    button.Background = Brushes.Orange;
                });
            }

            return CellTypes.Free;
        }

        /// <summary>
        /// Update selection on combo box update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cast sender to combo box
            var combobox = (ComboBox)sender;
            // Update game type to match selection
            mGameType = (GameTypes)combobox.SelectedIndex;
            // Start a new game
            NewGame();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NewGame();
        }



        /// <summary>
        /// Minimax algorithm used to determine best move
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <param name="isMax"></param>
        /// <returns></returns>
        private int MiniMax(CellTypes[,] board, int depth, bool isMax, int alpha = -100, int beta = 100)
        {
            int score = EvalutateBoard(board);

            if (depth == 0 || score == 100 || score == -100)
            {
                return score;
            }

            if (isMax)
            {
                int maxValue = -100;

                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (!DropCoin(board, CellTypes.Yellow, col))
                        continue;

                    maxValue = Math.Max(maxValue, MiniMax(board, depth - 1, false));

                    RemoveTopCoin(board, col);

                    alpha = Math.Max(alpha, maxValue);

                    if (alpha >= beta)
                        break;
                }

                return maxValue;
            }
            else
            {
                int minValue = 100;

                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (!DropCoin(board, CellTypes.Red, col))
                        continue;

                    minValue = Math.Min(minValue, MiniMax(board, depth - 1, true));

                    RemoveTopCoin(board, col);

                    beta = Math.Min(beta, minValue);

                    if (beta <= alpha)
                        break;
                }

                return minValue;
            }
        }

        /// <summary>
        /// Evaluate board score
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private int EvalutateBoard(CellTypes[,] board)
        {
            // Horizontal check
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1) - 3; j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i, j + 1] == player)
                    {
                        int hCount = 2;
                        if (board[i, j + 2] == player)
                        {
                            hCount++;
                            if (board[i, j + 3] == player)
                                hCount = 100;
                        }
                        return player == CellTypes.Yellow ? hCount : -hCount;
                    }

                    //if (board[i, j] == player && board[i, j + 1] == player && board[i, j + 2] == player && board[i, j + 3] == player)
                    //return player == CellTypes.Yellow ? 10 : -10;
                }
            }

            // Vertical check
            for (int i = 0; i < board.GetLength(0) - 3; i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i + 1, j] == player)
                    {
                        int vCount = 2;
                        if (board[i + 2, j] == player)
                        {
                            vCount++;
                            if (board[i + 3, j] == player)
                                vCount = 100;
                        }
                        return player == CellTypes.Yellow ? vCount : -vCount;
                    }

                    //if (board[i, j] == player && board[i + 1, j] == player && board[i + 2, j] == player && board[i + 3, j] == player)
                    //return player == CellTypes.Yellow ? 10 : -10;
                }
            }

            // Ascending diagonal check
            for (int i = 3; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1) - 3; j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i - 1, j + 1] == player)
                    {
                        int adCount = 2;
                        if (board[i - 2, j + 2] == player)
                        {
                            adCount++;
                            if (board[i - 3, j + 3] == player)
                                adCount = 100;
                        }
                        return player == CellTypes.Yellow ? adCount : -adCount;
                    }

                    //if (board[i, j] == player && board[i - 1, j + 1] == player && board[i - 2, j + 2] == player && board[i - 3, j + 3] == player)
                    //return player == CellTypes.Yellow ? 10 : -10;
                }
            }

            // Descending diagonal check
            for (int i = 3; i < board.GetLength(0); i++)
            {
                for (int j = 3; j < board.GetLength(1); j++)
                {
                    var player = board[i, j];
                    if (player == CellTypes.Free)
                        continue;

                    if (board[i, j] == player && board[i - 1, j - 1] == player)
                    {
                        int ddCount = 2;
                        if (board[i - 2, j - 2] == player)
                        {
                            ddCount++;
                            if (board[i - 3, j - 3] == player)
                                ddCount = 100;
                        }
                        return player == CellTypes.Yellow ? ddCount : -ddCount;
                    }

                    //if (board[i, j] == player && board[i - 1, j - 1] == player && board[i - 2, j - 2] == player && board[i - 3, j - 3] == player)
                    //return player == CellTypes.Yellow ? 10 : -10;
                }
            }

            return 0;
        }
    }
}
