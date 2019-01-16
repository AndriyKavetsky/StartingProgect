using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    public delegate double IntegralFunctionXY(double x, double y);
    public delegate double IntegralFunctionX(double x);
    public class Integration
    {
        public int activeNumber1 { get; set; }
        public int activeNumber2 { get; set; }

        private Point[] pVector;
        private int activeBoundaryNumber;//номер границі(для отримання відповідних граничних умов)
        private int activeSubBoundaryNumber;//номер підграниці(для отримання відповідних граничних умов)

        private int n = 3; //approximation degree
        private int n2 = 2;// boundary element point count degree 

        public Integration(int n, Point[] mas)
        {
            this.n = n;
            pVector = new Point[n];
            for (int i = 0; i < n; i++)
            {
                pVector[i] = mas[i];
            }
            activeNumber1 = -1;
            activeNumber2 = -1;
        }   

        public Integration(int n2, Point[] mas, int activeBoundaryNumber, int activeSubBoundaryNumber)
        {
            this.n2 = n2;
            pVector = new Point[n2];
            for (int i = 0; i < n2; i++)
            {
                pVector[i] = mas[i];
            }
            this.activeBoundaryNumber = activeBoundaryNumber;
            this.activeSubBoundaryNumber = activeSubBoundaryNumber;
            activeNumber1 = -1;
            activeNumber2 = -1;
        }
        public static double lineIntegration(Point pStart,Point pEnd, IntegralFunctionXY function)
        {
            return GaussIntegration(pStart.X, pEnd.X, 5, new IntegralFunctionX(x => function(x, pStart.Y + (x - pStart.X) * (pEnd.Y - pStart.Y) / (pEnd.X - pStart.X))*Math.Sqrt(1+Math.Pow((pEnd.Y - pStart.Y) / (pEnd.X - pStart.X), 2))));
        }

        public static double SubIntegral(double x)
        {
            return x;
        }

        public double mePidIntegralFunction2D(double ksi, double eta) //method підінтегральна функція для me матриці
        {
            return BaseFunctions2D1.baseFunction(activeNumber1, ksi, eta) * BaseFunctions2D1.baseFunction(activeNumber2, ksi, eta) * determinant(ksi, eta);
        }
        public double kePidIntegralFunction2D(double ksi, double eta) //method підінтегральна функція для ke матриці // коефіцієнти a11 a22 беруться із класу Physics
        {
            double[][] jacobianMultiplication = matrixMultiplication(jacobian_1_T(jacobian_1(ksi, eta)), jacobian_1(ksi, eta));
            return (Math.Pow(Physics.a11, 1) * (BaseFunctions2D1.baseFunctionDKsi(activeNumber1, ksi, eta) * jacobianMultiplication[0][0] + BaseFunctions2D1.baseFunctionDEta(activeNumber1, ksi, eta) * jacobianMultiplication[1][0]) * BaseFunctions2D1.baseFunctionDKsi(activeNumber2, ksi, eta) +
                   +Math.Pow(Physics.a22, 1) * (BaseFunctions2D1.baseFunctionDKsi(activeNumber1, ksi, eta) * jacobianMultiplication[0][1] + BaseFunctions2D1.baseFunctionDEta(activeNumber1, ksi, eta) * jacobianMultiplication[1][1]) * BaseFunctions2D1.baseFunctionDEta(activeNumber2, ksi, eta)) * determinant(ksi, eta);
        }
        public double qePidIntegralFunction2D(double ksi, double eta) //method підінтегральна функція для qe матриці
        {
            // double n = 3;
            double x = 0.0;
            double y = 0.0;
            for (int i = 0; i < n; i++)//appriximation degree
            {
                x += pVector[i].X * BaseFunctions2D1.baseFunction(i, ksi, eta);
                y += pVector[i].Y * BaseFunctions2D1.baseFunction(i, ksi, eta);
            }
            return BaseFunctions2D1.baseFunction(activeNumber1, ksi, eta) * Physics.f(x, y) * determinant(ksi, eta);
        }
        public double lePidIntegralFunction1D(double eta) //method підінтегральна функція для le матриці_
        {
            double y = BaseFunctions1D1.baseFunction(activeNumber1, eta) * BaseFunctions1D1.baseFunction(activeNumber2, eta) * determinant_1D(eta);
            return BaseFunctions1D1.baseFunction(activeNumber1, eta) * BaseFunctions1D1.baseFunction(activeNumber2, eta) * determinant_1D(eta);
        }
        public double rePidIntegralFunction1D(double eta) //method підінтегральна функція для re2 матриці_
        {
            double x = (pVector[1].X - pVector[0].X) * eta + pVector[0].X;
            double y = (pVector[1].Y - pVector[0].Y) * eta + pVector[0].Y;
            return BaseFunctions1D1.baseFunction(activeNumber1, eta)* (x * x + y * y) * Physics.Tc[activeBoundaryNumber][activeSubBoundaryNumber] * determinant_1D(eta);
        }
        public double[][] jacobian(double ksi, double eta)//обчислення якобіана двовимірної області інтегрування трикутного елемента
        {
            double[][] jac = new double[2][];
            for (int i = 0; i < 2; i++)
            {
                jac[i] = new double[2];
                for (int j = 0; j < 2; j++)
                {
                    jac[i][j] = 0.0;
                }
            }
            for (int i = 0; i < n; i++)
            {
                jac[0][0] += pVector[i].X * BaseFunctions2D1.baseFunctionDKsi(i, ksi, eta);
            }
            for (int i = 0; i < n; i++)
            {
                jac[1][1] += pVector[i].Y * BaseFunctions2D1.baseFunctionDEta(i, ksi, eta);
            }
            for (int i = 0; i < n; i++)
            {
                jac[1][0] += pVector[i].X * BaseFunctions2D1.baseFunctionDEta(i, ksi, eta);
            }
            for (int i = 0; i < n; i++)
            {
                jac[0][1] += pVector[i].Y * BaseFunctions2D1.baseFunctionDKsi(i, ksi, eta);
            }
            return jac;
        }
        public double[][] jacobian_1(double ksi, double eta)// Jacobian^-1 
        {
            double[][] jac = jacobian(ksi, eta);
            double[][] jac_1 = new double[2][];
            for (int i = 0; i < 2; i++)
            {
                jac_1[i] = new double[2];
                for (int j = 0; j < 2; j++)
                {
                    jac_1[i][j] = 0.0;
                }
            }
            double determinant = jac[0][0] * jac[1][1] - jac[0][1] * jac[1][0];

            jac_1[0][0] = jac[1][1] / determinant;
            jac_1[1][1] = jac[0][0] / determinant;
            jac_1[0][1] = -jac[0][1] / determinant;
            jac_1[1][0] = -jac[1][0] / determinant;
            return jac_1;
        }

        public double[][] jacobian_1_T(double[][] jac)// // Jacobian^-1 транспонований
        {
            double[][] resJac = new double[jac[0].Length][];
            for (int i = 0; i < resJac.Length; i++)
            {
                resJac[i] = new double[jac.Length];
                for (int j = 0; j < resJac.Length; j++)
                {
                    resJac[i][j] = 0.0;
                }
            }
            for (int i = 0; i < resJac.Length; i++)
            {
                for (int j = 0; j < resJac[i].Length; j++)
                {
                    resJac[i][j] = jac[j][i];
                }
            }
            return resJac;
        }
        public double determinant(double ksi, double eta) //визначник матриці Якобі
        {
            double s1 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s1 += pVector[i].X * BaseFunctions2D1.baseFunctionDKsi(i, ksi, eta);
            }
            double s2 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s2 += pVector[i].Y * BaseFunctions2D1.baseFunctionDEta(i, ksi, eta);
            }
            double s3 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s3 += pVector[i].X * BaseFunctions2D1.baseFunctionDEta(i, ksi, eta);
            }

            double s4 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s4 += pVector[i].Y * BaseFunctions2D1.baseFunctionDKsi(i, ksi, eta);
            }
            double s = s1 * s2 - s3 * s4;

            return s;
        }

        public double determinant_1D(double teta)
        {
            double s = Math.Sqrt(Math.Pow((pVector[0].X - pVector[1].X), 2) + Math.Pow((pVector[0].Y - pVector[1].Y), 2));
            return s;
        }

        public static double[][] matrixMultiplication(double[][] matrix1, double[][] matrix2)
        {
            if (matrix1[0].Length != matrix2.Length)
                return matrix1;
            double[][] res = new double[matrix1.Length][];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new double[matrix2[0].Length];
                for (int j = 0; j < res[i].Length; j++)
                {
                    res[i][j] = 0.0;
                }
            }
            for (int i = 0; i < res.Length; i++)
            {
                for (int j = 0; j < res[i].Length; j++)
                {
                    for (int k = 0; k < matrix1[0].Length; k++)
                    {
                        res[i][j] += matrix1[i][k] * matrix2[k][j];
                    }
                }
            }
            return res;
        }

        public static double areaIntegration(IntegralFunctionXY f, int nn) //метод для знаходження подвійного інтеграла від заданої підінтегральної функції
        {
            double[] ksi = new double[nn];
            double[] c = new double[nn];

            switch (nn)
            {
                case 1:
                    {
                        c[0] = 2;
                        ksi[0] = 0.5;
                    }
                    break;
                case 2:
                    {
                        c[0] = 1;//0.5;
                        c[1] = 1;//0.5;
                        ksi[0] = -0.577350;//0.2113;
                        ksi[1] = 0.577350;//0.7887;
                    }
                    break;
                case 3:
                    {
                        c[0] = 0.555555;
                        c[1] = 0.888889;
                        c[2] = 0.555555;
                        ksi[0] = -0.774597;
                        ksi[1] = 0.0;
                        ksi[2] = 0.774597;
                    }
                    break;
                case 4:
                    {
                        c[0] = 0.347855;
                        c[1] = 0.652145;
                        c[2] = 0.652145;
                        c[3] = 0.347855;
                        ksi[0] = -0.861136;
                        ksi[1] = -0.339981;
                        ksi[2] = 0.339981;
                        ksi[3] = 0.861136;
                    }
                    break;
                case 5:
                    {
                        c[0] = 0.236927;
                        c[1] = 0.478629;
                        c[2] = 0.56888889;
                        c[3] = 0.478629;
                        c[4] = 0.236927;
                        ksi[0] = -0.906180;
                        ksi[1] = -0.538470;
                        ksi[2] = 0.0;
                        ksi[3] = 0.538470;
                        ksi[4] = 0.906180;
                    }
                    break;
                case 6:
                    {
                        c[0] = 0.171324;
                        c[1] = 0.360761;
                        c[2] = 0.467914;
                        c[3] = 0.467914;
                        c[4] = 0.360761;
                        c[5] = 0.171324;
                        ksi[0] = -0.932470;
                        ksi[1] = -0.661210;
                        ksi[2] = -0.238620;
                        ksi[3] = 0.238620;
                        ksi[4] = 0.661210;
                        ksi[5] = 0.932470;
                    }
                    break;
                case 7:
                    {
                        c[0] = 0.129485;
                        c[1] = 0.279705;
                        c[2] = 0.381830;
                        c[3] = 0.417960;
                        c[4] = 0.381830;
                        c[5] = 0.279705;
                        c[6] = 0.129485;
                        ksi[0] = -0.949108;
                        ksi[1] = -0.741531;
                        ksi[2] = -0.405845;
                        ksi[3] = 0.0;
                        ksi[4] = 0.405845;
                        ksi[5] = 0.741531;
                        ksi[6] = 0.949108;
                    }
                    break;
                case 8:
                    {
                        c[0] = 0.101228;
                        c[1] = 0.222381;
                        c[2] = 0.313707;
                        c[3] = 0.362684;
                        c[4] = 0.362684;
                        c[5] = 0.313707;
                        c[6] = 0.222381;
                        c[7] = 0.101228;
                        ksi[0] = -0.960290;
                        ksi[1] = -0.796666;
                        ksi[2] = -0.525532;
                        ksi[3] = -0.183434;
                        ksi[4] = 0.183434;
                        ksi[5] = 0.525532;
                        ksi[6] = 0.796666;
                        ksi[7] = 0.960290;
                    }
                    break;
            }
            //a=0 b=1
            double s = 0.0;
            for (int j = 0; j < nn; j++)
            {
                for (int i = 0; i < nn; i++)
                {
                    //s += c[i] * c[j] * f(ksi[j], ksi[i]) * (1.0 - ksi[i]);
                    s += c[i] * c[j] * f(0.5 * ksi[j] + 0.5, 0.5 * g(0.5 * ksi[j] + 0.5) * ksi[i] + 0.5 * g(0.5 * ksi[j] + 0.5)) * 0.5 * g(0.5 * ksi[j] + 0.5) * 0.5;
                }
            }
            
            return s;
        }
        public static double areaIntegration(BaseFunctions f, int nn) //метод для знаходження подвійного інтеграла від заданої підінтегральної функції
        {
            double[] ksi = new double[nn];
            double[] c = new double[nn];

            switch (nn)
            {
                case 1:
                    {
                        c[0] = 2;
                        ksi[0] = 0.5;
                    }
                    break;
                case 2:
                    {
                        c[0] = 1;
                        c[1] = 1;
                        ksi[0] = -0.577350;
                        ksi[1] = 0.577350;
                    }
                    break;
                case 3:
                    {
                        c[0] = 0.555555;
                        c[1] = 0.888889;
                        c[2] = 0.555555;
                        ksi[0] = -0.774597;
                        ksi[1] = 0.0;
                        ksi[2] = 0.774597;
                    }
                    break;
                case 4:
                    {
                        c[0] = 0.347855;
                        c[1] = 0.652145;
                        c[2] = 0.652145;
                        c[3] = 0.347855;
                        ksi[0] = -0.861136;
                        ksi[1] = -0.339981;
                        ksi[2] = 0.339981;
                        ksi[3] = 0.861136;
                    }
                    break;
                case 5:
                    {
                        c[0] = 0.236927;
                        c[1] = 0.478629;
                        c[2] = 0.56888889;
                        c[3] = 0.478629;
                        c[4] = 0.236927;
                        ksi[0] = -0.906180;
                        ksi[1] = -0.538470;
                        ksi[2] = 0.0;
                        ksi[3] = 0.538470;
                        ksi[4] = 0.906180;
                    }
                    break;
                case 6:
                    {
                        c[0] = 0.171324;
                        c[1] = 0.360761;
                        c[2] = 0.467914;
                        c[3] = 0.467914;
                        c[4] = 0.360761;
                        c[5] = 0.171324;
                        ksi[0] = -0.932470;
                        ksi[1] = -0.661210;
                        ksi[2] = -0.238620;
                        ksi[3] = 0.238620;
                        ksi[4] = 0.661210;
                        ksi[5] = 0.932470;
                    }
                    break;
                case 7:
                    {
                        c[0] = 0.129485;
                        c[1] = 0.279705;
                        c[2] = 0.381830;
                        c[3] = 0.417960;
                        c[4] = 0.381830;
                        c[5] = 0.279705;
                        c[6] = 0.129485;
                        ksi[0] = -0.949108;
                        ksi[1] = -0.741531;
                        ksi[2] = -0.405845;
                        ksi[3] = 0.0;
                        ksi[4] = 0.405845;
                        ksi[5] = 0.741531;
                        ksi[6] = 0.949108;
                    }
                    break;
                case 8:
                    {
                        c[0] = 0.101228;
                        c[1] = 0.222381;
                        c[2] = 0.313707;
                        c[3] = 0.362684;
                        c[4] = 0.362684;
                        c[5] = 0.313707;
                        c[6] = 0.222381;
                        c[7] = 0.101228;
                        ksi[0] = -0.960290;
                        ksi[1] = -0.796666;
                        ksi[2] = -0.525532;
                        ksi[3] = -0.183434;
                        ksi[4] = 0.183434;
                        ksi[5] = 0.525532;
                        ksi[6] = 0.796666;
                        ksi[7] = 0.960290;
                    }
                    break;
            }
            //a=0 b=1
            double s = 0.0;
            for (int j = 0; j < nn; j++)
            {
                for (int i = 0; i < nn; i++)
                {
                    double[] point = new double[] { 0.5 * ksi[j] + 0.5, 0.5 * g(0.5 * ksi[j] + 0.5) * ksi[i] + 0.5 * g(0.5 * ksi[j] + 0.5) };
                    s += c[i] * c[j] * f(point) * 0.5 * g(0.5 * ksi[j] + 0.5) * 0.5;
                }
            }
            return s;
        }
        public static double g(double ksi)
        {
            return (1.0 - ksi);
        }

        public static double GaussIntegration(double a , double b ,int n,IntegralFunctionX f)//n - кількість вузлів у м-ді гаусса
        {
            double[] ksi = new double[n];
            double[] c = new double[n];

            switch (n)
            {
                case 1:
                    {
                        c[0] = 2;
                        ksi[0] = 0.5;
                    }
                    break;
                case 2:
                    {
                        c[0] = 1;//0.5;
                        c[1] = 1;//0.5;
                        ksi[0] = -0.577350;//0.2113;
                        ksi[1] = 0.577350;//0.7887;
                    }
                    break;
                case 3:
                    {
                        c[0] = 0.555555;
                        c[1] = 0.888889;
                        c[2] = 0.555555;
                        ksi[0] = -0.774597;
                        ksi[1] = 0.0;
                        ksi[2] = 0.774597;
                    }
                    break;
                case 4:
                    {
                        c[0] = 0.347855;
                        c[1] = 0.652145;
                        c[2] = 0.652145;
                        c[3] = 0.347855;
                        ksi[0] = -0.861136;
                        ksi[1] = -0.339981;
                        ksi[2] = 0.339981;
                        ksi[3] = 0.861136;
                    }
                    break;
                case 5:
                    {
                        c[0] = 0.236927;
                        c[1] = 0.478629;
                        c[2] = 0.56888889;
                        c[3] = 0.478629;
                        c[4] = 0.236927;
                        ksi[0] = -0.906180;
                        ksi[1] = -0.538470;
                        ksi[2] = 0.0;
                        ksi[3] = 0.538470;
                        ksi[4] = 0.906180;
                    }
                    break;
                case 6:
                    {
                        c[0] = 0.171324;
                        c[1] = 0.360761;
                        c[2] = 0.467914;
                        c[3] = 0.467914;
                        c[4] = 0.360761;
                        c[5] = 0.171324;
                        ksi[0] = -0.932470;
                        ksi[1] = -0.661210;
                        ksi[2] = -0.238620;
                        ksi[3] = 0.238620;
                        ksi[4] = 0.661210;
                        ksi[5] = 0.932470;
                    }
                    break;
                case 7:
                    {
                        c[0] = 0.129485;
                        c[1] = 0.279705;
                        c[2] = 0.381830;
                        c[3] = 0.417960;
                        c[4] = 0.381830;
                        c[5] = 0.279705;
                        c[6] = 0.129485;
                        ksi[0] = -0.949108;
                        ksi[1] = -0.741531;
                        ksi[2] = -0.405845;
                        ksi[3] = 0.0;
                        ksi[4] = 0.405845;
                        ksi[5] = 0.741531;
                        ksi[6] = 0.949108;
                    }
                    break;
                case 8:
                    {
                        c[0] = 0.101228;
                        c[1] = 0.222381;
                        c[2] = 0.313707;
                        c[3] = 0.362684;
                        c[4] = 0.362684;
                        c[5] = 0.313707;
                        c[6] = 0.222381;
                        c[7] = 0.101228;
                        ksi[0] = -0.960290;
                        ksi[1] = -0.796666;
                        ksi[2] = -0.525532;
                        ksi[3] = -0.183434;
                        ksi[4] = 0.183434;
                        ksi[5] = 0.525532;
                        ksi[6] = 0.796666;
                        ksi[7] = 0.960290;
                    }
                    break;
            }

            double s = 0;

            for (int i = 0; i < n; i++)
            {
                double x = ((b - a) / 2) * ksi[i] + (a + b) / 2;//x = (b - a) * ksi[i] + a;
                s += c[i] * f(x);
            }

            return s * (b - a) / 2.0;//s*(b-a);
        }
    }
}
