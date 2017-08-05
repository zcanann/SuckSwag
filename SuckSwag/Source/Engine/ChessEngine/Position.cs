using System;
using System.Security.Cryptography;

namespace SuckSwag.Source.ChessEngine
{
    /**
     * Stores the state of a chess position.
     * All required state is stored, except for all previous positions
     * since the last capture or pawn move. That state is only needed
     * for three-fold repetition draw detection, and is better stored
     * in a separate hash table.
     * @author petero
     */
    public class Position
    {

        public int[] squares;

        // Bitboards
        public ulong[] pieceTypeBB;
        public ulong whiteBB, blackBB;

        // Piece square table scores
        public /*short[]*/ int[] psScore1, psScore2;

        public bool whiteMove;

        /** Bit definitions for the castleMask bit mask. */
        public static int A1_CASTLE = 0; /** White ulong castle. */
        public static int H1_CASTLE = 1; /** White short castle. */
        public static int A8_CASTLE = 2; /** Black ulong castle. */
        public static int H8_CASTLE = 3; /** Black short castle. */

        private int castleMask;

        private int epSquare;

        /** Number of half-moves since last 50-move reset. */
        public int halfMoveClock;

        /** Game move number, starting from 1. */
        public int fullMoveCounter;

        private ulong hashKey;           // Cached Zobrist hash key
        private ulong pHashKey;
        public int wKingSq, bKingSq;   // Cached king positions
        public int wMtrl;      // Total value of all white pieces and pawns
        public int bMtrl;      // Total value of all black pieces and pawns
        public int wMtrlPawns; // Total value of all white pawns
        public int bMtrlPawns; // Total value of all black pawns

        /** Initialize board to empty position. */
        public Position()
        {
            Co1();
            squares = new int[64];
            for (int i = 0; i < 64; i++)
                squares[i] = Piece.EMPTY;
            pieceTypeBB = new ulong[Piece.nPieceTypes];
            psScore1 = new /*short*/ int[Piece.nPieceTypes];
            psScore2 = new /*short*/ int[Piece.nPieceTypes];
            for (int i = 0; i < Piece.nPieceTypes; i++)
            {
                pieceTypeBB[i] = 0L;
                psScore1[i] = 0;
                psScore2[i] = 0;
            }
            whiteBB = blackBB = 0L;
            whiteMove = true;
            castleMask = 0;
            epSquare = -1;
            halfMoveClock = 0;
            fullMoveCounter = 1;
            hashKey = computeZobristHash();
            wKingSq = bKingSq = -1;
            wMtrl = bMtrl = -Evaluate.kV;
            wMtrlPawns = bMtrlPawns = 0;
        }

        public Position(Position other)
        {
            squares = new int[64];
            for (int i = 0; i < 64; i++)
                squares[i] = other.squares[i];
            pieceTypeBB = new ulong[Piece.nPieceTypes];
            psScore1 = new /*short*/ int[Piece.nPieceTypes];
            psScore2 = new /*short*/ int[Piece.nPieceTypes];
            for (int i = 0; i < Piece.nPieceTypes; i++)
            {
                pieceTypeBB[i] = other.pieceTypeBB[i];
                psScore1[i] = other.psScore1[i];
                psScore2[i] = other.psScore2[i];
            }
            whiteBB = other.whiteBB;
            blackBB = other.blackBB;
            whiteMove = other.whiteMove;
            castleMask = other.castleMask;
            epSquare = other.epSquare;
            halfMoveClock = other.halfMoveClock;
            fullMoveCounter = other.fullMoveCounter;
            hashKey = other.hashKey;
            pHashKey = other.pHashKey;
            wKingSq = other.wKingSq;
            bKingSq = other.bKingSq;
            wMtrl = other.wMtrl;
            bMtrl = other.bMtrl;
            wMtrlPawns = other.wMtrlPawns;
            bMtrlPawns = other.bMtrlPawns;
        }

