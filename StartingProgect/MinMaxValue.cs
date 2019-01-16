using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    class MinMaxValue
    {
        private List<Point> points;

        public MinMaxValue(List<Point> points)
        {
            this.points = points;
        }

        public double minX()
        {
            if(points.Count==0)//
            {
                MessageBox.Show("input file should have at least one point");
                return 0;
            }
            double min = points[0].X;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < min)
                    min = points[i].X;
            }
            return min;
        }

        public double maxX()
        {
            if (points.Count == 0)
            {
                MessageBox.Show("input file should have at least one point");
                return 0;
            }
            double max = points[0].X;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X > max)
                    max = points[i].X;
            }
            return max;
        }

        public double minY()
        {
            if (points.Count == 0)
            {
                MessageBox.Show("input file should have at least one point");
                return 0;
            }
            double min = points[0].Y;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < min)
                    min = points[i].Y;
            }
            return min;
        }

        public double maxY( )
        {
            if (points.Count == 0)
            {
                MessageBox.Show("input file should have at least one point");
                return 0;
            }
            double max = points[0].Y;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y > max)
                    max = points[i].Y;
            }
            return max;
        }

        public double[] MinMaxValues()
        {
            return new double[]{minX(),maxX(),minY(),maxY()} ;
        }
    }
}
