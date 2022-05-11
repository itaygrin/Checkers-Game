using System.Collections.Generic;

namespace Ex02
{
    public class Player
    {

        private string m_UserName;
        private char m_Sign;
        private char m_QueenSign;
        private int m_Score;
        private List<Tile> m_SimplePucks = new List<Tile>();
        private List<Tile> m_QueenPucks = new List<Tile>();
        private bool m_HumanOrComputer;
        private int m_Orientation;

        public Player(string i_Username, char i_Sign, bool i_HumanOrComputer = true)
        {
            this.m_HumanOrComputer = i_HumanOrComputer;
            //this.xOrO = xOrO;
            this.m_UserName = i_Username;
            this.m_Sign = i_Sign;
            if (this.m_Sign == 'O')
            {
                this.m_Orientation = 1;
                this.m_QueenSign = 'Q';
            }
            else
            {
                this.m_Orientation = -1;
                this.m_QueenSign = 'Z';
            }
        }

        public int Orientaion
        {
            get { return m_Orientation; }
        }

        public string Username
        {
            get { return m_UserName; }
        }

        public char Sign
        {
            get { return m_Sign; }
        }

        public char QueenSign
        {
            get { return m_QueenSign; }
        }

        public List<Tile> SimplePucks
        {
            get
            {
                return m_SimplePucks;
            }
        }
        public List<Tile> QueenPucks
        {
            get
            {
                return m_QueenPucks;
            }
        }

        public int Score
        {
            get
            {
                return m_Score;
            }
            set
            {
                m_Score = value;
            }
        }

        public bool isHuman
        {
            get
            {
                return m_HumanOrComputer;
            }
        }

        //// Ask from the user for user name until the user name is a valid user name. If the player is a computer return null.
        //public string ChooseUserName()
        //{
        //    if (this.m_HumanOrComputer == false)
        //    {
        //        return null;
        //    }

        //    string checkUserName = "";
        //    while ((checkUserName.Length == 0) || (checkUserName.Length > 10) || (checkUserName.Contains(" ")))
        //    {
        //        Console.Write("Enter a user name (maximum 10 chars without spacing):");
        //        checkUserName = Console.ReadLine();
        //    }

        //    return checkUserName;
        //}



    }
}
