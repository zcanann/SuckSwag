using System;

namespace SuckSwag.Source.ChessEngine
{
    public class Evaluate
    {

        public Evaluate()
        {
            Co1(); Co2(); Co3(); Co4(); Co5(); Co6();
        }

        public static int pV = 100;
        public static int nV = 400;
        public static int bV = 400;
        public static int rV = 600;
        public static int qV = 1200;
        public static int kV = 9900;

        public static int[] pieceValue;
        private static void Co1()
        {
            // Initialize material table
            pieceValue = new int[Piece.nPieceTypes];
            pieceValue[Piece.WKING] = kV;
            pieceValue[Piece.WQUEEN] = qV;
            pieceValue[Piece.WROOK] = rV;
            pieceValue[Piece.WBISHOP] = bV;
            pieceValue[Piece.WKNIGHT] = nV;
            pieceValue[Piece.WPAWN] = pV;
            pieceValue[Piece.BKING] = kV;
            pieceValue[Piece.BQUEEN] = qV;
            pieceValue[Piece.BROOK] = rV;
            pieceValue[Piece.BBISHOP] = bV;
            pieceValue[Piece.BKNIGHT] = nV;
            pieceValue[Piece.BPAWN] = pV;
            pieceValue[Piece.EMPTY] = 0;
        }

        /** Piece/square table for king during middle game. */
        static int[] kt1b = { -22,-35,-40,-40,-40,-40,-35,-22,
                                -22,-35,-40,-40,-40,-40,-35,-22,
                                -25,-35,-40,-45,-45,-40,-35,-25,
                                -15,-30,-35,-40,-40,-35,-30,-15,
                                -10,-15,-20,-25,-25,-20,-15,-10,
                                  4, -2, -5,-15,-15, -5, -2,  4,
                                 16, 14,  7, -3, -3,  7, 14, 16,
                                 24, 24,  9,  0,  0,  9, 24, 24 };

        /** Piece/square table for king during end game. */
        static int[] kt2b = {  0,  8, 16, 24, 24, 16,  8,  0,
                                 8, 16, 24, 32, 32, 24, 16,  8,
                                16, 24, 32, 40, 40, 32, 24, 16,
                                24, 32, 40, 48, 48, 40, 32, 24,
                                24, 32, 40, 48, 48, 40, 32, 24,
                                16, 24, 32, 40, 40, 32, 24, 16,
                                 8, 16, 24, 32, 32, 24, 16,  8,
                                 0,  8, 16, 24, 24, 16,  8,  0 };

        /** Piece/square table for pawns during middle game. */
        static int[] pt1b = {  0,  0,  0,  0,  0,  0,  0,  0,
                                 8, 16, 24, 32, 32, 24, 16,  8,
                                 3, 12, 20, 28, 28, 20, 12,  3,
                                -5,  4, 10, 20, 20, 10,  4, -5,
                                -6,  4,  5, 16, 16,  5,  4, -6,
                                -6,  4,  2,  5,  5,  2,  4, -6,
                                -6,  4,  4,-15,-15,  4,  4, -6,
                                 0,  0,  0,  0,  0,  0,  0,  0 };

        /** Piece/square table for pawns during end game. */
        static int[] pt2b = {   0,  0,  0,  0,  0,  0,  0,  0,
                                 25, 40, 45, 45, 45, 45, 40, 25,
                                 17, 32, 35, 35, 35, 35, 32, 17,
                                  5, 24, 24, 24, 24, 24, 24,  5,
                                 -9, 11, 11, 11, 11, 11, 11, -9,
                                -17,  3,  3,  3,  3,  3,  3,-17,
                                -20,  0,  0,  0,  0,  0,  0,-20,
                                  0,  0,  0,  0,  0,  0,  0,  0 };

        /** Piece/square table for knights during middle game. */
        static int[] nt1b = { -53,-42,-32,-21,-21,-32,-42,-53,
                                -42,-32,-10,  0,  0,-10,-32,-42,
                                -21,  5, 10, 16, 16, 10,  5,-21,
                                -18,  0, 10, 21, 21, 10,  0,-18,
                                -18,  0,  3, 21, 21,  3,  0,-18,
                                -21,-10,  0,  0,  0,  0,-10,-21,
                                -42,-32,-10,  0,  0,-10,-32,-42,
                                -53,-42,-32,-21,-21,-32,-42,-53 };

        /** Piece/square table for knights during end game. */
        static int[] nt2b = { -56,-44,-34,-22,-22,-34,-44,-56,
                                -44,-34,-10,  0,  0,-10,-34,-44,
                                -22,  5, 10, 17, 17, 10,  5,-22,
                                -19,  0, 10, 22, 22, 10,  0,-19,
                                -19,  0,  3, 22, 22,  3,  0,-19,
                                -22,-10,  0,  0,  0,  0,-10,-22,
                                -44,-34,-10,  0,  0,-10,-34,-44,
                                -56,-44,-34,-22,-22,-34,-44,-56 };

        /** Piece/square table for bishops during middle game. */
        static int[] bt1b = {  0,  0,  0,  0,  0,  0,  0,  0,
                                 0,  4,  2,  2,  2,  2,  4,  0,
                                 0,  2,  4,  4,  4,  4,  2,  0,
                                 0,  2,  4,  4,  4,  4,  2,  0,
                                 0,  2,  4,  4,  4,  4,  2,  0,
                                 0,  3,  4,  4,  4,  4,  3,  0,
                                 0,  4,  2,  2,  2,  2,  4,  0,
                                 0,  0, -2,  0,  0, -2,  0,  0 };

        /** Piece/square table for queens during middle game. */
        static int[] qt1b = { -10, -5,  0,  0,  0,  0, -5,-10,
                                 -5,  0,  5,  5,  5,  5,  0, -5,
                                  0,  5,  5,  6,  6,  5,  5,  0,
                                  0,  5,  6,  6,  6,  6,  5,  0,
                                  0,  5,  6,  6,  6,  6,  5,  0,
                                  0,  5,  5,  6,  6,  5,  5,  0,
                                 -5,  0,  5,  5,  5,  5,  0, -5,
                                -10, -5,  0,  0,  0,  0, -5,-10 };

        /** Piece/square table for rooks during middle game. */
        static int[] rt1b = {  0,  3,  5,  5,  5,  5,  3,  0,
                                15, 20, 20, 20, 20, 20, 20, 15,
                                 0,  0,  0,  0,  0,  0,  0,  0,
                                 0,  0,  0,  0,  0,  0,  0,  0,
                                -2,  0,  0,  0,  0,  0,  0, -2,
                                -2,  0,  0,  2,  2,  0,  0, -2,
                                -3,  2,  5,  5,  5,  5,  2, -3,
                                 0,  3,  5,  5,  5,  5,  3,  0 };

