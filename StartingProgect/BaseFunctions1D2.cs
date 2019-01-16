using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    public class BaseFunctions1D2
    {
        public static double baseFunction(int numberOfFunction, double teta)
        {
            switch (numberOfFunction)
            {
                case 0:
                    {
                        return (1.0 - teta) * (1.0 - 2.0 * teta);
                    }
                case 2:
                    {
                        return -1.0 * teta + 2 * teta * teta;
                    }
                case 1:
                    {
                        return 4.0 * (1.0 - teta) * teta;
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
                        return -3.0 + 4.0 * teta;
                        //return -(1.0 - 2.0 * teta) - 2.0 * (1.0 - teta);
                    }
                case 1:
                    {
                        return 4.0 - 8.0 * teta;
                        //return -teta + 4.0 * teta;
                    }
                case 2:
                    {
                        return -1.0 + 4.0 * teta;
                        //return 4.0 * (1 - teta) - 4.0 * teta;
                    }
            }
            return 0.0;
        }
    }
}
