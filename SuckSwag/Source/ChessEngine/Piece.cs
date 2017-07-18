namespace SuckSwag.Source.ChessEngine
{

    /// <summary>
    /// Constants for different piece types.
    /// </summary>
    public class Piece
    {
        public const int EMPTY = 0;

        public const int WKING = 1;
        public const int WQUEEN = 2;
        public const int WROOK = 3;
        public const int WBISHOP = 4;
        public const int WKNIGHT = 5;
        public const int WPAWN = 6;

        public const int BKING = 7;
        public const int BQUEEN = 8;
        public const int BROOK = 9;
        public const int BBISHOP = 10;
        public const int BKNIGHT = 11;
        public const int BPAWN = 12;

        public const int nPieceTypes = 13;

        /**
         * Return true if p is a white piece, false otherwise.
         * Note that if p is EMPTY, an unspecified value is returned.
         */
        public static bool isWhite(int pType)
        {
            return pType < BKING;
        }
        public static int makeWhite(int pType)
        {
            return pType < BKING ? pType : pType - (BKING - WKING);
        }
        public static int makeBlack(int pType)
        {
            return ((pType > EMPTY) && (pType < BKING)) ? pType + (BKING - WKING) : pType;
        }
    }
    //// End class
}
//// End namespace