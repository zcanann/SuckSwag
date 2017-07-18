namespace SuckSwag.Source.ChessEngine
{
    /// <summary>
    /// Contains enough information to undo a previous move. Set by makeMove(). Used by unMakeMove().
    /// </summary>
    public class UndoInfo
    {
        public int capturedPiece;
        public int castleMask;
        public int epSquare;
        public int halfMoveClock;
    }
    //// End class
}
//// End namespace