        /*@Override*/
        public bool equals(Position o)
        {
            if ((o == null) /*|| (o.getClass() != this.getClass())*/)
                return false;
            Position other = (Position)o;
            if (!drawRuleEquals(other))
                return false;
            if (halfMoveClock != other.halfMoveClock)
                return false;
            if (fullMoveCounter != other.fullMoveCounter)
                return false;
            if (hashKey != other.hashKey)
                return false;
            if (pHashKey != other.pHashKey)
                return false;
            return true;
        }
        /*@Override*/
        public int hashCode()
        {
            return (int)hashKey;
        }

        /**
         * Return Zobrist hash value for the current position.
         * Everything except the move counters are included in the hash value.
         */
        public ulong zobristHash()
        {
            return hashKey;
        }
        public ulong pawnZobristHash()
        {
            return pHashKey;
        }
        public ulong kingZobristHash()
        {
            return psHashKeys[Piece.WKING][wKingSq] ^
                   psHashKeys[Piece.BKING][bKingSq];
        }

        public ulong historyHash()
        {
            ulong ret = hashKey;
            if (halfMoveClock >= 80)
            {
                ret ^= moveCntKeys[Math.Min(halfMoveClock, 100)];
            }
            return ret;
        }

        /**
         * Decide if two positions are equal in the sense of the draw by repetition rule.
         * @return True if positions are equal, false otherwise.
         */
        public bool drawRuleEquals(Position other)
        {
            for (int i = 0; i < 64; i++)
            {
                if (squares[i] != other.squares[i])
                    return false;
            }
            if (whiteMove != other.whiteMove)
                return false;
            if (castleMask != other.castleMask)
                return false;
            if (epSquare != other.epSquare)
                return false;
            return true;
        }

        public void setWhiteMove(bool whiteMove)
        {
            if (whiteMove != this.whiteMove)
            {
                hashKey ^= whiteHashKey;
                this.whiteMove = whiteMove;
            }
        }
        /** Return index in squares[] vector corresponding to (x,y). */
        public static int getSquare(int x, int y)
        {
            return y * 8 + x;
        }
        /** Return x position (file) corresponding to a square. */
        public static int getX(int square)
        {
            return square & 7;
        }
        /** Return y position (rank) corresponding to a square. */
        public static int getY(int square)
        {
            return square >> 3;
        }
        /** Return true if (x,y) is a dark square. */
        public static bool darkSquare(int x, int y)
        {
            return (x & 1) == (y & 1);
        }

        /** Return piece occupying a square. */
        public int getPiece(int square)
        {
            return squares[square];
        }

        /** Move a non-pawn piece to an empty square. */
        private void movePieceNotPawn(int from, int to)
        {
            int piece = squares[from];
            hashKey ^= psHashKeys[piece][from];
            hashKey ^= psHashKeys[piece][to];
            hashKey ^= psHashKeys[Piece.EMPTY][from];
            hashKey ^= psHashKeys[Piece.EMPTY][to];

            squares[from] = Piece.EMPTY;
            squares[to] = piece;

            ulong sqMaskF = 1UL << from;
            ulong sqMaskT = 1UL << to;
            pieceTypeBB[piece] &= ~sqMaskF;
            pieceTypeBB[piece] |= sqMaskT;
            if (Piece.isWhite(piece))
            {
                whiteBB &= ~sqMaskF;
                whiteBB |= sqMaskT;
                if (piece == Piece.WKING)
                    wKingSq = to;
            }
            else
            {
                blackBB &= ~sqMaskF;
                blackBB |= sqMaskT;
                if (piece == Piece.BKING)
                    bKingSq = to;
            }

            psScore1[piece] += Evaluate.psTab1[piece][to] - Evaluate.psTab1[piece][from];
            psScore2[piece] += Evaluate.psTab2[piece][to] - Evaluate.psTab2[piece][from];
        }

