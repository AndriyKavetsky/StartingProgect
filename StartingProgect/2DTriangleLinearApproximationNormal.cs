using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    
    public class D2TriangleLinearApproximationNormal
    {
        Point p1;
        Point p2;
        Point p3;
        Dictionary<Point, double> pointValue;
        bool isOutwardNormal;//чи обхід є за годинниковою стрілкою

        public D2TriangleLinearApproximationNormal(Point p1, Point p2, Point p3, bool isPlusNormal)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.isOutwardNormal = isPlusNormal;
        }       

        public D2TriangleLinearApproximationNormal(Dictionary<Point,double> pointValue, bool isPlusNormal)
        {
            this.pointValue = pointValue;
            this.isOutwardNormal = isPlusNormal;
        }

        public Point FindNormal()
        {
            //counterClockwise
            //first two points normal vector
            double norm11=0.0;
            double norm12=0.0;            
            if (Math.Abs(p1.X-p2.X)>0)
            {
                double a = (p1.Y - p2.Y) / (p1.X - p2.X);
                norm11 = -a;
                norm12 = 1.0;
                //normalizing vector of normal
                norm11 = norm11 / Math.Sqrt(a * a + 1);
                norm12 = norm12 / Math.Sqrt(a * a + 1);
                if((p1.X-p2.X)>0)
                {
                    norm11 = -norm11;
                    norm12 = -norm12;
                }
            }else
            {
                double a = (p1.X - p2.X) / (p1.Y - p2.Y);
                norm11 = -1.0;
                norm12 = a;
                //normalizing vector of normal
                norm11 = norm11 / Math.Sqrt(a * a + 1);
                norm12 = norm12 / Math.Sqrt(a * a + 1);
                if((p1.Y-p2.Y)>0)
                {
                    norm11 = -norm11;
                    norm12 = -norm12;
                }
            }
            //second two points normal vector
            double norm21=0.0;
            double norm22=0.0;
            if (Math.Abs(p3.X - p2.X) > 0)
            {
                double a = (p3.Y - p2.Y) / (p3.X - p2.X);
                norm21 = -a;
                norm22 = 1.0;
                //normalizing vector of normal
                norm21 = norm21 / Math.Sqrt(a * a + 1);
                norm22 = norm22 / Math.Sqrt(a * a + 1);
                if ((p2.X - p3.X) > 0)
                {
                    norm21 = -norm21;
                    norm22 = -norm22;
                }
            }
            else
            {
                double a = (p3.X - p2.X) / (p3.Y - p2.Y);
                norm21 = -1.0;
                norm22 = a;
                //normalizing vector of normal
                norm21 = norm21 / Math.Sqrt(a * a + 1);
                norm22 = norm22 / Math.Sqrt(a * a + 1);
                if ((p2.Y - p3.Y) > 0)
                {
                    norm21 = -norm21;
                    norm22 = -norm22;
                }
            }
            double n1 = norm11 + norm21;
            double n2 = norm12 + norm22;
            double l = Math.Sqrt(n1 * n1 + n2 * n2);
            n1 /= l;
            n2 /= l;
            if(isOutwardNormal)
                return new Point(n1, n2);
            else
                return new Point(-n1, -n2);
        }

        //public Point NormalCalculating()
        //{
        //    //Observable collection (interface) - datagrid
        //    if (isOutwardNormal)
        //    {
        //        if(p1.X==p2.X)
        //        {
        //            if(p1.Y>p2.Y)
        //            {
        //                return new Point(1, 0);
        //            }
        //            else 
        //            {
        //                return new Point(-1, 0);
        //            }
        //        }
        //        double k1;
        //        k1 = (p1.Y - p2.Y) / (p1.X - p2.X);
        //        double x0 = 0;
        //        double y0 = 0;
        //        //if (isOutwardNormal)
        //        //{
        //        if((k1>0)&&(p2.X>p1.X))
        //        { 
        //            x0 = p2.X - Math.Sqrt(k1 * k1 / (k1 * k1 + 1));
        //        }
        //        else if ((k1 > 0) && (p2.X < p1.X))
        //        {
        //            x0 = p2.X + Math.Sqrt(k1 * k1 / (k1 * k1 + 1));
        //        }
        //        else if ((k1 < 0) && (p2.X < p1.X))
        //        {
        //            x0 = p2.X - Math.Sqrt(k1 * k1 / (k1 * k1 + 1));
        //        }
        //        else if ((k1 < 0) && (p2.X > p1.X))
        //        {
        //           x0 = p2.X + Math.Sqrt(k1 * k1 / (k1 * k1 + 1));
        //        }
        //        else if((k1==0)&&(p2.X>p1.X))
        //        {
        //            return new Point(0, 1);
        //        }
        //        else if ((k1 == 0) && (p2.X < p1.X))
        //        {
        //            return new Point(0, -1);
        //        }
        //        y0 = p2.Y - 1 / k1 * (x0 - p2.X);
        //        double n11 = (x0 - p2.X) / Math.Sqrt((x0 - p2.X) * (x0 - p2.X) + (y0 - p2.Y) * (y0 - p2.Y));
        //        double n21 = (y0 - p2.Y) / Math.Sqrt((x0 - p2.X) * (x0 - p2.X) + (y0 - p2.Y) * (y0 - p2.Y));
        //        //MessageBox.Show("k1 = "+k1+" x0 = " + x0 + " y0 = " + y0 + " n11 = " + n11 + " n21 = " + n21);
        //        double k2;
        //        k2 = (p3.Y - p2.Y) / (p3.X - p2.X);
        //        //k2 = (p2.Y - p3.Y) / (p2.X - p3.X);
        //        x0 = 0;
        //        y0 = 0;
        //        //if (isOutwardNormal)
        //        //{
        //        if((k2 < 0)&&(p3.X>p2.X))
        //        { 
        //            x0 = p2.X + Math.Sqrt(k2 * k2 / (k2 * k2 + 1));
        //        }
        //        else if ((k2 < 0) && (p3.X < p2.X))
        //        {
        //            x0 = p2.X - Math.Sqrt(k2 * k2 / (k2 * k2 + 1));
        //        }
        //        else if ((k2 > 0) && (p3.X > p2.X))
        //        {
        //            x0 = p2.X - Math.Sqrt(k2 * k2 / (k2 * k2 + 1));
        //        }
        //        else if ((k2 == 0) && (p2.Y > p1.Y))
        //        {
        //            return new Point(0,1);
        //        }
        //        else if ((k2 == 0) && (p2.Y < p1.Y))
        //        {
        //            return new Point(0,-1);
        //        }
        //        else
        //        {
        //            x0 = p2.X + Math.Sqrt(k2 * k2 / (k2 * k2 + 1));
        //        }
        //        //if(k2!=0)
        //        y0 = p2.Y - 1 / k2 * (x0 - p2.X);
                
        //        double n12 = (x0 - p2.X) / Math.Sqrt((x0 - p2.X) * (x0 - p2.X) + (y0 - p2.Y) * (y0 - p2.Y));
        //        double n22 = (y0 - p2.Y) / Math.Sqrt((x0 - p2.X) * (x0 - p2.X) + (y0 - p2.Y) * (y0 - p2.Y));
        //       // MessageBox.Show("k2 = " + k2 + "x0 = " + x0 + " y0 = " + y0 + " n12 = " + n12 + " n22 = " + n22);

        //        n1 = n11 + n12;
        //        n2 = n21 + n22;
        //        double l = Math.Sqrt(n1 * n1 + n2 * n2);
        //        n1 /= l;
        //        n2 /= l;
        //    }
        //    else //internalNormal
        //    { 
        //        // протилежна точка
                
        //    }
        //    return new Point(n1, n2);
        //}
    }
}
