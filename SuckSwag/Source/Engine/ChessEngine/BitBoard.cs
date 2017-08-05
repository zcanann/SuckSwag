namespace SuckSwag.Source.ChessEngine
{
    public class BitBoard
    {
        public BitBoard()
        {
            Co1(); Co2(); Co3(); Co4();
        }

        /** Squares attacked by a king on a given square. */
        public static ulong[] kingAttacks;
        public static ulong[] knightAttacks;
        public static ulong[] wPawnAttacks, bPawnAttacks;

        // Squares preventing a pawn from being a passed pawn, if occupied by enemy pawn
        public static ulong[] wPawnBlockerMask, bPawnBlockerMask;

        public static ulong maskAToGFiles = 0x7F7F7F7F7F7F7F7FL;
        public static ulong maskBToHFiles = 0xFEFEFEFEFEFEFEFEL;
        public static ulong maskAToFFiles = 0x3F3F3F3F3F3F3F3FL;
        public static ulong maskCToHFiles = 0xFCFCFCFCFCFCFCFCL;

        public static ulong[] maskFile = {
        0x0101010101010101UL,
        0x0202020202020202L,
        0x0404040404040404L,
        0x0808080808080808L,
        0x1010101010101010L,
        0x2020202020202020L,
        0x4040404040404040L,
        0x8080808080808080L
    };

        public static ulong maskRow1 = 0x00000000000000FFL;
        public static ulong maskRow2 = 0x000000000000FF00L;
        public static ulong maskRow3 = 0x0000000000FF0000L;
        public static ulong maskRow4 = 0x00000000FF000000L;
        public static ulong maskRow5 = 0x000000FF00000000L;
        public static ulong maskRow6 = 0x0000FF0000000000L;
        public static ulong maskRow7 = 0x00FF000000000000L;
        public static ulong maskRow8 = 0xFF00000000000000L;
        public static ulong maskRow1Row8 = 0xFF000000000000FFL;

        public static ulong maskDarkSq = 0xAA55AA55AA55AA55L;
        public static ulong maskLightSq = 0x55AA55AA55AA55AAL;

        public static ulong maskCorners = 0x8100000000000081UL;

        private static void Co1()
        {
            // Compute king attacks
            kingAttacks = new ulong[64];

            for (int sq = 0; sq < 64; sq++)
            {
                ulong m = 1UL << sq;
                ulong mask = (((m >> 1) | (m << 7) | (m >> 9)) & maskAToGFiles) |
                            (((m << 1) | (m << 9) | (m >> 7)) & maskBToHFiles) |
                            (m << 8) | (m >> 8);
                kingAttacks[sq] = mask;
            }

            // Compute knight attacks
            knightAttacks = new ulong[64];
            for (int sq = 0; sq < 64; sq++)
            {
                ulong m = 1UL << sq;
                ulong mask = (((m << 6) | (m >> 10)) & maskAToFFiles) |
                            (((m << 15) | (m >> 17)) & maskAToGFiles) |
                            (((m << 17) | (m >> 15)) & maskBToHFiles) |
                            (((m << 10) | (m >> 6)) & maskCToHFiles);
                knightAttacks[sq] = mask;
            }

            // Compute pawn attacks
            wPawnAttacks = new ulong[64];
            bPawnAttacks = new ulong[64];
            wPawnBlockerMask = new ulong[64];
            bPawnBlockerMask = new ulong[64];
            for (int sq = 0; sq < 64; sq++)
            {
                ulong m = 1UL << sq;
                ulong mask = ((m << 7) & maskAToGFiles) | ((m << 9) & maskBToHFiles);
                wPawnAttacks[sq] = mask;
                mask = ((m >> 9) & maskAToGFiles) | ((m >> 7) & maskBToHFiles);
                bPawnAttacks[sq] = mask;

                int x = Position.getX(sq);
                int y = Position.getY(sq);
                m = 0;
                for (int y2 = y + 1; y2 < 8; y2++)
                {
                    if (x > 0) m |= 1UL << Position.getSquare(x - 1, y2);
                    m |= 1UL << Position.getSquare(x, y2);
                    if (x < 7) m |= 1UL << Position.getSquare(x + 1, y2);
                }
                wPawnBlockerMask[sq] = m;
                m = 0;
                for (int y2 = y - 1; y2 >= 0; y2--)
                {
                    if (x > 0) m |= 1UL << Position.getSquare(x - 1, y2);
                    m |= 1UL << Position.getSquare(x, y2);
                    if (x < 7) m |= 1UL << Position.getSquare(x + 1, y2);
                }
                bPawnBlockerMask[sq] = m;
            }
        }

        private static ulong[][] rTables;
        private static ulong[] rMasks;
        private static int[] rBits = { 12, 11, 11, 11, 11, 11, 11, 12,
                                         11, 10, 10, 10, 10, 10, 10, 11,
                                         11, 10, 10, 10, 10, 10, 10, 11,
                                         11, 10, 10, 10, 10, 10, 10, 11,
                                         11, 10, 10, 10, 10, 10, 10, 11,
                                         11, 10, 10, 10, 10, 10, 10, 11,
                                         10,  9,  9,  9,  9,  9, 10, 10,
                                         11, 10, 10, 10, 10, 11, 11, 11 };
        private static ulong[] rMagics = {
        0x0080011084624000L, 0x1440031000200141UL, 0x2080082004801000L, 0x0100040900100020L,
        0x0200020010200408L, 0x0300010008040002L, 0x040024081000a102L, 0x0080003100054680L,
        0x1100800040008024L, 0x8440401000200040L, 0x0432001022008044L, 0x0402002200100840L,
        0x4024808008000400L, 0x100a000410820008L, 0x8042001144020028L, 0x2451000041002082L,
        0x1080004000200056L, 0xd41010c020004000L, 0x0004410020001104L, 0x0000818050000800L,
        0x0000050008010010L, 0x0230808002000400L, 0x2000440090022108L, 0x0488020000811044L,
        0x8000410100208006L, 0x2000a00240100140L, 0x2088802200401600L, 0x0a10100180080082L,
        0x0000080100110004L, 0x0021002300080400L, 0x8400880400010230L, 0x2001008200004401UL,
        0x0000400022800480L, 0x00200040e2401000L, 0x4004100084802000L, 0x0218800800801002L,
        0x0420800800800400L, 0x002a000402001008L, 0x0e0b000401008200L, 0x0815908072000401UL,
        0x1840008002498021UL, 0x1070122002424000L, 0x1040200100410010L, 0x0600080010008080L,
        0x0215001008010004L, 0x0000020004008080L, 0x1300021051040018L, 0x0004040040820001UL,
        0x48fffe99fecfaa00L, 0x48fffe99fecfaa00L, 0x497fffadff9c2e00L, 0x613fffddffce9200L,
        0xffffffe9ffe7ce00L, 0xfffffff5fff3e600L, 0x2000080281100400L, 0x510ffff5f63c96a0L,
        0xebffffb9ff9fc526L, 0x61fffeddfeedaeaeL, 0x53bfffedffdeb1a2L, 0x127fffb9ffdfb5f6L,
        0x411fffddffdbf4d6L, 0x0005000208040001UL, 0x264038060100d004L, 0x7645fffecbfea79eL,
    };
        private static ulong[][] bTables;
        private static ulong[] bMasks;
        private static int[] bBits = { 5, 4, 5, 5, 5, 5, 4, 5,
                                         4, 4, 5, 5, 5, 5, 4, 4,
                                         4, 4, 7, 7, 7, 7, 4, 4,
                                         5, 5, 7, 9, 9, 7, 5, 5,
                                         5, 5, 7, 9, 9, 7, 5, 5,
                                         4, 4, 7, 7, 7, 7, 4, 4,
                                         4, 4, 5, 5, 5, 5, 4, 4,
                                         5, 4, 5, 5, 5, 5, 4, 5 };
        private static ulong[] bMagics = {
        0xffedf9fd7cfcffffL, 0xfc0962854a77f576L, 0x9010210041047000L, 0x52242420800c0000L,
        0x884404220480004aL, 0x0002080248000802L, 0xfc0a66c64a7ef576L, 0x7ffdfdfcbd79ffffL,
        0xfc0846a64a34fff6L, 0xfc087a874a3cf7f6L, 0x02000888010a2211UL, 0x0040044040801808L,
        0x0880040420000000L, 0x0000084110109000L, 0xfc0864ae59b4ff76L, 0x3c0860af4b35ff76L,
        0x73c01af56cf4cffbL, 0x41a01cfad64aaffcL, 0x1010000200841104L, 0x802802142a006000L,
        0x0a02000412020020L, 0x0000800040504030L, 0x7c0c028f5b34ff76L, 0xfc0a028e5ab4df76L,
        0x0020082044905488L, 0xa572211102080220L, 0x0014020001280300L, 0x0220208058008042L,
        0x0001010000104016L, 0x0005114028080800L, 0x0202640000848800L, 0x040040900a008421UL,
        0x400e094000600208L, 0x800a100400120890L, 0x0041229001480020L, 0x0000020080880082L,
        0x0040002020060080L, 0x1819100100c02400L, 0x04112a4082c40400L, 0x0001240130210500L,
        0xdcefd9b54bfcc09fL, 0xf95ffa765afd602bL, 0x008200222800a410L, 0x0100020102406400L,
        0x80a8040094000200L, 0x002002006200a041UL, 0x43ff9a5cf4ca0c01UL, 0x4bffcd8e7c587601UL,
        0xfc0ff2865334f576L, 0xfc0bf6ce5924f576L, 0x0900420442088104L, 0x0062042084040010L,
        0x01380810220a0240L, 0x0000101002082800L, 0xc3ffb7dc36ca8c89L, 0xc3ff8a54f4ca2c89L,
        0xfffffcfcfd79edffL, 0xfc0863fccb147576L, 0x0050009040441000L, 0x00139a0000840400L,
        0x9080000412220a00L, 0x0000002020010a42L, 0xfc087e8e4bb2f736L, 0x43ff9e4ef4ca2c89L,
    };

        private static ulong createPattern(int i, ulong mask)
        {
            ulong ret = 0L;
            for (int j = 0; ; j++)
            {
                ulong nextMask = mask & (mask - 1);
                ulong bit = mask ^ nextMask;
                if ((i & (1 /*1L*/ << j)) != 0)
                    ret |= bit;
                mask = nextMask;
                if (mask == 0)
                    break;
            }
            return ret;
        }

        private static ulong addRookRays(int x, int y, ulong occupied, bool inner)
        {
            ulong mask = 0;
            mask = addRay(mask, x, y, 1, 0, occupied, inner);
            mask = addRay(mask, x, y, -1, 0, occupied, inner);
            mask = addRay(mask, x, y, 0, 1, occupied, inner);
            mask = addRay(mask, x, y, 0, -1, occupied, inner);
            return mask;
        }
        private static ulong addBishopRays(int x, int y, ulong occupied, bool inner)
        {
            ulong mask = 0;
            mask = addRay(mask, x, y, 1, 1, occupied, inner);
            mask = addRay(mask, x, y, -1, -1, occupied, inner);
            mask = addRay(mask, x, y, 1, -1, occupied, inner);
            mask = addRay(mask, x, y, -1, 1, occupied, inner);
            return mask;
        }

        private static ulong addRay(ulong mask, int x, int y, int dx, int dy,
                                         ulong occupied, bool inner)
        {
            int lo = inner ? 1 : 0;
            int hi = inner ? 6 : 7;
            while (true)
            {
                if (dx != 0)
                {
                    x += dx; if ((x < lo) || (x > hi)) break;
                }
                if (dy != 0)
                {
                    y += dy; if ((y < lo) || (y > hi)) break;
                }
                int sq = Position.getSquare(x, y);
                mask |= 1UL << sq;
                if ((occupied & (1UL << sq)) != 0)
                    break;
            }
            return mask;
        }

        private static void Co2()
        { // Rook magics
            rTables = new ulong[64][];
            rMasks = new ulong[64];
            for (int sq = 0; sq < 64; sq++)
            {
                int x = Position.getX(sq);
                int y = Position.getY(sq);
                rMasks[sq] = addRookRays(x, y, 0L, true);
                int tableSize = 1 << rBits[sq];
                ulong[] table = new ulong[tableSize];
                for (int i = 0; i < tableSize; i++) table[i] = Defs.ulongN1;    /* -1 */
                int nPatterns = 1 << BITS.bitCount(rMasks[sq]);
                for (int i = 0; i < nPatterns; i++)
                {
                    ulong p = createPattern(i, rMasks[sq]);
                    int entry = (int)((p * rMagics[sq]) >> (64 - rBits[sq]));
                    ulong atks = addRookRays(x, y, p, false);
                    if (table[entry] == Defs.ulongN1 /* -1 */)
                    {
                        table[entry] = atks;
                    }
                    else if (table[entry] != atks)
                    {
                        throw new RuntimeException();
                    }
                }
                rTables[sq] = table;
            }
        }

        private static void Co3()
        { // Bishop magics
            bTables = new ulong[64][];
            bMasks = new ulong[64];
            for (int sq = 0; sq < 64; sq++)
            {
                int x = Position.getX(sq);
                int y = Position.getY(sq);
                bMasks[sq] = addBishopRays(x, y, 0L, true);
                int tableSize = 1 << bBits[sq];
                ulong[] table = new ulong[tableSize];
                for (int i = 0; i < tableSize; i++) table[i] = Defs.ulongN1;    /* -1 */
                int nPatterns = 1 << BITS.bitCount(bMasks[sq]);
                for (int i = 0; i < nPatterns; i++)
                {
                    ulong p = createPattern(i, bMasks[sq]);
                    int entry = (int)((p * bMagics[sq]) >> (64 - bBits[sq]));
                    ulong atks = addBishopRays(x, y, p, false);
                    if (table[entry] == Defs.ulongN1 /* -1 */)
                    {
                        table[entry] = atks;
                    }
                    else if (table[entry] != atks)
                    {
                        throw new RuntimeException();
                    }
                }
                bTables[sq] = table;
            }
        }

        public static ulong bishopAttacks(int sq, ulong occupied)
        {
            return bTables[sq][(int)(((occupied & bMasks[sq]) * bMagics[sq]) >> (64 - bBits[sq]))];
        }

        public static ulong rookAttacks(int sq, ulong occupied)
        {
            return rTables[sq][(int)(((occupied & rMasks[sq]) * rMagics[sq]) >> (64 - rBits[sq]))];
        }

        public static ulong[][] squaresBetween;
        private static void Co4()
        {
            squaresBetween = new ulong[64][];
            for (int sq1 = 0; sq1 < 64; sq1++)
            {
                squaresBetween[sq1] = new ulong[64];
                for (int j = 0; j < 64; j++)
                    squaresBetween[sq1][j] = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if ((dx == 0) && (dy == 0))
                            continue;
                        ulong m = 0;
                        int x = Position.getX(sq1);
                        int y = Position.getY(sq1);
                        while (true)
                        {
                            x += dx; y += dy;
                            if ((x < 0) || (x > 7) || (y < 0) || (y > 7))
                                break;
                            int sq2 = Position.getSquare(x, y);
                            squaresBetween[sq1][sq2] = m;
                            m |= 1UL << sq2;
                        }
                    }
                }
            }
        }

        private static /*byte[]*/ int[] dirTable = {
            -9,   0,   0,   0,   0,   0,   0,  -8,   0,   0,   0,   0,   0,   0,  -7,
        0,   0,  -9,   0,   0,   0,   0,   0,  -8,   0,   0,   0,   0,   0,  -7,   0,
        0,   0,   0,  -9,   0,   0,   0,   0,  -8,   0,   0,   0,   0,  -7,   0,   0,
        0,   0,   0,   0,  -9,   0,   0,   0,  -8,   0,   0,   0,  -7,   0,   0,   0,
        0,   0,   0,   0,   0,  -9,   0,   0,  -8,   0,   0,  -7,   0,   0,   0,   0,
        0,   0,   0,   0,   0,   0,  -9, -17,  -8, -15,  -7,   0,   0,   0,   0,   0,
        0,   0,   0,   0,   0,   0, -10,  -9,  -8,  -7,  -6,   0,   0,   0,   0,   0,
        0,  -1,  -1,  -1,  -1,  -1,  -1,  -1,   0,   1,   1,   1,   1,   1,   1,   1,
        0,   0,   0,   0,   0,   0,   6,   7,   8,   9,  10,   0,   0,   0,   0,   0,
        0,   0,   0,   0,   0,   0,   7,  15,   8,  17,   9,   0,   0,   0,   0,   0,
        0,   0,   0,   0,   0,   7,   0,   0,   8,   0,   0,   9,   0,   0,   0,   0,
        0,   0,   0,   0,   7,   0,   0,   0,   8,   0,   0,   0,   9,   0,   0,   0,
        0,   0,   0,   7,   0,   0,   0,   0,   8,   0,   0,   0,   0,   9,   0,   0,
        0,   0,   7,   0,   0,   0,   0,   0,   8,   0,   0,   0,   0,   0,   9,   0,
        0,   7,   0,   0,   0,   0,   0,   0,   8,   0,   0,   0,   0,   0,   0,   9
    };

        public static int getDirection(int from, int to)
        {
            int offs = to + (to | 7) - from - (from | 7) + 0x77;
            return dirTable[offs];
        }

        public static ulong southFill(ulong mask)
        {
            mask |= (mask >> 8);
            mask |= (mask >> 16);
            mask |= (mask >> 32);
            return mask;
        }

        public static ulong northFill(ulong mask)
        {
            mask |= (mask << 8);
            mask |= (mask << 16);
            mask |= (mask << 32);
            return mask;
        }

        private static int[] trailingZ = {
        63,  0, 58,  1, 59, 47, 53,  2,
        60, 39, 48, 27, 54, 33, 42,  3,
        61, 51, 37, 40, 49, 18, 28, 20,
        55, 30, 34, 11, 43, 14, 22,  4,
        62, 57, 46, 52, 38, 26, 32, 41,
        50, 36, 17, 19, 29, 10, 13, 21,
        56, 45, 25, 31, 35, 16,  9, 12,
        44, 24, 15,  8, 23,  7,  6,  5
    };

        public static int numberOfTrailingZeros(ulong mask)
        {
            return trailingZ[(int)(((mask & BITS.Neg(mask) /* -mask */) * 0x07EDD5E59A4E28C2L) >> 58)];
        }
    }
    //// End class
}
//// End namespace