        static int[] kt1w, qt1w, rt1w, bt1w, nt1w, pt1w, kt2w, nt2w, pt2w;
        private static void Co2()
        {
            kt1w = new int[64];
            qt1w = new int[64];
            rt1w = new int[64];
            bt1w = new int[64];
            nt1w = new int[64];
            pt1w = new int[64];
            kt2w = new int[64];
            nt2w = new int[64];
            pt2w = new int[64];
            for (int i = 0; i < 64; i++)
            {
                kt1w[i] = kt1b[63 - i];
                qt1w[i] = qt1b[63 - i];
                rt1w[i] = rt1b[63 - i];
                bt1w[i] = bt1b[63 - i];
                nt1w[i] = nt1b[63 - i];
                pt1w[i] = pt1b[63 - i];
                kt2w[i] = kt2b[63 - i];
                nt2w[i] = nt2b[63 - i];
                pt2w[i] = pt2b[63 - i];
            }
        }

        private static int[] empty = new int[64];   /* already 0 */
        public static int[][] psTab1, psTab2;

        static int[][] distToH1A8 = { new int[] { 0, 1, 2, 3, 4, 5, 6, 7 },
                                        new int[] { 1, 2, 3, 4, 5, 6, 7, 6 },
                                        new int[] { 2, 3, 4, 5, 6, 7, 6, 5 },
                                        new int[] { 3, 4, 5, 6, 7, 6, 5, 4 },
                                        new int[] { 4, 5, 6, 7, 6, 5, 4, 3 },
                                        new int[] { 5, 6, 7, 6, 5, 4, 3, 2 },
                                        new int[] { 6, 7, 6, 5, 4, 3, 2, 1 },
                                        new int[] { 7, 6, 5, 4, 3, 2, 1, 0 } };

        static int[] rookMobScore = { -10, -7, -4, -1, 2, 5, 7, 9, 11, 12, 13, 14, 14, 14, 14 };
        static int[] bishMobScore = { -15, -10, -6, -2, 2, 6, 10, 13, 16, 18, 20, 22, 23, 24 };
        static int[] queenMobScore = { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };

        private class PawnHashData
        {
            public ulong key;
            public int score;         // Positive score means good for white
            public short passedBonusW;
            public short passedBonusB;
            public ulong passedPawnsW;     // The most advanced passed pawns for each file
            public ulong passedPawnsB;
        }
        private static PawnHashData[] pawnHash;
        private static void Co3()
        {
            int numEntries = 1 << 16;
            pawnHash = new PawnHashData[numEntries];
            for (int i = 0; i < numEntries; i++)
            {
                PawnHashData phd = new PawnHashData();
                phd.key = Defs.ulongN1 /*-1*/; // Non-zero to avoid collision for positions with no pawns
                phd.score = 0;
                pawnHash[i] = phd;
            }
        }

        public static byte[] kpkTable = null;

        // King safety variables
        private ulong wKingZone, bKingZone;       // Squares close to king that are worth attacking
        private int wKingAttacks, bKingAttacks; // Number of attacks close to white/black king
        private ulong wAttacksBB, bAttacksBB;

        /** Constructor. */
        private void Co6()
        {
            if (kpkTable == null)
            {
                /* table contains file /kpk.bitbase now */
                kpkTable = KPKbitbase.DATA;     /*new byte[2 * 32 * 64 * 48 / 8] */

                /* multi-array elements  definition */
                psTab1 = new[] { empty, kt1w, qt1w, rt1w, bt1w, nt1w, pt1w,
                                           kt1b, qt1b, rt1b, bt1b, nt1b, pt1b };
                psTab2 = new[] { empty, kt2w, qt1w, rt1w, bt1w, nt2w, pt2w,
                                           kt2b, qt1b, rt1b, bt1b, nt2b, pt2b };
            }
        }

        /**
         * Static evaluation of a position.
         * @param pos The position to evaluate.
         * @return The evaluation score, measured in centipawns.
         *         Positive values are good for the side to make the next move.
         */
        public int evalPos(Position pos)
        {
            int score = pos.wMtrl - pos.bMtrl;

            wKingAttacks = bKingAttacks = 0;
            wKingZone = BitBoard.kingAttacks[pos.getKingSq(true)]; wKingZone |= wKingZone << 8;
            bKingZone = BitBoard.kingAttacks[pos.getKingSq(false)]; bKingZone |= bKingZone >> 8;
            wAttacksBB = bAttacksBB = 0L;

            score += pieceSquareEval(pos);
            score += pawnBonus(pos);
            score += tradeBonus(pos);
            score += castleBonus(pos);

            score += rookBonus(pos);
            score += bishopEval(pos, score);
            score += threatBonus(pos);
            score += kingSafety(pos);
            score = endGameEval(pos, score);

            if (!pos.whiteMove)
                score = -score;
            return score;

            // FIXME! Test penalty if side to move has >1 hanging piece

            // FIXME! Test "tempo value"
        }

        /** Compute white_material - black_material. */
        static int material(Position pos)
        {
            return pos.wMtrl - pos.bMtrl;
        }

