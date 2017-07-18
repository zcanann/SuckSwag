namespace SuckSwag.Source.GameState
{
    using AForge.Imaging;
    using SuckSwag.Source.Utils;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Handles piece recognition for the board.
    /// </summary>
    internal class BoardRecognition
    {
        /// <summary>
        /// 
        /// </summary>
        public BoardRecognition()
        {
            this.ImageRecognition = new ExhaustiveTemplateMatching();

            Assembly self = Assembly.GetExecutingAssembly();

            HashSet<Bitmap> allPieces = new HashSet<Bitmap>();

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.board.bmp"))
            {
                BoardRecognition.Board = new Bitmap(resourceStream);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.black_bishop.bmp"))
            {
                BoardRecognition.BlackBishop = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.BlackBishop);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.black_king.bmp"))
            {
                BoardRecognition.BlackKing = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.BlackKing);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.black_knight.bmp"))
            {
                BoardRecognition.BlackKnight = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.BlackKnight);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.black_pawn.bmp"))
            {
                BoardRecognition.BlackPawn = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.BlackPawn);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.black_queen.bmp"))
            {
                BoardRecognition.BlackQueen = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.BlackQueen);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.black_rook.bmp"))
            {
                BoardRecognition.BlackRook = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.BlackRook);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.white_bishop.bmp"))
            {
                BoardRecognition.WhiteBishop = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.WhiteBishop);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.white_king.bmp"))
            {
                BoardRecognition.WhiteKing = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.WhiteKing);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.white_knight.bmp"))
            {
                BoardRecognition.WhiteKnight = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.WhiteKnight);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.white_pawn.bmp"))
            {
                BoardRecognition.WhitePawn = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.WhitePawn);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.white_queen.bmp"))
            {
                BoardRecognition.WhiteQueen = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.WhiteQueen);
            }

            using (Stream resourceStream = self.GetManifestResourceStream("Enfoiree.Content.Images.white_rook.bmp"))
            {
                BoardRecognition.WhiteRook = new Bitmap(resourceStream);
                allPieces.Add(BoardRecognition.WhiteRook);
            }

            BoardRecognition.AllPieces = allPieces.ToArray();
        }

        private Bitmap CurrentBoard { get; set; }

        private ExhaustiveTemplateMatching ImageRecognition { get; set; }

        private static Bitmap Board { get; set; }

        public static int BoardPixelSize
        {
            get
            {
                return BoardRecognition.Board?.Width ?? 0;
            }
        }

        public static int SquarePixelSize
        {
            get
            {
                return BoardRecognition.BoardPixelSize / GameBoard.SquareCount;
            }
        }

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
        /// Updates the given game board with all parsed pieces, and returns a bitmap of the board.
        /// </summary>
        /// <param name="gameBoard">The game state.</param>
        /// <returns>A screen shot of the game board.</returns>
        public Bitmap UpdateGameBoard(GameBoard gameBoard)
        {
            Bitmap capturedBoard = this.FindGameBoard(ImageUtils.CollectScreenCapture());

            if (capturedBoard == null)
            {
                return this.CurrentBoard;
            }

            int squareSize = BoardRecognition.Board.Size.Width / GameBoard.SquareCount;

            for (int col = 0; col < GameBoard.SquareCount; col++)
            {
                for (int row = 0; row < GameBoard.SquareCount; row++)
                {
                    Bitmap square = ImageUtils.Copy(capturedBoard, new Rectangle(row * squareSize, col * squareSize, squareSize, squareSize));
                    Bitmap bestMatch = this.BestMatch(new Bitmap[] { ImageUtils.PolarizeBlackWhite(square) }, BoardRecognition.AllPieces);

                    if (bestMatch == BoardRecognition.WhitePawn)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Pawn, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == BoardRecognition.WhiteKnight)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Knight, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == BoardRecognition.WhiteBishop)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Bishop, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == BoardRecognition.WhiteRook)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Rook, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == BoardRecognition.WhiteQueen)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Queen, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == BoardRecognition.WhiteKing)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.King, GamePiece.PieceColor.White);
                    }
                    else if (bestMatch == BoardRecognition.BlackPawn)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Pawn, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == BoardRecognition.BlackKnight)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Knight, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == BoardRecognition.BlackBishop)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Bishop, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == BoardRecognition.BlackRook)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Rook, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == BoardRecognition.BlackQueen)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.Queen, GamePiece.PieceColor.Black);
                    }
                    else if (bestMatch == BoardRecognition.BlackKing)
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.King, GamePiece.PieceColor.Black);
                    }
                    else
                    {
                        gameBoard.UpdateSquare(row, col, GamePiece.PieceName.None, GamePiece.PieceColor.Empty);
                    }
                }
            }

            this.CurrentBoard = capturedBoard;
            return this.CurrentBoard;
        }

        /// <summary>
        /// Takes a screen shot and locates the game board.
        /// </summary>
        /// <param name="screenShot">The screen shot bitmap.</param>
        /// <returns>The game board bitmap.</returns>
        private Bitmap FindGameBoard(Bitmap screenShot)
        {
            List<Bitmap> potentialBoards = new List<Bitmap>();

            // Create an instance of blob counter algorithm
            BlobCounter bc = new BlobCounter();
            bc.BackgroundThreshold = Color.Gray;
            bc.ProcessImage(screenShot);

            IEnumerable<Rectangle> rects = bc.GetObjectsRectangles().Where(x => x.Width > 196 && x.Height > 196 && x.Width <= 840 && x.Height <= 840);

            // Process blobs
            foreach (Rectangle rect in rects)
            {
                // Create the new bitmap and associated graphics object
                Bitmap parsedRectangle = ImageUtils.Copy(screenShot, rect);

                Bitmap resizedRectangle = new Bitmap(parsedRectangle, new Size(BoardRecognition.Board.Width, BoardRecognition.Board.Height));

                potentialBoards.Add(ImageUtils.Clone(resizedRectangle));
            }

            return this.BestMatch(potentialBoards.ToArray(), BoardRecognition.Board);
        }

        /// <summary>
        /// Finds the best match for the given candidate image against the provided templates.
        /// </summary>
        /// <param name="candidates">The image being compared.</param>
        /// <param name="templates">The templates against which to compare the image.</param>
        /// <returns></returns>
        private Bitmap BestMatch(IEnumerable<Bitmap> candidates, params Bitmap[] templates)
        {
            const float similarityThreshold = 0.25f;

            if (candidates == null || candidates.Count() <= 0)
            {
                return null;
            }

            Bitmap bestMatch = candidates
              // Get the similarity to all template images
              .Select(candidate =>
                  new
                  {
                      bitmap = candidate,
                      matchings = templates.Select(template => this.ImageRecognition.ProcessImage(candidate, template)),
                  })
              .Select(candidate =>
                  new
                  {
                      bitmap = candidate.bitmap,
                      similarity = candidate.matchings.Select(match => match.Count() > 0 ? match[0].Similarity : 0.0f).Max(),
                  })

               // Threshold the similarity
               .Where(board => board.similarity > similarityThreshold)

               // Pick the best
               .OrderByDescending(template => template.similarity)
               .FirstOrDefault()?.bitmap;

            return bestMatch;
        }
    }
    //// End class
}
//// End namespace
