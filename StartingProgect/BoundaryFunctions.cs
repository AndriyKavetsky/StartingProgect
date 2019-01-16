using System;
using System.Windows;
namespace StartingProgect
{
    public class BoundaryFunctions //клас який містить приклади для параметричної побудови границі області
    {
        public static Point function(double alpha)
        {
            double x = (1 + 0.24 * Math.Cos(2.0 * alpha) + 0.2 * Math.Sin(2.0 * alpha) + 0.12 * Math.Cos(3.0 * alpha) +
                0.1 * Math.Sin(3.0 * alpha) + 0.08 * Math.Cos(5.0 * alpha) + 0.14 * Math.Sin(6.0 * alpha)) * Math.Cos(alpha);
            double y = (1 + 0.52 * Math.Cos(2.0 * alpha) + 0.3 * Math.Sin(2.0 * alpha) + 0.12 * Math.Cos(3.0 * alpha) +
                0.1 * Math.Sin(3.0 * alpha) + 0.08 * Math.Cos(5.0 * alpha) + 0.14 * Math.Sin(6.0 * alpha)) * Math.Sin(alpha);
            //double y = 1 + (1 + 0.24 * Math.Cos(2.0 * alpha) + 0.2 * Math.Sin(2.0 * alpha) + 0.12 * Math.Cos(3.0 * alpha) +
            //  0.1 * Math.Sin(3.0 * alpha) + 0.08 * Math.Cos(5.0 * alpha) + 0.14 * Math.Sin(6.0 * alpha)) * Math.Sin(alpha);
            //(2+0.24cos2α+0.2sin2α +0.12cos3α+0.1sin3α+0.08cos5α+0.14sin6α)(cosα,sinα)
            return new Point(x, y);
        }
        public static Point function2(double alpha, double tau)
        {
            double x = (2.0 + Math.Cos(2.0 * alpha)) * Math.Cos(alpha) + tau;
            double y = (6.0 + 0.1 * Math.Cos(2.0 * alpha)) * Math.Sin(alpha);
            //(x(α),y(α)) = (2+0.1cos2α)(cosα,sinα). 
            return new Point(x, y);
        }
        public static Point function3(double t)
        {
            double x = 6 * Math.Cos(t) - 4 * Math.Pow(Math.Cos(t), 3);
            double y = 4 * Math.Pow(Math.Sin(t), 3);
            return new Point(x, y);
        }        
    }
}