        /** Set a square to a piece value. */
        public void setPiece(int square, int piece)
        {
            // Update hash key
            int removedPiece = squares[square];
            hashKey ^= psHashKeys[removedPiece][square];
            hashKey ^= psHashKeys[piece][square];
            if ((removedPiece == Piece.WPAWN) || (removedPiece == Piece.BPAWN))
                pHashKey ^= psHashKeys[removedPiece][square];
            if ((piece == Piece.WPAWN) || (piece == Piece.BPAWN))
                pHashKey ^= psHashKeys[piece][square];

            // Update material balance
            int pVal = Evaluate.pieceValue[removedPiece];
            if (Piece.isWhite(removedPiece))
            {
                wMtrl -= pVal;
                if (removedPiece == Piece.WPAWN)
                    wMtrlPawns -= pVal;
            }
            else
            {
                bMtrl -= pVal;
                if (removedPiece == Piece.BPAWN)
                    bMtrlPawns -= pVal;
            }
            pVal = Evaluate.pieceValue[piece];
            if (Piece.isWhite(piece))
            {
                wMtrl += pVal;
                if (piece == Piece.WPAWN)
                    wMtrlPawns += pVal;
            }
            else
            {
                bMtrl += pVal;
                if (piece == Piece.BPAWN)
                    bMtrlPawns += pVal;
            }

            // Update board
            squares[square] = piece;

            // Update bitboards
            ulong sqMask = 1UL << square;
            pieceTypeBB[removedPiece] &= ~sqMask;
            pieceTypeBB[piece] |= sqMask;
            if (removedPiece != Piece.EMPTY)
            {
                if (Piece.isWhite(removedPiece))
                    whiteBB &= ~sqMask;
                else
                    blackBB &= ~sqMask;
            }
            if (piece != Piece.EMPTY)
            {
                if (Piece.isWhite(piece))
                    whiteBB |= sqMask;
                else
                    blackBB |= sqMask;
            }

            // Update king position 
            if (piece == Piece.WKING)
            {
                wKingSq = square;
            }
            else if (piece == Piece.BKING)
            {
                bKingSq = square;
            }

            // Update piece/square table scores
            psScore1[removedPiece] -= Evaluate.psTab1[removedPiece][square];
            psScore2[removedPiece] -= Evaluate.psTab2[removedPiece][square];
            psScore1[piece] += Evaluate.psTab1[piece][square];
            psScore2[piece] += Evaluate.psTab2[piece][square];
        }

        /**
         * Set a square to a piece value.
         * Special version that only updates enough of the state for the SEE function to be happy.
         */
        public void setSEEPiece(int square, int piece)
        {
            int removedPiece = squares[square];

            // Update board
            squares[square] = piece;

            // Update bitboards
            ulong sqMask = 1UL << square;
            pieceTypeBB[removedPiece] &= ~sqMask;
            pieceTypeBB[piece] |= sqMask;
            if (removedPiece != Piece.EMPTY)
            {
                if (Piece.isWhite(removedPiece))
                    whiteBB &= ~sqMask;
                else
                    blackBB &= ~sqMask;
            }
            if (piece != Piece.EMPTY)
            {
                if (Piece.isWhite(piece))
                    whiteBB |= sqMask;
                else
                    blackBB |= sqMask;
            }
        }

        /** Return true if white ulong castling right has not been lost. */
        public bool a1Castle()
        {
            return (castleMask & (1 << A1_CASTLE)) != 0;
        }
        /** Return true if white short castling right has not been lost. */
        public bool h1Castle()
        {
            return (castleMask & (1 << H1_CASTLE)) != 0;
        }
        /** Return true if black ulong castling right has not been lost. */
        public bool a8Castle()
        {
            return (castleMask & (1 << A8_CASTLE)) != 0;
        }
        /** Return true if black short castling right has not been lost. */
        public bool h8Castle()
        {
            return (castleMask & (1 << H8_CASTLE)) != 0;
        }
        /** Bitmask describing castling rights. */
        public int getCastleMask()
        {
            return castleMask;
        }
        public void setCastleMask(int castleMask)
        {
            hashKey ^= castleHashKeys[this.castleMask];
            hashKey ^= castleHashKeys[castleMask];
            this.castleMask = castleMask;
        }

        /** En passant square, or -1 if no ep possible. */
        public int getEpSquare()
        {
            return epSquare;
        }
        public void setEpSquare(int epSquare)
        {
            if (this.epSquare != epSquare)
            {
                hashKey ^= epHashKeys[(this.epSquare >= 0) ? getX(this.epSquare) + 1 : 0];
                hashKey ^= epHashKeys[(epSquare >= 0) ? getX(epSquare) + 1 : 0];
                this.epSquare = epSquare;
            }
        }


