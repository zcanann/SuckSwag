using System;
using System.Collections.Generic;

namespace SuckSwag
{
    /// <summary>
    /// Implements an opening book.
    /// </summary>
    public class Book
    {
        public class BookEntry
        {
            public Move move;
            public int count;
            public BookEntry(Move move)
            {
                this.move = move;
                count = 1;
            }
        }

        private static Dictionary<ulong, List<BookEntry>> bookMap;
        private static int numBookMoves = -1;

        public Book(bool verbose)
        {
            if (numBookMoves < 0)
            {
                initBook(verbose);
            }
        }

        private void initBook(bool verbose)
        {
            bookMap = new Dictionary<ulong, List<BookEntry>>();
            long t0 = SystemHelper.currentTimeMillis();
            numBookMoves = 0;
            try
            {
                /* read /book.bin into buf */
                Byte[] buf = Bookbin.DATA;
                Position startPos = TextIO.readFEN(TextIO.startPosFEN);
                Position pos = new Position(startPos);
                UndoInfo ui = new UndoInfo();
                int len = buf.Length;
                for (int i = 0; i < len; i += 2)
                {
                    int b0 = buf[i]; if (b0 < 0) b0 += 256;
                    int b1 = buf[i + 1]; if (b1 < 0) b1 += 256;
                    int move = (b0 << 8) + b1;
                    if (move == 0)
                    {
                        pos = new Position(startPos);
                    }
                    else
                    {
                        bool bad = ((move >> 15) & 1) != 0;
                        int prom = (move >> 12) & 7;
                        Move m = new Move(move & 63, (move >> 6) & 63,
                                          promToPiece(prom, pos.whiteMove));
                        if (!bad)
                            addToBook(pos, m);
                        pos.makeMove(m, ui);
                    }
                }
            }
            catch
            {
                SystemHelper.println("Can't read opening book resource");
                throw new RuntimeException();
            }
            if (verbose)
            {
                long t1 = SystemHelper.currentTimeMillis();
                SystemHelper.println("Book moves: " + numBookMoves.ToString() +
                    "(parse time: " + ((t1 - t0) / 1000).ToString() + ")");
            }
        }

        /** Add a move to a position in the opening book. */
        private void addToBook(Position pos, Move moveToAdd)
        {
            ulong key = pos.zobristHash();
            bool iskey = bookMap.ContainsKey(key);
            List<BookEntry> ent = (iskey ? bookMap[key] : null);
            if (ent == null)
            {
                ent = new List<BookEntry>();
                bookMap.Add(pos.zobristHash(), ent);
            }
            for (int i = 0; i < ent.Count; i++)
            {
                BookEntry be = ent[i];
                if (be.move.equals(moveToAdd))
                {
                    be.count++;
                    return;
                }
            }
            BookEntry be2 = new BookEntry(moveToAdd);
            ent.Add(be2);
            numBookMoves++;
        }

        /** Return a random book move for a position, or null if out of book. */
        public Move getBookMove(Position pos)
        {

            long t0 = SystemHelper.currentTimeMillis();
            Random rndGen = new Random((int)t0);
            ulong key = pos.zobristHash();
            bool iskey = bookMap.ContainsKey(key);
            List<BookEntry> bookMoves = (iskey ? bookMap[key] : null);
            if (bookMoves == null)
            {
                return null;
            }

            MoveGen.MoveList legalMoves = new MoveGen().pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, legalMoves);
            int sum = 0;
            for (int i = 0; i < bookMoves.Count; i++)
            {
                BookEntry be = bookMoves[i];
                bool contains = false;
                for (int mi = 0; mi < legalMoves.size; mi++)
                    if (legalMoves.m[mi].equals(be.move))
                    {
                        contains = true;
                        break;
                    }
                if (!contains)
                {
                    // If an illegal move was found, it means there was a hash collision.
                    return null;
                }
                sum += getWeight(bookMoves[i].count);
            }
            if (sum <= 0)
            {
                return null;
            }
            int rnd = rndGen.Next(sum);
            sum = 0;
            for (int i = 0; i < bookMoves.Count; i++)
            {
                sum += getWeight(bookMoves[i].count);
                if (rnd < sum)
                {
                    return bookMoves[i].move;
                }
            }
            // Should never get here
            throw new RuntimeException();
        }

        private int getWeight(int count)
        {
            double tmp = Math.Sqrt(count);
            return (int)(tmp * Math.Sqrt(tmp) * 100 + 1);
        }

        /** Return a string describing all book moves. */
        public string getAllBookMoves(Position pos)
        {
            string ret = "";
            List<BookEntry> bookMoves = bookMap[pos.zobristHash()];
            if (bookMoves != null)
            {
                BookEntry be;
                for (int i = 0; i < bookMoves.Count; i++)
                {
                    be = bookMoves[i];
                    string moveStr = TextIO.moveTostring(pos, be.move, false);
                    ret = ret + (moveStr + "(" + be.count.ToString() + ") ");
                }
            }
            return ret;
        }

        /** Creation of the book.bin file is omitted while porting from java to C# */


        private static int pieceToProm(int p)
        {
            switch (p)
            {
                case Piece.WQUEEN:
                case Piece.BQUEEN:
                    return 1;
                case Piece.WROOK:
                case Piece.BROOK:
                    return 2;
                case Piece.WBISHOP:
                case Piece.BBISHOP:
                    return 3;
                case Piece.WKNIGHT:
                case Piece.BKNIGHT:
                    return 4;
                default:
                    return 0;
            }
        }

        private static int promToPiece(int prom, bool whiteMove)
        {
            switch (prom)
            {
                case 1: return whiteMove ? Piece.WQUEEN : Piece.BQUEEN;
                case 2: return whiteMove ? Piece.WROOK : Piece.BROOK;
                case 3: return whiteMove ? Piece.WBISHOP : Piece.BBISHOP;
                case 4: return whiteMove ? Piece.WKNIGHT : Piece.BKNIGHT;
                default: return Piece.EMPTY;
            }
        }
    }
    //// End class
}
//// End namespace