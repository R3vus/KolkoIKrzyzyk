﻿
using System;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;


namespace KolkoIKrzyzyk
{

    public partial class MainWindow : Window
    {
        #region Private members 
        /// <summary>
        /// array of buttons
        /// </summary>
        Button[] mButtons;
        /// <summary>
        /// True if it is player 1's turn (X) or player 2's turn (O)
        /// </summary>
        private bool mPlayer1Turn;
        /// <summary>
        /// True if game ended
        /// </summary>
        private bool mGameEnded;
        /// <summary>
        /// size of the board
        /// </summary>
        private int mSize;
        /// <summary>
        /// limit of marks in line needed to win game
        /// </summary>
        private int mLimit;




        /// <summary>
        /// Holds the current results of cells in the active game
        /// </summary>
        private char[,] mBoard;

        /// <summary>
        /// human player
        /// </summary>
        char player = 'X';
        /// <summary>
        /// AI player
        /// </summary>
        char opponent = 'O';
        #endregion
        #region Contructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }
        #endregion

        private void NewGame()
        {
            ///Create a new blan array of free cells

            mSize = 3;
            mLimit = 3;
            mBoard = new char[mSize, mSize];

            for (var j = 0; j < mSize; j++)
            {
                for (var i = 0; i < mSize; i++)
                    mBoard[i,j] = '_';
            }
            ///Make sure player 1 starts the game
            mPlayer1Turn = true;
            mButtons = Container.Children.Cast<Button>().ToArray();
            //Iterate every button on the grid
            Container.Children.Cast<Button>().ToList().ForEach(button =>
            {
                // Change background, foreground and content to default values
                button.Content = "";
                button.Background = Brushes.White;
                button.Foreground = Brushes.Blue;
            });

            //Make sure the game hasn't finished
            mGameEnded = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {     
            //Start a new game on the click after it finished
            if (mGameEnded)
            {
                NewGame();
                return;
            }
            //Cast the sendet to a button
            var button = (Button)sender;
            //finds buttons position in the array
            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            var index = column + (row * mSize);
            // Dont do enything if the cell already has a value in it
            if (mBoard[column,row] != '_')
                return;

            //Set the cell value based on which players turn it is
            mBoard[column,row] = mPlayer1Turn ? 'X': 'O';

            //set button text to the result
            button.Content = mPlayer1Turn ? "X" : "O";

            //change noughts to green
            if (!mPlayer1Turn)
                button.Foreground = Brushes.Red;

            //Toggle the players turns
            mPlayer1Turn ^= true;

            //Check for a winner
            CheckForWinner();
            if (mGameEnded)
            {
                return;
            }
            //AI move
            AIMove();
            //Check for a winner
            CheckForWinner();
        }

        private void AIMove()
        {
            Move bestMove = Minimax(mBoard);
            mButtons[bestMove.col +((bestMove.row )* mSize)].Content = "O";
            mBoard[bestMove.col,bestMove.row] = 'O';
            mPlayer1Turn ^= true;
        }

        /// <summary>
        /// Checks if ther is a winner of a 3 line straight
        /// </summary>
        private void CheckForWinner()
        {
            {
                var licznikWin = 0;
                var licznikLose = 0;
                #region horizontal wins
                // Check for horizontal wins
                for (var j = 0; j < mSize; j++)
                {
                    licznikLose = licznikWin = 0;
                    for (var i = 0; i < mSize; i++)
                    {
                        if (mBoard[i, j] == player)
                        {
                            licznikWin++;
                            licznikLose = 0;
                        }
                        if (mBoard[i, j] == opponent)
                        {
                            licznikLose++;
                            licznikWin = 0;
                        }
                        if (licznikWin >= mLimit)
                        {
                            // Game ends
                            mGameEnded = true;
                            // Highlight winning cells in green
                            mButtons[j * mSize].Background = mButtons[j * mSize + 1].Background = mButtons[j * mSize + 2].Background = Brushes.Green;
                        }
                        if (licznikLose >= mLimit)
                        {
                            // Game ends
                            mGameEnded = true;
                            mButtons[j * mSize].Background = mButtons[j * mSize + 1].Background = mButtons[j * mSize + 2].Background = Brushes.Red;
                        }
                        }
                }
                #endregion
                #region vertical wins
                // Check for vertical wins 
                licznikWin = 0;
                licznikLose = 0;
                for (var j = 0; j < mSize; j++)
                {
                    licznikLose = licznikWin = 0;
                    for (var i = 0; i < mSize; i++)
                    {
                        if (mBoard[j, i] == player)
                        {
                            licznikWin++;
                            licznikLose = 0;
                        }
                        if (mBoard[j, i] == opponent)
                        {
                            licznikLose++;
                            licznikWin = 0;
                        }
                        if (licznikWin >= mLimit)
                        {
                            // Game ends
                            mGameEnded = true;
                            // Highlight winning cells in green
                            mButtons[ 0 * mSize+j].Background = mButtons[1 * mSize + j].Background = mButtons[2 * mSize + j].Background = Brushes.Green;
                        }
                        if (licznikLose >= mLimit)
                        {
                            mGameEnded = true;
                            mButtons[0 * mSize + j].Background = mButtons[1 * mSize + j].Background = mButtons[2 * mSize + j].Background = Brushes.Red;
                        }
                    }
                }
                #endregion
                #region diagonal wins
                // Check for diagonal wins 
                licznikWin = 0;
                licznikLose = 0;
                for (var i = 0; i < mSize; i++)
                {
                    if (mBoard[i, i] == player)
                    {
                        licznikWin++;
                        licznikLose = 0;
                    }
                    if (mBoard[i, i] == opponent)
                    {
                        licznikLose++;
                        licznikWin = 0;
                    }
                    if (licznikWin >= mLimit)
                    {
                        // Game ends
                        mGameEnded = true;
                        // Highlight winning cells in green
                        mButtons[0].Background = mButtons[4].Background = mButtons[8].Background = Brushes.Green;
                    }
                    if (licznikLose >= mLimit)
                    {
                        // Game ends
                        mGameEnded = true;
                        // Highlight winning cells in green
                        mButtons[0].Background = mButtons[4].Background = mButtons[8].Background = Brushes.Red;
                    }
                }
                licznikWin = 0;
                licznikLose = 0;
                for (var i = 0; i < mSize; i++)
                {
                    if (mBoard[mSize -1 - i, i] == player)
                    {
                        licznikWin++;
                        licznikLose = 0;
                    }
                    if (mBoard[mSize-1 - i, i] == opponent)
                    {
                        licznikLose++;
                        licznikWin = 0;
                    }
                    if (licznikWin >= mLimit)
                    {
                        // Game ends
                        mGameEnded = true;
                        // Highlight winning cells in green
                        mButtons[2].Background = mButtons[4].Background = mButtons[6].Background = Brushes.Green;
                    }
                    if (licznikLose >= mLimit)
                    {
                        // Game ends
                        mGameEnded = true;
                        // Highlight winning cells in green
                        mButtons[2].Background = mButtons[4].Background = mButtons[6].Background = Brushes.Red;
                    }
                }
                #endregion
            }
 
            //Check for no winner and full mBoard 
            var fFull = 1;
            for (int j = 0; j < mSize; j++)
                for (int i = 0; i < mSize; i++)
                    if (mBoard[i, j] == '_')
                        fFull = 0;
            if (fFull == 1)
            {
                //Game ended
                mGameEnded = true;

                //Turn all cells orange
                Container.Children.Cast<Button>().ToList().ForEach(button =>
                {
                    button.Background = Brushes.Orange;
                });
            }
        }



        private int Evaluate(char[,] board,int key)
        #region Evaluate board
        {
            var licznikWin = 0;
            var licznikLose = 0;
            #region horizontal wins
            // Check for horizontal wins
            for (var j = 0; j < mSize; j++)
            {
                licznikLose = licznikWin = 0;
                for (var i = 0; i < mSize; i++)
                {
                    if (board[i, j] == opponent)
                    {
                        licznikWin++;
                        licznikLose = 0;
                    }
                    if (board[i, j] == player)
                    {
                        licznikLose++;
                        licznikWin = 0;
                    }
                    if (licznikWin >= mLimit)
                        return  10;
                    if (licznikLose >= mLimit)
                        return  -10;
                }
            }
            #endregion
            #region vertical wins
            // Check for vertical wins 
            licznikWin = 0;
            licznikLose = 0;
            for (var j = 0; j < mSize; j++)
            {
                licznikLose = licznikWin = 0;
                for (var i = 0; i < mSize; i++)
                {
                    if (board[j, i] == opponent)
                    {
                        licznikWin++;
                        licznikLose = 0;
                    }
                    if (board[j, i] == player)
                    {
                        licznikLose++;
                        licznikWin = 0;
                    }
                    if (licznikWin >= mLimit)
                        return 10;
                    if (licznikLose >= mLimit)
                        return -10;

                }
            }
            #endregion
            #region diagonal wins
            // Check for diagonal wins 
            licznikWin = 0;
            licznikLose = 0;
            for (var i = 0; i < mSize; i++)
            {
                if (board[i, i] == opponent)
                {
                    licznikWin++;
                    licznikLose = 0;
                }
                if (board[i, i] == player)
                {
                    licznikLose++;
                    licznikWin = 0;
                }
                if (licznikWin >= mLimit)
                    return 10;
                if (licznikLose >= mLimit)
                    return -10;
            }
            licznikWin = 0;
            licznikLose = 0;
            for (var i = 0; i < mSize; i++)
            {
                if (board[mSize-1-i, i] == opponent)
                {
                    licznikWin++;
                    licznikLose = 0;
                }
                if (board[mSize-1-i, i] == player)
                {
                    licznikLose++;
                    licznikWin = 0;
                }
                if (licznikWin >= mLimit)
                    return 10;
                if (licznikLose >= mLimit)
                    return -10;
            }
            #endregion
            return 0;
        }
        #endregion

        // This is the minimax function. It considers all
        // the possible ways the game can go and returns
        // the value of the board
        Move Minimax(char[,] board)
        #region minmax function
        {
            int bestScore = -1000;
            var bestMove = new Move
            {
                row = -1,
                col = -1
            };
            var key = 10;
            // Traverse all cells, evalutae minimax function for
            // all empty cells. And return the cell with optimal
            // value.
            for (int j = 0; j < mSize; j++)
            {
                for (int i = 0; i < mSize; i++)
                {
                    // Check if celll is empty
                    if (board[i, j] == '_')
                    {
                        // Make the move
                        board[i, j] = opponent;

                        // compute evaluation function for this
                        // move.
                        int score = MIN(board,key);

                        // Undo the move
                        board[i, j] = '_';

                        // If the value of the current move is
                        // more than the best value, then update
                        // best/
                        if (score > bestScore)
                        {
                            bestMove.col = i;
                            bestMove.row = j;
                            bestScore = score;
                        }
                    }
                }
            }
            return bestMove;
        }
        #endregion

        private int MIN(char[,] board, int key)
        #region min
        {
            var score = Evaluate(board, key);
            if (score != 0)
                return score;
            // If there are no more moves and no winner then
            // it is a tie
            var fFull = 1;
            for (int j = 0; j < mSize; j++)
                for (int i = 0; i < mSize; i++)
                    if (board[i, j] == '_')
                        fFull = 0;
            if (fFull == 1)
                return Evaluate(board, key);
            score = 0;
            var bestscore = 1000;
            // Traverse all cells
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    // Check if cell is empty
                    if (board[i, j] == '_')
                    {
                        // Make the move
                        board[i, j] = player;

                        // Call minimax recursively and choose
                        // the maximum value
                        score = MAX(board,key-1);
                        if (score < bestscore)
                            bestscore = score;
                        // Undo the move
                        board[i, j] = '_';
                    }
                }
            }
            return bestscore;
        }
#endregion
        private int MAX(char [,] board, int key)
        #region max
        {
            var score = Evaluate(board, key);
            if (score != 0)
                return score;

            // If there are no more moves and no winner then
            // it is a tie
            var fFull = 1;
            for (int j = 0; j < mSize; j++)
                for (int i = 0; i < mSize; i++)
                    if (board[i, j] == '_')
                        fFull = 0;
            if (fFull == 1)
                return Evaluate(board,key); ;
             score = 0;
            var bestscore = -1000;
            // Traverse all cells
            for (int j = 0; j < mSize; j++)
            {
                for (int i = 0; i < mSize; i++)
                {
                    // Check if cell is empty
                    if (board[i, j] == '_')
                    {
                        // Make the move
                        board[i, j] = opponent;

                        // Call minimax recursively and choose
                        // the maximum value
                        score = MIN(board,key-1);
                        if (score > bestscore)
                            bestscore = score;       
                        // Undo the move
                        board[i, j] = '_';
                    }
                }
            }
            return bestscore;
        }
        #endregion


    }
}