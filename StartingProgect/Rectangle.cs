using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    class Rectangle:Figure
    {
        private double width;
        private double n;
        private double heigth;
        private double m;
        private List<Point> points;

        public Rectangle(Point startingPoint, double width, double n, double heigth, double m)
            : base(startingPoint)
        {
            points = new List<Point>();
            this.width = width;
            this.n = n;
            this.heigth = heigth;
            this.m = m;
            FillRectangle();
        }

        public void FillRectangle()
        {
            for (int i = 1; i < n; i++)
            {
                points.Add(new Point(startingPoint.X + i * width / (n-1), startingPoint.Y));
            }
            for (int i = 1; i < m; i++)
            {
                points.Add(new Point(startingPoint.X + width, startingPoint.Y + i * heigth / (m-1)));
            }
            for (int i = 1; i < n; i++)
            {
                points.Add(new Point(startingPoint.X + width - i * width / (n-1), startingPoint.Y + heigth));
            }
            for (int i = 1; i < m; i++)
            {
                points.Add(new Point(startingPoint.X, startingPoint.Y + heigth - i * heigth / (m-1)));
            }
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
