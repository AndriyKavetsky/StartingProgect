using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    public class IntegrationOnSquaredApproximationElement
    {
        public int activeNumber1 { get; set; }
        public int activeNumber2 { get; set; }

        private Point[] pVector;
        private double [] concentrationVector=new double[] { 0.31930, 0.31930, 0.31930 };//передача!!!!
        private int activeBoundaryNumber;//номер границі(для отримання відповідних граничних умов)
        private int activeSubBoundaryNumber;//номер підграниці(для отримання відповідних граничних умов)
        public int n = 6; //approximation degree
        public int n2 = 3;// boundary element point count degree 

        public IntegrationOnSquaredApproximationElement(int n, Point[] mas)
        {
            this.n = n;
            pVector = new Point[n];
            for (int i = 0; i < n; i++)
            {
                pVector[i] = mas[i];
            }
        }
        public IntegrationOnSquaredApproximationElement(int n2, Point[] mas, int activeBoundaryNumber, int activeSubBoundaryNumber, double[] concentrationVector)
        {
            this.n2 = n2;
            this.concentrationVector = concentrationVector;
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
        public IntegrationOnSquaredApproximationElement(int n2, Point[] mas, int activeBoundaryNumber, int activeSubBoundaryNumber)
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
        public double mePidIntegralFunction2D2(double ksi, double eta) //method підінтегральна функція для me матриці
        {
            return BaseFunctions2D2.baseFunction(activeNumber1, ksi, eta) * BaseFunctions2D2.baseFunction(activeNumber2, ksi, eta) * determinant(ksi, eta);
        }
        public double kePidIntegralFunction2D2(double ksi, double eta) //method підінтегральна функція для ke матриці квадратичні апроксимації// коефіцієнти a11 a22 беруться із класу FEMSolver
        {
            double[][] jacobianMultiplication = Integration.matrixMultiplication(jacobian_1_T(jacobian_1(ksi, eta)), jacobian_1(ksi, eta));// для квадратичних апроксимацій
            return (Math.Pow(Physics.a11, 1) * (BaseFunctions2D2.baseFunctionDKsi(activeNumber1, ksi, eta) * jacobianMultiplication[0][0] + BaseFunctions2D2.baseFunctionDEta(activeNumber1, ksi, eta) * jacobianMultiplication[1][0]) * BaseFunctions2D2.baseFunctionDKsi(activeNumber2, ksi, eta) +
                   +Math.Pow(Physics.a22, 1) * (BaseFunctions2D2.baseFunctionDKsi(activeNumber1, ksi, eta) * jacobianMultiplication[0][1] + BaseFunctions2D2.baseFunctionDEta(activeNumber1, ksi, eta) * jacobianMultiplication[1][1]) * BaseFunctions2D2.baseFunctionDEta(activeNumber2, ksi, eta)) * determinant(ksi, eta);// детермінант для квадратичних апроксимацій
        }
        public double qePidIntegralFunction2D2(double ksi, double eta) //method підінтегральна функція для qe матриці
        {
            double x = 0.0;
            double y = 0.0;
            for (int i = 0; i < n; i++)//appriximation degree
            {
                x += pVector[i].X * BaseFunctions2D2.baseFunction(i, ksi, eta);
                y += pVector[i].Y * BaseFunctions2D2.baseFunction(i, ksi, eta);
            }
            return BaseFunctions2D2.baseFunction(activeNumber1, ksi, eta) * Physics.f(x, y) * determinant(ksi, eta);
        }
        public double lePidIntegralFunction1D2(double eta) //method підінтегральна функція для le матриці_
        {
            double y = BaseFunctions1D2.baseFunction(activeNumber1, eta) * BaseFunctions1D2.baseFunction(activeNumber2, eta) * determinant_1D2(eta);
            return BaseFunctions1D2.baseFunction(activeNumber1, eta) * BaseFunctions1D2.baseFunction(activeNumber2, eta) * determinant_1D2(eta);
        }
        public double rePidIntegralFunction1D2(double eta) //method підінтегральна функція для re2 матриці_
        {
            return BaseFunctions1D2.baseFunction(activeNumber1, eta) * Physics.Tc[activeBoundaryNumber][activeSubBoundaryNumber] * determinant_1D2(eta);
        }
        public double rePidIntegralFunction1D2Pressure(double eta) //method підінтегральна функція для re2 матриці_ для обчислення граничної умови із тиском
        {             
            double concSum = 0.0;
            double x = 0.0;
            double y = 0.0;
                        
            for (int i = 0; i < n2; i++)
            {
                concSum += concentrationVector[i] * BaseFunctions1D2.baseFunction(i, eta);
                x += pVector[i].X * BaseFunctions1D2.baseFunction(i, eta);
                y += pVector[i].Y * BaseFunctions1D2.baseFunction(i, eta);
                //double baseFuncNumber= BaseFunctions1D2.baseFunction(i, -1);
            }

            //double y2 = 0.0;
            //if (y > 0)// метод ітерацій підпростору
            //{
            //    y2 = Math.Sqrt(4 - x * x);
            //}
            //else
            //{
            //    y2 = -Math.Sqrt(4 - x * x);
            //}
            //x = (pVector[2].X - pVector[0].X) * eta + pVector[0].X;
            //y = (pVector[2].Y - pVector[0].Y) * eta + pVector[0].Y;
            //x = Math.Sqrt(Math.Abs(4.0 - y * y));
            //double p = 200.0*(x*x+y*y)/4.0;//- Physics.A*Physics.G*(x*x+y*y)/(4.0)-Physics.G+(Physics.G-0.0)*concentrationVector[0];//k-AG(x*x)/2d+(G-ksi)*(Hama)-G;//A=0.5 G=20.0 d=2 //1.0 + (20 - 1.0) * concSum-20-0.5*20*(x*x+y*y)/4.0
            //p=Math.Sqrt(x*x+y*y);//Math.Pow((pVector[2].X-pVector[0].X)*eta+pVector[0].X,2);
            //return BaseFunctions1D2.baseFunction(activeNumber1, eta) *p* determinant_1D2(eta)* Physics.Tc[activeBoundaryNumber][activeSubBoundaryNumber];
            double p = 0.0;
            double p2 = (x * x + y * y);//- Physics.A*Physics.G*(x*x+y*y)/(4.0)-Physics.G+(Physics.G-0.0)*concentrationVector[0];//k-AG(x*x)/2d+(G-ksi)*(Hama)-G;//A=0.5 G=20.0 d=2 //1.0 + (20 - 1.0) * concSum-20-0.5*20*(x*x+y*y)/4.0
            //p=Math.Sqrt(x*x+y*y);//Math.Pow((pVector[2].X-pVector[0].X)*eta+pVector[0].X,2);
            p = BaseFunctions1D2.baseFunction(activeNumber1, eta) * p2 * determinant_1D2(eta) * Physics.Tc[activeBoundaryNumber][activeSubBoundaryNumber];
            return p;
        }
        public double determinant_1D2(double teta)//check for corectness
        {
            double sx1 = 0.0;
            double sy1 = 0.0;
            for (int i = 0; i < n2; i++)
            {
                sx1 += pVector[i].X * BaseFunctions1D2.baseFunctionDEta(i, teta);
                sy1 += pVector[i].Y * BaseFunctions1D2.baseFunctionDEta(i, teta);
            }
            //double s = Math.Sqrt(Math.Pow((pVector[0].X - pVector[n2 - 1].X), 2) + Math.Pow((pVector[0].Y - pVector[n2 - 1].Y), 2));
            return Math.Sqrt(sx1 * sx1 + sy1 * sy1);
        }
        public double determinant(double ksi, double eta) //визначник матриці Якобі
        {
            double s1 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s1 += pVector[i].X * BaseFunctions2D2.baseFunctionDKsi(i, ksi, eta);
            }
            double s2 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s2 += pVector[i].Y * BaseFunctions2D2.baseFunctionDEta(i, ksi, eta);
            }
            double s3 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s3 += pVector[i].X * BaseFunctions2D2.baseFunctionDEta(i, ksi, eta);
            }

            double s4 = 0.0;

            for (int i = 0; i < n; i++)
            {
                s4 += pVector[i].Y * BaseFunctions2D2.baseFunctionDKsi(i, ksi, eta);
            }
            double s = s1 * s2 - s3 * s4;

            return s;
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
                jac[0][0] += pVector[i].X * BaseFunctions2D2.baseFunctionDKsi(i, ksi, eta);
            }
            for (int i = 0; i < n; i++)
            {
                jac[1][1] += pVector[i].Y * BaseFunctions2D2.baseFunctionDEta(i, ksi, eta);
            }
            for (int i = 0; i < n; i++)
            {
                jac[1][0] += pVector[i].X * BaseFunctions2D2.baseFunctionDEta(i, ksi, eta);
            }
            for (int i = 0; i < n; i++)
            {
                jac[0][1] += pVector[i].Y * BaseFunctions2D2.baseFunctionDKsi(i, ksi, eta);
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
    }
}
