namespace SuckSwag.Source
{
    using AForge.Imaging;
    using GameState;
    using Squalr.Source.ActionScheduler;
    using SuckSwag.Source.ChessEngine;
    using SuckSwag.Source.PieceFinder;
    using SuckSwag.Source.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

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
            this.SearchDepth = 5;
        }

        private GameBoard GameBoard { get; set; }

        private string LastFen { get; set; }

        private Action<Bitmap, string, bool> UpdateBoardCallback { get; set; }

        private Dictionary<string, string> BestMoveCache { get; set; }

        private int SearchDepth { get; set; }

        protected override void OnUpdate()
        {
            DateTime startTime = DateTime.Now;
            string nextMove = string.Empty;

            Bitmap board = PieceFinderViewModel.GetInstance().FindPieces(this.GameBoard);

            // Calculate best move
            string newFen = this.GameBoard.GenerateFEN();

            if (newFen != this.LastFen)
            {
                // Use the engine to calculate the next best move
                nextMove = Cuckoo.simplyCalculateMove(newFen, this.SearchDepth);

                // Inform view of updates
                this.UpdateBoardCallback(board, nextMove, this.GameBoard.PlayingWhite);

                this.LastFen = newFen;
            }

            TimeSpan elapsedTime = DateTime.Now - startTime;

            // TODO: Adjust depth
        }

        private Bitmap FindSquares(Bitmap screenShot)
        {
            List<Bitmap> potentialBoards = new List<Bitmap>();

            // Create an instance of blob counter algorithm
            BlobCounter bc = new BlobCounter();
            bc.BackgroundThreshold = Color.FromArgb(145, 145, 145);
            bc.ProcessImage(screenShot);

            IEnumerable<Rectangle> rects = bc.GetObjectsRectangles().Where(x => x.Width > 196 && x.Height > 196);

            // Process blobs
            using (Graphics g = Graphics.FromImage(screenShot))
            {
                RectangleF[] rectangles = rects.Select(x => new RectangleF(x.X, x.Y, x.Width, x.Height)).ToArray();

                if (!rectangles.IsNullOrEmpty())
                {
                    Pen pen = new Pen(Color.Red);
                    pen.Width = 7;
                    g.DrawRectangles(pen, rectangles);
                    pen.Dispose();
                }
            }

            return screenShot;
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