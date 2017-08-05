namespace SuckSwag.Source.ChessEngine
{
    public class TextIO
    {
        static public string startPosFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        /// Parse a FEN string and return a chess Position object. 
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public static Position readFEN(string fen)
        {
            Position pos = new Position();
            string[] words = fen.Split(' ');

            if (words.Length < 2)
            {
                throw new ChessParseError("Too few pieces");
            }

            // Piece placement
            int row = 7;
            int col = 0;

            for (int i = 0; i < words[0].Length; i++)
            {
                char c = words[0][i];

                switch (c)
                {
                    case '1':
                        col += 1;
                        break;
                    case '2':
                        col += 2;
                        break;
                    case '3':
                        col += 3;
                        break;
                    case '4':
                        col += 4;
                        break;
                    case '5':
                        col += 5;
                        break;
                    case '6':
                        col += 6;
                        break;
                    case '7':
                        col += 7;
                        break;
                    case '8':
                        col += 8;
                        break;
                    case '/':
                        row--; col = 0;
                        break;
                    case 'P':
                        safeSetPiece(pos, col, row, Piece.WPAWN); col++;
                        break;
                    case 'N':
                        safeSetPiece(pos, col, row, Piece.WKNIGHT); col++;
                        break;
                    case 'B':
                        safeSetPiece(pos, col, row, Piece.WBISHOP); col++;
                        break;
                    case 'R':
                        safeSetPiece(pos, col, row, Piece.WROOK); col++;
                        break;
                    case 'Q':
                        safeSetPiece(pos, col, row, Piece.WQUEEN); col++;
                        break;
                    case 'K':
                        safeSetPiece(pos, col, row, Piece.WKING); col++;
                        break;
                    case 'p':
                        safeSetPiece(pos, col, row, Piece.BPAWN); col++;
                        break;
                    case 'n':
                        safeSetPiece(pos, col, row, Piece.BKNIGHT); col++;
                        break;
                    case 'b':
                        safeSetPiece(pos, col, row, Piece.BBISHOP); col++;
                        break;
                    case 'r':
                        safeSetPiece(pos, col, row, Piece.BROOK); col++;
                        break;
                    case 'q':
                        safeSetPiece(pos, col, row, Piece.BQUEEN); col++;
                        break;
                    case 'k':
                        safeSetPiece(pos, col, row, Piece.BKING); col++;
                        break;
                    default: throw new ChessParseError("Invalid piece");
                }
            }

            if (words[1].Length == 0)
            {
                throw new ChessParseError("Invalid side");
            }

            pos.setWhiteMove(words[1][0] == 'w');

            // Castling rights
            int castleMask = 0;
            if (words.Length > 2)
            {
                for (int i = 0; i < words[2].Length; i++)
                {
                    char c = words[2][i];
                    switch (c)
                    {
                        case 'K':
                            castleMask |= (1 << Position.H1_CASTLE);
                            break;
                        case 'Q':
                            castleMask |= (1 << Position.A1_CASTLE);
                            break;
                        case 'k':
                            castleMask |= (1 << Position.H8_CASTLE);
                            break;
                        case 'q':
                            castleMask |= (1 << Position.A8_CASTLE);
                            break;
                        case '-':
                            break;
                        default:
                            throw new ChessParseError("Invalid castling flags");
                    }
                }
            }
            pos.setCastleMask(castleMask);

            if (words.Length > 3)
            {
                // En passant target square
                string epstring = words[3];
                if (epstring != "-")
                {
                    if (epstring.Length < 2)
                    {
                        throw new ChessParseError("Invalid en passant square");
                    }
                    pos.setEpSquare(getSquare(epstring));
                }
            }

            try
            {
                if (words.Length > 4)
                {
                    pos.halfMoveClock = int.Parse(words[4]);
                }
                if (words.Length > 5)
                {
                    pos.fullMoveCounter = int.Parse(words[5]);
                }
            }
            catch
            {
                // Ignore errors here, since the fields are optional
            }

            // Each side must have exactly one king
            int wKings = 0;
            int bKings = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int p = pos.getPiece(Position.getSquare(x, y));
                    if (p == Piece.WKING)
                    {
                        wKings++;
                    }
                    else if (p == Piece.BKING)
                    {
                        bKings++;
                    }
                }
            }

            if (wKings != 1)
            {
                throw new ChessParseError("White must have exactly one king");
            }

            if (bKings != 1)
            {
                throw new ChessParseError("Black must have exactly one king");
            }

            // Make sure king can not be captured
            Position pos2 = new Position(pos);
            pos2.setWhiteMove(!pos.whiteMove);

            if (MoveGen.inCheck(pos2))
            {
                throw new ChessParseError("King capture possible");
            }

            fixupEPSquare(pos);

            return pos;
        }

        /// <summary>
        /// Remove pseudo-legal EP square if it is not legal, ie would leave king in check.
        /// </summary>
        /// <param name="pos"></param>
        public static void fixupEPSquare(Position pos)
        {
            int epSquare = pos.getEpSquare();
            if (epSquare >= 0)
            {
                MoveGen MG = new MoveGen();
                MoveGen.MoveList moves = MG.pseudoLegalMoves(pos);
                MoveGen.RemoveIllegal(pos, moves);
                bool epValid = false;
                for (int mi = 0; mi < moves.size; mi++)
                {
                    Move m = moves.m[mi];
                    if (m.to == epSquare)
                    {
                        if (pos.getPiece(m.from) == (pos.whiteMove ? Piece.WPAWN : Piece.BPAWN))
                        {
                            epValid = true;
                            break;
                        }
                    }
                }
                if (!epValid)
                {
                    pos.setEpSquare(-1);
                }
            }
        }

        private static void safeSetPiece(Position pos, int col, int row, int p)
        {
            if (row < 0)
            {
                throw new ChessParseError("Too many rows");
            }

            if (col > 7)
            {
                throw new ChessParseError("Too many columns");
            }

            if ((p == Piece.WPAWN) || (p == Piece.BPAWN))
            {
                if ((row == 0) || (row == 7))
                {
                    throw new ChessParseError("Pawn on first/last rank");
                }
            }

            pos.setPiece(Position.getSquare(col, row), p);
        }

        /// <summary>
        /// Return a FEN string corresponding to a chess Position object.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string toFEN(Position pos)
        {
            string ret = "";

            // Piece placement
            for (int r = 7; r >= 0; r--)
            {
                int numEmpty = 0;
                for (int c = 0; c < 8; c++)
                {
                    int p = pos.getPiece(Position.getSquare(c, r));
                    if (p == Piece.EMPTY)
                    {
                        numEmpty++;
                    }
                    else
                    {
                        if (numEmpty > 0)
                        {
                            ret += numEmpty.ToString();
                            numEmpty = 0;
                        }
                        switch (p)
                        {
                            case Piece.WKING: ret += ('K'); break;
                            case Piece.WQUEEN: ret += ('Q'); break;
                            case Piece.WROOK: ret += ('R'); break;
                            case Piece.WBISHOP: ret += ('B'); break;
                            case Piece.WKNIGHT: ret += ('N'); break;
                            case Piece.WPAWN: ret += ('P'); break;
                            case Piece.BKING: ret += ('k'); break;
                            case Piece.BQUEEN: ret += ('q'); break;
                            case Piece.BROOK: ret += ('r'); break;
                            case Piece.BBISHOP: ret += ('b'); break;
                            case Piece.BKNIGHT: ret += ('n'); break;
                            case Piece.BPAWN: ret += ('p'); break;
                            default: throw new RuntimeException();
                        }
                    }
                }
                if (numEmpty > 0)
                {
                    ret += numEmpty.ToString();
                }
                if (r > 0)
                {
                    ret += ('/');
                }
            }
            ret += (pos.whiteMove ? " w " : " b ");

            // Castling rights
            bool anyCastle = false;
            if (pos.h1Castle())
            {
                ret += ('K');
                anyCastle = true;
            }
            if (pos.a1Castle())
            {
                ret += ('Q');
                anyCastle = true;
            }
            if (pos.h8Castle())
            {
                ret += ('k');
                anyCastle = true;
            }
            if (pos.a8Castle())
            {
                ret += ('q');
                anyCastle = true;
            }
            if (!anyCastle)
            {
                ret += ('-');
            }

            // En passant target square
            {
                ret += (' ');
                if (pos.getEpSquare() >= 0)
                {
                    int x = Position.getX(pos.getEpSquare());
                    int y = Position.getY(pos.getEpSquare());
                    ret += ((char)(x + 'a'));
                    ret += ((char)(y + '1'));
                }
                else
                {
                    ret += ('-');
                }
            }

            // Move counters
            ret += " " + pos.halfMoveClock;
            ret += " " + pos.fullMoveCounter;

            return ret;
        }

        /// <summary>
        /// To get all valid moves in one text-string
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ulongForm"></param>
        /// <returns></returns>
        public static string AllMovesTostring(Position pos, bool ulongForm)
        {
            string ret = "";
            MoveGen MG = new MoveGen();
            MoveGen.MoveList moves = MG.pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            for (int i = 0; i < moves.size; i++)
            {
                ret += moveTostring(pos, moves.m[i], ulongForm, moves) + ";";
            }
            return ret;
        }

        /// <summary>
        /// Convert a chess move to human readable form.
        /// </summary>
        /// <param name="pos">The chess position.</param>
        /// <param name="move">The executed move.</param>
        /// <param name="ulongForm">If true, use ulong notation, eg Ng1-f3. Otherwise, use short notation, eg Nf3</param>
        /// <returns></returns>
        public static string moveTostring(Position pos, Move move, bool ulongForm)
        {
            MoveGen MG = new MoveGen();
            MoveGen.MoveList moves = MG.pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            return moveTostring(pos, move, ulongForm, moves);
        }

        private static string moveTostring(Position pos, Move move, bool ulongForm, MoveGen.MoveList moves)
        {
            string ret = "";
            int wKingOrigPos = Position.getSquare(4, 0);
            int bKingOrigPos = Position.getSquare(4, 7);
            if (move.from == wKingOrigPos && pos.getPiece(wKingOrigPos) == Piece.WKING)
            {
                // Check white castle
                if (move.to == Position.getSquare(6, 0))
                {
                    ret += ("O-O");
                }
                else if (move.to == Position.getSquare(2, 0))
                {
                    ret += ("O-O-O");
                }
            }
            else if (move.from == bKingOrigPos && pos.getPiece(bKingOrigPos) == Piece.BKING)
            {
                // Check white castle
                if (move.to == Position.getSquare(6, 7))
                {
                    ret += ("O-O");
                }
                else if (move.to == Position.getSquare(2, 7))
                {
                    ret += ("O-O-O");
                }
            }
            if (ret.Length == 0)
            {
                int p = pos.getPiece(move.from);
                ret += pieceToChar(p);
                int x1 = Position.getX(move.from);
                int y1 = Position.getY(move.from);
                int x2 = Position.getX(move.to);
                int y2 = Position.getY(move.to);
                if (ulongForm)
                {
                    ret += ((char)(x1 + 'a'));
                    ret += ((char)(y1 + '1'));
                    ret += (isCapture(pos, move) ? 'x' : '-');
                }
                else
                {
                    if (p == (pos.whiteMove ? Piece.WPAWN : Piece.BPAWN))
                    {
                        if (isCapture(pos, move))
                        {
                            ret += ((char)(x1 + 'a'));
                        }
                    }
                    else
                    {
                        int numSameTarget = 0;
                        int numSameFile = 0;
                        int numSameRow = 0;
                        for (int mi = 0; mi < moves.size; mi++)
                        {
                            Move m = moves.m[mi];
                            if (m == null)
                                break;
                            if ((pos.getPiece(m.from) == p) && (m.to == move.to))
                            {
                                numSameTarget++;
                                if (Position.getX(m.from) == x1)
                                    numSameFile++;
                                if (Position.getY(m.from) == y1)
                                    numSameRow++;
                            }
                        }
                        if (numSameTarget < 2)
                        {
                            // No file/row info needed
                        }
                        else if (numSameFile < 2)
                        {
                            // Only file info needed
                            ret += ((char)(x1 + 'a'));
                        }
                        else if (numSameRow < 2)
                        {
                            // Only row info needed
                            ret += ((char)(y1 + '1'));
                        }
                        else
                        {
                            // File and row info needed
                            ret += ((char)(x1 + 'a'));
                            ret += ((char)(y1 + '1'));
                        }
                    }
                    if (isCapture(pos, move))
                    {
                        ret += ('x');
                    }
                }
                ret += ((char)(x2 + 'a'));
                ret += ((char)(y2 + '1'));
                if (move.promoteTo != Piece.EMPTY)
                {
                    ret += (pieceToChar(move.promoteTo));
                }
            }
            UndoInfo ui = new UndoInfo();
            if (MoveGen.givesCheck(pos, move))
            {
                pos.makeMove(move, ui);
                MoveGen MG = new MoveGen();
                MoveGen.MoveList nextMoves = MG.pseudoLegalMoves(pos);
                MoveGen.RemoveIllegal(pos, nextMoves);
                if (nextMoves.size == 0)
                {
                    ret += ('#');
                }
                else
                {
                    ret += ('+');
                }
                pos.unMakeMove(move, ui);
            }

            return ret;
        }

        /// <summary>
        /// Convert a move object to UCI string format.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string moveToUCIstring(Move m)
        {
            string ret = squareTostring(m.from);
            ret += squareTostring(m.to);
            switch (m.promoteTo)
            {
                case Piece.WQUEEN:
                case Piece.BQUEEN:
                    ret += "q";
                    break;
                case Piece.WROOK:
                case Piece.BROOK:
                    ret += "r";
                    break;
                case Piece.WBISHOP:
                case Piece.BBISHOP:
                    ret += "b";
                    break;
                case Piece.WKNIGHT:
                case Piece.BKNIGHT:
                    ret += "n";
                    break;
                default:
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Convert a string to a Move object. Return A move object, or null if move has invalid syntax.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static Move ucistringToMove(string move)
        {
            Move m = null;
            if ((move.Length < 4) || (move.Length > 5))
                return m;
            int fromSq = TextIO.getSquare(move.Substring(0, 2));
            int toSq = TextIO.getSquare(move.Substring(2, 4));
            if ((fromSq < 0) || (toSq < 0))
            {
                return m;
            }
            char prom = ' ';
            bool white = true;
            if (move.Length == 5)
            {
                prom = move[4];
                if (Position.getY(toSq) == 7)
                {
                    white = true;
                }
                else if (Position.getY(toSq) == 0)
                {
                    white = false;
                }
                else
                {
                    return m;
                }
            }
            int promoteTo;
            switch (prom)
            {
                case ' ':
                    promoteTo = Piece.EMPTY;
                    break;
                case 'q':
                    promoteTo = white ? Piece.WQUEEN : Piece.BQUEEN;
                    break;
                case 'r':
                    promoteTo = white ? Piece.WROOK : Piece.BROOK;
                    break;
                case 'b':
                    promoteTo = white ? Piece.WBISHOP : Piece.BBISHOP;
                    break;
                case 'n':
                    promoteTo = white ? Piece.WKNIGHT : Piece.BKNIGHT;
                    break;
                default:
                    return m;
            }
            m = new Move(fromSq, toSq, promoteTo);
            return m;
        }

        private static bool isCapture(Position pos, Move move)
        {
            if (pos.getPiece(move.to) == Piece.EMPTY)
            {
                int p = pos.getPiece(move.from);
                if ((p == (pos.whiteMove ? Piece.WPAWN : Piece.BPAWN)) && (move.to == pos.getEpSquare()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Convert a chess move string to a Move object. Any prefix of the string representation of a valid move counts as a legal move string, as ulong as the string only matches one valid move.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="strMove"></param>
        /// <returns></returns>
        public static Move stringToMove(Position pos, string strMove)
        {
            strMove = strMove.Replace("=", "");
            Move move = null;
            if (strMove.Length == 0)
                return move;
            MoveGen MG = new MoveGen();
            MoveGen.MoveList moves = MG.pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            {
                char lastChar = strMove[strMove.Length - 1];
                if ((lastChar == '#') || (lastChar == '+'))
                {
                    MoveGen.MoveList subMoves = new MoveGen.MoveList();
                    int len = 0;
                    for (int mi = 0; mi < moves.size; mi++)
                    {
                        Move m = moves.m[mi];
                        string str1 = TextIO.moveTostring(pos, m, true, moves);
                        if (str1[str1.Length - 1] == lastChar)
                        {
                            subMoves.m[len++] = m;
                        }
                    }
                    subMoves.size = len;
                    moves = subMoves;
                    strMove = normalizeMovestring(strMove);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                // Search for full match
                for (int mi = 0; mi < moves.size; mi++)
                {
                    Move m = moves.m[mi];
                    string str1 = normalizeMovestring(TextIO.moveTostring(pos, m, true, moves));
                    string str2 = normalizeMovestring(TextIO.moveTostring(pos, m, false, moves));
                    if (i == 0)
                    {
                        if ((str1 == strMove) || (str2 == strMove))
                        {
                            return m;
                        }
                    }
                    else
                    {
                        if ((strMove.ToLower() == str1.ToLower()) ||
                                (strMove.ToLower() == str2.ToLower()))
                        {
                            return m;
                        }
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                // Search for unique substring match
                for (int mi = 0; mi < moves.size; mi++)
                {
                    Move m = moves.m[mi];
                    string str1 = normalizeMovestring(TextIO.moveTostring(pos, m, true));
                    string str2 = normalizeMovestring(TextIO.moveTostring(pos, m, false));
                    bool match;
                    if (i == 0)
                    {
                        match = (str1.StartsWith(strMove) || str2.StartsWith(strMove));
                    }
                    else
                    {
                        match = (str1.ToLower().StartsWith(strMove.ToLower()) ||
                                str2.ToLower().StartsWith(strMove.ToLower()));
                    }
                    if (match)
                    {
                        if (move != null)
                        {
                            // More than one match, not ok
                            return null;
                        }
                        else
                        {
                            move = m;
                        }
                    }
                }
                if (move != null)
                    return move;
            }
            return move;
        }

        /// <summary>
        /// Convert a string, such as "e4" to a square number. Return The square number, or -1 if not a legal square.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int getSquare(string s)
        {
            int x = s[0] - 'a';
            int y = s[1] - '1';
            if ((x < 0) || (x > 7) || (y < 0) || (y > 7))
                return -1;
            return Position.getSquare(x, y);
        }

        /// <summary>
        /// Convert a square number to a string, such as "e4".
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public static string squareTostring(int square)
        {
            string ret = "";
            int x = Position.getX(square);
            int y = Position.getY(square);
            ret += ((char)(x + 'a'));
            ret += ((char)(y + '1'));
            return ret;
        }

        /// <summary>
        /// Prints board to console.
        /// </summary>
        /// <param name="pos"></param>
        public static void DispBoard(Position pos)
        {
            string ll = "    +----+----+----+----+----+----+----+----+";
            SystemHelper.println(ll);
            for (int y = 7; y >= 0; y--)
            {
                string ret = "    |";
                for (int x = 0; x < 8; x++)
                {
                    ret += ' ';
                    int p = pos.getPiece(Position.getSquare(x, y));
                    if (p == Piece.EMPTY)
                    {
                        bool dark = Position.darkSquare(x, y);
                        ret += (dark ? ".. |" : "   |");
                    }
                    else
                    {
                        ret += (Piece.isWhite(p) ? ' ' : '*');
                        string pieceName = pieceToChar(p);
                        if (pieceName.Length == 0)
                            pieceName = "P";
                        ret += pieceName;
                        ret += " |";
                    }
                }
                SystemHelper.println(ret);
                SystemHelper.println(ll);
            }
        }

        /// <summary>
        /// Convert move string to lower case and remove special check/mate symbols.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string normalizeMovestring(string str)
        {
            if (str.Length > 0)
            {
                char lastChar = str[str.Length - 1];
                if ((lastChar == '#') || (lastChar == '+'))
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            return str;
        }

        private static string pieceToChar(int p)
        {
            switch (p)
            {
                case Piece.WQUEEN: case Piece.BQUEEN: return "Q";
                case Piece.WROOK: case Piece.BROOK: return "R";
                case Piece.WBISHOP: case Piece.BBISHOP: return "B";
                case Piece.WKNIGHT: case Piece.BKNIGHT: return "N";
                case Piece.WKING: case Piece.BKING: return "K";
            }
            return "";
        }
    }
    //// End class
}
//// End namespace