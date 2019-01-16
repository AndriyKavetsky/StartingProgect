using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    class TestIntegration
    {
        public TestIntegration()
        {
        }

        public static double testCase1()
        {
            double a = 3.0;
            double b = 4.0;
            IntegralFunctionX d = new IntegralFunctionX(x => 1 + x);            
            IntegralFunctionX c = new IntegralFunctionX(y => 0);
            IntegralFunctionX u = new IntegralFunctionX(x => 1 - x);
            IntegralFunctionX v = new IntegralFunctionX(y => 0);
            IntegralFunctionX xd = new IntegralFunctionX(z => (b - a) * z + a);            
            
            //f = x + 3y
            //IntegralFunctionXY f = new IntegralFunctionXY((z, t) => (b - a)*(d(xd(z))-c(xd(z)))/(u(xd(z))-v(xd(z)))*(xd(z)+3*( (d(xd(z)-c(xd(z))))/(u(xd(z))-v(xd(z)))*t + (u(xd(z))*c(xd(z)) - v(xd(z))*d(xd(z))) /(u(xd(z)) - v(xd(z)))    )));


            return Integration.areaIntegration(subIntegralFunction, 2);
        }

        public static double subIntegralFunction(double z, double t)
        {
            double a = 3.0;
            double b = 4.0;
            double x = (b - a) * z + a;
            double y = (1 + x) / (1 - x) * t;
            double jacobian = (b - a) * (1 + x) / (1 - x);

            return jacobian * (x + 3 * y);
        }
    }
}
