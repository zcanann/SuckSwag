namespace SuckSwag.View
{
    using Source.Main;
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
    }
    //// End class
}
//// End namespace