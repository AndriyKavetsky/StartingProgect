using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    class FEMSolver2D1P : FEMSolver, InnerPolygonBoundaryProblemApproximation
    {
        public FEMSolver2D1P(Domain domain):base(domain,3,2)
        {
            // this.domain = domain;
        }
        public FEMSolver2D1P()
        {
        //    domain = new Domain(new TwoDimentionalMeshOfTriangularElements(), new Boundaries());
        }    
        public InnerPolygonBoundaryProblemOutput solve()// method for solving problems using finite elements method with linear approximations
        {            
            StreamWriter writer = new StreamWriter("Result.txt");

            double[][] matrix = new double[domain.Meshes.PointsCount][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new double[domain.Meshes.PointsCount];
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    matrix[i][j] = 0.0;
                }
            }
            double[] vector = new double[domain.Meshes.PointsCount];

            for (int i = 0; i < domain.Meshes.ElementCount; i++)
            {
                Point[] trianglePoints = new Point[n]; //{ new Point() points.PointMas[elements[i][0]], points.PointMas[elements[i][1]], points.PointMas[elements[i][2]] };

                for(int j=0;j<n;j++)
                {
                    trianglePoints[j] = new Point(domain.Meshes.Points[domain.Meshes.Elements[i][j]][0], domain.Meshes.Points[domain.Meshes.Elements[i][j]][1]); //2d1
                }

                double[][] keMatrix = matrixFormation2(trianglePoints);
                writeResultMatrix(keMatrix, "\n keMatrix ", writer);//writing into file keMatrix                

                double[][] meMatrix = matrixFormation(trianglePoints);
                writeResultMatrix(meMatrix, "\n meMatrix ", writer); //writing into file meMatrix  without multiplication by d              

                double[] qe = vectorFormation(trianglePoints);
                writeResultVector(qe, "\n qeVector ", writer);//writing into file qeVector
                
                for (int j = 0; j < n; j++)
                {
                    vector[domain.Meshes.Elements[i][j]] += qe[j];
                    for (int k = 0; k < n; k++)
                    {
                        matrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += keMatrix[j][k] + Physics.d * meMatrix[k][j];//
                    }
                }
            }

            for (int i = 0; i <domain.Boundaries.BoundaryArray.Count;i++) // boundaryPointsNumber.Count; i++)
            {
                for (int j = 0; j < domain.Boundaries.BoundaryArray[i].Count;j++)// boundaryPointsNumber[i].Count - 1; j++)
                {
                    for (int r = 0; r < domain.Boundaries.BoundaryArray[i][j].Count; r++)
                    {
                        double[] coeficients = setCoeficients(i, j, r);
                        double leftSideCoeficient = coeficients[0];// sugma/beta*coef
                        double rightSideCoeficient = coeficients[1];// sugma/beta*coef

                        int number1 = domain.Boundaries.BoundaryArray[i][j][r].numberP1;
                        int number2 = domain.Boundaries.BoundaryArray[i][j][r].numberP2;                       

                        Point pStart = new Point(domain.Meshes.Points[number1][0], domain.Meshes.Points[number1][1]);
                        Point pEnd = new Point(domain.Meshes.Points[number2][0], domain.Meshes.Points[number2][1]);

                        Point[] boundaryPoints = new Point[] { pStart, pEnd };
                        int[] boundaryPointsNumbers = new int[] { number1, number2 };//індекси загальної матриці

                        double[][] geMatrix = matrixFormation_1D(boundaryPoints,leftSideCoeficient);// ліва частина інтегрування по границі                        
                        writeResultMatrix(geMatrix, "\n" +" Condition "+ domain.Boundaries.BoundaryArray[i][j][r].condition.ToString() + "\n" + "  ge matrix " , writer);//writing matrix into file

                        double[] leVector = vectorFormation1D(boundaryPoints, i, j, rightSideCoeficient);
                        writeResultVector(leVector, "\n leVector ", writer);//writing leVector into file   

                        for (int k = 0; k < n2; k++)
                        {
                            vector[boundaryPointsNumbers[k]] += leVector[k];
                            for (int p = 0; p < n2; p++)
                            {
                                matrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += geMatrix[k][p];
                            }
                        }                       
                                              
                    }
                }
            }

            writeResultMatrix(matrix, "\n Matrix ", writer);//writing matrix into file
            writeResultVector(vector, "\n Vector ", writer);//writing vector into file 
            
            SlaeSolver slar = new SlaeSolver();
            double[] res = SlaeSolver.GaussMethod(matrix, vector);

            writeResultVector(res, "\n Result ", writer);//writing result vector into file            
            writer.Close();

            StreamWriter resWriter = new StreamWriter("SolvedResult.txt");
            for (int i = 0; i < domain.Meshes.Points.Length; i++)
            {
                resWriter.WriteLine( domain.Meshes.Points[i][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[i][1].ToString().Replace(',', '.') + " " + res[i].ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
            }
            resWriter.Close();
            return new InnerPolygonBoundaryProblemOutput(SlaeSolver.GaussMethod(matrix, vector));
        }

       
        public static double[][] matrixFormation(Point[] pVector)
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
            Integration integral = new Integration(n,pVector);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    integral.activeNumber1 = i;
                    integral.activeNumber2 = j;
                    IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.mePidIntegralFunction2D);
                    matrix[i][j] = Integration.areaIntegration(ifXY, 4);
                }
            }
            return matrix;
        }

        public static double[][] matrixFormation2(Point[] pVector)
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
            Integration integral = new Integration(n,pVector);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    integral.activeNumber1 = i;
                    integral.activeNumber2 = j;
                    IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.kePidIntegralFunction2D);
                    matrix[i][j] = Integration.areaIntegration(ifXY, 4);
                }
            }
            return matrix;
        }
        public static double[][] matrixFormation_1D(Point[] pVector, double leftSideCoeficient)
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
            Integration integral = new Integration(n2,pVector);

            for (int i = 0; i < n2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    integral.activeNumber1 = i;
                    integral.activeNumber2 = j;
                    IntegralFunctionX ifXY = new IntegralFunctionX(integral.lePidIntegralFunction1D);
                    matrix[i][j] = leftSideCoeficient * Integration.GaussIntegration(0.0, 1.0, 4, ifXY);
                }
            }
            return matrix;
        }
        public static double[] vectorFormation(Point[] pVector)
        {
            double[] vector = new double[n];
            for (int i = 0; i < n; i++)
            {
                vector[i] = 0.0;
            }
            Integration integral = new Integration(n,pVector);

            for (int i = 0; i < n; i++)
            {
                integral.activeNumber1 = i;
                IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.qePidIntegralFunction2D);
                vector[i] = Integration.areaIntegration(ifXY, 4);
            }
            return vector;
        }

        public static double[] vectorFormation1D(Point[] pVector, int activeBoundaryNumber, int activeSubBoundaryNumber, double rightSideCoeficient)
        {
            double[] vector = new double[n2];
            for (int i = 0; i < n2; i++)
            {
                vector[i] = 0.0;
            }
            Integration integral = new Integration(n2,pVector, activeBoundaryNumber,activeSubBoundaryNumber);

            for (int i = 0; i < n2; i++)
            {
                integral.activeNumber1 = i;
                IntegralFunctionX ifXY = new IntegralFunctionX(integral.rePidIntegralFunction1D);
                vector[i] = rightSideCoeficient * Integration.GaussIntegration(0.0, 1.0, 4, ifXY);
            }
            return vector;
        }

        public static double[] multiply(double[][] matrix, double[] f)
        {
            double[] result = new double[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = 0;
            }
            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = 0;
                for (int k = 0; k < matrix[0].Length; k++)
                {
                    result[i] += matrix[i][k] * f[k];
                } 
            }
            return result;
        }
    }
}
