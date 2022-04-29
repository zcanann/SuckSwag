namespace SuckSwag.Source.SquareViewer
{
    using Accord.Imaging;
    using Docking;
    using Main;
    using SuckSwag.Source.BoardFinder;
    using SuckSwag.Source.Utils;
    using SuckSwag.Source.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

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

        private BitmapImage squaresImage;

        /// <summary>
        /// Prevents a default instance of the <see cref="SquareViewerViewModel" /> class from being created.
        /// </summary>
        private SquareViewerViewModel() : base("Square Viewer")
        {
            this.ContentId = SquareViewerViewModel.ToolContentId;

            this.RedPen = new System.Drawing.Pen(color: System.Drawing.Color.Red, width: 7);
            this.Tint = new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 0, 0, 255));
            this.Tint.Freeze();

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

        /// <summary>
        /// 
        /// </summary>
        public BitmapImage SquaresImage
        {
            get
            {
                return this.squaresImage;
            }

            set
            {
                this.squaresImage = value;
                this.RaisePropertyChanged(nameof(this.SquaresImage));
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

        /// <summary>
        /// 
        /// </summary>
        private System.Drawing.Pen RedPen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Bitmap> FindSquares()
        {
            List<Bitmap> potentialBoards = new List<Bitmap>();
            Bitmap screenShot = ImageUtils.CollectScreenCapture();

            // Create an instance of blob counter algorithm
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.BackgroundThreshold = System.Drawing.Color.FromArgb(145, 145, 145);
            blobCounter.ProcessImage(screenShot);

            // Enforce a minimum square size, and ensure the squares are true squares and not rectangular
            IEnumerable<Rectangle> rectangles = blobCounter.GetObjectsRectangles()
                .Where(x => x.Width > 196)
                .Where(x => x.Height > 196)
                .Where(x => ((float)x.Width / (float)x.Height).AlmostEquals(1.0f));

            // Process rectangles
            foreach (Rectangle rectangle in rectangles)
            {
                Bitmap parsedRectangle = ImageUtils.Copy(screenShot, rectangle);
                Bitmap resizedRectangle = new Bitmap(parsedRectangle, new Size(BoardFinderViewModel.Board.Width, BoardFinderViewModel.Board.Height));
                potentialBoards.Add(ImageUtils.Clone(resizedRectangle));
            }

            // Draw rectangles
            using (Graphics graphics = Graphics.FromImage(screenShot))
            {
                RectangleF[] floatRectangles = rectangles.Select(x => new RectangleF(x.X, x.Y, x.Width, x.Height)).ToArray();

                if (!floatRectangles.IsNullOrEmpty())
                {
                    graphics.DrawRectangles(this.RedPen, floatRectangles);
                }
            }

            this.SquaresImage = ImageUtils.BitmapToBitmapImage(screenShot);

            return potentialBoards;
        }
    }
    //// End class
}
//// End namespace