        public int getKingSq(bool whiteMove)
        {
            return whiteMove ? wKingSq : bKingSq;
        }

        /**
         * Count number of pieces of a certain type.
         */
        public int nPieces(int pType)
        {
            int ret = 0;
            for (int sq = 0; sq < 64; sq++)
            {
                if (squares[sq] == pType)
                    ret++;
            }
            return ret;
        }

        /** Apply a move to the current position. */
        public void makeMove(Move move, UndoInfo ui)
        {
            ui.capturedPiece = squares[move.to];
            ui.castleMask = castleMask;
            ui.epSquare = epSquare;
            ui.halfMoveClock = halfMoveClock;
            bool wtm = whiteMove;

            int p = squares[move.from];
            int capP = squares[move.to];
            ulong fromMask = 1UL << move.from;

            int prevEpSquare = epSquare;
            setEpSquare(-1);

            if ((capP != Piece.EMPTY) || (((pieceTypeBB[Piece.WPAWN] | pieceTypeBB[Piece.BPAWN]) & fromMask) != 0))
            {
                halfMoveClock = 0;

                // Handle en passant and epSquare
                if (p == Piece.WPAWN)
                {
                    if (move.to - move.from == 2 * 8)
                    {
                        int x = Position.getX(move.to);
                        if (((x > 0) && (squares[move.to - 1] == Piece.BPAWN)) ||
                                ((x < 7) && (squares[move.to + 1] == Piece.BPAWN)))
                        {
                            setEpSquare(move.from + 8);
                        }
                    }
                    else if (move.to == prevEpSquare)
                    {
                        setPiece(move.to - 8, Piece.EMPTY);
                    }
                }
                else if (p == Piece.BPAWN)
                {
                    if (move.to - move.from == -2 * 8)
                    {
                        int x = Position.getX(move.to);
                        if (((x > 0) && (squares[move.to - 1] == Piece.WPAWN)) ||
                                ((x < 7) && (squares[move.to + 1] == Piece.WPAWN)))
                        {
                            setEpSquare(move.from - 8);
                        }
                    }
                    else if (move.to == prevEpSquare)
                    {
                        setPiece(move.to + 8, Piece.EMPTY);
                    }
                }

                if (((pieceTypeBB[Piece.WKING] | pieceTypeBB[Piece.BKING]) & fromMask) != 0)
                {
                    if (wtm)
                    {
                        setCastleMask(castleMask & ~(1 << Position.A1_CASTLE));
                        setCastleMask(castleMask & ~(1 << Position.H1_CASTLE));
                    }
                    else
                    {
                        setCastleMask(castleMask & ~(1 << Position.A8_CASTLE));
                        setCastleMask(castleMask & ~(1 << Position.H8_CASTLE));
                    }
                }

                // Perform move
                setPiece(move.from, Piece.EMPTY);
                // Handle promotion
                if (move.promoteTo != Piece.EMPTY)
                {
                    setPiece(move.to, move.promoteTo);
                }
                else
                {
                    setPiece(move.to, p);
                }
            }
            else
            {
                halfMoveClock++;

                // Handle castling
                if (((pieceTypeBB[Piece.WKING] | pieceTypeBB[Piece.BKING]) & fromMask) != 0)
                {
                    int k0 = move.from;
                    if (move.to == k0 + 2)
                    { // O-O
                        movePieceNotPawn(k0 + 3, k0 + 1);
                    }
                    else if (move.to == k0 - 2)
                    { // O-O-O
                        movePieceNotPawn(k0 - 4, k0 - 1);
                    }
                    if (wtm)
                    {
                        setCastleMask(castleMask & ~(1 << Position.A1_CASTLE));
                        setCastleMask(castleMask & ~(1 << Position.H1_CASTLE));
                    }
                    else
                    {
                        setCastleMask(castleMask & ~(1 << Position.A8_CASTLE));
                        setCastleMask(castleMask & ~(1 << Position.H8_CASTLE));
                    }
                }

                // Perform move
                movePieceNotPawn(move.from, move.to);
            }
            if (!wtm)
            {
                fullMoveCounter++;
            }

            // Update castling rights when rook moves
            if ((BitBoard.maskCorners & fromMask) != 0)
            {
                int rook = wtm ? Piece.WROOK : Piece.BROOK;
                if (p == rook)
                    removeCastleRights(move.from);
            }
            if ((BitBoard.maskCorners & (1UL << move.to)) != 0)
            {
                int oRook = wtm ? Piece.BROOK : Piece.WROOK;
                if (capP == oRook)
                    removeCastleRights(move.to);
            }

            hashKey ^= whiteHashKey;
            whiteMove = !wtm;
        }

