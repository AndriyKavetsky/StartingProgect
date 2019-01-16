using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect.PolZap
{
    class Identuficator:IComparable
    {
        private string ident;
        private double znachennya;

        public Identuficator()
        {
            ident = "x";
            znachennya = 0;
        }

        public Identuficator(string ident, double znachennya)
        {
            this.ident = ident;
            this.znachennya = znachennya;
        }

        public string IDENT
        {
            get { return ident; }
        }

        public double ZNACHENNYA
        {
            get { return znachennya; }
            set { znachennya = value; }
        }

        public bool IsIdentuficator(string el)
        {
            bool b = true;
            try
            {
                if (el.Length > 8)
                    return false;

                if ((el.CompareTo("cos") == 0) || (el.CompareTo("sin") == 0) || (el.CompareTo("ln") == 0) || (el.CompareTo("exp") == 0))
                    return false;

                if (!((char.IsLetter(el[0])) || (el[0] == '_')))
                {
                    b = false;
                    return b;
                }

                for (int i = 1; i < el.Length; i++)
                {
                    if (!((char.IsDigit(el[i])) || (char.IsLetter(el[i])) || (el[i] == '_')))
                    {
                        b = false;
                        return b;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There is an error in one of formulas", ex);
                //MessageBox.Show("There is an error in one of formulas");   //could be changed             
            }
            return b;
        }

        public override string ToString()
        {
            return " Identuficator= " + ident + "   Znachennya= " + znachennya + "\n";
        }

        public int CompareTo(object obj)
        {
            //throw new NotImplementedException();

            return ident.CompareTo((obj as Identuficator).ident);
        }
    }
}
