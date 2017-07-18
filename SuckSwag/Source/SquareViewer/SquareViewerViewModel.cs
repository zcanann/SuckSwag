namespace SuckSwag.Source.SquareViewer
{
    using Docking;
    using Main;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// View model for the Square Viewer.
    /// </summary>
    internal class SquareViewerViewModel : ToolViewModel
    {
        /// <summary>
        /// The content id for the docking library associated with this view model.
        /// </summary>
        public const String ToolContentId = nameof(SquareViewerViewModel);

        /// <summary>
        /// Singleton instance of the <see cref="SquareViewerViewModel" /> class.
        /// </summary>
        private static Lazy<SquareViewerViewModel> squareViewerViewModelInstance = new Lazy<SquareViewerViewModel>(
                () => { return new SquareViewerViewModel(); },
                LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Prevents a default instance of the <see cref="SquareViewerViewModel" /> class from being created.
        /// </summary>
        private SquareViewerViewModel() : base("Square Viewer")
        {
            this.ContentId = SquareViewerViewModel.ToolContentId;

            Task.Run(() => MainViewModel.GetInstance().RegisterTool(this));
        }

        /// <summary>
        /// Gets a singleton instance of the <see cref="SquareViewerViewModel"/> class.
        /// </summary>
        /// <returns>A singleton instance of the class.</returns>
        public static SquareViewerViewModel GetInstance()
        {
            return SquareViewerViewModel.squareViewerViewModelInstance.Value;
        }
    }
    //// End class
}
//// End namespace