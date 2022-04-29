namespace SuckSwag.Source
{
    using Squalr.Source.ActionScheduler;
    using SuckSwag.Source.BoardFinder;
    using SuckSwag.Source.PieceFinder;
    using System.Drawing;

    /// <summary>
    /// Analyzes the game state to produce a best move.
    /// </summary>
    internal class ParseTask : ScheduledTask
    {
        public ParseTask() : base("Parsing", isRepeated: true, trackProgress: false)
        {
        }

        protected override void OnUpdate()
        {
            Bitmap board = PieceFinderViewModel.GetInstance().FindPieces(EngineViewModel.GetInstance().GameBoard);
            EngineViewModel.GetInstance().UpdateBoard(board);
        }
    }
    //// End class
}
//// End namespace