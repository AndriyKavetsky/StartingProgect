using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    abstract class Figure
    {
        protected Point startingPoint;

        public Figure()
        {
            startingPoint = new Point(); 
        }
        public Figure(Point p)
        {
            startingPoint = p;
        }

        public Point StartingPoint
        {
            get
            { 
                return startingPoint;
            }
            set
            {
                startingPoint = value;
            }
        }
    }
}
