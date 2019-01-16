using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    public class BaseFunctions1D1
    {
        public static double baseFunction(int numberOfFunction, double teta)
        {
            switch (numberOfFunction)
            {
                case 0:
                    {
                        return 1.0 - teta;
                    }
                case 1:
                    {
                        return teta;
                    }
            }
            return 0.0;
        }

        public static double baseFunctionDEta(int numberOfFunction, double teta)
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
            }
            return 0.0;
        }
    }
}
