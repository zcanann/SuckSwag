namespace SuckSwag.Source.GameState
{
    public class GamePiece
    {
        public enum PieceName
        {
            None,
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }

        public enum PieceColor
        {
            Empty,
            White,
            Black,
        }

        public PieceName Name { get; set; }
        public PieceColor Color { get; set; }

        public GamePiece(PieceName name, PieceColor color)
        {
            this.Name = name;
            this.Color = color;
        }
    }
    //// End class
}
//// End namespace