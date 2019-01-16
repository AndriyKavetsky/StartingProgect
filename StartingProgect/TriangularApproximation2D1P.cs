using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    class TriangularApproximation2D1P : Approximation//---квадратичні апроксимації 2d triangular aproximation
    {
        public TriangularApproximation2D1P():base(1,2,3)
        {
            //    this.ApproximationDegree = 1;//
            //    this.domainDimention = 2;//domainDimention;
            //    this.elementForm = 3;

            baseFunctions = new List<BaseFunctions>();
            baseFunctions.Add(baseFunction0);
            baseFunctions.Add(baseFunction1);
            baseFunctions.Add(baseFunction2);           
        }

        public static double baseFunction0(double[] point)
        {
            return 1.0 - point[0] - point[1];
        }
        public static double baseFunction1(double[] point)
        {
            return point[0];
        }
        public static double baseFunction2(double[] point)
        {
            return point[1];
        }
        private double baseFunction(int numberOfFunction, double ksi, double teta)
        {
            switch (numberOfFunction)
            {
                case 1:
                    {
                        return 1.0-ksi-teta;
                    }
                case 2:
                    {
                        return ksi;
                    }
                case 3:
                    {
                        return teta;
                    }
            }
            return 0.0;
        }

    }
}
