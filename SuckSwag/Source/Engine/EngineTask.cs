namespace SuckSwag.Source
{
    using AForge.Imaging;
    using GameState;
    using Squalr.Source.ActionScheduler;
    using SuckSwag.Source.ChessEngine;
    using SuckSwag.Source.SquareViewer;
    using SuckSwag.Source.Utils;
    using SuckSwag.Source.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Analyzes the game state to produce a best move.
    /// </summary>
    internal class EngineTask : ScheduledTask
    {
        public EngineTask(Action<Bitmap> updateBoardCallback) : base("Analysis", isRepeated: true, trackProgress: false)
        {
            this.UpdateBoardCallback = updateBoardCallback;

            this.BestMoveCache = new Dictionary<string, string>();
            this.GameBoard = new GameBoard();
            this.SearchDepth = 5;
        }

        private GameBoard GameBoard { get; set; }

        private string LastFen { get; set; }

        private Action<Bitmap> UpdateBoardCallback { get; set; }

        private Action<string> UpdateFENCallback { get; set; }

        private Dictionary<string, string> BestMoveCache { get; set; }

        private int SearchDepth { get; set; }

        public static Graphics graphics;

        private static Pen Pen = new Pen(Color.Red, 3);

        private static Point Source = new Point();

        private static Point Destination = new Point();

        protected override void OnUpdate()
        {
            SquareViewerViewModel.GetInstance().Update();
            this.UpdateBoardCallback(this.FindSquares(ImageUtils.CollectScreenCapture()));

            return;
            DateTime startTime = DateTime.Now;
            string nextMove = string.Empty;

            // Update the board and collect the screenshot
            Bitmap newBoard = this.GameBoard.Update();

            // Calculate best move
            string newFen = this.GameBoard.GenerateFEN();

            if (newFen != this.LastFen)
            {
                // Use the engine to calculate the next best move
                nextMove = Cuckoo.simplyCalculateMove(newFen, this.SearchDepth);

                // Draw move suggestion line
                this.DrawMoveSuggestion(newBoard, nextMove, this.GameBoard);

                this.LastFen = newFen;
            }

            // Invoke callbacks
            this.UpdateBoardCallback(newBoard);
            // this.UpdateFENCallback(newFen);

            // Determine how long it took to do all updates
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

        public void Begin(Action<Bitmap> updateBoardCallback, Action<string> updateFENCallback)
        {
            this.UpdateBoardCallback = updateBoardCallback;
            this.UpdateFENCallback = updateFENCallback;

            Task.Run(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(400);
                    }
                }
            );
        }


        /// <summary>
        /// Sanity checks for the current depth
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

        private void UpdateLoop()
        {
        }

        public void SetPlayingWhite(bool whiteToPlay)
        {
            GameBoard.SetPlayingWhite(whiteToPlay);
        }

        private void DrawMoveSuggestion(Bitmap boardBitmap, string nextMove, GameBoard gameBoard)
        {
            if (boardBitmap == null)
            {
                return;
            }

            char[] Move = nextMove.ToCharArray();

            graphics = Graphics.FromImage(boardBitmap);

            if (Move.Length < 4)
                return;

            Source.X = ((byte)'h' - (byte)Move[0]);
            Source.Y = ((byte)'8' - (byte)Move[1]);

            Destination.X = ((byte)'h' - (byte)Move[2]);
            Destination.Y = ((byte)'8' - (byte)Move[3]);

            if (gameBoard.PlayingWhite)
            {
                Source.X = 8 - Source.X;
                Destination.X = 8 - Destination.X;
                Source.Y++;
                Destination.Y++;
            }
            else
            {
                Source.X = Source.X + 1;
                Destination.X = Destination.X + 1;
                Source.Y = 8 - Source.Y;
                Destination.Y = 8 - Destination.Y;
            }


            Source.X = Source.X * BoardRecognition.SquarePixelSize - BoardRecognition.SquarePixelSize / 2;
            Source.Y = Source.Y * BoardRecognition.SquarePixelSize - BoardRecognition.SquarePixelSize / 2;
            Destination.X = Destination.X * BoardRecognition.SquarePixelSize - BoardRecognition.SquarePixelSize / 2;
            Destination.Y = Destination.Y * BoardRecognition.SquarePixelSize - BoardRecognition.SquarePixelSize / 2;

            graphics.DrawLine(Pen, Source, Destination);
            graphics.DrawEllipse(Pen, Destination.X - 3, Destination.Y - 3, 5, 5);
            graphics.Dispose();
        }
    }
    //// End class
}
//// End namespace