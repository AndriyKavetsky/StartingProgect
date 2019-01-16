using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    public class BaseFunctions2D2 
    {
        public static double baseFunction(int numberOfFunction, double ksi, double teta)
        {
            switch (numberOfFunction)
            {
                case 0:
                case 1:
                case 2:
                    {
                        return BaseFunctions2D1.baseFunction(numberOfFunction, ksi, teta) * (2.0 * BaseFunctions2D1.baseFunction(numberOfFunction, ksi, teta) - 1.0);
                    }
                case 3:
                    {
                        return 4.0 * BaseFunctions2D1.baseFunction(0, ksi, teta) * BaseFunctions2D1.baseFunction(1, ksi, teta);
                    }
                case 4:
                    {
                        return 4.0 * BaseFunctions2D1.baseFunction(1, ksi, teta) * BaseFunctions2D1.baseFunction(2, ksi, teta);
                    }
                case 5:
                    {
                        return 4.0 * BaseFunctions2D1.baseFunction(0, ksi, teta) * BaseFunctions2D1.baseFunction(2, ksi, teta);
                    }//default
            }
            return 0.0;// throw exception // generate class
        }
        public static double baseFunctionDKsi(int numberOfFunction, double ksi, double teta)
        {
            switch (numberOfFunction)
            {
                case 0:
                case 1:
                case 2:
                    {
                        return BaseFunctions2D1.baseFunctionDKsi(numberOfFunction, ksi, teta) * (4.0 * BaseFunctions2D1.baseFunction(numberOfFunction, ksi, teta) - 1.0);
                    }
                case 3:
                    {
                        return 4.0 * (BaseFunctions2D1.baseFunctionDKsi(0, ksi, teta) * BaseFunctions2D1.baseFunction(1, ksi, teta) + BaseFunctions2D1.baseFunction(0, ksi, teta) * BaseFunctions2D1.baseFunctionDKsi(1, ksi, teta));
                    }
                case 4:
                    {
                        return 4.0 * (BaseFunctions2D1.baseFunctionDKsi(1, ksi, teta) * BaseFunctions2D1.baseFunction(2, ksi, teta) + BaseFunctions2D1.baseFunction(1, ksi, teta) * BaseFunctions2D1.baseFunctionDKsi(2, ksi, teta));
                    }
                case 5:
                    {
                        return 4.0 * (BaseFunctions2D1.baseFunctionDKsi(0, ksi, teta) * BaseFunctions2D1.baseFunction(2, ksi, teta) + BaseFunctions2D1.baseFunction(0, ksi, teta) * BaseFunctions2D1.baseFunctionDKsi(2, ksi, teta));
                    }//default
            }
            return 0.0;// throw exception // generate class
        }

        public static double baseFunctionDEta(int numberOfFunction, double ksi, double teta)
        {
            switch (numberOfFunction)
            {
                case 0:
                case 1:
                case 2:
                    {
                        return BaseFunctions2D1.baseFunctionDEta(numberOfFunction, ksi, teta) * (4.0 * BaseFunctions2D1.baseFunction(numberOfFunction, ksi, teta) - 1.0);
                    }
                case 3:
                    {
                        return 4.0 * (BaseFunctions2D1.baseFunctionDEta(0, ksi, teta) * BaseFunctions2D1.baseFunction(1, ksi, teta) + BaseFunctions2D1.baseFunction(0, ksi, teta) * BaseFunctions2D1.baseFunctionDEta(1, ksi, teta));
                    }
                case 4:
                    {
                        return 4.0 * (BaseFunctions2D1.baseFunctionDEta(1, ksi, teta) * BaseFunctions2D1.baseFunction(2, ksi, teta) + BaseFunctions2D1.baseFunction(1, ksi, teta) * BaseFunctions2D1.baseFunctionDEta(2, ksi, teta));
                    }
                case 5:
                    {
                        return 4.0 * (BaseFunctions2D1.baseFunctionDEta(0, ksi, teta) * BaseFunctions2D1.baseFunction(2, ksi, teta) + BaseFunctions2D1.baseFunction(0, ksi, teta) * BaseFunctions2D1.baseFunctionDEta(2, ksi, teta));
                    }//default
            }
            return 0.0;// throw exception // generate class
        }
    }
}
