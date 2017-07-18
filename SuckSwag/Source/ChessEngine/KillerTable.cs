namespace SuckSwag
{

    /// <summary>
    /// Implement a table of killer moves for the killer heuristic.
    /// </summary>
    public class KillerTable
    {
        /** There is one KTEntry for each ply in the search tree. */
        public class KTEntry
        {
            public KTEntry()
            {
                move0 = move1 = 0;
            }
            public int move0;
            public int move1;
        }
        KTEntry[] ktList;

        /** Create an empty killer table. */
        public KillerTable()
        {
            ktList = new KTEntry[200];
            for (int i = 0; i < ktList.Length; i++)
                ktList[i] = new KTEntry();
        }

        /** Add a killer move to the table. Moves are replaced on an LRU basis. */
        public void addKiller(int ply, Move m)
        {
            if (ply >= ktList.Length)
                return;
            int move = (short)(m.from + (m.to << 6) + (m.promoteTo << 12));
            KTEntry ent = ktList[ply];
            if (move != ent.move0)
            {
                ent.move1 = ent.move0;
                ent.move0 = move;
            }
        }

        /**
         * Get a score for move m based on hits in the killer table.
         * The score is 4 for primary   hit at ply.
         * The score is 3 for secondary hit at ply.
         * The score is 2 for primary   hit at ply - 2.
         * The score is 1 for secondary hit at ply - 2.
         * The score is 0 otherwise.
         */
        public int getKillerScore(int ply, Move m)
        {
            int move = (short)(m.from + (m.to << 6) + (m.promoteTo << 12));
            if (ply < ktList.Length)
            {
                KTEntry ent = ktList[ply];
                if (move == ent.move0)
                {
                    return 4;
                }
                else if (move == ent.move1)
                {
                    return 3;
                }
            }
            if ((ply - 2 >= 0) && (ply - 2 < ktList.Length))
            {
                KTEntry ent = ktList[ply - 2];
                if (move == ent.move0)
                {
                    return 2;
                }
                else if (move == ent.move1)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
    //// End class
}
//// End namespace