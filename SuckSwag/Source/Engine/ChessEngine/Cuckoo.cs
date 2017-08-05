using System;
using System.Diagnostics;

namespace SuckSwag.Source.ChessEngine
{
    public class Cuckoo
    {
        // This calls lots of constructors needed for variables definition
        public static BitBoard defBitBoard = new BitBoard();
        public static Evaluate defEvaluate = new Evaluate();

        public static ComputerPlayer CuckComp;
        public static HumanPlayer CuckHumn;
        public static Game CuckGM;
        public static Book CuckBK;

        public Cuckoo()
        {
            // Or just run samples:
            Sample();

        }

        public static string simplyCalculateMove(string sFEN, int depth)
        {
            bool w2m = sFEN.Contains(" w ");
            CuckComp = new ComputerPlayer();
            CuckHumn = new HumanPlayer();
            if (w2m) CuckGM = new Game(CuckComp, CuckHumn);
            else CuckGM = new Game(CuckHumn, CuckComp);
            CuckGM.handleCommand("setpos " + sFEN);
            Position pos = CuckGM.getPos();

            CuckComp.maxTimeMillis = 1 * 100;
            CuckComp.maxTimeMillis = 6 * 100;
            CuckComp.maxDepth = depth;

            string CurrentPositionFEN = TextIO.toFEN(pos);

            string cmd = CuckComp.getCommand(new Position(pos),
                    CuckGM.haveDrawOffer(), CuckGM.getHistory());

            string a = cmd;
            if ((a.Length > 1) && (("NBRQK").IndexOf(a[0]) >= 0))
                a = a.Substring(1);
            if (a[0] == 'O')
                a = (a.Length == 3 ? (w2m ? "e1g1" : "e8g8") : (w2m ? "e1c1" : "e8c8"));
            else
                a = ((a.Length > 4) ? a.Substring(0, 2) + a.Substring(3) : "");
            return a;
        }

        private void Sample()
        {
            CuckComp = new ComputerPlayer();
            CuckHumn = new HumanPlayer();
            CuckBK = new Book(false);
            CuckGM = new Game(CuckHumn, CuckComp);
            Position pos = CuckGM.getPos();

            // e4(102) d4(31) ...
            string CurrentBookMoves = CuckBK.getAllBookMoves(pos);

            // Nb1-a3;Nb1-c3;...;a2-a3;a2-a4;...
            string CurrentValidMoves = TextIO.AllMovesTostring(pos, true);

            // RNB...w KQkq... 
            string CurrentPositionFEN = TextIO.toFEN(pos);

            // Display board to console
            TextIO.DispBoard(pos);

            // Swap & move
            CuckGM.whitePlayer = CuckComp;
            CuckGM.blackPlayer = CuckHumn;
            ////CuckComp.bookEnabled = false;
            CuckComp.maxTimeMillis = 1 * 100;
            CuckComp.maxTimeMillis = 6 * 100;
            ////CuckComp.maxDepth = 6;

            // Ng1-f3
            string CommandFromComp = CuckComp.getCommand(new Position(pos), CuckGM.haveDrawOffer(), CuckGM.getHistory());
        }
    }

    // Helping classes
    public class Defs
    {
        public static ulong ulongN1 = 0xFFFFFFFFFFFFFFFF;
    }

    public class SystemHelper
    {
        public static long currentTimeMillis()
        {
            return DateTime.UtcNow.Millisecond;
        }

        public static void println(string s)
        {
            Debug.WriteLine(s);
        }
    }

    public class RuntimeException : System.Exception
    {
        public RuntimeException()
        {
            SystemHelper.println("RuntimeException");
        }
    }

    public class NumberFormatException : Exception
    {
        public NumberFormatException()
        {
            SystemHelper.println("NumberFormatException");
        }
    }

    public class IOException : Exception
    {
        public IOException()
        {
            SystemHelper.println("IOException");
        }
    }

    public class NoSuchAlgorithmException : Exception
    {
        public NoSuchAlgorithmException()
        {
            SystemHelper.println("NoSuchAlgorithmException");
        }
    }

    public class UnsupportedOperationException : Exception
    {
        public UnsupportedOperationException()
        {
            SystemHelper.println("UnsupportedOperationException");
        }
    }

    public class UnsupportedSHA1OperationException : Exception
    {
        public UnsupportedSHA1OperationException()
        {
            SystemHelper.println("Unsupported SHA-1 OperationException");
        }
    }

    public class ChessParseError : Exception
    {
        public ChessParseError(string error)
        {
            SystemHelper.println(error);
        }
    }

    public static class BITS
    {
        private static bool bitcntinit = false;
        private static ulong[] BIT;
        private static byte[] LSB;
        private static byte[] BITC;

        private static int LOW16(ulong x) { return (int)((x) & 0xFFFF); }
        private static int LOW32(ulong x) { return (int)((x) & 0xFFFFFFFFL); }
        private static ulong L32(ulong x) { return ((x) & 0xFFFFFFFFL); }

        public static ulong Neg(ulong n) { return ((~n) + 1); }

        private static byte _bitcnt(ulong bit)
        {
            byte c = 0;
            while (bit != 0) { bit &= (bit - 1); c++; }
            return c;
        }

        public static byte bitCount(ulong n)
        {
            if (!bitcntinit)
            {
                BIT = new ulong[64];
                LSB = new byte[0x10000];
                BITC = new byte[0x10000];
                for (ulong i = 0; i < 0x10000; i++) BITC[i] = _bitcnt(i);
                bitcntinit = true;
            }
            byte a1 = (BITC[LOW16(n)]);
            ulong g2 = n >> 16;
            byte a2 = (BITC[LOW16(g2)]);
            ulong g3 = n >> 32;
            byte a3 = (BITC[LOW16(g3)]);
            ulong g4 = n >> 48;
            byte a4 = (BITC[LOW16(g4)]);

            return (byte)(BITC[LOW16(n)]
                + BITC[LOW16(n >> 16)]
                + BITC[LOW16(n >> 32)]
                + BITC[LOW16(n >> 48)]);
        }
    }
    //// End class
}
//// End namespace