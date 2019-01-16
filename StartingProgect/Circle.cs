using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    class Circle:Figure //клас для знаходження точок кола
    {
        private double radius;
        private double n;//amount of points on circle needed to make
        private List<Point> points;

        public Circle() :base()
        {            
            radius = 2;
            n=36;
            points = new List<Point>();
            FillPoints();
        }
        public Circle(Point startingPoint, double radius, double n)
            : base(startingPoint)
        {
            this.radius = radius;
            this.n = n;
            points = new List<Point>();
            FillPoints();
        }

        private void FillPoints()
        {
            double kut = (double)360 / (double)n;
            
            for (double i = 0; i < 360; i += kut)
            {
                points.Add(new Point(findCoordinates(i,radius)[0]+startingPoint.X,
                                  findCoordinates(i,radius)[1]+startingPoint.Y)); 
            }
        }

        private double[] findCoordinates(double degree, double radius)
        {            
            double x = (double)(radius * Math.Cos(degree * 2 * Math.PI / 360));//(int)(Math.Pow(radius, 2) * Math.Pow(Math.Cos(degree), 2));
            double y = (double)(radius * Math.Sin(degree * 2 * Math.PI / 360));//(radius*radius - x*x);
            return new double[] { x, y };
        }

        public List<Point> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
            }
        }
    }
}
