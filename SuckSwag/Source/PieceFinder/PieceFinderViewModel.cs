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

        private static Bitmap BlackPawn { get; set; }

        private static Bitmap BlackBishop { get; set; }

        private static Bitmap BlackKnight { get; set; }

        private static Bitmap BlackRook { get; set; }

        private static Bitmap BlackQueen { get; set; }

        private static Bitmap BlackKing { get; set; }

        private static Bitmap WhitePawn { get; set; }

        private static Bitmap WhiteBishop { get; set; }

        private static Bitmap WhiteKnight { get; set; }

        private static Bitmap WhiteRook { get; set; }

        private static Bitmap WhiteQueen { get; set; }

        private static Bitmap WhiteKing { get; set; }

        private static Bitmap[] AllPieces { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="PieceFinderViewModel" /> class from being created.
        /// </summary>
        private PieceFinderViewModel() : base("Piece Finder")
        {
            this.ContentId = PieceFinderViewModel.ToolContentId;

            Assembly self = Assembly.GetExecutingAssembly();
            HashSet<Bitmap> allPieces = new HashSet<Bitmap>();

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_bishop.bmp"))
            {
                PieceFinderViewModel.BlackBishop = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.BlackBishop);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_king.bmp"))
            {
                PieceFinderViewModel.BlackKing = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.BlackKing);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_knight.bmp"))
            {
                PieceFinderViewModel.BlackKnight = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.BlackKnight);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_pawn.bmp"))
            {
                PieceFinderViewModel.BlackPawn = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.BlackPawn);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_queen.bmp"))
            {
                PieceFinderViewModel.BlackQueen = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.BlackQueen);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.black_rook.bmp"))
            {
                PieceFinderViewModel.BlackRook = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.BlackRook);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_bishop.bmp"))
            {
                PieceFinderViewModel.WhiteBishop = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.WhiteBishop);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_king.bmp"))
            {
                PieceFinderViewModel.WhiteKing = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.WhiteKing);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_knight.bmp"))
            {
                PieceFinderViewModel.WhiteKnight = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.WhiteKnight);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_pawn.bmp"))
            {
                PieceFinderViewModel.WhitePawn = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.WhitePawn);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_queen.bmp"))
            {
                PieceFinderViewModel.WhiteQueen = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.WhiteQueen);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("SuckSwag.Content.Images.white_rook.bmp"))
            {
                PieceFinderViewModel.WhiteRook = new Bitmap(resourceStream);
                allPieces.Add(PieceFinderViewModel.WhiteRook);
            }

            PieceFinderViewModel.AllPieces = allPieces.ToArray();

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

        public Bitmap FindPieces(GameBoard gameBoard)
        {
            Bitmap board = BoardFinderViewModel.GetInstance().FindBoard();

            if (board == null)
            {
                return board;
            }

            int squareSize = BoardFinderViewModel.Board.Size.Width / GameBoard.SquareCount;

            for (int col = 0; col < GameBoard.SquareCount; col++)
            {
                for (int row = 0; row < GameBoard.SquareCount; row++)
                {
                    Bitmap square = ImageUtils.Copy(board, new Rectangle(row * squareSize, col * squareSize, squareSize, squareSize));
                    Bitmap bestMatch = ImageRecognition.BestMatch(new Bitmap[] { ImageUtils.PolarizeBlackWhite(square) }, PieceFinderViewModel.AllPieces);

                    if (bestMatch == PieceFinderViewModel.WhitePawn)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Pawn, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == PieceFinderViewModel.WhiteKnight)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Knight, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == PieceFinderViewModel.WhiteBishop)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Bishop, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == PieceFinderViewModel.WhiteRook)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Rook, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == PieceFinderViewModel.WhiteQueen)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Queen, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == PieceFinderViewModel.WhiteKing)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.King, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == PieceFinderViewModel.BlackPawn)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Pawn, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == PieceFinderViewModel.BlackKnight)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Knight, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == PieceFinderViewModel.BlackBishop)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Bishop, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == PieceFinderViewModel.BlackRook)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Rook, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == PieceFinderViewModel.BlackQueen)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Queen, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == PieceFinderViewModel.BlackKing)
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