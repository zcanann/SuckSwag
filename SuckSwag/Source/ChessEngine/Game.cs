using System.Collections.Generic;

namespace SuckSwag
{
    public class Game
    {
        protected List<Move> moveList = null;
        protected List<UndoInfo> uiInfoList = null;
        List<bool> drawOfferList = null;
        protected int currentMove;
        bool pendingDrawOffer;
        private static GameState drawState;
        private string drawStateMoveStr; // Move required to claim DRAW_REP or DRAW_50
        private static GameState resignState;
        public static Position pos = null;
        public Player whitePlayer;
        public Player blackPlayer;

        public Game(Player whitePlayer, Player blackPlayer)
        {
            this.whitePlayer = whitePlayer;
            this.blackPlayer = blackPlayer;
            handleCommand("new");
        }
        public Position getPos() { return pos; }

        /**
         * Update the game state according to move/command string from a player.
         * @param str The move or command to process.
         * @return True if str was understood, false otherwise.
         */
        public bool processstring(string str)
        {
            if (handleCommand(str))
            {
                return true;
            }
            if (getGameState() != GameState.ALIVE)
            {
                return false;
            }

            Move m = TextIO.stringToMove(pos, str);
            if (m == null)
            {
                return false;
            }

            UndoInfo ui = new UndoInfo();
            pos.makeMove(m, ui);
            TextIO.fixupEPSquare(pos);
            while (currentMove < moveList.Count)
            {
                moveList.RemoveAt(currentMove);
                uiInfoList.RemoveAt(currentMove);
                drawOfferList.RemoveAt(currentMove);
            }
            moveList.Add(m);
            uiInfoList.Add(ui);
            drawOfferList.Add(pendingDrawOffer);
            pendingDrawOffer = false;
            currentMove++;
            return true;
        }

        public string getGameStatestring()
        {
            switch (getGameState())
            {
                case GameState.ALIVE:
                    return "";
                case GameState.WHITE_MATE:
                    return "Game over, white mates!";
                case GameState.BLACK_MATE:
                    return "Game over, black mates!";
                case GameState.WHITE_STALEMATE:
                case GameState.BLACK_STALEMATE:
                    return "Game over, draw by stalemate!";
                case GameState.DRAW_REP:
                    {
                        string ret = "Game over, draw by repetition!";
                        if ((drawStateMoveStr != null) && (drawStateMoveStr.Length > 0))
                        {
                            ret = ret + " [" + drawStateMoveStr + "]";
                        }
                        return ret;
                    }
                case GameState.DRAW_50:
                    {
                        string ret = "Game over, draw by 50 move rule!";
                        if ((drawStateMoveStr != null) && (drawStateMoveStr.Length > 0))
                        {
                            ret = ret + " [" + drawStateMoveStr + "]";
                        }
                        return ret;
                    }
                case GameState.DRAW_NO_MATE:
                    return "Game over, draw by impossibility of mate!";
                case GameState.DRAW_AGREE:
                    return "Game over, draw by agreement!";
                case GameState.RESIGN_WHITE:
                    return "Game over, white resigns!";
                case GameState.RESIGN_BLACK:
                    return "Game over, black resigns!";
                default:
                    throw new RuntimeException();
            }
        }

        /**
         * Get the last played move, or null if no moves played yet.
         */
        public Move getLastMove()
        {
            Move m = null;
            if (currentMove > 0)
            {
                m = moveList[currentMove - 1];
            }
            return m;
        }

        public enum GameState
        {
            ALIVE,
            WHITE_MATE,         // White mates
            BLACK_MATE,         // Black mates
            WHITE_STALEMATE,    // White is stalemated
            BLACK_STALEMATE,    // Black is stalemated
            DRAW_REP,           // Draw by 3-fold repetition
            DRAW_50,            // Draw by 50 move rule
            DRAW_NO_MATE,       // Draw by impossibility of check mate
            DRAW_AGREE,         // Draw by agreement
            RESIGN_WHITE,       // White resigns
            RESIGN_BLACK        // Black resigns
        }

        /**
         * Get the current state of the game.
         */
        public static GameState getGameState()
        {
            MoveGen.MoveList moves = new MoveGen().pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            if (moves.size == 0)
            {
                if (MoveGen.inCheck(pos))
                {
                    return pos.whiteMove ? GameState.BLACK_MATE : GameState.WHITE_MATE;
                }
                else
                {
                    return pos.whiteMove ? GameState.WHITE_STALEMATE : GameState.BLACK_STALEMATE;
                }
            }
            if (insufficientMaterial())
            {
                return GameState.DRAW_NO_MATE;
            }
            if (resignState != GameState.ALIVE)
            {
                return resignState;
            }
            return drawState;
        }

