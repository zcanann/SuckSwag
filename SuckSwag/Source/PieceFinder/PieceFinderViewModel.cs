namespace SuckSwag.Source.PieceFinder
{
    using Docking;
    using Main;
    using SuckSwag.Source.BoardFinder;
    using SuckSwag.Source.GameState;
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

        private String boardText;

        private Dictionary<string, Bitmap> ImageHashes { get; set; }

        private Bitmap BlackPawn { get; set; }

        private Bitmap BlackBishop { get; set; }

        private Bitmap BlackKnight { get; set; }

        private Bitmap BlackRook { get; set; }

        private Bitmap BlackQueen { get; set; }

        private Bitmap BlackKing { get; set; }

        private Bitmap WhitePawn { get; set; }

        private Bitmap WhiteBishop { get; set; }

        private Bitmap WhiteKnight { get; set; }

        private Bitmap WhiteRook { get; set; }

        private Bitmap WhiteQueen { get; set; }

        private Bitmap WhiteKing { get; set; }

        private Bitmap Empty { get; set; }

        private Bitmap[] AllPieces { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceFinderViewModel" /> class from being created.
        /// </summary>
        private PieceFinderViewModel() : base("Piece Finder")
        {
            this.ContentId = PieceFinderViewModel.ToolContentId;

            this.ImageHashes = new Dictionary<string, Bitmap>();
            Assembly self = Assembly.GetExecutingAssembly();
            HashSet<Bitmap> allPieces = new HashSet<Bitmap>();
            this.Tint = new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 0, 0, 255));
            this.Tint.Freeze();

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_bishop.png"))
            {
                this.BlackBishop = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.BlackBishop.Tag = "black_bishop";
                allPieces.Add(this.BlackBishop);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_king.png"))
            {
                this.BlackKing = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.BlackKing.Tag = "black_king";
                allPieces.Add(this.BlackKing);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_knight.png"))
            {
                this.BlackKnight = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.BlackBishop.Tag = "black_knight";
                allPieces.Add(this.BlackKnight);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_pawn.png"))
            {
                this.BlackPawn = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.BlackPawn.Tag = "black_pawn";
                allPieces.Add(this.BlackPawn);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_queen.png"))
            {
                this.BlackQueen = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.BlackQueen.Tag = "black_queen";
                allPieces.Add(this.BlackQueen);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_rook.png"))
            {
                this.BlackRook = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.BlackRook.Tag = "black_rook";
                allPieces.Add(this.BlackRook);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_bishop.png"))
            {
                this.WhiteBishop = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.WhiteBishop.Tag = "white_bishop";
                allPieces.Add(this.WhiteBishop);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_king.png"))
            {
                this.WhiteKing = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.WhiteKing.Tag = "white_king";
                allPieces.Add(this.WhiteKing);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_knight.png"))
            {
                this.WhiteKnight = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.WhiteKnight.Tag = "white_knight";
                allPieces.Add(this.WhiteKnight);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_pawn.png"))
            {
                this.WhitePawn = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.WhitePawn.Tag = "white_pawn";
                allPieces.Add(this.WhitePawn);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_queen.png"))
            {
                this.WhiteQueen = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.WhiteQueen.Tag = "white_queen";
                allPieces.Add(this.WhiteQueen);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_rook.png"))
            {
                this.WhiteRook = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.WhiteRook.Tag = "white_rook";
                allPieces.Add(this.WhiteRook);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.empty.png"))
            {
                this.Empty = ImageUtils.PolarizeBlackWhite(new Bitmap(resourceStream));
                this.Empty.Tag = "empty";
                allPieces.Add(this.Empty);
            }

            this.AllPieces = allPieces.ToArray();

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

        public String BoardText
        {
            get
            {
                return this.boardText;
            }

            set
            {
                this.boardText = value;
                this.RaisePropertyChanged(nameof(this.BoardText));
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

        private string LastBoardHash { get; set; }

        public Bitmap FindPieces(GameBoard gameBoard)
        {
            Bitmap board = BoardFinderViewModel.GetInstance().FindBoard();

            // Hash the board and see if we've already processed this
            string boardHash = ImageUtils.ComputeImageHash(board);
            if (boardHash == this.LastBoardHash)
            {
                return board;
            }
            this.LastBoardHash = boardHash;

            Bitmap parsedPieces = ImageUtils.DiffBitmapsAndPrepareForTemplateMatching(BoardFinderViewModel.Board, ImageUtils.Clone(board));

            if (parsedPieces == null)
            {
                this.BoardImage = null;
                return parsedPieces;
            }

            this.BoardImage = ImageUtils.BitmapToBitmapImage(parsedPieces);

            int squareSize = BoardFinderViewModel.Board.Size.Width / GameBoard.SquareCount;

            for (int col = 0; col < GameBoard.SquareCount; col++)
            {
                for (int row = 0; row < GameBoard.SquareCount; row++)
                {
                    Bitmap square = ImageUtils.Copy(parsedPieces, new Rectangle(row * squareSize, col * squareSize, squareSize, squareSize));
                    Bitmap bestMatch;

                    string imageHash = ImageUtils.ComputeImageHash(square);

                    if (this.ImageHashes.ContainsKey(imageHash))
                    {
                        bestMatch = this.ImageHashes[imageHash];
                    }
                    else
                    {
                        bestMatch = ImageUtils.BestTemplateMatch(new Bitmap[] { square }, this.AllPieces);
                        this.ImageHashes.Add(imageHash, bestMatch);
                    }

                    if (bestMatch == this.Empty)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.None, GamePiece.PieceColor.Empty);
                    }
                    else if (bestMatch == this.WhitePawn)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Pawn, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == this.WhiteKnight)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Knight, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == this.WhiteBishop)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Bishop, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == this.WhiteRook)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Rook, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == this.WhiteQueen)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Queen, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == this.WhiteKing)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.King, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == this.BlackPawn)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Pawn, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == this.BlackKnight)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Knight, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == this.BlackBishop)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Bishop, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == this.BlackRook)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Rook, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == this.BlackQueen)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Queen, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == this.BlackKing)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.King, GamePiece.PieceColor.Black);
                    }
                    else
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.None, GamePiece.PieceColor.Empty);
                    }
                }
            }

            this.BoardText = gameBoard.ToString();

            return board;
        }
    }
    //// End class
}
//// End namespace