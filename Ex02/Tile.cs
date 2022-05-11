namespace Ex02
{
    public class Tile
    {
        private int m_RowNumber;
        private int m_ColumnNumber;
        private char m_Puck;
        //private char m_Puck;

        public Tile(int i_RowNumber, int i_ColumnNumber, char i_Puck)
        {
            this.m_RowNumber = i_RowNumber;
            this.m_ColumnNumber = i_ColumnNumber;
            this.m_Puck = i_Puck;
            //  X/Z for player1
            //  O/Q for player2
        }

        public int RowNumber
        {
            get
            {
                return m_RowNumber;
            }
            set
            {
                m_RowNumber = value;
            }
        }

        public int ColumnNumber
        {
            get
            {
                return m_ColumnNumber;
            }
            set
            {
                m_ColumnNumber = value;
            }
        }
        public char Puck
        {
            get
            {
                return m_Puck;
            }
            set
            {
                m_Puck = value;
            }
        }


        public override string ToString()
        {
            return "" + Puck;
        }
    }

}