        /** Compute score based on piece square tables. Positive values are good for white. */
        private int pieceSquareEval(Position pos)
        {
            int score = 0;
            int wMtrl = pos.wMtrl;
            int bMtrl = pos.bMtrl;
            int wMtrlPawns = pos.wMtrlPawns;
            int bMtrlPawns = pos.bMtrlPawns;

            // Kings
            {
                int t1 = qV + 2 * rV + 2 * bV;
                int t2 = rV;
                {
                    int k1 = pos.psScore1[Piece.WKING];
                    int k2 = pos.psScore2[Piece.WKING];
                    int t = bMtrl - bMtrlPawns;
                    score += interpolate(t, t2, k2, t1, k1);
                }
                {
                    int k1 = pos.psScore1[Piece.BKING];
                    int k2 = pos.psScore2[Piece.BKING];
                    int t = wMtrl - wMtrlPawns;
                    score -= interpolate(t, t2, k2, t1, k1);
                }
            }

            // Pawns
            {
                int t1 = qV + 2 * rV + 2 * bV;
                int t2 = rV;
                int wp1 = pos.psScore1[Piece.WPAWN];
                int wp2 = pos.psScore2[Piece.WPAWN];
                if ((wp1 != 0) || (wp2 != 0))
                {
                    int tw = bMtrl - bMtrlPawns;
                    score += interpolate(tw, t2, wp2, t1, wp1);
                }
                int bp1 = pos.psScore1[Piece.BPAWN];
                int bp2 = pos.psScore2[Piece.BPAWN];
                if ((bp1 != 0) || (bp2 != 0))
                {
                    int tb = wMtrl - wMtrlPawns;
                    score -= interpolate(tb, t2, bp2, t1, bp1);
                }
            }

            // Knights
            {
                int t1 = qV + 2 * rV + 1 * bV + 1 * nV + 6 * pV;
                int t2 = nV + 8 * pV;
                int n1 = pos.psScore1[Piece.WKNIGHT];
                int n2 = pos.psScore2[Piece.WKNIGHT];
                if ((n1 != 0) || (n2 != 0))
                {
                    score += interpolate(bMtrl, t2, n2, t1, n1);
                }
                n1 = pos.psScore1[Piece.BKNIGHT];
                n2 = pos.psScore2[Piece.BKNIGHT];
                if ((n1 != 0) || (n2 != 0))
                {
                    score -= interpolate(wMtrl, t2, n2, t1, n1);
                }
            }

            // Bishops
            {
                score += pos.psScore1[Piece.WBISHOP];
                score -= pos.psScore1[Piece.BBISHOP];
            }

            // Queens
            {
                ulong occupied = pos.whiteBB | pos.blackBB;
                score += pos.psScore1[Piece.WQUEEN];
                ulong m = pos.pieceTypeBB[Piece.WQUEEN];
                while (m != 0)
                {
                    int sq = BitBoard.numberOfTrailingZeros(m);
                    ulong atk = BitBoard.rookAttacks(sq, occupied) | BitBoard.bishopAttacks(sq, occupied);
                    wAttacksBB |= atk;
                    score += queenMobScore[BITS.bitCount(atk & ~pos.whiteBB)];
                    bKingAttacks += BITS.bitCount(atk & bKingZone) * 2;
                    m &= m - 1;
                }
                score -= pos.psScore1[Piece.BQUEEN];
                m = pos.pieceTypeBB[Piece.BQUEEN];
                while (m != 0)
                {
                    int sq = BitBoard.numberOfTrailingZeros(m);
                    ulong atk = BitBoard.rookAttacks(sq, occupied) | BitBoard.bishopAttacks(sq, occupied);
                    bAttacksBB |= atk;
                    score -= queenMobScore[BITS.bitCount(atk & ~pos.blackBB)];
                    wKingAttacks += BITS.bitCount(atk & wKingZone) * 2;
                    m &= m - 1;
                }
            }

            // Rooks
            {
                int r1 = pos.psScore1[Piece.WROOK];
                if (r1 != 0)
                {
                    int nP = bMtrlPawns / pV;
                    int s = r1 * Math.Min(nP, 6) / 6;
                    score += s;
                }
                r1 = pos.psScore1[Piece.BROOK];
                if (r1 != 0)
                {
                    int nP = wMtrlPawns / pV;
                    int s = r1 * Math.Min(nP, 6) / 6;
                    score -= s;
                }
            }

            return score;
        }

        /** Implement the "when ahead trade pieces, when behind trade pawns" rule. */
        private int tradeBonus(Position pos)
        {
            int wM = pos.wMtrl;
            int bM = pos.bMtrl;
            int wPawn = pos.wMtrlPawns;
            int bPawn = pos.bMtrlPawns;
            int deltaScore = wM - bM;

            int pBonus = 0;
            pBonus += interpolate((deltaScore > 0) ? wPawn : bPawn, 0, -30 * deltaScore / 100, 6 * pV, 0);
            pBonus += interpolate((deltaScore > 0) ? bM : wM, 0, 30 * deltaScore / 100, qV + 2 * rV + 2 * bV + 2 * nV, 0);

            return pBonus;
        }

        private static int[] castleFactor;
        private static void Co4()
        {
            castleFactor = new int[256];
            for (int i = 0; i < 256; i++)
            {
                int h1Dist = 100;
                bool h1Castle = (i & (1 << 7)) != 0;
                if (h1Castle)
                    h1Dist = 2 + BITS.bitCount((ulong)i & 0x0000000000000060L); // f1,g1
                int a1Dist = 100;
                bool a1Castle = (i & 1) != 0;
                if (a1Castle)
                    a1Dist = 2 + BITS.bitCount((ulong)i & 0x000000000000000EL); // b1,c1,d1
                castleFactor[i] = 1024 / Math.Min(a1Dist, h1Dist);
            }
        }

        /** Score castling ability. */
        private int castleBonus(Position pos)
        {
            if (pos.getCastleMask() == 0) return 0;

            int k1 = kt1b[7 * 8 + 6] - kt1b[7 * 8 + 4];
            int k2 = kt2b[7 * 8 + 6] - kt2b[7 * 8 + 4];
            int t1 = qV + 2 * rV + 2 * bV;
            int t2 = rV;
            int t = pos.bMtrl - pos.bMtrlPawns;
            int ks = interpolate(t, t2, k2, t1, k1);

            int castleValue = ks + rt1b[7 * 8 + 5] - rt1b[7 * 8 + 7];
            if (castleValue <= 0)
                return 0;

            ulong occupied = pos.whiteBB | pos.blackBB;
            int tmp = (int)(occupied & 0x6E);
            if (pos.a1Castle()) tmp |= 1;
            if (pos.h1Castle()) tmp |= (1 << 7);
            int wBonus = (castleValue * castleFactor[tmp]) >> 10;

            tmp = (int)((occupied >> 56) & 0x6E);
            if (pos.a8Castle()) tmp |= 1;
            if (pos.h8Castle()) tmp |= (1 << 7);
            int bBonus = (castleValue * castleFactor[tmp]) >> 10;

            return wBonus - bBonus;
        }

