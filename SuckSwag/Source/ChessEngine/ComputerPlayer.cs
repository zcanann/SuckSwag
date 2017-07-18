using System;
using System.Collections.Generic;

namespace SuckSwag
{

    /**
     * A computer algorithm player.
     * @author petero
     */
    public class ComputerPlayer : Player
    {
        public static string engineName = "CuckooChess 1.11";

        public int minTimeMillis;
        public int maxTimeMillis;
        public int maxDepth;
        public int maxNodes;
        public bool verbose;
        TranspositionTable tt;
        Book book;
        public bool bookEnabled;
        bool randomMode;
        public Search currentSearch;
        public Move bestmv;

        public ComputerPlayer()
        {
            minTimeMillis = 10000;
            maxTimeMillis = 10000;
            maxDepth = 100;
            maxNodes = -1;
            verbose = true;
            setTTLogSize(15);
            book = new Book(verbose);
            bookEnabled = true;
            randomMode = false;
        }

        public void setTTLogSize(int logSize)
        {
            tt = new TranspositionTable(logSize);
        }

        //@Override
        public bool isHumanPlayer()
        {
            return false;
        }

        Search.Listener listener;
        public void setListener(Search.Listener listener)
        {
            this.listener = listener;
        }

        public string getCommand(Position pos, bool drawOffer, List<Position> history)
        {
            // Create a search object
            ulong[] posHashList = new ulong[200 + history.Count];
            int posHashListSize = 0;
            for (int i = 0; i < history.Count; i++)
            {
                Position p = history[i];
                posHashList[posHashListSize++] = p.zobristHash();
            }
            tt.nextGeneration();
            Search sc = new Search(pos, posHashList, posHashListSize, tt);

            // Determine all legal moves
            MoveGen.MoveList moves = new MoveGen().pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            sc.scoreMoveList(moves, 0);

            // Test for "game over"
            if (moves.size == 0)
            {
                // Switch sides so that the human can decide what to do next.
                return "swap";
            }

            if (bookEnabled)
            {
                Move bookMove = book.getBookMove(pos);
                if (bookMove != null)
                {
                    SystemHelper.println("Book moves: " + book.getAllBookMoves(pos));
                    return TextIO.moveTostring(pos, bookMove, true);
                }
            }

            // Find best move using iterative deepening
            currentSearch = sc;
            sc.setListener(listener);
            Move bestM;
            if ((moves.size == 1) && (canClaimDraw(pos, posHashList, posHashListSize, moves.m[0]) == ""))
            {
                bestM = moves.m[0];
                bestM.score = 0;
            }
            else if (randomMode)
            {
                bestM = findSemiRandomMove(sc, moves);
            }
            else
            {
                sc.timeLimit(minTimeMillis, maxTimeMillis);
                bestM = sc.iterativeDeepening(moves, maxDepth, maxNodes, verbose);
            }
            currentSearch = null;
            //        tt.printStats();
            string strMove = TextIO.moveTostring(pos, bestM, true);
            bestmv = bestM;

            // Claim draw if appropriate
            if (bestM.score <= 0)
            {
                string drawClaim = canClaimDraw(pos, posHashList, posHashListSize, bestM);
                if (drawClaim != "")
                    strMove = drawClaim;
            }
            return strMove;
        }

        /** Check if a draw claim is allowed, possibly after playing "move".
         * @param move The move that may have to be made before claiming draw.
         * @return The draw string that claims the draw, or empty string if draw claim not valid.
         */
        private string canClaimDraw(Position pos, ulong[] posHashList, int posHashListSize, Move move)
        {
            string drawStr = "";
            if (Search.canClaimDraw50(pos))
            {
                drawStr = "draw 50";
            }
            else if (Search.canClaimDrawRep(pos, posHashList, posHashListSize, posHashListSize))
            {
                drawStr = "draw rep";
            }
            else
            {
                string strMove = TextIO.moveTostring(pos, move, false);
                posHashList[posHashListSize++] = pos.zobristHash();
                UndoInfo ui = new UndoInfo();
                pos.makeMove(move, ui);
                if (Search.canClaimDraw50(pos))
                {
                    drawStr = "draw 50 " + strMove;
                }
                else if (Search.canClaimDrawRep(pos, posHashList, posHashListSize, posHashListSize))
                {
                    drawStr = "draw rep " + strMove;
                }
                pos.unMakeMove(move, ui);
            }
            return drawStr;
        }

        public void useBook(bool bookOn)
        {
            bookEnabled = bookOn;
        }

        public void timeLimit(int minTimeLimit, int maxTimeLimit, bool randomMode)
        {
            if (randomMode)
            {
                minTimeLimit = 0;
                maxTimeLimit = 0;
            }
            minTimeMillis = minTimeLimit;
            maxTimeMillis = maxTimeLimit;
            this.randomMode = randomMode;
            if (currentSearch != null)
            {
                currentSearch.timeLimit(minTimeLimit, maxTimeLimit);
            }
        }

        public void clearTT()
        {
            tt.clear();
        }

        /** Search a position and return the best move and score. Used for test suite processing. */
        public TwoReturnValues<Move, string> searchPosition(Position pos, int maxTimeMillis)
        {
            // Create a search object
            ulong[] posHashList = new ulong[200];
            tt.nextGeneration();
            Search sc = new Search(pos, posHashList, 0, tt);

            // Determine all legal moves
            MoveGen.MoveList moves = new MoveGen().pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            sc.scoreMoveList(moves, 0);

            // Find best move using iterative deepening
            sc.timeLimit(maxTimeMillis, maxTimeMillis);
            Move bestM = sc.iterativeDeepening(moves, -1, -1, false);

            // Extract PV
            string PV = TextIO.moveTostring(pos, bestM, false) + " ";
            UndoInfo ui = new UndoInfo();
            pos.makeMove(bestM, ui);
            PV += tt.extractPV(pos);
            pos.unMakeMove(bestM, ui);

            //        tt.printStats();

            // Return best move and PV
            return new TwoReturnValues<Move, string>(bestM, PV);
        }

        private Move findSemiRandomMove(Search sc, MoveGen.MoveList moves)
        {
            sc.timeLimit(minTimeMillis, maxTimeMillis);
            Move bestM = sc.iterativeDeepening(moves, 1, maxNodes, verbose);
            int bestScore = bestM.score;

            long t0 = SystemHelper.currentTimeMillis();
            Random rndGen = new Random((int)t0);

            int sum = 0;
            for (int mi = 0; mi < moves.size; mi++)
            {
                sum += moveProbWeight(moves.m[mi].score, bestScore);
            }
            int rnd = rndGen.Next(sum);
            for (int mi = 0; mi < moves.size; mi++)
            {
                int weight = moveProbWeight(moves.m[mi].score, bestScore);
                if (rnd < weight)
                {
                    return moves.m[mi];
                }
                rnd -= weight;
            }
            SystemHelper.println("Assert error. Should never get here!");
            return null;
        }

        private static int moveProbWeight(int moveScore, int bestScore)
        {
            double d = (bestScore - moveScore) / 100.0;
            double w = 100 * Math.Exp(-d * d / 2);
            return (int)Math.Ceiling(w);
        }

        // FIXME: Test Botvinnik-Markoff extension
    }
    //// End class
}
//// End namespace