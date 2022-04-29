namespace SuckSwag.Source.BoardFinder
{
    using Docking;
    using Main;
    using SuckSwag.Source.SquareViewer;
    using SuckSwag.Source.Utils;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// View model for the Board Finder.
    /// </summary>
    internal class BoardFinderViewModel : ToolViewModel
    {
        /// <summary>
        /// The content id for the docking library associated with this view model.
        /// </summary>
        public const String ToolContentId = nameof(BoardFinderViewModel);

        /// <summary>
        /// Singleton instance of the <see cref="BoardFinderViewModel" /> class.
        /// </summary>
        private static Lazy<BoardFinderViewModel> boardFinderViewModelInstance = new Lazy<BoardFinderViewModel>(
                () => { return new BoardFinderViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        private BitmapImage boardImage;

        public static Bitmap Board { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="BoardFinderViewModel" /> class from being created.
        /// </summary>
        private BoardFinderViewModel() : base("Board Finder")
        {
            this.ContentId = BoardFinderViewModel.ToolContentId;
            this.Tint = new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 0, 0, 255));
            this.Tint.Freeze();

            Assembly self = Assembly.GetExecutingAssembly();

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.board.png"))
            {
                BoardFinderViewModel.Board = ImageUtils.Clone(new Bitmap(resourceStream));
            }

            Task.Run(() => MainViewModel.GetInstance().RegisterTool(this));
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="BoardFinderViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static BoardFinderViewModel GetInstance()
        {
            return BoardFinderViewModel.boardFinderViewModelInstance.Value;
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

        public Bitmap FindBoard()
        {
            IEnumerable<Bitmap> potentialBoards = SquareViewerViewModel.GetInstance().FindSquares();

            Bitmap board = ImageUtils.BestCandidateMatch(potentialBoards.ToArray(), BoardFinderViewModel.Board);

            this.BoardImage = ImageUtils.BitmapToBitmapImage(board);

            return board;
        }
    }
    //// End class
}
//// End namespace