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
                    CheckForWinner();

                    return;
                }
            }
        }

        private void CheckForWinner()
        {
            
        }
    }
    #endregion
}