        public void unMakeMove(Move move, UndoInfo ui)
        {
            hashKey ^= whiteHashKey;
            whiteMove = !whiteMove;
            int p = squares[move.to];
            setPiece(move.from, p);
            setPiece(move.to, ui.capturedPiece);
            setCastleMask(ui.castleMask);
            setEpSquare(ui.epSquare);
            halfMoveClock = ui.halfMoveClock;
            bool wtm = whiteMove;
            if (move.promoteTo != Piece.EMPTY)
            {
                p = wtm ? Piece.WPAWN : Piece.BPAWN;
                setPiece(move.from, p);
            }
            if (!wtm)
            {
                fullMoveCounter--;
            }

            // Handle castling
            int king = wtm ? Piece.WKING : Piece.BKING;
            if (p == king)
            {
                int k0 = move.from;
                if (move.to == k0 + 2)
                { // O-O
                    movePieceNotPawn(k0 + 1, k0 + 3);
                }
                else if (move.to == k0 - 2)
                { // O-O-O
                    movePieceNotPawn(k0 - 1, k0 - 4);
                }
            }

            // Handle en passant
            if (move.to == epSquare)
            {
                if (p == Piece.WPAWN)
                {
                    setPiece(move.to - 8, Piece.BPAWN);
                }
                else if (p == Piece.BPAWN)
                {
                    setPiece(move.to + 8, Piece.WPAWN);
                }
            }
        }

        /**
         * Apply a move to the current position.
         * Special version that only updates enough of the state for the SEE function to be happy.
         */
        public void makeSEEMove(Move move, UndoInfo ui)
        {
            ui.capturedPiece = squares[move.to];
            bool wtm = whiteMove;

            int p = squares[move.from];
            ulong fromMask = 1UL << move.from;

            // Handle castling
            if (((pieceTypeBB[Piece.WKING] | pieceTypeBB[Piece.BKING]) & fromMask) != 0)
            {
                int k0 = move.from;
                if (move.to == k0 + 2)
                { // O-O
                    setSEEPiece(k0 + 1, squares[k0 + 3]);
                    setSEEPiece(k0 + 3, Piece.EMPTY);
                }
                else if (move.to == k0 - 2)
                { // O-O-O
                    setSEEPiece(k0 - 1, squares[k0 - 4]);
                    setSEEPiece(k0 - 4, Piece.EMPTY);
                }
            }

            // Handle en passant
            if (move.to == epSquare)
            {
                if (p == Piece.WPAWN)
                {
                    setSEEPiece(move.to - 8, Piece.EMPTY);
                }
                else if (p == Piece.BPAWN)
                {
                    setSEEPiece(move.to + 8, Piece.EMPTY);
                }
            }

            // Perform move
            setSEEPiece(move.from, Piece.EMPTY);
            setSEEPiece(move.to, p);
            whiteMove = !wtm;
        }

