using System;
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
        #endregion
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }

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

            // Return if cell isn't free
            for (int i = mCellTypes.GetLength(0) - 1; i >= 0; i--)
            {
                if (mCellTypes[i, col] == CellTypes.Free)
                {
                    var CellToFill = Container.Children.Cast<Button>().First(ButtonToMove => Grid.GetRow(ButtonToMove) == i && Grid.GetColumn(ButtonToMove) == col);

                    // Set the cell value based on turn
                    mCellTypes[i, col] = mPlayer1Turn ? CellTypes.Red : CellTypes.Yellow;

                    // Set the cell colour based on turn
                    CellToFill.Background = mPlayer1Turn ? Brushes.Red : Brushes.Yellow;

                    // Toggle turn
                    mPlayer1Turn = !mPlayer1Turn;

                    // Check for a winner
                    var Winner = CheckForWinner(mCellTypes);

                    if (Winner == CellTypes.Red)
                    {
                        mGameEnded = true;
                    }
                    else if (Winner == CellTypes.Yellow)
                    {
                        mGameEnded = true;
                    }

                    return;
                }
            }
        }

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
    }
    #endregion
}
