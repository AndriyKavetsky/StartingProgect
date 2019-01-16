using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    class BaseFunctions2D1:Approximation
    {
        public BaseFunctions2D1():base(1,2,3)//int approximationDegree, int domainDimention, int elementForm
        {            
            baseFunctions = new List<BaseFunctions>();
            baseFunctions.Add(baseFunction0);
            baseFunctions.Add(baseFunction1);
            baseFunctions.Add(baseFunction2);
        }

        public static double baseFunctionDKsi(int numberOfFunction, double ksi, double eta)
        {
            switch (numberOfFunction)
            {
                case 0:
                    {
                        return -1.0;
                    }
                case 1:
                    {
                        return 1.0;
                    }
                case 2:
                    {
                        return 0.0;
                    }
            }
            return 0.0;
        }
        public static double baseFunctionDEta(int numberOfFunction, double ksi, double eta)
        {
            switch (numberOfFunction)
            {
                case 0:
                    {
                        return -1.0;
                    }
                case 1:
                    {
                        return 0.0;
                    }
                case 2:
                    {
                        return 1.0;
                    }
            }
            return 0.0;
        }
        public static double baseFunction0(double [] point)
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
        public static double baseFunction(int numberOfFunction, double ksi, double teta)
        {
            switch (numberOfFunction)
            {
                case 0:
                    {
                        return 1.0 - ksi - teta;
                    }
                case 1:
                    {
                        return ksi;
                    }
                case 2:
                    {
                        return teta;
                    }
            }
            return 0.0;
        }
    }
}
