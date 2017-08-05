namespace SuckSwag.Source.BoardFinder
{
    using Docking;
    using Main;
    using SuckSwag.Source.Utils;
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

        private void OnBoardUpdate(Bitmap board)
        {
            this.BoardImage = ImageUtils.BitmapToBitmapImage(board);
        }
    }
    //// End class
}
//// End namespace