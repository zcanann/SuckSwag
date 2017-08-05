using System;

namespace SuckSwag.Source.GameState
{
    internal class GameBoard
    {
        public const int SquareCount = 8;

        public GameBoard()
        {
            this.Pieces = new GamePiece[SquareCount, SquareCount];
            this.PlayingWhite = true;
            this.WhiteToMove = true;

            // Initialize board to be empty
            for (int y = 0; y < SquareCount; y++)
            {
                for (int x = 0; x < SquareCount; x++)
                {
                    Pieces[x, y] = new GamePiece(GamePiece.PieceName.None, GamePiece.PieceColor.Empty);
                }
            }
        }

        public GamePiece[,] Pieces { get; private set; }

        public bool PlayingWhite { get; private set; }

        public bool WhiteToMove { get; private set; }

        public bool EnPassantAvilable { get; private set; }

        public bool WhiteCanCastleKS { get; private set; }

        public bool WhiteCanCastleQS { get; private set; }

        public bool BlackCanCastleKS { get; private set; }

        public bool BlackCanCastleQS { get; private set; }

        public int GetPieceCount()
        {
            int pieceCount = 0;

            for (int y = 0; y < GameBoard.SquareCount; y++)
            {
                for (int x = 0; x < GameBoard.SquareCount; x++)
                {
                    if (this.Pieces[x, y].Name != GamePiece.PieceName.None)
                    {
                        pieceCount++;
                    }
                }
            }

            return pieceCount;
        }
        public void SetPlayingWhite(bool playingWhite)
        {
            this.PlayingWhite = playingWhite;
        }

        private void QuickSetup()
        {
            this.WhiteCanCastleKS = true;
            this.WhiteCanCastleQS = true;
            this.BlackCanCastleKS = true;
            this.BlackCanCastleQS = true;

            for (int column = 0; column < GameBoard.SquareCount; column++)
            {
                for (int row = 0; row < GameBoard.SquareCount; row++)
                {
                    if (this.Pieces[row, column].Name == GamePiece.PieceName.None)
                    {
                        continue;
                    }

                    if (this.Pieces[row, column].Color == GamePiece.PieceColor.White)
                    {
                        if (this.PlayingWhite)
                        {
                            this.PlayingWhite = false;
                        }
                        else
                        {
                            this.PlayingWhite = true;
                        }

                        return;
                    }
                    else if (this.Pieces[row, column].Color == GamePiece.PieceColor.Black)
                    {
                        if (this.PlayingWhite)
                        {
                            this.PlayingWhite = true;
                        }
                        else
                        {
                            this.PlayingWhite = false;
                        }

                        return;
                    }
                }
            }
        }

        public string GenerateFEN()
        {
            string fen = "";
            string next = "";
            int spaces = 0;

            for (int y = 0; y < GameBoard.SquareCount; y++)
            {

                for (int x = 0; x < GameBoard.SquareCount; x++)
                {
                    next = "";

                    if (this.Pieces[x, y].Name == GamePiece.PieceName.None)
                    {
                        spaces++;
                    }

                    // Write spaces if necessary
                    if ((this.Pieces[x, y].Name != GamePiece.PieceName.None || x == GameBoard.SquareCount - 1) && spaces > 0)
                    {
                        next += spaces.ToString();
                        spaces = 0;
                    }

                    switch (this.Pieces[x, y].Name)
                    {
                        case GamePiece.PieceName.Pawn:
                            next += "p";
                            break;
                        case GamePiece.PieceName.Knight:
                            next += "n";
                            break;
                        case GamePiece.PieceName.Bishop:
                            next += "b";
                            break;
                        case GamePiece.PieceName.Rook:
                            next += "r";
                            break;
                        case GamePiece.PieceName.Queen:
                            next += "q";
                            break;
                        case GamePiece.PieceName.King:
                            next += "k";
                            break;
                        case GamePiece.PieceName.None:
                        default:
                            break;
                    }

                    // White is upper case
                    if (this.Pieces[x, y].Color == GamePiece.PieceColor.White)
                    {
                        next = next.ToUpper();
                    }

                    fen += next;
                }

                fen += "/";
            }

            if (this.WhiteToMove)
            {
                fen += " w ";
            }
            else
            {
                fen += " b ";
            }

            if (this.WhiteCanCastleKS)
            {
                fen += "K";
            }

            if (this.WhiteCanCastleQS)
            {
                fen += "Q";
            }

            if (!this.WhiteCanCastleKS && !this.WhiteCanCastleQS)
            {
                fen += "-";
            }

            if (this.BlackCanCastleKS)
            {
                fen += "k";
            }

            if (this.BlackCanCastleQS)
            {
                fen += "q";
            }

            if (!this.BlackCanCastleKS && !this.BlackCanCastleQS)
            {
                fen += "-";
            }

            fen += " ";

            // if (this.EnPassantAvilable)
            fen += "-";

            // TODO: Maybe parse this from the screen too, should be the move #s or something?
            fen += " 0 25";

            return fen;
        }

