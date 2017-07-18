using System.Collections.Generic;

namespace SuckSwag
{
    /// <summary>
    /// A player that reads input from the keyboard.
    /// </summary>
    public class HumanPlayer : Player
    {
        private string lastCmd = "";
        public string inp;

        public HumanPlayer()
        {
            inp = ""; // commands come here
        }

        // @Override
        public string getCommand(Position pos, bool drawOffer, List<Position> history)
        {
            try
            {
                string moveStr = inp;
                if (moveStr == null)
                    return "quit";
                if (moveStr.Length == 0)
                {
                    return lastCmd;
                }
                else
                {
                    lastCmd = moveStr;
                }
                return moveStr;
            }
            catch
            {
                return "quit";
            }
        }

        // @Override
        public bool isHumanPlayer()
        {
            return true;
        }

        // @Override
        public void useBook(bool bookOn)
        {
        }

        // @Override
        public void timeLimit(int minTimeLimit, int maxTimeLimit, bool randomMode)
        {
        }

        // @Override
        public void clearTT()
        {
        }
    }
    //// End class
}
//// End namespace