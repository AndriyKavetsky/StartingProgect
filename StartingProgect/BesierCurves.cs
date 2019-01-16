using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;
using System.Windows;

namespace StartingProgect
{
    public class BesierCurves
    {
        List<Point> points;

        public BesierCurves(List<Point> points)
        {
            this.points = points;
        }

        public BesierCurves()
        {
            points = new List<Point>();
        }

        public Point Besier(Point p1, Point p2, Point p3, Point p4, double t)
        {
            Point resP = new Point();
            double var1, var2, var3;
            var1 = 1.0 - t;
            var2 = Math.Pow(var1, 3);
            var3 = Math.Pow(t, 3);

            resP.X = var2 * p1.X + 3.0*t * Math.Pow(var1, 2) * p2.X + 3.0 * Math.Pow(t, 2) * var1 * p3.X + var3 * p4.X;
            resP.Y = var2 * p1.Y + 3.0*t * Math.Pow(var1, 2) * p2.Y + 3.0 * Math.Pow(t, 2) * var1 * p3.Y + var3 * p4.Y;

            return resP;
        }

        public Point BesierDerrivative(Point p1, Point p2, Point p3, Point p4, double t)
        {
            Point resP = new Point();

            resP.X = -3.0 * Math.Pow(1.0 - t, 2) * p1.X + (3.0 * Math.Pow(1.0 - t, 2) - 6.0 * t * (1.0 - t)) * p2.X + (6.0 * t * (1.0 - t) - 3.0 * t * t) * p3.X + 3.0 * t * t * p4.X;
            resP.Y = -3.0 * Math.Pow(1.0 - t, 2) * p1.Y + (3.0 * Math.Pow(1.0 - t, 2) - 6.0 * t * (1.0 - t)) * p2.Y + (6.0 * t * (1.0 - t) - 3.0 * t * t) * p3.Y + 3.0 * t * t * p4.Y;

            return resP;
        }
    }
}
