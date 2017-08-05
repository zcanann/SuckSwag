namespace SuckSwag.View
{
    using Source.Main;
    using SuckSwag.Source.BoardFinder;
    using SuckSwag.Source.PieceFinder;
    using SuckSwag.Source.SquareViewer;

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    internal class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
        }

        /// <summary>
        /// Gets the Main view model.
        /// </summary>
        public MainViewModel MainViewModel
        {
            get
            {
                return MainViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Square Viewer view model.
        /// </summary>
        public SquareViewerViewModel SquareViewerViewModel
        {
            get
            {
                return SquareViewerViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Board Finder view model.
        /// </summary>
        public BoardFinderViewModel BoardFinderViewModel
        {
            get
            {
                return BoardFinderViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the Piece Finder view model.
        /// </summary>
        public PieceFinderViewModel PieceFinderViewModel
        {
            get
            {
                return PieceFinderViewModel.GetInstance();
            }
        }

        /// <summary>
        /// Gets the EngineViewModel view model.
        /// </summary>
        public EngineViewModel EngineViewModel
        {
            get
            {
                return EngineViewModel.GetInstance();
            }
        }
    }
    //// End class
}
//// End namespace