        private int pawnBonus(Position pos)
        {
            ulong key = pos.pawnZobristHash();
            PawnHashData phd = pawnHash[(int)key & (pawnHash.Length - 1)];
            if (phd.key != key)
                computePawnHashData(pos, phd);
            int score = phd.score;

            int hiMtrl = qV + rV;
            score += interpolate(pos.bMtrl - pos.bMtrlPawns, 0, 2 * phd.passedBonusW, hiMtrl, phd.passedBonusW);
            score -= interpolate(pos.wMtrl - pos.wMtrlPawns, 0, 2 * phd.passedBonusB, hiMtrl, phd.passedBonusB);

            // Passed pawns are more dangerous if enemy king is far away
            int mtrlNoPawns;
            int highMtrl = qV + rV;
            ulong m = phd.passedPawnsW;
            if (m != 0)
            {
                mtrlNoPawns = pos.bMtrl - pos.bMtrlPawns;
                if (mtrlNoPawns < highMtrl)
                {
                    int kingPos = pos.getKingSq(false);
                    int kingX = Position.getX(kingPos);
                    int kingY = Position.getY(kingPos);
                    while (m != 0)
                    {
                        int sq = BitBoard.numberOfTrailingZeros(m);
                        int x = Position.getX(sq);
                        int y = Position.getY(sq);
                        int pawnDist = Math.Min(5, 7 - y);
                        int kingDistX = Math.Abs(kingX - x);
                        int kingDistY = Math.Abs(kingY - 7);
                        int kingDist = Math.Max(kingDistX, kingDistY);
                        int kScore = kingDist * 4;
                        if (kingDist > pawnDist) kScore += (kingDist - pawnDist) * (kingDist - pawnDist);
                        score += interpolate(mtrlNoPawns, 0, kScore, highMtrl, 0);
                        if (!pos.whiteMove)
                            kingDist--;
                        if ((pawnDist < kingDist) && (mtrlNoPawns == 0))
                            score += 500; // King can't stop pawn
                        m &= m - 1;
                    }
                }
            }
            m = phd.passedPawnsB;
            if (m != 0)
            {
                mtrlNoPawns = pos.wMtrl - pos.wMtrlPawns;
                if (mtrlNoPawns < highMtrl)
                {
                    int kingPos = pos.getKingSq(true);
                    int kingX = Position.getX(kingPos);
                    int kingY = Position.getY(kingPos);
                    while (m != 0)
                    {
                        int sq = BitBoard.numberOfTrailingZeros(m);
                        int x = Position.getX(sq);
                        int y = Position.getY(sq);
                        int pawnDist = Math.Min(5, y);
                        int kingDistX = Math.Abs(kingX - x);
                        int kingDistY = Math.Abs(kingY - 0);
                        int kingDist = Math.Max(kingDistX, kingDistY);
                        int kScore = kingDist * 4;
                        if (kingDist > pawnDist) kScore += (kingDist - pawnDist) * (kingDist - pawnDist);
                        score -= interpolate(mtrlNoPawns, 0, kScore, highMtrl, 0);
                        if (pos.whiteMove)
                            kingDist--;
                        if ((pawnDist < kingDist) && (mtrlNoPawns == 0))
                            score -= 500; // King can't stop pawn
                        m &= m - 1;
                    }
                }
            }

            return score;
        }

        /** Compute pawn hash data for pos. */
        private void computePawnHashData(Position pos, PawnHashData ph)
        {
            int score = 0;

            // Evaluate double pawns and pawn islands
            ulong wPawns = pos.pieceTypeBB[Piece.WPAWN];
            ulong wPawnFiles = BitBoard.southFill(wPawns) & 0xff;
            int wDouble = BITS.bitCount(wPawns) - BITS.bitCount(wPawnFiles);
            int wIslands = BITS.bitCount(((~wPawnFiles) >> 1) & wPawnFiles);
            int wIsolated = BITS.bitCount(~(wPawnFiles << 1) & wPawnFiles & ~(wPawnFiles >> 1));


            ulong bPawns = pos.pieceTypeBB[Piece.BPAWN];
            ulong bPawnFiles = BitBoard.southFill(bPawns) & 0xff;
            int bDouble = BITS.bitCount(bPawns) - BITS.bitCount(bPawnFiles);
            int bIslands = BITS.bitCount(((~bPawnFiles) >> 1) & bPawnFiles);
            int bIsolated = BITS.bitCount(~(bPawnFiles << 1) & bPawnFiles & ~(bPawnFiles >> 1));

            score -= (wDouble - bDouble) * 25;
            score -= (wIslands - bIslands) * 15;
            score -= (wIsolated - bIsolated) * 15;

            // Evaluate backward pawns, defined as a pawn that guards a friendly pawn,
            // can't be guarded by friendly pawns, can advance, but can't advance without 
            // being captured by an enemy pawn.
            ulong wPawnAttacks = (((wPawns & BitBoard.maskBToHFiles) << 7) |
                                 ((wPawns & BitBoard.maskAToGFiles) << 9));
            ulong bPawnAttacks = (((bPawns & BitBoard.maskBToHFiles) >> 9) |
                                 ((bPawns & BitBoard.maskAToGFiles) >> 7));
            ulong wBackward = wPawns & ~((wPawns | bPawns) >> 8) & (bPawnAttacks >> 8) &
                             ~BitBoard.northFill(wPawnAttacks);
            wBackward &= (((wPawns & BitBoard.maskBToHFiles) >> 9) |
                          ((wPawns & BitBoard.maskAToGFiles) >> 7));
            wBackward &= ~BitBoard.northFill(bPawnFiles);
            ulong bBackward = bPawns & ~((wPawns | bPawns) << 8) & (wPawnAttacks << 8) &
                             ~BitBoard.southFill(bPawnAttacks);
            bBackward &= (((bPawns & BitBoard.maskBToHFiles) << 7) |
                          ((bPawns & BitBoard.maskAToGFiles) << 9));
            bBackward &= ~BitBoard.northFill(wPawnFiles);
            score -= (BITS.bitCount(wBackward) - BITS.bitCount(bBackward)) * 15;

            // Evaluate passed pawn bonus, white
            ulong passedPawnsW = wPawns & ~BitBoard.southFill(bPawns | bPawnAttacks | (wPawns >> 8));
            int[] ppBonus = { -1, 24, 26, 30, 36, 47, 64, -1 };
            int passedBonusW = 0;
            if (passedPawnsW != 0)
            {
                ulong guardedPassedW = passedPawnsW & (((wPawns & BitBoard.maskBToHFiles) << 7) |
                                                      ((wPawns & BitBoard.maskAToGFiles) << 9));
                passedBonusW += 15 * BITS.bitCount(guardedPassedW);
                ulong m = passedPawnsW;
                while (m != 0)
                {
                    int sq = /*long.numberOfTrailingZeros(m) */  BitBoard.numberOfTrailingZeros(m);
                    int y = Position.getY(sq);
                    passedBonusW += ppBonus[y];
                    m &= m - 1;
                }
            }

            // Evaluate passed pawn bonus, black
            ulong passedPawnsB = bPawns & ~BitBoard.northFill(wPawns | wPawnAttacks | (bPawns << 8));
            int passedBonusB = 0;
            if (passedPawnsB != 0)
            {
                ulong guardedPassedB = passedPawnsB & (((bPawns & BitBoard.maskBToHFiles) >> 9) |
                                                      ((bPawns & BitBoard.maskAToGFiles) >> 7));
                passedBonusB += 15 * BITS.bitCount(guardedPassedB);
                ulong m = passedPawnsB;
                while (m != 0)
                {
                    int sq = /*long.numberOfTrailingZeros(m) */  BitBoard.numberOfTrailingZeros(m);
                    int y = Position.getY(sq);
                    passedBonusB += ppBonus[7 - y];
                    m &= m - 1;
                }
            }

            // Connected passed pawn bonus. Seems logical but doesn't help in tests
            //        if (passedPawnsW != 0)
            //            passedBonusW += 15 * BITS.bitCount(passedPawnsW & ((passedPawnsW & BitBoard.maskBToHFiles) >> 1));
            //        if (passedPawnsB != 0)
            //            passedBonusB += 15 * BITS.bitCount(passedPawnsB & ((passedPawnsB & BitBoard.maskBToHFiles) >> 1));

            ph.key = pos.pawnZobristHash();
            ph.score = score;
            ph.passedBonusW = (short)passedBonusW;
            ph.passedBonusB = (short)passedBonusB;
            ph.passedPawnsW = passedPawnsW;
            ph.passedPawnsB = passedPawnsB;
        }

