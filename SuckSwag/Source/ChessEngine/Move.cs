using System.Collections.Generic;

namespace SuckSwag
{
    public class Move
    {
        /** From square, 0-63. */
        public int from;

        /** To square, 0-63. */
        public int to;

        /** Promotion piece. */
        public int promoteTo;

        public int score;

        /** Create a move object. */
        public Move(int from, int to, int promoteTo)
        {
            this.from = from;
            this.to = to;
            this.promoteTo = promoteTo;
            this.score = 0;
        }

        public Move(int from, int to, int promoteTo, int score)
        {
            this.from = from;
            this.to = to;
            this.promoteTo = promoteTo;
            this.score = score;
        }

        public class SortByScore : IComparer<Move>
        {
            public int Compare(Move sm1, Move sm2)
            {
                return sm2.score - sm1.score;
            }
        }

        public Move(Move m)
        {
            this.from = m.from;
            this.to = m.to;
            this.promoteTo = m.promoteTo;
            this.score = m.score;
        }

        public void copyFrom(Move m)
        {
            from = m.from;
            to = m.to;
            promoteTo = m.promoteTo;
            //        score = m.score;
        }

        /** Note that score is not included in the comparison. */


        /*@Override*/
        public bool equals(Move o)
        {
            if ((o == null) /*|| (o.getClass() != this.getClass()) */)
                return false;
            Move other = (Move)o;
            if (from != other.from)
                return false;
            if (to != other.to)
                return false;
            if (promoteTo != other.promoteTo)
                return false;
            return true;
        }
        /*@Override*/

        public int hashCode()
        {
            return (from * 64 + to) * 16 + promoteTo;
        }

        /** Useful for debugging. */
        public string tostring()
        {
            return TextIO.moveToUCIstring(this);
        }
    }
    //// End class
}
//// End namespace