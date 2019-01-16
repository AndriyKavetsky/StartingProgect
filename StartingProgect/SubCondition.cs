using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
     public class SubCondition
    {
        private Condition condition;
        private Point startPoint;
        private Point endPoint;

        public SubCondition(Condition condition, Point startPoint, Point endPoint)
        {
            this.condition = condition;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public Condition GetCondition
        {
            get
            {
                return condition;
            }
        }

        public Point StartPoint
        {
            get
            {
                return startPoint;
            }
        }

        public Point EndPoint
        {
            get
            {
                return endPoint;
            }
        }
    }
}
