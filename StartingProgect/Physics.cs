using System;
using System.Collections.Generic;
using System.Windows;

namespace StartingProgect
{
    public delegate double BoundaryFunction(double[] x, List<Point> points);
    public class Physics
    {
        //public static int boundaryCount;
        public static List<double> parametrs { get; set; }
        public static double a11 = 1.0;
        public static double a22 = 1.0;
        public static double d = 0;
        public static List<List<double>> beta = new List<List<double>> { new List<double> { 1.0 } };
        public static List<List<double>> sigma = new List<List<double>> { new List<double> { 1.0 } };
        public static List<List<double>> Tc = new List<List<double>> { new List<double> { 200.0 } };
        public static double C = 1.0;
        public static double RO = 1.0;

        public static double DH = 1.0;
        public static double DT = 1.0;
        public static double A = 0.5;
        public static double G = 20.0;

        public static double coefD(BoundaryFunction func, double [] x, List<Point> points)
        {
            double res = 0.0;
            if(func(x,points)<0)
            {
                res = 1.0;
            }
            else
            {
                res = DH / DT;
            }
            return res;
        }

        public Physics(int boundaryCount, int subBoundaryCount)
        {

        }
        public static double f(double x=0, double y=0,double z=0)//ф-ція джерел
        {
            return 0.0;//y * y * (y - 1) * (y - 1) * (12 * x * x - 12 * x + 2) + x * x * (x - 1) * (x - 1) * (12 * y * y * -12 * y + 2);//2*(x*x+y*y)-2*(y+x);//-2(b*y+a*x) a=1,b=1  x * (x - 1) * y * (y - 1);//
        }
                       
     }
}
