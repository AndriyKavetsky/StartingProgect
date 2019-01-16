using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;

namespace StartingProgect
{
    abstract class FEMSolver
    {
        protected Domain domain;
        protected static int n = 3;
        protected static int n2 = 2;

        public FEMSolver(Domain domain, int nn, int nn2)
        {
            this.domain = domain;
            n = nn;
            n2 = nn2;
        }
        public FEMSolver( )
        {

        }
        public double[][] matrixFormation(Point[] pVector, IntegralFunctionXY meFunc)// forming Me Matrix
        {
            double[][] matrix = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    matrix[i][j] = 0.0;
                }
            }
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n, pVector);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    integral.activeNumber1 = i;
                    integral.activeNumber2 = j;
                    //IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.mePidIntegralFunction2D2);
                    matrix[i][j] = Integration.areaIntegration(meFunc, 4);
                }
            }
            return matrix;
        }

        public double[][] matrixFormation2(Point[] pVector,IntegralFunctionXY keFunc)// forming Ke Matrix
        {
            double[][] matrix = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    matrix[i][j] = 0.0;
                }
            }
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n, pVector);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    integral.activeNumber1 = i;
                    integral.activeNumber2 = j;
                    //IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.kePidIntegralFunction2D2);
                    matrix[i][j] = Integration.areaIntegration(keFunc, 4);
                }
            }
            return matrix;
        }
        public double[][] matrixFormation_1D(Point[] pVector,IntegralFunctionX leFunc, double leftSideCoeficient) // forming le Matrix 
        {
            double[][] matrix = new double[n2][];
            for (int i = 0; i < n2; i++)
            {
                matrix[i] = new double[n];
                for (int j = 0; j < n2; j++)
                {
                    matrix[i][j] = 0.0;
                }
            }
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n2, pVector);

            for (int i = 0; i < n2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    integral.activeNumber1 = i;
                    integral.activeNumber2 = j;
                    //IntegralFunctionX ifXY = new IntegralFunctionX(integral.lePidIntegralFunction1D2);
                    matrix[i][j] = leftSideCoeficient * Integration.GaussIntegration(0.0, 1.0, 4, leFunc);
                }
            }
            return matrix;
        }
        public double[] vectorFormation(Point[] pVector,IntegralFunctionXY qeFunc) // forming qe vector
        {
            double[] vector = new double[n];
            for (int i = 0; i < n; i++)
            {
                vector[i] = 0.0;
            }
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n, pVector);

            for (int i = 0; i < n; i++)
            {
                integral.activeNumber1 = i;
                //IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.qePidIntegralFunction2D2);
                vector[i] = Integration.areaIntegration(qeFunc, 4);
            }
            return vector;
        }
        public double[] vectorFormation(Point[] pVector, List<BaseFunctions> qeFunc) // forming qe vector
        {
            double[] vector = new double[n];
            for (int i = 0; i < n; i++)
            {
                vector[i] = 0.0;
            }
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n, pVector);

            for (int i = 0; i < n; i++)
            {
                integral.activeNumber1 = i;
                //IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.qePidIntegralFunction2D2);
                vector[i] = Integration.areaIntegration(qeFunc[i], 4);
            }
            return vector;
        }
        public double[] vectorFormation1D(Point[] pVector,IntegralFunctionX reFunc, int activeBoundaryNumber, int activeSubBoundaryNumber, double rightSideCoeficient)// forming re vector from boundary conditions
        {
            double[] vector = new double[n2];
            for (int i = 0; i < n2; i++)
            {
                vector[i] = 0.0;
            }
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n2, pVector, activeBoundaryNumber, activeSubBoundaryNumber);

            for (int i = 0; i < n2; i++)
            {
                integral.activeNumber1 = i;
                //IntegralFunctionX ifXY = new IntegralFunctionX(integral.rePidIntegralFunction1D2);
                vector[i] = rightSideCoeficient * Integration.GaussIntegration(0.0, 1.0, 4, reFunc);
            }
            return vector;
        }
        public double[] setCoeficients(int i, int j, int r)
        {
            double leftSideCoeficient = 1.0;// sugma/beta*coef
            double rightSideCoeficient = 1.0;// sugma/beta*coef

            switch (domain.Boundaries.BoundaryArray[i][j][r].condition)
            {
                case (Condition.Dirichlet):
                    {
                        leftSideCoeficient = (Physics.sigma[i][j] * Math.Pow(10, 10)) / (Physics.beta[i][j]);
                        rightSideCoeficient = (Physics.sigma[i][j] * Math.Pow(10, 10)) / (Physics.beta[i][j]);
                    }
                    break;
                case (Condition.Neumann):
                    {
                        leftSideCoeficient = (Physics.sigma[i][j] * Math.Pow(10, -10)) / (Physics.beta[i][j]);
                        rightSideCoeficient = (Physics.sigma[i][j]) / (Physics.beta[i][j]);
                    }
                    break;
                case (Condition.Robin):
                    {
                        leftSideCoeficient = (Physics.sigma[i][j]) / (Physics.beta[i][j]);
                        rightSideCoeficient = (Physics.sigma[i][j]) / (Physics.beta[i][j]);
                    }
                    break;
            }
            return new double[] { leftSideCoeficient, rightSideCoeficient };
        }
        public static void writeResultMatrix(double[][] matrix, string matrixName, StreamWriter writer)
        {
            writer.WriteLine(matrixName);
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    writer.Write(String.Format("{0,20:0.000}", matrix[i][j]) + " ");
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }
        public static void writeResultVector(double[] vector, string vectorName, StreamWriter writer)
        {
            writer.WriteLine(vectorName);
            for (int i = 0; i < vector.Length; i++)
            {
                writer.Write(String.Format("{0,20:0.000}", vector[i]) + " ");
            }
            writer.WriteLine();
        }
    }
}
