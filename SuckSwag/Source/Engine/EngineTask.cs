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
        public EngineTask() : base("Analysis", isRepeated: true, trackProgress: false)
        {
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
            DateTime startTime = DateTime.Now;

            // Calculate best move
            string newFen = EngineViewModel.GetInstance().GameBoard.GenerateFEN();

            if (newFen != EngineViewModel.GetInstance().LastFen)
            {
                // Use the engine to calculate the next best move
                string nextMove = Cuckoo.simplyCalculateMove(newFen, depth);

                EngineViewModel.GetInstance().NextMove = nextMove;
                EngineViewModel.GetInstance().LastFen = newFen;
            }

            TimeSpan elapsedTime = DateTime.Now - startTime;
        }
        private bool PassesSanityChecks()
        {
            bool hasWhiteKing = false;
            bool hasBlackKing = false;

            foreach (GamePiece piece in EngineViewModel.GetInstance().GameBoard.Pieces)
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

            int pieceCount = EngineViewModel.GetInstance().GameBoard.GetPieceCount();

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
    }
    //// End class
}
//// End namespace