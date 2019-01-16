using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    public class Boundary
    {
        public int numberP1 { get; }
        public int numberP2 { get; }
        public int numberP3 { get; }
        public Condition condition { get; }

        //public int boundaryNumber;

        public Boundary(int numberP1, int numberP2, Condition condition)
        {
            this.numberP1 = numberP1;
            this.numberP2 = numberP2;
            this.condition = condition;
        }
        public Boundary(int numberP1, int numberP2, int numberP3, Condition condition)
        {
            this.numberP1 = numberP1;
            this.numberP2 = numberP2;
            this.numberP3 = numberP3;
            this.condition = condition;
        }
    }
}
