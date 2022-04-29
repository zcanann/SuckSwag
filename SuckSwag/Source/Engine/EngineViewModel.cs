namespace SuckSwag.Source.BoardFinder
{
    using Docking;
    using Main;
    using SuckSwag.Source.GameState;
    using SuckSwag.Source.Mvvm.Command;
    using SuckSwag.Source.Utils;
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
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

        public GameBoard GameBoard { get; set; }

        public string LastFen { get; set; }

        public string NextMove
        {
            get
            {
                return this.nextMove;
            }
            set
            {
                this.nextMove = value;
                this.RaisePropertyChanged(nameof(this.NextMove));
            }
        }

        private BitmapImage boardImage;

        private bool playingWhite;

        private bool whiteToMove;

        public bool fastMode;

        public bool enPassantAvilable;

        public bool whiteCanCastleKS;

        public bool whiteCanCastleQS;

        public bool blackCanCastleKS;

        public bool blackCanCastleQS;

        public string nextMove;

        private static Graphics graphics;

        private static System.Drawing.Pen Pen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);

        private static Point Source = new Point();

        private static Point Destination = new Point();

        /// <summary>
        /// Prevents a default instance of the <see cref="EngineViewModel" /> class from being created.
        /// </summary>
        private EngineViewModel() : base("Engine")
        {
            this.ContentId = EngineViewModel.ToolContentId;

            this.GameBoard = new GameBoard();
            this.ParseTask = new ParseTask();
            this.EngineTask = new EngineTask();
            this.AutoSetupCommand = new RelayCommand(() => this.GameBoard.AutoSetup(), () => true);
            this.Tint = new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 0, 0, 255));
            this.Tint.Freeze();

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

        public ICommand AutoSetupCommand { get; private set; }

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

        public bool FastMode
        {
            get
            {
                return this.fastMode;
            }

            set
            {
                this.fastMode = value;
                this.RaisePropertyChanged(nameof(this.FastMode));
            }
        }

        public bool EnPassantAvilable
        {
            get
            {
                return this.enPassantAvilable;
            }

            set
            {
                this.enPassantAvilable = value;
                this.RaisePropertyChanged(nameof(this.EnPassantAvilable));
            }
        }

        public bool WhiteCanCastleKS
        {
            get
            {
                return this.whiteCanCastleKS;
            }

            set
            {
                this.whiteCanCastleKS = value;
                this.RaisePropertyChanged(nameof(this.WhiteCanCastleKS));
            }
        }

        public bool WhiteCanCastleQS
        {
            get
            {
                return this.whiteCanCastleQS;
            }

            set
            {
                this.whiteCanCastleQS = value;
                this.RaisePropertyChanged(nameof(this.WhiteCanCastleQS));
            }
        }

        public bool BlackCanCastleKS
        {
            get
            {
                return this.blackCanCastleKS;
            }

            set
            {
                this.blackCanCastleKS = value;
                this.RaisePropertyChanged(nameof(this.BlackCanCastleKS));
            }
        }

        public bool BlackCanCastleQS
        {
            get
            {
                return this.blackCanCastleQS;
            }

            set
            {
                this.blackCanCastleQS = value;
                this.RaisePropertyChanged(nameof(this.BlackCanCastleQS));
            }
        }

        public bool PlayingWhite
        {
            get
            {
                return this.playingWhite;
            }

            set
            {
                this.playingWhite = value;
                this.RaisePropertyChanged(nameof(this.PlayingWhite));
                this.RaisePropertyChanged(nameof(this.PlayingBlack));
            }
        }

        public bool PlayingBlack
        {
            get
            {
                return !this.playingWhite;
            }

            set
            {
                this.PlayingWhite = !value;
            }
        }

        public bool WhiteToMove
        {
            get
            {
                return this.whiteToMove;
            }

            set
            {
                this.whiteToMove = value;
                this.RaisePropertyChanged(nameof(this.WhiteToMove));
                this.RaisePropertyChanged(nameof(this.BlackToMove));
            }
        }

        public bool BlackToMove
        {
            get
            {
                return !this.whiteToMove;
            }

            set
            {
                this.WhiteToMove = !value;
            }
        }

        private ParseTask ParseTask { get; set; }

        private EngineTask EngineTask { get; set; }

        public SolidColorBrush tint;

        public SolidColorBrush Tint
        {
            get
            {
                return this.tint;
            }

            set
            {
                this.tint = value;
                this.RaisePropertyChanged(nameof(this.Tint));
            }
        }

        public void BeginAnalysis()
        {
            ParseTask.Begin();
            EngineTask.Begin();
        }

        public void UpdateBoard(Bitmap boardBitmap)
        {
            boardBitmap = this.DrawMoveSuggestion(boardBitmap);
            this.BoardImage = ImageUtils.BitmapToBitmapImage(boardBitmap);
        }

        private Bitmap DrawMoveSuggestion(Bitmap boardBitmap)
        {
            if (boardBitmap == null || this.NextMove == null || this.NextMove.Length < 4)
            {
                return boardBitmap;
            }

            char[] move = this.NextMove.ToCharArray();

            graphics = Graphics.FromImage(boardBitmap);

            Source.X = ((byte)'h' - (byte)move[0]);
            Source.Y = ((byte)'8' - (byte)move[1]);

            Destination.X = ((byte)'h' - (byte)move[2]);
            Destination.Y = ((byte)'8' - (byte)move[3]);

            if (this.PlayingWhite)
            {
                Source.X = GameBoard.SquareCount - Source.X;
                Destination.X = GameBoard.SquareCount - Destination.X;
                Source.Y++;
                Destination.Y++;
            }
            else
            {
                Source.X = Source.X + 1;
                Destination.X = Destination.X + 1;
                Source.Y = GameBoard.SquareCount - Source.Y;
                Destination.Y = GameBoard.SquareCount - Destination.Y;
            }

            int squarePixelSize = BoardFinderViewModel.Board.Width / GameBoard.SquareCount;

            Source.X = Source.X * squarePixelSize - squarePixelSize / 2;
            Source.Y = Source.Y * squarePixelSize - squarePixelSize / 2;
            Destination.X = Destination.X * squarePixelSize - squarePixelSize / 2;
            Destination.Y = Destination.Y * squarePixelSize - squarePixelSize / 2;

            int circleSize = 12;

            graphics.DrawLine(Pen, Source, Destination);
            graphics.DrawEllipse(Pen, Destination.X - circleSize / 2, Destination.Y - circleSize / 2, circleSize, circleSize);
            graphics.Dispose();

            return boardBitmap;
        }
    }
    //// End class
}
//// End namespace