        public void UpdateSquare(int x, int y, GamePiece.PieceName pieceName, GamePiece.PieceColor pieceColor)
        {
            // Adjust coords since we're looking at it 'upside down'
            if (!this.PlayingWhite)
            {
                y = (GameBoard.SquareCount - 1) - y;
                x = (GameBoard.SquareCount - 1) - x;
            }

            Pieces[x, y].Name = pieceName;
            Pieces[x, y].Color = pieceColor;
        }

        private void CheckCastling()
        {
            // Out of position rooks:
            if (this.Pieces[0, 0].Name != GamePiece.PieceName.Rook)
            {
                this.BlackCanCastleQS = false;
            }

            if (this.Pieces[7, 0].Name != GamePiece.PieceName.Rook)
            {
                this.BlackCanCastleKS = false;
            }

            if (this.Pieces[0, 7].Name != GamePiece.PieceName.Rook)
            {
                this.WhiteCanCastleQS = false;
            }

            if (this.Pieces[7, 7].Name != GamePiece.PieceName.Rook)
            {
                this.WhiteCanCastleKS = false;
            }

            // Out of position black king:
            if (this.Pieces[4, 0].Name != GamePiece.PieceName.King)
            {
                this.BlackCanCastleKS = false;
                this.BlackCanCastleQS = false;
            }

            // Out of position white king:
            if (this.Pieces[4, 7].Name != GamePiece.PieceName.King)
            {
                this.WhiteCanCastleKS = false;
                this.WhiteCanCastleQS = false;
            }
        }

        public override string ToString()
        {
            string boardString = string.Empty;

            // Print regularly
            if (this.WhiteToMove)
            {
                for (int Row = 0; Row < GameBoard.SquareCount; Row++)
                {
                    for (int Column = 0; Column < GameBoard.SquareCount; Column++)
                    {
                        boardString += this.GetPieceText(Row, Column) + " ";
                    }

                    // New row
                    if (Row != GameBoard.SquareCount - 1)
                    {
                        boardString += "\n";
                    }
                }
            }
            // Print board flipped
            else
            {
                for (int Row = GameBoard.SquareCount - 1; Row >= 0; Row--)
                {
                    for (int Column = GameBoard.SquareCount - 1; Column >= 0; Column--)
                    {
                        boardString += this.GetPieceText(Row, Column);
                    }

                    // New row
                    if (Row != 0)
                    {
                        boardString += "\n";
                    }
                }
            }

            return boardString;
        }

        private string GetPieceText(int row, int column)
        {
            // Place peice (or lack thereof)
            switch (this.Pieces[column, row].Name)
            {
                case GamePiece.PieceName.None:
                    if ((row % 2 == 1 && column % 2 == 0 || row % 2 == 0 && column % 2 == 1) && this.PlayingWhite ||
                       (row % 2 == 0 && column % 2 == 0 || row % 2 == 1 && column % 2 == 1) && !this.PlayingWhite)
                    {
                        return "⬜"; // ⬛
                    }
                    else
                    {
                        return "⬜";
                    }
                case GamePiece.PieceName.Pawn:
                    if ((this.Pieces[column, row].Color == GamePiece.PieceColor.Black && this.PlayingWhite) ||
                        (this.Pieces[column, row].Color == GamePiece.PieceColor.White && !this.PlayingWhite))
                    {
                        return "♟";
                    }
                    else
                    {
                        return "♙";
                    }
                case GamePiece.PieceName.Knight:
                    if ((this.Pieces[column, row].Color == GamePiece.PieceColor.Black && this.PlayingWhite) ||
                        (this.Pieces[column, row].Color == GamePiece.PieceColor.White && !this.PlayingWhite))
                    {
                        return "♞";
                    }
                    else
                    {
                        return "♘";
                    }
                case GamePiece.PieceName.Bishop:
                    if ((this.Pieces[column, row].Color == GamePiece.PieceColor.Black && this.PlayingWhite) ||
                        (this.Pieces[column, row].Color == GamePiece.PieceColor.White && !this.PlayingWhite))
                    {
                        return "♝";
                    }
                    else
                    {
                        return "♗";
                    }
                case GamePiece.PieceName.Rook:
                    if ((this.Pieces[column, row].Color == GamePiece.PieceColor.Black && this.PlayingWhite) ||
                        (this.Pieces[column, row].Color == GamePiece.PieceColor.White && !this.PlayingWhite))
                    {
                        return "♜";
                    }
                    else
                    {
                        return "♖";
                    }
                case GamePiece.PieceName.Queen:
                    if ((this.Pieces[column, row].Color == GamePiece.PieceColor.Black && this.PlayingWhite) ||
                        (this.Pieces[column, row].Color == GamePiece.PieceColor.White && !this.PlayingWhite))
                    {
                        return "♛";
                    }
                    else
                    {
                        return "♕";
                    }
                case GamePiece.PieceName.King:
                    if ((this.Pieces[column, row].Color == GamePiece.PieceColor.Black && this.PlayingWhite) ||
                        (this.Pieces[column, row].Color == GamePiece.PieceColor.White && !this.PlayingWhite))
                    {
                        return "♚";
                    }
                    else
                    {
                        return "♔";
                    }
                default:
                    throw new Exception("Unable to find text for provided piece");
            }
        }
    }
    //// End class
}
//// End namespace