namespace SuckSwag.Source
{
    using GameState;
    using Squalr.Source.ActionScheduler;
    using SuckSwag.Source.BoardFinder;
    using SuckSwag.Source.ChessEngine;
    using SuckSwag.Source.PieceFinder;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Analyzes the game state to produce a best move.
    /// </summary>
    internal class EngineTask : ScheduledTask
    {
        public EngineTask(Action<Bitmap, string, bool> updateBoardCallback) : base("Analysis", isRepeated: true, trackProgress: false)
        {
            this.UpdateBoardCallback = updateBoardCallback;

            this.BestMoveCache = new Dictionary<string, string>();
            this.GameBoard = new GameBoard();
        }

        private GameBoard GameBoard { get; set; }

        private string LastFen { get; set; }

        private Action<Bitmap, string, bool> UpdateBoardCallback { get; set; }

        private Dictionary<string, string> BestMoveCache { get; set; }

        public void AutoSetup()
        {
            this.GameBoard.AutoSetup();
        }

        protected override void OnUpdate()
        {
            if (EngineViewModel.GetInstance().FastMode)
            {
                this.PerformMoveCalculations(3);
            }
            else
            {
                this.PerformMoveCalculations(7);
            }
        }

        private void PerformMoveCalculations(int depth)
        {

            Bitmap board = PieceFinderViewModel.GetInstance().FindPieces(this.GameBoard);

            // Ensure kings are on the board and game state makes some degree of sense
            if (!this.PassesSanityChecks())
            {
                return;
            }

            DateTime startTime = DateTime.Now;
            string nextMove = string.Empty;

            // Calculate best move
            string newFen = this.GameBoard.GenerateFEN();

            if (newFen != this.LastFen)
            {
                // Use the engine to calculate the next best move
                nextMove = Cuckoo.simplyCalculateMove(newFen, depth);

                // Inform view of updates
                this.UpdateBoardCallback(board, nextMove, EngineViewModel.GetInstance().PlayingWhite);

                this.LastFen = newFen;
            }

            TimeSpan elapsedTime = DateTime.Now - startTime;
        }

        private bool PassesSanityChecks()
        {
            bool hasWhiteKing = false;
            bool hasBlackKing = false;

            foreach (GamePiece piece in this.GameBoard.Pieces)
            {
                if (piece.Color == GamePiece.PieceColor.White && piece.Name == GamePiece.PieceName.King)
                {
                    hasWhiteKing = true;
                }
                else if (piece.Color == GamePiece.PieceColor.Black && piece.Name == GamePiece.PieceName.King)
                {
                    hasBlackKing = true;
                }
            }

            if (!hasWhiteKing || !hasBlackKing)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sanity checks for the current depth.
        /// </summary>
        /// <param name="currentValue">The current depth</param>
        /// <returns></returns>
        private int LimitDepth(int currentValue)
        {
            int maxDepth = 0;

            int pieceCount = this.GameBoard.GetPieceCount();

            if (pieceCount == 0)
            {
                return maxDepth = 5;
            }

            // NORMALLY 32 peices total
            else if (pieceCount < 8)
            {
                maxDepth = 15;
            }
            else if (pieceCount < 10)
            {
                maxDepth = 14;
            }
            else if (pieceCount < 12)
            {
                maxDepth = 13;
            }
            else if (pieceCount < 14)
            {
                maxDepth = 12;
            }
            else if (pieceCount < 16)
            {
                maxDepth = 11;
            }
            else if (pieceCount < 20)
            {
                maxDepth = 10;
            }
            else if (pieceCount < 24)
            {
                maxDepth = 9;
            }
            else if (pieceCount < 28)
            {
                maxDepth = 8;
            }
            else
            {
                maxDepth = 7;
            }

            if (currentValue > maxDepth)
            {
                return maxDepth;
            }

            return currentValue;
        }

        public void SetPlayingWhite(bool whiteToPlay)
        {
            GameBoard.SetPlayingWhite(whiteToPlay);
        }
    }
    //// End class
}
//// End namespace