        /**
         * Check if a draw offer is available.
         * @return True if the current player has the option to accept a draw offer.
         */
        public bool haveDrawOffer()
        {
            if (currentMove > 0)
            {
                return drawOfferList[currentMove - 1];
            }
            else
            {
                return false;
            }
        }

        /**
         * Handle a special command.
         * @param moveStr  The command to handle
         * @return  True if command handled, false otherwise.
         */
        public bool handleCommand(string moveStr)
        {
            if (moveStr == "new")
            {
                moveList = new List<Move>();
                uiInfoList = new List<UndoInfo>();
                drawOfferList = new List<bool>();
                currentMove = 0;
                pendingDrawOffer = false;
                drawState = GameState.ALIVE;
                resignState = GameState.ALIVE;
                try
                {
                    pos = TextIO.readFEN(TextIO.startPosFEN);
                }
                catch
                {
                    throw new RuntimeException();
                }
                whitePlayer.clearTT();
                blackPlayer.clearTT();
                activateHumanPlayer();
                return true;
            }
            else if (moveStr == "undo")
            {
                if (currentMove > 0)
                {
                    pos.unMakeMove(moveList[currentMove - 1], uiInfoList[currentMove - 1]);
                    currentMove--;
                    pendingDrawOffer = false;
                    drawState = GameState.ALIVE;
                    resignState = GameState.ALIVE;
                    return handleCommand("swap");
                }
                else
                {
                    SystemHelper.println("Nothing to undo");
                }
                return true;
            }
            else if (moveStr == "redo")
            {
                if (currentMove < moveList.Count)
                {
                    pos.makeMove(moveList[currentMove], uiInfoList[currentMove]);
                    currentMove++;
                    pendingDrawOffer = false;
                    return handleCommand("swap");
                }
                else
                {
                    SystemHelper.println("Nothing to redo");
                }
                return true;
            }
            else if ((moveStr == "swap") || (moveStr == "go"))
            {
                Player tmp = whitePlayer;
                whitePlayer = blackPlayer;
                blackPlayer = tmp;
                return true;
            }
            else if (moveStr == "list")
            {
                listMoves();
                return true;
            }
            else if (moveStr.StartsWith("setpos "))
            {
                string fen = moveStr.Substring(moveStr.IndexOf(" ") + 1);
                Position newPos = null;
                try
                {
                    newPos = TextIO.readFEN(fen);
                }
                catch
                {
                    SystemHelper.println("Invalid FEN: " + fen);
                }
                if (newPos != null)
                {
                    handleCommand("new");
                    pos = newPos;
                    activateHumanPlayer();
                }
                return true;
            }
            else if (moveStr == "getpos")
            {
                string fen = TextIO.toFEN(pos);
                SystemHelper.println(fen);
                return true;
            }
            else if (moveStr.StartsWith("draw "))
            {
                if (getGameState() == GameState.ALIVE)
                {
                    string drawCmd = moveStr.Substring(moveStr.IndexOf(" ") + 1);
                    return handleDrawCmd(drawCmd);
                }
                else
                {
                    return true;
                }
            }
            else if (moveStr == "resign")
            {
                if (getGameState() == GameState.ALIVE)
                {
                    resignState = pos.whiteMove ? GameState.RESIGN_WHITE : GameState.RESIGN_BLACK;
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else if (moveStr.StartsWith("book"))
            {
                string bookCmd = moveStr.Substring(moveStr.IndexOf(" ") + 1);
                return handleBookCmd(bookCmd);
            }
            else if (moveStr.StartsWith("time"))
            {
                try
                {
                    string timeStr = moveStr.Substring(moveStr.IndexOf(" ") + 1);
                    int timeLimit = int.Parse(timeStr);
                    whitePlayer.timeLimit(timeLimit, timeLimit, false);
                    blackPlayer.timeLimit(timeLimit, timeLimit, false);
                    return true;
                }
                catch
                {
                    SystemHelper.println("Number format exception: " + moveStr);
                    return false;
                }
            }
            else if (moveStr.StartsWith("perft "))
            {
                try
                {
                    string depthStr = moveStr.Substring(moveStr.IndexOf(" ") + 1);
                    int depth = int.Parse(depthStr);
                    MoveGen moveGen = new MoveGen();
                    long t0 = SystemHelper.currentTimeMillis();
                    ulong nodes = perfT(moveGen, pos, depth);
                    long t1 = SystemHelper.currentTimeMillis();
                    SystemHelper.println("perft(" + depth.ToString() + ") = " +
                        nodes.ToString() + " t=" + ((t1 - t0) / 1000).ToString() + "s");
                }
                catch
                {
                    SystemHelper.println("Number format exception: " + moveStr);
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /** Swap players around if needed to make the human player in control of the next move. */
        protected void activateHumanPlayer()
        {
            if (!(pos.whiteMove ? whitePlayer : blackPlayer).isHumanPlayer())
            {
                Player tmp = whitePlayer;
                whitePlayer = blackPlayer;
                blackPlayer = tmp;
            }
        }

        public List<string> getPosHistory()
        {
            List<string> ret = new List<string>();

            Position pos2 = new Position(pos /*this.pos*/);
            for (int i = currentMove; i > 0; i--)
            {
                pos2.unMakeMove(moveList[i - 1], uiInfoList[i - 1]);
            }
            ret.Add(TextIO.toFEN(pos2)); // Store initial FEN

            string moves = "";
            for (int i = 0; i < moveList.Count; i++)
            {
                Move move = moveList[i];
                string strMove = TextIO.moveTostring(pos2, move, false);
                moves += " " + strMove;
                UndoInfo ui = new UndoInfo();
                pos2.makeMove(move, ui);
            }
            ret.Add(moves); // Store move list string
            int numUndo = moveList.Count - currentMove;
            ret.Add(((int)numUndo).ToString());
            return ret;
        }

        /**
         * Print a list of all moves.
         */
        private void listMoves()
        {
            string movesStr = getMoveListstring(false);
            SystemHelper.println(movesStr);
        }

        public string getMoveListstring(bool compressed)
        {
            string ret = "";

            // Undo all moves in move history.
            Position pos2 = new Position(pos /*this.pos*/);
            for (int i = currentMove; i > 0; i--)
            {
                pos2.unMakeMove(moveList[i - 1], uiInfoList[i - 1]);
            }

            // Print all moves
            string whiteMove = "";
            string blackMove = "";
            for (int i = 0; i < currentMove; i++)
            {
                Move move = moveList[i];
                string strMove = TextIO.moveTostring(pos2, move, false);
                if (drawOfferList[i])
                {
                    strMove += " (d)";
                }
                if (pos2.whiteMove)
                {
                    whiteMove = strMove;
                }
                else
                {
                    blackMove = strMove;
                    if (whiteMove.Length == 0)
                    {
                        whiteMove = "...";
                    }
                    if (compressed)
                    {
                        ret += pos2.fullMoveCounter.ToString() + ". " +
                            whiteMove + " " + blackMove + " ";
                    }
                    else
                    {
                        ret += pos2.fullMoveCounter.ToString() + ".   " +
                            whiteMove.PadRight(10) + " " + blackMove.PadRight(10) + " ";
                    }
                    whiteMove = "";
                    blackMove = "";
                }
                UndoInfo ui = new UndoInfo();
                pos2.makeMove(move, ui);
            }
            if ((whiteMove.Length > 0) || (blackMove.Length > 0))
            {
                if (whiteMove.Length == 0)
                {
                    whiteMove = "...";
                }

                if (compressed)
                {
                    ret += pos2.fullMoveCounter.ToString() + ". " +
                        whiteMove + " " + blackMove + " ";
                }
                else
                {
                    ret += pos2.fullMoveCounter.ToString() + ".   " +
                        whiteMove.PadRight(10) + " " + blackMove.PadRight(10) + " ";
                }

            }
            string gameResult = getPGNResultstring();
            if (gameResult != "*")
            {
                ret += gameResult;
            }
            return ret;
        }

        public string getPGNResultstring()
        {
            string gameResult = "*";
            switch (getGameState())
            {
                case GameState.ALIVE:
                    break;
                case GameState.WHITE_MATE:
                case GameState.RESIGN_BLACK:
                    gameResult = "1-0";
                    break;
                case GameState.BLACK_MATE:
                case GameState.RESIGN_WHITE:
                    gameResult = "0-1";
                    break;
                case GameState.WHITE_STALEMATE:
                case GameState.BLACK_STALEMATE:
                case GameState.DRAW_REP:
                case GameState.DRAW_50:
                case GameState.DRAW_NO_MATE:
                case GameState.DRAW_AGREE:
                    gameResult = "1/2-1/2";
                    break;
            }
            return gameResult;
        }

        /** Return a list of previous positions in this game, back to the last "zeroing" move. */
        public List<Position> getHistory()
        {
            List<Position> posList = new List<Position>();
            Position pos2 = new Position(pos /*this.pos*/);

            for (int i = currentMove; i > 0; i--)
            {
                if (pos2.halfMoveClock == 0)
                    break;
                pos2.unMakeMove(moveList[i - 1], uiInfoList[i - 1]);
                posList.Add(new Position(pos2));
            }
            posList.Reverse();
            return posList;
        }

        private bool handleDrawCmd(string drawCmd)
        {
            if (drawCmd.StartsWith("rep") || drawCmd.StartsWith("50"))
            {
                bool rep = drawCmd.StartsWith("rep");
                Move m = null;
                string ms = drawCmd.Substring(drawCmd.IndexOf(" ") + 1);
                if (ms.Length > 0)
                {
                    m = TextIO.stringToMove(pos, ms);
                }
                bool valid;
                if (rep)
                {
                    valid = false;
                    List<Position> oldPositions = new List<Position>();
                    Position tmpPos;
                    if (m != null)
                    {
                        UndoInfo ui = new UndoInfo();
                        tmpPos = new Position(pos);
                        tmpPos.makeMove(m, ui);
                        oldPositions.Add(tmpPos);
                    }
                    oldPositions.Add(pos);
                    tmpPos = pos;
                    for (int i = currentMove - 1; i >= 0; i--)
                    {
                        tmpPos = new Position(tmpPos);
                        tmpPos.unMakeMove(moveList[i], uiInfoList[i]);
                        oldPositions.Add(tmpPos);
                    }
                    int repetitions = 0;
                    Position firstPos = oldPositions[0];
                    for (int i = 0; i < oldPositions.Count; i++)
                    {
                        Position p = oldPositions[i];
                        if (p.drawRuleEquals(firstPos))
                            repetitions++;
                    }
                    if (repetitions >= 3)
                    {
                        valid = true;
                    }
                }
                else
                {
                    Position tmpPos = new Position(pos);
                    if (m != null)
                    {
                        UndoInfo ui = new UndoInfo();
                        tmpPos.makeMove(m, ui);
                    }
                    valid = tmpPos.halfMoveClock >= 100;
                }
                if (valid)
                {
                    drawState = rep ? GameState.DRAW_REP : GameState.DRAW_50;
                    drawStateMoveStr = null;
                    if (m != null)
                    {
                        drawStateMoveStr = TextIO.moveTostring(pos, m, false);
                    }
                }
                else
                {
                    pendingDrawOffer = true;
                    if (m != null)
                    {
                        processstring(ms);
                    }
                }
                return true;
            }
            else if (drawCmd.StartsWith("offer "))
            {
                pendingDrawOffer = true;
                string ms = drawCmd.Substring(drawCmd.IndexOf(" ") + 1);
                if (TextIO.stringToMove(pos, ms) != null)
                {
                    processstring(ms);
                }
                return true;
            }
            else if (drawCmd == "accept")
            {
                if (haveDrawOffer())
                {
                    drawState = GameState.DRAW_AGREE;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool handleBookCmd(string bookCmd)
        {
            if (bookCmd == "off")
            {
                whitePlayer.useBook(false);
                blackPlayer.useBook(false);
                return true;
            }
            else if (bookCmd == "on")
            {
                whitePlayer.useBook(true);
                whitePlayer.useBook(true);
                return true;
            }
            return false;
        }

        public static bool insufficientMaterial()
        {
            if (pos.nPieces(Piece.WQUEEN) > 0) return false;
            if (pos.nPieces(Piece.WROOK) > 0) return false;
            if (pos.nPieces(Piece.WPAWN) > 0) return false;
            if (pos.nPieces(Piece.BQUEEN) > 0) return false;
            if (pos.nPieces(Piece.BROOK) > 0) return false;
            if (pos.nPieces(Piece.BPAWN) > 0) return false;
            int wb = pos.nPieces(Piece.WBISHOP);
            int wn = pos.nPieces(Piece.WKNIGHT);
            int bb = pos.nPieces(Piece.BBISHOP);
            int bn = pos.nPieces(Piece.BKNIGHT);
            if (wb + wn + bb + bn <= 1)
            {
                return true;    // King + bishop/knight vs king is draw
            }
            if (wn + bn == 0)
            {
                // Only bishops. If they are all on the same color, the position is a draw.
                bool bSquare = false;
                bool wSquare = false;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        int p = pos.getPiece(Position.getSquare(x, y));
                        if ((p == Piece.BBISHOP) || (p == Piece.WBISHOP))
                        {
                            if (Position.darkSquare(x, y))
                            {
                                bSquare = true;
                            }
                            else
                            {
                                wSquare = true;
                            }
                        }
                    }
                }
                if (!bSquare || !wSquare)
                {
                    return true;
                }
            }

            return false;
        }

        static ulong perfT(MoveGen moveGen, Position pos, int depth)
        {
            if (depth == 0)
                return 1;
            ulong nodes = 0;
            MoveGen.MoveList moves = moveGen.pseudoLegalMoves(pos);
            MoveGen.RemoveIllegal(pos, moves);
            if (depth == 1)
            {
                int ret = moves.size;
                moveGen.returnMoveList(moves);
                return (ulong)ret;
            }
            UndoInfo ui = new UndoInfo();
            for (int mi = 0; mi < moves.size; mi++)
            {
                Move m = moves.m[mi];
                pos.makeMove(m, ui);
                nodes += perfT(moveGen, pos, depth - 1);
                pos.unMakeMove(m, ui);
            }
            moveGen.returnMoveList(moves);
            return nodes;
        }
    }
    //// End class
}
//// End namespace