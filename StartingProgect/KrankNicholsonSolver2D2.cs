using System;
using System.IO;
using System.Windows;

namespace StartingProgect
{
    class KrankNicholsonSolver2D2: FEMSolver// розв'язування заданої нестаціонарної задачі методом Кранка-Ніклсона для квадратичної апроксимації
    {
        public KrankNicholsonSolver2D2(Domain domain):base(domain,6,3)
        {
            this.domain = domain;
        }
        public double[] KrankNichlSolver()
        {
            double[] T_0 = new double[domain.Meshes.PointsCount];
            for (int i = 0; i < T_0.Length; i++)
            {
                T_0[i] = 50.0;
            }
            double a = 0.0;
            double b = 10.0;
            int nn = 10;
            double tau = (b - a) / nn;
            tempSolve(b, nn, T_0, 1.0);
                        
            return T_0;
        }

        public double[][] matrixFormation(Point[] pVector)
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
                    IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.mePidIntegralFunction2D2);
                    matrix[i][j] = Integration.areaIntegration(ifXY, 4);
                }
            }
            return matrix;
        }

        public double[][] matrixFormation2(Point[] pVector)
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
                    IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.kePidIntegralFunction2D2);
                    matrix[i][j] = Integration.areaIntegration(ifXY, 4);
                }
            }
            return matrix;
        }
        public double[][] matrixFormation_1D(Point[] pVector)
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
                    IntegralFunctionX ifXY = new IntegralFunctionX(integral.lePidIntegralFunction1D2);
                    matrix[i][j] = Integration.GaussIntegration(0.0, 1.0, 4, ifXY);
                }
            }
            return matrix;
        }
        public double[] vectorFormation(Point[] pVector)
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
                IntegralFunctionXY ifXY = new IntegralFunctionXY(integral.qePidIntegralFunction2D2);
                vector[i] = Integration.areaIntegration(ifXY, 4);
            }
            return vector;
        }
        public double[] vectorFormation1D(Point[] pVector, int activeBoundaryNumber, int activeSubBoundaryNumber)
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
                IntegralFunctionX ifXY = new IntegralFunctionX(integral.rePidIntegralFunction1D2);
                vector[i] = (Physics.sigma[activeBoundaryNumber][activeSubBoundaryNumber]) / (Physics.beta[activeBoundaryNumber][activeSubBoundaryNumber]) * Integration.GaussIntegration(0.0, 1.0, 4, ifXY);
            }
            return vector;
        }
        public double[][] tempSolve(double teta, int nDivision, double[] T_0, double stationary)
        {
            double deltaTau = teta / nDivision;
            double[][] resMatrix = new double[nDivision + 1][];
            for (int i = 0; i < resMatrix.Length; i++)
            {
                resMatrix[i] = new double[T_0.Length];
            }
            for (int j = 0; j < T_0.Length; j++)
            {
                resMatrix[0][j] = T_0[j];
            }
            StreamWriter writer = new StreamWriter("Matrices.txt");
            StreamWriter resWriter = new StreamWriter("SolvedResult.txt");

            for (int kTime = 1; kTime <= nDivision; kTime++)
            {
                double[][] matrix = new double[domain.Meshes.PointsCount][];
                for (int i = 0; i < matrix.Length; i++)
                {
                    matrix[i] = new double[domain.Meshes.PointsCount];
                    for (int j = 0; j < matrix[i].Length; j++)
                    {
                        matrix[i][j] = 0;
                    }
                }
                double[][] tempMatrix = new double[domain.Meshes.PointsCount][];
                for (int i = 0; i < tempMatrix.Length; i++)
                {
                    tempMatrix[i] = new double[domain.Meshes.PointsCount];
                    for (int j = 0; j < tempMatrix[i].Length; j++)
                    {
                        tempMatrix[i][j] = 0;
                    }
                }
                double[] vector = new double[domain.Meshes.PointsCount];

                for (int i = 0; i < domain.Meshes.ElementCount; i++)
                {
                    Point[] trianglePoints = new Point[n];

                    for (int j = 0; j < n; j++)
                    {
                        trianglePoints[j] = new Point(domain.Meshes.Points[domain.Meshes.Elements[i][j]][0], domain.Meshes.Points[domain.Meshes.Elements[i][j]][1]); //2d1
                    }                   
              
                    double[][] keMatrix = matrixFormation2(trianglePoints);
                    writeResultMatrix(keMatrix, "\n keMatrix ", writer);//writing into file keMatrix    
                    
                    double[][] meMatrix = matrixFormation(trianglePoints);
                    double[][] ceMatrix = matrixFormation(trianglePoints);
                    writeResultMatrix(meMatrix, "\n meMatrix ", writer); //writing into file meMatrix  without multiplication by d 
                    
                    double[] qe = vectorFormation(trianglePoints);//D2TriangleApproximation1.multiply(meMatrix, fvector); // Physics should be different for different areas of triangulation
                    writeResultVector(qe, "\n qeVector ", writer);//writing into file qeVector
                   
                    for (int j = 0; j < n; j++)
                    {
                        vector[domain.Meshes.Elements[i][j]] += 2.0 * qe[j];//???
                    }
                    for (int j = 0; j < n; j++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            matrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += 0.5 * deltaTau * (keMatrix[j][k] + Physics.d * meMatrix[k][j]) + Physics.C * Physics.RO * ceMatrix[k][j];//
                            tempMatrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += keMatrix[j][k] + Physics.d * meMatrix[k][j];
                        }
                    }
                }
                for (int i = 0; i < domain.Boundaries.BoundaryArray.Count; i++)
                {
                    for (int j = 0; j < domain.Boundaries.BoundaryArray[i].Count; j++)
                    {
                        for (int r = 0; r < domain.Boundaries.BoundaryArray[i][j].Count; r++)
                        {
                            double[] coeficients = setCoeficients(i, j, r);
                            double leftSideCoeficient = coeficients[0];// sugma/beta*coef
                            double rightSideCoeficient = coeficients[1];// sugma/beta*coef

                            int number1 = domain.Boundaries.BoundaryArray[i][j][r].numberP1;
                            int number2 = domain.Boundaries.BoundaryArray[i][j][r].numberP2;
                            int number3 = domain.Boundaries.BoundaryArray[i][j][r].numberP3;

                            Point pStart = new Point(domain.Meshes.Points[number1][0], domain.Meshes.Points[number1][1]);
                            Point pMiddle = new Point(domain.Meshes.Points[number2][0], domain.Meshes.Points[number2][1]);
                            Point pEnd = new Point(domain.Meshes.Points[number3][0], domain.Meshes.Points[number3][1]);

                            Point[] boundaryPoints = new Point[] { pStart, pMiddle, pEnd };
                            int[] boundaryPointsNumbers = new int[] { number1, number2, number3 };//індекси загальної матриці

                            double[][] geMatrix = matrixFormation_1D(boundaryPoints);// ліва частина інтегрування по границі
                            writeResultMatrix(geMatrix, "\n" + " Condition " + domain.Boundaries.BoundaryArray[i][j][r].condition.ToString() + "\n" + "  ge matrix ", writer);//writing matrix into file
                            
                            for (int k = 0; k < n2; k++)
                            {
                                for (int p = 0; p < n2; p++)
                                {
                                    matrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += 0.5 * deltaTau * leftSideCoeficient * geMatrix[k][p];
                                    tempMatrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += leftSideCoeficient * geMatrix[k][p];
                                }
                            }

                            double[] leVector = vectorFormation1D(boundaryPoints, i, j);
                            writeResultVector(leVector, "\n leVector ", writer);//writing leVector into file  

                            for (int k = 0; k < n2; k++)
                            {
                                vector[boundaryPointsNumbers[k]] += rightSideCoeficient * leVector[k];
                            }            
                            
                        }
                    }
                }
                writeResultMatrix(matrix, "\n Matrix ", writer);//writing matrix into file
                writeResultVector(vector, "\n Vector ", writer);//writing vector into file 

                ///----///
                double[] dobutok = new double[T_0.Length];
                for (int i = 0; i < tempMatrix.Length; i++)
                {
                    dobutok[i] = 0;
                    for (int j = 0; j < tempMatrix[i].Length; j++)
                    {
                        dobutok[i] += tempMatrix[i][j] * resMatrix[kTime - 1][j];
                    }
                    //writer.Write(String.Format("{0:0.000}", dobutok[i]) + "   ");
                }


                double[] tempVector = new double[vector.Length];
                for (int i = 0; i < tempVector.Length; i++)
                {
                    vector[i] = 0.5 * (deltaTau * vector[i] - stationary * deltaTau * dobutok[i]);//changed ???
                    tempVector[i] = vector[i];
                }

                SlaeSolver slar = new SlaeSolver();
                double[] res = SlaeSolver.GaussMethod(matrix, vector);

                writer.WriteLine("result");
                for (int i = 0; i < domain.Meshes.PointsCount; i++)
                {
                    resMatrix[kTime][i] = res[i] + resMatrix[kTime - 1][i];
                    writer.Write(String.Format("{0:0.000}", res[i]) + "   ");
                }
                writer.WriteLine();

                //resWriter.WriteLine("ktime = " + kTime);

                //resWriter.WriteLine();
                //for (int i = 0; i < domain.Meshes.PointsCount; i++)
                //{
                //    if(i%10==0)
                //    {
                //        resWriter.WriteLine();
                //    }
                //    resWriter.Write(" deltaTau[" + i + "] = " + res[i]);
                //}
                //resWriter.WriteLine();

                //resWriter.WriteLine();
                //for (int i = 0; i < resMatrix[kTime].Length; i++)
                //{
                //    if (i % 10 == 0)
                //    {
                //        resWriter.WriteLine();
                //    }
                //    resWriter.Write(" point[" + i + "]=" + resMatrix[kTime][i] + "    ");
                //}
                //resWriter.WriteLine();

                StreamWriter resultWr = new StreamWriter("result" + kTime.ToString() + ".txt");

                for (int i = 0; i < domain.Meshes.PointsCount; i++)
                {
                    resultWr.WriteLine(domain.Meshes.Points[i][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[i][1].ToString().Replace(',', '.') + " " + resMatrix[kTime][i].ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
                }
                resultWr.Close();

            }
            writer.Close();
            resWriter.Close();
            return resMatrix;
        }
        public bool isPointInsideOfSomeArea(Point p)//checking whether point is inside of circle
        {
            return (p.X * p.X + p.Y * p.Y - 4.0) < 0;//x^2+y^2=2^2
        }
        public double[][] tempSolve2(double teta, int nDivision, double[] T_0, double stationary)
        {
            double deltaTau = teta / nDivision;
            double[][] resMatrix = new double[nDivision + 1][];
            for (int i = 0; i < resMatrix.Length; i++)
            {
                resMatrix[i] = new double[T_0.Length];
            }
            for (int j = 0; j < T_0.Length; j++)
            {
                resMatrix[0][j] = T_0[j];
            }
            StreamWriter writer = new StreamWriter("Matrices.txt");
            StreamWriter resWriter = new StreamWriter("SolvedResult.txt");

            for (int kTime = 1; kTime <= nDivision; kTime++)
            {
                double[][] matrix = new double[domain.Meshes.PointsCount][];
                for (int i = 0; i < matrix.Length; i++)
                {
                    matrix[i] = new double[domain.Meshes.PointsCount];
                    for (int j = 0; j < matrix[i].Length; j++)
                    {
                        matrix[i][j] = 0;
                    }
                }
                double[][] tempMatrix = new double[domain.Meshes.PointsCount][];
                for (int i = 0; i < tempMatrix.Length; i++)
                {
                    tempMatrix[i] = new double[domain.Meshes.PointsCount];
                    for (int j = 0; j < tempMatrix[i].Length; j++)
                    {
                        tempMatrix[i][j] = 0;
                    }
                }
                double[] vector = new double[domain.Meshes.PointsCount];
                ///////////////////
                double a1 = 1.0; // Physics.a11;
                double a2 = 1.0; // Physics.a22;

                double a3 = 2.0;
                double a4 = 2.0;

                double d1 = 1.0;
                double d2 = 0.0;
                ///////////////////
                for (int i = 0; i < domain.Meshes.ElementCount; i++)
                {
                    Point[] trianglePoints = new Point[n];

                    for (int j = 0; j < n; j++)
                    {
                        trianglePoints[j] = new Point(domain.Meshes.Points[domain.Meshes.Elements[i][j]][0], domain.Meshes.Points[domain.Meshes.Elements[i][j]][1]); //2d1
                    }
                    Point centroid = new Point((trianglePoints[0].X + trianglePoints[1].X + trianglePoints[2].X) / 3, (trianglePoints[0].Y + trianglePoints[1].Y + trianglePoints[2].Y) / 3);
                    Physics.a11 = a1;
                    Physics.a22 = a2;
                    Physics.d = d1;
                    if (isPointInsideOfSomeArea(centroid))
                    {
                        Physics.a11 = a3;
                        Physics.a22 = a4;
                        Physics.d = d2;
                    }

                    double[][] keMatrix = matrixFormation2(trianglePoints);
                    writeResultMatrix(keMatrix, "\n keMatrix ", writer);//writing into file keMatrix    

                    double[][] meMatrix = matrixFormation(trianglePoints);
                    double[][] ceMatrix = matrixFormation(trianglePoints);
                    writeResultMatrix(meMatrix, "\n meMatrix ", writer); //writing into file meMatrix  without multiplication by d 

                    double[] qe = vectorFormation(trianglePoints);//D2TriangleApproximation1.multiply(meMatrix, fvector); // Physics should be different for different areas of triangulation
                    writeResultVector(qe, "\n qeVector ", writer);//writing into file qeVector

                    for (int j = 0; j < n; j++)
                    {
                        vector[domain.Meshes.Elements[i][j]] += 2.0 * qe[j];//???
                    }
                    for (int j = 0; j < n; j++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            matrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += 0.5 * deltaTau * (keMatrix[j][k] + Physics.d * meMatrix[k][j]) + Physics.C * Physics.RO * ceMatrix[k][j];//
                            tempMatrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += keMatrix[j][k] + Physics.d * meMatrix[k][j];
                        }
                    }
                }
                for (int i = 0; i < domain.Boundaries.BoundaryArray.Count; i++)
                {
                    for (int j = 0; j < domain.Boundaries.BoundaryArray[i].Count; j++)
                    {
                        for (int r = 0; r < domain.Boundaries.BoundaryArray[i][j].Count; r++)
                        {
                            double[] coeficients = setCoeficients(i, j, r);
                            double leftSideCoeficient = coeficients[0];// sugma/beta*coef
                            double rightSideCoeficient = coeficients[1];// sugma/beta*coef

                            int number1 = domain.Boundaries.BoundaryArray[i][j][r].numberP1;
                            int number2 = domain.Boundaries.BoundaryArray[i][j][r].numberP2;
                            int number3 = domain.Boundaries.BoundaryArray[i][j][r].numberP3;

                            Point pStart = new Point(domain.Meshes.Points[number1][0], domain.Meshes.Points[number1][1]);
                            Point pMiddle = new Point(domain.Meshes.Points[number2][0], domain.Meshes.Points[number2][1]);
                            Point pEnd = new Point(domain.Meshes.Points[number3][0], domain.Meshes.Points[number3][1]);

                            Point[] boundaryPoints = new Point[] { pStart, pMiddle, pEnd };
                            int[] boundaryPointsNumbers = new int[] { number1, number2, number3 };//індекси загальної матриці

                            double[][] geMatrix = matrixFormation_1D(boundaryPoints);// ліва частина інтегрування по границі
                            writeResultMatrix(geMatrix, "\n" + " Condition " + domain.Boundaries.BoundaryArray[i][j][r].condition.ToString() + "\n" + "  ge matrix ", writer);//writing matrix into file

                            for (int k = 0; k < n2; k++)
                            {
                                for (int p = 0; p < n2; p++)
                                {
                                    matrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += 0.5 * deltaTau * leftSideCoeficient * geMatrix[k][p];
                                    tempMatrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += leftSideCoeficient * geMatrix[k][p];
                                }
                            }

                            double[] leVector = vectorFormation1D(boundaryPoints, i, j);
                            writeResultVector(leVector, "\n leVector ", writer);//writing leVector into file  

                            for (int k = 0; k < n2; k++)
                            {
                                vector[boundaryPointsNumbers[k]] += rightSideCoeficient * leVector[k];
                            }

                        }
                    }
                }
                writeResultMatrix(matrix, "\n Matrix ", writer);//writing matrix into file
                writeResultVector(vector, "\n Vector ", writer);//writing vector into file 

                ///----///
                double[] dobutok = new double[T_0.Length];
                for (int i = 0; i < tempMatrix.Length; i++)
                {
                    dobutok[i] = 0;
                    for (int j = 0; j < tempMatrix[i].Length; j++)
                    {
                        dobutok[i] += tempMatrix[i][j] * resMatrix[kTime - 1][j];
                    }
                    //writer.Write(String.Format("{0:0.000}", dobutok[i]) + "   ");
                }


                double[] tempVector = new double[vector.Length];
                for (int i = 0; i < tempVector.Length; i++)
                {
                    vector[i] = 0.5 * (deltaTau * vector[i] - stationary * deltaTau * dobutok[i]);//changed ???
                    tempVector[i] = vector[i];
                }

                SlaeSolver slar = new SlaeSolver();
                double[] res = SlaeSolver.GaussMethod(matrix, vector);

                writer.WriteLine("result");
                for (int i = 0; i < domain.Meshes.PointsCount; i++)
                {
                    resMatrix[kTime][i] = res[i] + resMatrix[kTime - 1][i];
                    writer.Write(String.Format("{0:0.000}", res[i]) + "   ");
                }
                writer.WriteLine();

                //resWriter.WriteLine("ktime = " + kTime);

                //resWriter.WriteLine();
                //for (int i = 0; i < domain.Meshes.PointsCount; i++)
                //{
                //    if(i%10==0)
                //    {
                //        resWriter.WriteLine();
                //    }
                //    resWriter.Write(" deltaTau[" + i + "] = " + res[i]);
                //}
                //resWriter.WriteLine();

                //resWriter.WriteLine();
                //for (int i = 0; i < resMatrix[kTime].Length; i++)
                //{
                //    if (i % 10 == 0)
                //    {
                //        resWriter.WriteLine();
                //    }
                //    resWriter.Write(" point[" + i + "]=" + resMatrix[kTime][i] + "    ");
                //}
                //resWriter.WriteLine();

                StreamWriter resultWr = new StreamWriter("result" + kTime.ToString() + ".txt");

                for (int i = 0; i < domain.Meshes.PointsCount; i++)
                {
                    resultWr.WriteLine(domain.Meshes.Points[i][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[i][1].ToString().Replace(',', '.') + " " + resMatrix[kTime][i].ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
                }
                resultWr.Close();

            }
            writer.Close();
            resWriter.Close();
            return resMatrix;
        }
    }
}
