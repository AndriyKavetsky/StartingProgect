using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    public class SlaeSolver
    {
        private double[] result;
        public SlaeSolver()
        {
            result = new double[] { };
        }
        
        private static void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }

        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        public static double[] GaussMethod(double[][] a, double[] b)
        {
            const double eps = 0.00000001;
            double[] x = new double[b.Length];
            for (int k = 0; k < b.Length - 1; k++)
            {
                int maxx = k;
                for (int j = k; j < k; j++)
                    if (a[maxx][ k] < a[j][ k])
                        maxx = j;
                for (int j = k; j < b.Length; j++)
                    Swap(ref a[maxx][ j], ref a[k][ j]);
                Swap(ref maxx, ref k);
                if (Math.Abs(a[k][ k]) < eps)
                    return x;
                for (int i = k + 1; i < b.Length; i++)
                {
                    double m = -a[i][ k] / a[k][ k];
                    for (int j = k; j < b.Length; j++)
                        a[i][ j] += a[k][ j] * m;
                    b[i] += b[k] * m;
                }
            }
            if (Math.Abs(a[b.Length - 1][b.Length - 1]) < eps)
                return x;

            for (int i = b.Length - 1; i > -1; i--)
            {
                double temp = b[i];
                for (int j = i + 1; j < b.Length; j++)
                    temp -= a[i][j] * x[j];

                x[i] = temp / a[i][i];
            }
            return x;
        }
    }
}
