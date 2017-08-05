namespace SuckSwag.Source.BoardFinder
{
    using Docking;
    using Main;
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// View model for the Engine.
    /// </summary>
    internal class EngineViewModel : ToolViewModel
    {
        /// <summary>
        /// The content id for the docking library associated with this view model.
        /// </summary>
        public const String ToolContentId = nameof(EngineViewModel);

        /// <summary>
        /// Singleton instance of the <see cref="EngineViewModel" /> class.
        /// </summary>
        private static Lazy<EngineViewModel> engineViewModelInstance = new Lazy<EngineViewModel>(
                () => { return new EngineViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        private BitmapImage boardImage;

        private static Graphics graphics;

        private static Pen Pen = new Pen(Color.Red, 3);

        private static Point Source = new Point();

        private static Point Destination = new Point();

        /// <summary>
        /// Prevents a default instance of the <see cref="EngineViewModel" /> class from being created.
        /// </summary>
        private EngineViewModel() : base("Engine")
        {
            this.ContentId = EngineViewModel.ToolContentId;

            this.EngineTask = new EngineTask(this.OnBoardUpdate);

            Task.Run(() => MainViewModel.GetInstance().RegisterTool(this));
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="EngineViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static EngineViewModel GetInstance()
        {
            return EngineViewModel.engineViewModelInstance.Value;
        }

        public BitmapImage BoardImage
        {
            get
            {
                return this.boardImage;
            }

            set
            {
                this.boardImage = value;
                this.RaisePropertyChanged(nameof(this.BoardImage));
            }
        }

        private EngineTask EngineTask { get; set; }

        public void BeginAnalysis()
        {
            EngineTask.Begin();
        }

        private void OnBoardUpdate(Bitmap boardBitmap, string bestMoveFen, bool playingWhite)
        {
            boardBitmap = this.DrawMoveSuggestion(boardBitmap, bestMoveFen, playingWhite);
            // this.BoardImage = ImageUtils.BitmapToBitmapImage(board);
        }

        private Bitmap DrawMoveSuggestion(Bitmap boardBitmap, string nextMove, bool playingWhite)
        {
            if (boardBitmap == null)
            {
                return boardBitmap;
            }

            char[] Move = nextMove.ToCharArray();

            graphics = Graphics.FromImage(boardBitmap);

            if (Move.Length < 4)
                return boardBitmap;

            Source.X = ((byte)'h' - (byte)Move[0]);
            Source.Y = ((byte)'8' - (byte)Move[1]);

            Destination.X = ((byte)'h' - (byte)Move[2]);
            Destination.Y = ((byte)'8' - (byte)Move[3]);

            if (playingWhite)
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

            int squarePixelSize = BoardFinderViewModel.Board.Width / 8;

            Source.X = Source.X * squarePixelSize - squarePixelSize / 2;
            Source.Y = Source.Y * squarePixelSize - squarePixelSize / 2;
            Destination.X = Destination.X * squarePixelSize - squarePixelSize / 2;
            Destination.Y = Destination.Y * squarePixelSize - squarePixelSize / 2;

            graphics.DrawLine(Pen, Source, Destination);
            graphics.DrawEllipse(Pen, Destination.X - 3, Destination.Y - 3, 5, 5);
            graphics.Dispose();

            return boardBitmap;
        }
    }
    //// End class
}
//// End namespace