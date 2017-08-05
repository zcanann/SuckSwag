namespace SuckSwag.Source.ChessEngine
{
    public class History
    {
        private int[][] countSuccess;
        private int[][] countFail;
        private int[][] score;

        public History()
        {
            countSuccess = new int[Piece.nPieceTypes][];
            countFail = new int[Piece.nPieceTypes][];
            score = new int[Piece.nPieceTypes][];

            for (int p = 0; p < Piece.nPieceTypes; p++)
            {

                countSuccess[p] = new int[64];
                countFail[p] = new int[64];
                score[p] = new int[64];

                for (int sq = 0; sq < 64; sq++)
                {
                    countSuccess[p][sq] = 0;
                    countFail[p][sq] = 0;
                    score[p][sq] = -1;
                }
            }
        }

        /** Record move as a success. */
        public void addSuccess(Position pos, Move m, int depth)
        {
            int p = pos.getPiece(m.from);
            int cnt = depth;
            int val = countSuccess[p][m.to] + cnt;
            if (val > 1000)
            {
                val /= 2;
                countFail[p][m.to] /= 2;
            }
            countSuccess[p][m.to] = val;
            score[p][m.to] = -1;
        }

        /** Record move as a failure. */
        public void addFail(Position pos, Move m, int depth)
        {
            int p = pos.getPiece(m.from);
            int cnt = depth;
            countFail[p][m.to] += cnt;
            score[p][m.to] = -1;
        }

        /** Get a score between 0 and 49, depending of the success/fail ratio of the move. */
        public int getHistScore(Position pos, Move m)
        {
            int p = pos.getPiece(m.from);
            int ret = score[p][m.to];
            if (ret >= 0)
                return ret;
            int succ = countSuccess[p][m.to];
            int fail = countFail[p][m.to];
            if (succ + fail > 0)
            {
                ret = succ * 49 / (succ + fail);
            }
            else
            {
                ret = 0;
            }
            score[p][m.to] = ret;
            return ret;
        }
    }
    //// End class
}
//// End namespace