        public void unMakeSEEMove(Move move, UndoInfo ui)
        {
            whiteMove = !whiteMove;
            int p = squares[move.to];
            setSEEPiece(move.from, p);
            setSEEPiece(move.to, ui.capturedPiece);
            bool wtm = whiteMove;

            // Handle castling
            int king = wtm ? Piece.WKING : Piece.BKING;
            if (p == king)
            {
                int k0 = move.from;
                if (move.to == k0 + 2)
                { // O-O
                    setSEEPiece(k0 + 3, squares[k0 + 1]);
                    setSEEPiece(k0 + 1, Piece.EMPTY);
                }
                else if (move.to == k0 - 2)
                { // O-O-O
                    setSEEPiece(k0 - 4, squares[k0 - 1]);
                    setSEEPiece(k0 - 1, Piece.EMPTY);
                }
            }

            // Handle en passant
            if (move.to == epSquare)
            {
                if (p == Piece.WPAWN)
                {
                    setSEEPiece(move.to - 8, Piece.BPAWN);
                }
                else if (p == Piece.BPAWN)
                {
                    setSEEPiece(move.to + 8, Piece.WPAWN);
                }
            }
        }

        private void removeCastleRights(int square)
        {
            if (square == Position.getSquare(0, 0))
            {
                setCastleMask(castleMask & ~(1 << Position.A1_CASTLE));
            }
            else if (square == Position.getSquare(7, 0))
            {
                setCastleMask(castleMask & ~(1 << Position.H1_CASTLE));
            }
            else if (square == Position.getSquare(0, 7))
            {
                setCastleMask(castleMask & ~(1 << Position.A8_CASTLE));
            }
            else if (square == Position.getSquare(7, 7))
            {
                setCastleMask(castleMask & ~(1 << Position.H8_CASTLE));
            }
        }

        /* ------------- Hashing code ------------------ */

        public static ulong[][] psHashKeys;    // [piece][square]
        private static ulong whiteHashKey;
        private static ulong[] castleHashKeys;  // [castleMask]
        private static ulong[] epHashKeys;      // [epFile + 1] (epFile==-1 for no ep)
        private static ulong[] moveCntKeys;     // [min(halfMoveClock, 100)]

        private static void Co1()
        {
            psHashKeys = new ulong[Piece.nPieceTypes][];
            castleHashKeys = new ulong[16];
            epHashKeys = new ulong[9];
            moveCntKeys = new ulong[101];
            int rndNo = 0;
            for (int k = 0; k < Piece.nPieceTypes; k++) psHashKeys[k] = new ulong[64];
            for (int p = 0; p < Piece.nPieceTypes; p++)
            {
                for (int sq = 0; sq < 64; sq++)
                {
                    psHashKeys[p][sq] = getRandomHashVal(rndNo++);
                }
            }
            whiteHashKey = getRandomHashVal(rndNo++);
            for (int cm = 0; cm < castleHashKeys.Length; cm++)
                castleHashKeys[cm] = getRandomHashVal(rndNo++);
            for (int f = 0; f < epHashKeys.Length; f++)
                epHashKeys[f] = getRandomHashVal(rndNo++);
            for (int mc = 0; mc < moveCntKeys.Length; mc++)
                moveCntKeys[mc] = getRandomHashVal(rndNo++);
        }

        /**
         * Compute the Zobrist hash value non-incrementally. Only useful for test programs.
         */
        ulong computeZobristHash()
        {
            ulong hash = 0;
            for (int sq = 0; sq < 64; sq++)
            {
                int p = squares[sq];
                hash ^= psHashKeys[p][sq];
                if ((p == Piece.WPAWN) || (p == Piece.BPAWN))
                    pHashKey ^= psHashKeys[p][sq];
            }
            if (whiteMove)
                hash ^= whiteHashKey;
            hash ^= castleHashKeys[castleMask];
            hash ^= epHashKeys[(epSquare >= 0) ? getX(epSquare) + 1 : 0];
            return hash;
        }

        private static ulong getRandomHashVal(int rndNo)
        {
            try
            {
                SHA1Managed md = new SHA1Managed();
                byte[] input = new byte[4];
                for (int i = 0; i < 4; i++)
                    input[i] = (byte)((rndNo >> (i * 8)) & 0xff);
                byte[] digest = md.ComputeHash(input, 0, 4);
                ulong ret = 0;
                for (int i = 0; i < 8; i++)
                {
                    ret ^= ((ulong)digest[i]) << (i * 8);
                }
                return ret;
            }
            catch
            {
                throw new UnsupportedSHA1OperationException();
            }
        }
    }
    //// End class
}
//// End namespace