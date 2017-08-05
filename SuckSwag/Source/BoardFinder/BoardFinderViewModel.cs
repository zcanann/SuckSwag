namespace SuckSwag.Source.BoardFinder
{
    using Docking;
    using Main;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
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

        /// <summary>
        /// Prevents a default instance of the <see cref="BoardFinderViewModel" /> class from being created.
        /// </summary>
        private BoardFinderViewModel() : base("Board Finder")
        {
            this.ContentId = BoardFinderViewModel.ToolContentId;

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

        public void ParseBoard()
        {

        }
    }
    //// End class
}
//// End namespace