        /** Compute rook bonus. Rook on open/half-open file. */
        private int rookBonus(Position pos)
        {
            int score = 0;
            ulong wPawns = pos.pieceTypeBB[Piece.WPAWN];
            ulong bPawns = pos.pieceTypeBB[Piece.BPAWN];
            ulong occupied = pos.whiteBB | pos.blackBB;
            ulong m = pos.pieceTypeBB[Piece.WROOK];
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                int x = Position.getX(sq);
                if ((wPawns & BitBoard.maskFile[x]) == 0)
                { // At least half-open file
                    score += (bPawns & BitBoard.maskFile[x]) == 0 ? 25 : 12;
                }
                ulong atk = BitBoard.rookAttacks(sq, occupied);
                wAttacksBB |= atk;
                score += rookMobScore[BITS.bitCount(atk & ~pos.whiteBB)];
                if ((atk & bKingZone) != 0)
                    bKingAttacks += BITS.bitCount(atk & bKingZone);
                m &= m - 1;
            }
            ulong r7 = pos.pieceTypeBB[Piece.WROOK] & 0x00ff000000000000L;
            if (((r7 & (r7 - 1)) != 0) &&
                ((pos.pieceTypeBB[Piece.BKING] & 0xff00000000000000L) != 0))
                score += 20; // Two rooks on 7:th row
            m = pos.pieceTypeBB[Piece.BROOK];
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                int x = Position.getX(sq);
                if ((bPawns & BitBoard.maskFile[x]) == 0)
                {
                    score -= (wPawns & BitBoard.maskFile[x]) == 0 ? 25 : 12;
                }
                ulong atk = BitBoard.rookAttacks(sq, occupied);
                bAttacksBB |= atk;
                score -= rookMobScore[BITS.bitCount(atk & ~pos.blackBB)];
                if ((atk & wKingZone) != 0)
                    wKingAttacks += BITS.bitCount(atk & wKingZone);
                m &= m - 1;
            }
            r7 = pos.pieceTypeBB[Piece.BROOK] & 0xff00L;
            if (((r7 & (r7 - 1)) != 0) &&
                ((pos.pieceTypeBB[Piece.WKING] & 0xffL) != 0))
                score -= 20; // Two rooks on 2:nd row
            return score;
        }

        /** Compute bishop evaluation. */
        private int bishopEval(Position pos, int oldScore)
        {
            int score = 0;
            ulong occupied = pos.whiteBB | pos.blackBB;
            ulong wBishops = pos.pieceTypeBB[Piece.WBISHOP];
            ulong bBishops = pos.pieceTypeBB[Piece.BBISHOP];
            if ((wBishops | bBishops) == 0)
                return 0;
            ulong m = wBishops;
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                ulong atk = BitBoard.bishopAttacks(sq, occupied);
                wAttacksBB |= atk;
                score += bishMobScore[BITS.bitCount(atk & ~pos.whiteBB)];
                if ((atk & bKingZone) != 0)
                    bKingAttacks += BITS.bitCount(atk & bKingZone);
                m &= m - 1;
            }
            m = bBishops;
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                ulong atk = BitBoard.bishopAttacks(sq, occupied);
                bAttacksBB |= atk;
                score -= bishMobScore[BITS.bitCount(atk & ~pos.blackBB)];
                if ((atk & wKingZone) != 0)
                    wKingAttacks += BITS.bitCount(atk & wKingZone);
                m &= m - 1;
            }

            bool whiteDark = (pos.pieceTypeBB[Piece.WBISHOP] & BitBoard.maskDarkSq) != 0;
            bool whiteLight = (pos.pieceTypeBB[Piece.WBISHOP] & BitBoard.maskLightSq) != 0;
            bool blackDark = (pos.pieceTypeBB[Piece.BBISHOP] & BitBoard.maskDarkSq) != 0;
            bool blackLight = (pos.pieceTypeBB[Piece.BBISHOP] & BitBoard.maskLightSq) != 0;
            int numWhite = (whiteDark ? 1 : 0) + (whiteLight ? 1 : 0);
            int numBlack = (blackDark ? 1 : 0) + (blackLight ? 1 : 0);

            // Bishop pair bonus
            if (numWhite == 2)
            {
                int numPawns = pos.wMtrlPawns / pV;
                score += 20 + (8 - numPawns) * 3;
            }
            if (numBlack == 2)
            {
                int numPawns = pos.bMtrlPawns / pV;
                score -= 20 + (8 - numPawns) * 3;
            }

            // FIXME!!! Bad bishop

            if ((numWhite == 1) && (numBlack == 1) && (whiteDark != blackDark) &&
                (pos.wMtrl - pos.wMtrlPawns == pos.bMtrl - pos.bMtrlPawns))
            {
                int penalty = (oldScore + score) / 2;
                int loMtrl = 2 * bV;
                int hiMtrl = 2 * (qV + rV + bV);
                int mtrl = pos.wMtrl + pos.bMtrl - pos.wMtrlPawns - pos.bMtrlPawns;
                score -= interpolate(mtrl, loMtrl, penalty, hiMtrl, 0);
            }

            // Penalty for bishop trapped behind pawn at a2/h2/a7/h7
            if (((wBishops | bBishops) & 0x0081000000008100L) != 0)
            {
                if ((pos.squares[48] == Piece.WBISHOP) && // a7
                    (pos.squares[41] == Piece.BPAWN) &&
                    (pos.squares[50] == Piece.BPAWN))
                    score -= pV * 3 / 2;
                if ((pos.squares[55] == Piece.WBISHOP) && // h7
                    (pos.squares[46] == Piece.BPAWN) &&
                    (pos.squares[53] == Piece.BPAWN))
                    score -= (pos.pieceTypeBB[Piece.WQUEEN] != 0) ? pV : pV * 3 / 2;
                if ((pos.squares[8] == Piece.BBISHOP) &&  // a2
                    (pos.squares[17] == Piece.WPAWN) &&
                    (pos.squares[10] == Piece.WPAWN))
                    score += pV * 3 / 2;
                if ((pos.squares[15] == Piece.BBISHOP) && // h2
                    (pos.squares[22] == Piece.WPAWN) &&
                    (pos.squares[13] == Piece.WPAWN))
                    score += (pos.pieceTypeBB[Piece.BQUEEN] != 0) ? pV : pV * 3 / 2;
            }

            return score;
        }

        private int threatBonus(Position pos)
        {
            int score = 0;

            // Sum values for all black pieces under attack
            ulong m = pos.pieceTypeBB[Piece.WKNIGHT];
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                wAttacksBB |= BitBoard.knightAttacks[sq];
                m &= m - 1;
            }
            wAttacksBB &= (pos.pieceTypeBB[Piece.BKNIGHT] |
                           pos.pieceTypeBB[Piece.BBISHOP] |
                           pos.pieceTypeBB[Piece.BROOK] |
                           pos.pieceTypeBB[Piece.BQUEEN]);
            ulong pawns = pos.pieceTypeBB[Piece.WPAWN];
            wAttacksBB |= (pawns & BitBoard.maskBToHFiles) << 7;
            wAttacksBB |= (pawns & BitBoard.maskAToGFiles) << 9;
            m = wAttacksBB & pos.blackBB & ~pos.pieceTypeBB[Piece.BKING];
            int tmp = 0;
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                tmp += pieceValue[pos.squares[sq]];
                m &= m - 1;
            }
            score += tmp + tmp * tmp / qV;

            // Sum values for all white pieces under attack
            m = pos.pieceTypeBB[Piece.BKNIGHT];
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                bAttacksBB |= BitBoard.knightAttacks[sq];
                m &= m - 1;
            }
            bAttacksBB &= (pos.pieceTypeBB[Piece.WKNIGHT] |
                           pos.pieceTypeBB[Piece.WBISHOP] |
                           pos.pieceTypeBB[Piece.WROOK] |
                           pos.pieceTypeBB[Piece.WQUEEN]);
            pawns = pos.pieceTypeBB[Piece.BPAWN];
            bAttacksBB |= (pawns & BitBoard.maskBToHFiles) >> 9;
            bAttacksBB |= (pawns & BitBoard.maskAToGFiles) >> 7;
            m = bAttacksBB & pos.whiteBB & ~pos.pieceTypeBB[Piece.WKING];
            tmp = 0;
            while (m != 0)
            {
                int sq = BitBoard.numberOfTrailingZeros(m);
                tmp += pieceValue[pos.squares[sq]];
                m &= m - 1;
            }
            score -= tmp + tmp * tmp / qV;
            return score / 64;
        }

        /** Compute king safety for both kings. */
        private int kingSafety(Position pos)
        {
            int minM = rV + bV;
            int m = (pos.wMtrl - pos.wMtrlPawns + pos.bMtrl - pos.bMtrlPawns) / 2;
            if (m <= minM)
                return 0;
            int maxM = qV + 2 * rV + 2 * bV + 2 * nV;
            int score = kingSafetyKPPart(pos);
            if (Position.getY(pos.wKingSq) == 0)
            {
                if (((pos.pieceTypeBB[Piece.WKING] & 0x60L) != 0) && // King on f1 or g1
                    ((pos.pieceTypeBB[Piece.WROOK] & 0xC0L) != 0) && // Rook on g1 or h1
                    ((pos.pieceTypeBB[Piece.WPAWN] & BitBoard.maskFile[6]) != 0) &&
                    ((pos.pieceTypeBB[Piece.WPAWN] & BitBoard.maskFile[7]) != 0))
                {
                    score -= 6 * 15;
                }
                else
                if (((pos.pieceTypeBB[Piece.WKING] & 0x6L) != 0) && // King on b1 or c1
                    ((pos.pieceTypeBB[Piece.WROOK] & 0x3L) != 0) && // Rook on a1 or b1
                    ((pos.pieceTypeBB[Piece.WPAWN] & BitBoard.maskFile[0]) != 0) &&
                    ((pos.pieceTypeBB[Piece.WPAWN] & BitBoard.maskFile[1]) != 0))
                {
                    score -= 6 * 15;
                }
            }
            if (Position.getY(pos.bKingSq) == 7)
            {
                if (((pos.pieceTypeBB[Piece.BKING] & 0x6000000000000000L) != 0) && // King on f8 or g8
                    ((pos.pieceTypeBB[Piece.BROOK] & 0xC000000000000000L) != 0) && // Rook on g8 or h8
                    ((pos.pieceTypeBB[Piece.BPAWN] & BitBoard.maskFile[6]) != 0) &&
                    ((pos.pieceTypeBB[Piece.BPAWN] & BitBoard.maskFile[7]) != 0))
                {
                    score += 6 * 15;
                }
                else
                if (((pos.pieceTypeBB[Piece.BKING] & 0x600000000000000L) != 0) && // King on b8 or c8
                    ((pos.pieceTypeBB[Piece.BROOK] & 0x300000000000000L) != 0) && // Rook on a8 or b8
                    ((pos.pieceTypeBB[Piece.BPAWN] & BitBoard.maskFile[0]) != 0) &&
                    ((pos.pieceTypeBB[Piece.BPAWN] & BitBoard.maskFile[1]) != 0))
                {
                    score += 6 * 15;
                }
            }
            score += (bKingAttacks - wKingAttacks) * 4;
            int kSafety = interpolate(m, minM, 0, maxM, score);
            return kSafety;

            // FIXME! g pawn is valuable (avoid g5, g4, gxf5)
        }

        private class KingSafetyHashData
        {
            public ulong key;
            public int score;
        }
        private static KingSafetyHashData[] kingSafetyHash;
        private static void Co5()
        {
            int numEntries = 1 << 15;
            kingSafetyHash = new KingSafetyHashData[numEntries];
            for (int i = 0; i < numEntries; i++)
            {
                KingSafetyHashData ksh = new KingSafetyHashData();
                ksh.key = Defs.ulongN1 /*-1 */;
                ksh.score = 0;
                kingSafetyHash[i] = ksh;
            }
        }

        private int kingSafetyKPPart(Position pos)
        {
            ulong key = pos.pawnZobristHash() ^ pos.kingZobristHash();
            KingSafetyHashData ksh = kingSafetyHash[(int)key & (kingSafetyHash.Length - 1)];
            if (ksh.key != key)
            {
                int score = 0;
                ulong wPawns = pos.pieceTypeBB[Piece.WPAWN];
                ulong bPawns = pos.pieceTypeBB[Piece.BPAWN];
                {
                    int safety = 0;
                    int halfOpenFiles = 0;
                    if (Position.getY(pos.wKingSq) < 2)
                    {
                        ulong shelter = 1UL << Position.getX(pos.wKingSq);
                        shelter |= ((shelter & BitBoard.maskBToHFiles) >> 1) |
                                   ((shelter & BitBoard.maskAToGFiles) << 1);
                        shelter <<= 8;
                        safety += 3 * BITS.bitCount(wPawns & shelter);
                        safety -= 2 * BITS.bitCount(bPawns & (shelter | (shelter << 8)));
                        shelter <<= 8;
                        safety += 2 * BITS.bitCount(wPawns & shelter);
                        shelter <<= 8;
                        safety -= BITS.bitCount(bPawns & shelter);

                        ulong wOpen = BitBoard.southFill(shelter) & (~BitBoard.southFill(wPawns)) & 0xff;
                        if (wOpen != 0)
                        {
                            halfOpenFiles += 25 * BITS.bitCount(wOpen & 0xe7);
                            halfOpenFiles += 10 * BITS.bitCount(wOpen & 0x18);
                        }
                        ulong bOpen = BitBoard.southFill(shelter) & (~BitBoard.southFill(bPawns)) & 0xff;
                        if (bOpen != 0)
                        {
                            halfOpenFiles += 25 * BITS.bitCount(bOpen & 0xe7);
                            halfOpenFiles += 10 * BITS.bitCount(bOpen & 0x18);
                        }
                        safety = Math.Min(safety, 8);
                    }
                    int kSafety = (safety - 9) * 15 - halfOpenFiles;
                    score += kSafety;
                }
                {
                    int safety = 0;
                    int halfOpenFiles = 0;
                    if (Position.getY(pos.bKingSq) >= 6)
                    {
                        ulong shelter = 1UL << (56 + Position.getX(pos.bKingSq));
                        shelter |= ((shelter & BitBoard.maskBToHFiles) >> 1) |
                                   ((shelter & BitBoard.maskAToGFiles) << 1);
                        shelter >>= 8;
                        safety += 3 * BITS.bitCount(bPawns & shelter);
                        safety -= 2 * BITS.bitCount(wPawns & (shelter | (shelter >> 8)));
                        shelter >>= 8;
                        safety += 2 * BITS.bitCount(bPawns & shelter);
                        shelter >>= 8;
                        safety -= BITS.bitCount(wPawns & shelter);

                        ulong wOpen = BitBoard.southFill(shelter) & (~BitBoard.southFill(wPawns)) & 0xff;
                        if (wOpen != 0)
                        {
                            halfOpenFiles += 25 * BITS.bitCount(wOpen & 0xe7);
                            halfOpenFiles += 10 * BITS.bitCount(wOpen & 0x18);
                        }
                        ulong bOpen = BitBoard.southFill(shelter) & (~BitBoard.southFill(bPawns)) & 0xff;
                        if (bOpen != 0)
                        {
                            halfOpenFiles += 25 * BITS.bitCount(bOpen & 0xe7);
                            halfOpenFiles += 10 * BITS.bitCount(bOpen & 0x18);
                        }
                        safety = Math.Min(safety, 8);
                    }
                    int kSafety = (safety - 9) * 15 - halfOpenFiles;
                    score -= kSafety;
                }
                ksh.key = key;
                ksh.score = score;
            }
            return ksh.score;
        }

        /** Implements special knowledge for some endgame situations. */
        private int endGameEval(Position pos, int oldScore)
        {
            int score = oldScore;
            if (pos.wMtrl + pos.bMtrl > 6 * rV)
                return score;
            int wMtrlPawns = pos.wMtrlPawns;
            int bMtrlPawns = pos.bMtrlPawns;
            int wMtrlNoPawns = pos.wMtrl - wMtrlPawns;
            int bMtrlNoPawns = pos.bMtrl - bMtrlPawns;

            bool handled = false;
            if ((wMtrlPawns + bMtrlPawns == 0) && (wMtrlNoPawns < rV) && (bMtrlNoPawns < rV))
            {
                // King + minor piece vs king + minor piece is a draw
                return 0;
            }
            if (!handled && (pos.wMtrl == qV) && (pos.bMtrl == pV) &&
                (BITS.bitCount(pos.pieceTypeBB[Piece.WQUEEN]) == 1))
            {
                int wk = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.WKING]);
                int wq = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.WQUEEN]);
                int bk = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.BKING]);
                int bp = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.BPAWN]);
                score = evalKQKP(wk, wq, bk, bp);
                handled = true;
            }
            if (!handled && (pos.bMtrl == qV) && (pos.wMtrl == pV) &&
                (BITS.bitCount(pos.pieceTypeBB[Piece.BQUEEN]) == 1))
            {
                int bk = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.BKING]);
                int bq = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.BQUEEN]);
                int wk = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.WKING]);
                int wp = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.WPAWN]);
                score = -evalKQKP(63 - bk, 63 - bq, 63 - wk, 63 - wp);
                handled = true;
            }
            if (!handled && (score > 0))
            {
                if ((wMtrlPawns == 0) && (wMtrlNoPawns <= bMtrlNoPawns + bV))
                {
                    if (wMtrlNoPawns < rV)
                    {
                        return -pos.bMtrl / 50;
                    }
                    else
                    {
                        score /= 8;         // Too little excess material, probably draw
                        handled = true;
                    }
                }
                else if ((pos.pieceTypeBB[Piece.WROOK] | pos.pieceTypeBB[Piece.WKNIGHT] |
                          pos.pieceTypeBB[Piece.WQUEEN]) == 0)
                {
                    // Check for rook pawn + wrong color bishop
                    if (((pos.pieceTypeBB[Piece.WPAWN] & BitBoard.maskBToHFiles) == 0) &&
                        ((pos.pieceTypeBB[Piece.WBISHOP] & BitBoard.maskLightSq) == 0) &&
                        ((pos.pieceTypeBB[Piece.BKING] & 0x0303000000000000L) != 0))
                    {
                        return 0;
                    }
                    else
                  if (((pos.pieceTypeBB[Piece.WPAWN] & BitBoard.maskAToGFiles) == 0) &&
                      ((pos.pieceTypeBB[Piece.WBISHOP] & BitBoard.maskDarkSq) == 0) &&
                      ((pos.pieceTypeBB[Piece.BKING] & 0xC0C0000000000000L) != 0))
                    {
                        return 0;
                    }
                }
            }
            if (!handled)
            {
                if (bMtrlPawns == 0)
                {
                    if (wMtrlNoPawns - bMtrlNoPawns > bV)
                    {
                        int wKnights = BITS.bitCount(pos.pieceTypeBB[Piece.WKNIGHT]);
                        int wBishops = BITS.bitCount(pos.pieceTypeBB[Piece.WBISHOP]);
                        if ((wKnights == 2) && (wMtrlNoPawns == 2 * nV) && (bMtrlNoPawns == 0))
                        {
                            score /= 50;    // KNNK is a draw
                        }
                        else if ((wKnights == 1) && (wBishops == 1) && (wMtrlNoPawns == nV + bV) && (bMtrlNoPawns == 0))
                        {
                            score /= 10;
                            score += nV + bV + 300;
                            int kSq = pos.getKingSq(false);
                            int x = Position.getX(kSq);
                            int y = Position.getY(kSq);
                            if ((pos.pieceTypeBB[Piece.WBISHOP] & BitBoard.maskDarkSq) != 0)
                            {
                                score += (7 - distToH1A8[7 - y][7 - x]) * 10;
                            }
                            else
                            {
                                score += (7 - distToH1A8[7 - y][x]) * 10;
                            }
                        }
                        else
                        {
                            score += 300;       // Enough excess material, should win
                        }
                        handled = true;
                    }
                    else if ((wMtrlNoPawns + bMtrlNoPawns == 0) && (wMtrlPawns == pV))
                    { // KPK
                        int wp = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.WPAWN]);
                        score = kpkEval(pos.getKingSq(true), pos.getKingSq(false),
                                        wp, pos.whiteMove);
                        handled = true;
                    }
                }
            }
            if (!handled && (score < 0))
            {
                if ((bMtrlPawns == 0) && (bMtrlNoPawns <= wMtrlNoPawns + bV))
                {
                    if (bMtrlNoPawns < rV)
                    {
                        return pos.wMtrl / 50;
                    }
                    else
                    {
                        score /= 8;         // Too little excess material, probably draw
                        handled = true;
                    }
                }
                else if ((pos.pieceTypeBB[Piece.BROOK] | pos.pieceTypeBB[Piece.BKNIGHT] |
                          pos.pieceTypeBB[Piece.BQUEEN]) == 0)
                {
                    // Check for rook pawn + wrong color bishop
                    if (((pos.pieceTypeBB[Piece.BPAWN] & BitBoard.maskBToHFiles) == 0) &&
                        ((pos.pieceTypeBB[Piece.BBISHOP] & BitBoard.maskDarkSq) == 0) &&
                        ((pos.pieceTypeBB[Piece.WKING] & 0x0303L) != 0))
                    {
                        return 0;
                    }
                    else
                    if (((pos.pieceTypeBB[Piece.BPAWN] & BitBoard.maskAToGFiles) == 0) &&
                        ((pos.pieceTypeBB[Piece.BBISHOP] & BitBoard.maskLightSq) == 0) &&
                        ((pos.pieceTypeBB[Piece.WKING] & 0xC0C0L) != 0))
                    {
                        return 0;
                    }
                }
            }
            if (!handled)
            {
                if (wMtrlPawns == 0)
                {
                    if (bMtrlNoPawns - wMtrlNoPawns > bV)
                    {
                        int bKnights = BITS.bitCount(pos.pieceTypeBB[Piece.BKNIGHT]);
                        int bBishops = BITS.bitCount(pos.pieceTypeBB[Piece.BBISHOP]);
                        if ((bKnights == 2) && (bMtrlNoPawns == 2 * nV) && (wMtrlNoPawns == 0))
                        {
                            score /= 50;    // KNNK is a draw
                        }
                        else if ((bKnights == 1) && (bBishops == 1) && (bMtrlNoPawns == nV + bV) && (wMtrlNoPawns == 0))
                        {
                            score /= 10;
                            score -= nV + bV + 300;
                            int kSq = pos.getKingSq(true);
                            int x = Position.getX(kSq);
                            int y = Position.getY(kSq);
                            if ((pos.pieceTypeBB[Piece.BBISHOP] & BitBoard.maskDarkSq) != 0)
                            {
                                score -= (7 - distToH1A8[7 - y][7 - x]) * 10;
                            }
                            else
                            {
                                score -= (7 - distToH1A8[7 - y][x]) * 10;
                            }
                        }
                        else
                        {
                            score -= 300;       // Enough excess material, should win
                        }
                        handled = true;
                    }
                    else if ((wMtrlNoPawns + bMtrlNoPawns == 0) && (bMtrlPawns == pV))
                    { // KPK
                        int bp = BitBoard.numberOfTrailingZeros(pos.pieceTypeBB[Piece.BPAWN]);
                        score = -kpkEval(63 - pos.getKingSq(false), 63 - pos.getKingSq(true),
                                         63 - bp, !pos.whiteMove);
                        handled = true;
                    }
                }
            }
            return score;

            // FIXME! Add evaluation of KRKP
            // FIXME! Add evaluation of KRPKR   : eg 8/8/8/5pk1/1r6/R7/8/4K3 w - - 0 74
            // FIXME! KRBKR is very hard to draw
        }

        private static int evalKQKP(int wKing, int wQueen, int bKing, int bPawn)
        {
            bool canWin = false;
            if (((1L << bKing) & 0xFFFF) == 0)
            {
                canWin = true; // King doesn't support pawn
            }
            else if (Math.Abs(Position.getX(bPawn) - Position.getX(bKing)) > 2)
            {
                canWin = true; // King doesn't support pawn
            }
            else
            {
                switch (bPawn)
                {
                    case 8:  // a2
                        canWin = ((1L << wKing) & 0x0F1F1F1F1FL) != 0;
                        break;
                    case 10: // c2
                        canWin = ((1L << wKing) & 0x071F1F1FL) != 0;
                        break;
                    case 13: // f2
                        canWin = ((1L << wKing) & 0xE0F8F8F8L) != 0;
                        break;
                    case 15: // h2
                        canWin = ((1L << wKing) & 0xF0F8F8F8F8L) != 0;
                        break;
                    default:
                        canWin = true;
                        break;
                }
            }

            int dist = Math.Max(Math.Abs(Position.getX(wKing) - Position.getX(bPawn)),
                                      Math.Abs(Position.getY(wKing) - Position.getY(bPawn)));
            int score = qV - pV - 20 * dist;
            if (!canWin)
                score /= 50;
            return score;
        }

        private static int kpkEval(int wKing, int bKing, int wPawn, bool whiteMove)
        {
            if (Position.getX(wKing) >= 4)
            { // Mirror X
                wKing ^= 7;
                bKing ^= 7;
                wPawn ^= 7;
            }
            int index = whiteMove ? 0 : 1;
            index = index * 32 + Position.getY(wKing) * 4 + Position.getX(wKing);
            index = index * 64 + bKing;
            index = index * 48 + wPawn - 8;

            int bytePos = index / 8;
            int bitPos = index % 8;
            bool draw = (((int)kpkTable[bytePos]) & (1 << bitPos)) == 0;
            if (draw)
                return 0;
            return qV - pV / 4 * (7 - Position.getY(wPawn));
        }

        /**
         * Interpolate between (x1,y1) and (x2,y2).
         * If x < x1, return y1, if x > x2 return y2. Otherwise, use linear interpolation.
         */
        public static int interpolate(int x, int x1, int y1, int x2, int y2)
        {
            if (x > x2)
            {
                return y2;
            }
            else if (x < x1)
            {
                return y1;
            }
            else
            {
                return (x - x1) * (y2 - y1) / (x2 - x1) + y1;
            }
        }
    }
    //// End class
}
//// End namespace