namespace SuckSwag.Source.PieceFinder
{
    using Docking;
    using Main;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// View model for the Piece Finder.
    /// </summary>
    internal class PieceFinderViewModel : ToolViewModel
    {
        /// <summary>
        /// The content id for the docking library associated with this view model.
        /// </summary>
        public const String ToolContentId = nameof(PieceFinderViewModel);

        /// <summary>
        /// Singleton instance of the <see cref="PieceFinderViewModel" /> class.
        /// </summary>
        private static Lazy<PieceFinderViewModel> pieceFinderViewModelInstance = new Lazy<PieceFinderViewModel>(
                () => { return new PieceFinderViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        private BitmapImage boardImage;

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceFinderViewModel" /> class from being created.
        /// </summary>
        private PieceFinderViewModel() : base("Piece Finder")
        {
            this.ContentId = PieceFinderViewModel.ToolContentId;

            Task.Run(() => MainViewModel.GetInstance().RegisterTool(this));
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="PieceFinderViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static PieceFinderViewModel GetInstance()
        {
            return PieceFinderViewModel.pieceFinderViewModelInstance.Value;
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

        public void ParsePieces()
        {

        }
    }
    //// End class
}
//// End namespace