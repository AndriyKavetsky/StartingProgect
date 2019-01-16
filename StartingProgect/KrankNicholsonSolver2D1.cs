using System;
using System.IO;
using System.Windows;

namespace StartingProgect
{
    class KrankNicholsonSolver2D1
    {
        protected Domain domain;
        protected int n = 3;
        protected int n2 = 2;
        public KrankNicholsonSolver2D1(Domain domain)
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

            //for(int i=0;i<n;i++)
            //{
            //    double [] delta_T = Solve(tau, T_0);
            //    for(int j=0;j<T_0.Length;j++)
            //    {
            //        T_0[j] += delta_T[j];
            //        writer.Write(" T[" + j + "] = " + T_0[j]);
            //    }
            //    writer.WriteLine();
            //}
            //writer.Close();
            return T_0;
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
                int n = 3;
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
                   
                    double doubleSquare = 0;
                    for (int j = 0; j < (n - 1); j++)
                    {
                        doubleSquare += trianglePoints[j].X * trianglePoints[j + 1].Y -
                             trianglePoints[j + 1].X * trianglePoints[j].Y;
                    }
                    doubleSquare += trianglePoints[n - 1].X * trianglePoints[0].Y -
                        trianglePoints[0].X * trianglePoints[n - 1].Y;
                    writer.WriteLine(" double area [" + i + "] = " + doubleSquare);

                    double fraction = 1.0 / (2.0 * doubleSquare);
                    double[] a = new double[n];
                    double[] b = new double[n];
                    double[] c = new double[n];
                    for (int j = 0; j < n; j++)
                    {
                        writer.WriteLine(" j+1 = " + ((j + 1) % 3) + " j+2 = " + ((j + 2) % 3));
                        a[j] = trianglePoints[(j + 1) % 3].X * trianglePoints[(j + 2) % 3].Y - trianglePoints[(j + 2) % 3].X * trianglePoints[(j + 1) % 3].Y;
                        b[j] = trianglePoints[(j + 1) % 3].Y - trianglePoints[(j + 2) % 3].Y;
                        c[j] = trianglePoints[(j + 2) % 3].X - trianglePoints[(j + 1) % 3].X;
                        writer.WriteLine(" a[" + j + "] = " + a[j] + " b[" + j + "] = " + b[j] + " c[" + j + "] = " + c[j]);
                    }
                    //another method

                    double[][] keMatrix1 = new double[n][];
                    for (int j = 0; j < n; j++)
                    {
                        keMatrix1[j] = new double[n];
                        keMatrix1[j][j] = 0;///Integration.GaussIntegration(//fraction * (Math.Pow(a11, 2) * b[j] * b[j] + Math.Pow(a22, 2) * c[j] * c[j]);
                    }

                    //
                    double[][] keMatrix = new double[n][];
                    for (int j = 0; j < n; j++)
                    {
                        keMatrix[j] = new double[n];
                        keMatrix[j][j] = fraction * (Math.Pow(Physics.a11, 1) * b[j] * b[j] + Math.Pow(Physics.a22, 1) * c[j] * c[j]);
                    }
                    for (int j = 1; j < n; j++)
                    {
                        for (int k = 0; k < j; k++)
                        {
                            keMatrix[k][j] = keMatrix[j][k] = fraction * (Math.Pow(Physics.a11, 1) * b[k] * b[j] + Math.Pow(Physics.a22, 1) * c[k] * c[j]);
                        }
                    }
                    writer.WriteLine("\n" + "ke matrix ");
                    for (int j = 0; j < n; j++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            writer.Write(String.Format("{0:0.000}", keMatrix[j][k]) + " ");
                        }
                        writer.WriteLine();
                    }

                    fraction = doubleSquare / 24.0;
                    double doublefraction = 2.0 * fraction;
                    double[][] meMatrix = new double[n][];
                    double[][] ceMatrix = new double[n][];

                    for (int j = 0; j < n; j++)
                    {
                        meMatrix[j] = new double[n];
                        meMatrix[j][j] = doublefraction;
                        ceMatrix[j] = new double[n];
                        ceMatrix[j][j] = doublefraction;
                    }
                    for (int j = 1; j < n; j++)
                    {
                        for (int k = 0; k < j; k++)
                        {
                            meMatrix[k][j] = meMatrix[j][k] = fraction;
                            ceMatrix[k][j] = ceMatrix[j][k] = fraction;
                        }
                    }
                    writer.WriteLine("\n" + "me matrix");
                    for (int j = 0; j < n; j++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            writer.Write(String.Format("{0:0.000}", Physics.d * meMatrix[j][k]) + " ");
                        }
                        writer.WriteLine();
                    }

                    double[] fvector = new double[n];
                    for (int j = 0; j < n; j++)
                    {
                        fvector[j] = Physics.f(trianglePoints[j].X, trianglePoints[j].Y);
                    }
                    double[] qe = FEMSolver2D1P.multiply(meMatrix, fvector);

                    writer.WriteLine("\n" + "qe vector ");
                    for (int j = 0; j < n; j++)
                    {
                        writer.Write(String.Format("{0:0.000}", qe[j]) + " ");
                    }
                    writer.WriteLine();
                    for (int j = 0; j < n; j++)
                    {
                        vector[domain.Meshes.Elements[i][j]] += 2.0 * qe[j];//???
                    }
                    for (int j = 0; j < n; j++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            // MessageBox.Show("j = " + j + "k = " + k + "num[j]" + numberOfPoints[j] + " num[k] = " + numberOfPoints[k]);
                            matrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += 0.5 * deltaTau * (keMatrix[j][k] + Physics.d * meMatrix[k][j]) + Physics.C * Physics.RO * ceMatrix[k][j];//
                            tempMatrix[domain.Meshes.Elements[i][j]][domain.Meshes.Elements[i][k]] += keMatrix[j][k] + Physics.d * meMatrix[k][j];
                        }
                    }
                }
                int n2 = 2;
                for (int i = 0; i < domain.Boundaries.BoundaryArray.Count; i++)
                {
                    for (int j = 0; j < domain.Boundaries.BoundaryArray[i].Count; j++)
                    {
                        for (int r = 0; r < domain.Boundaries.BoundaryArray[i][j].Count; r++)
                        {
                            int number1 = domain.Boundaries.BoundaryArray[i][j][r].numberP1;
                            int number2 = domain.Boundaries.BoundaryArray[i][j][r].numberP2;
                            int[] boundaryNumbers = new int[] { number1, number2 };//індекси загальної матриці

                            Point pStart = new Point(domain.Meshes.Points[number1][0], domain.Meshes.Points[number1][1]);
                            Point pEnd = new Point(domain.Meshes.Points[number2][0], domain.Meshes.Points[number2][1]);

                            switch (domain.Boundaries.BoundaryArray[i][j][r].condition)
                            {
                                case (Condition.Dirichlet):
                                    {
                                        double length = Math.Sqrt(Math.Pow(pEnd.X - pStart.X, 2) + Math.Pow(pEnd.Y - pStart.Y, 2));
                                        double[][] geMatrix = new double[n2][];//ліва частина 
                                        for (int k = 0; k < n2; k++)
                                        {
                                            geMatrix[k] = new double[n2];
                                            geMatrix[k][k] = (Physics.sigma[i][j] * Math.Pow(10, 16) * 2 * length) / (Physics.beta[i][j] * 6);
                                        }
                                        for (int k = 1; k < n2; k++)
                                        {
                                            for (int p = 0; p < k; p++)
                                            {
                                                geMatrix[k][p] = geMatrix[p][k] = (Physics.sigma[i][j] * Math.Pow(10, 16) * length) / (Physics.beta[i][j] * 6);
                                            }
                                        }
                                        writer.WriteLine("\n" + "Dirichlet  " + "\n" + "ge matrix ");
                                        for (int k = 0; k < n2; k++)
                                        {
                                            for (int p = 0; p < n2; p++)
                                            {
                                                writer.Write(String.Format("{0:0.000}", geMatrix[k][p]) + " ");
                                            }
                                            writer.WriteLine();
                                        }
                                        for (int k = 0; k < n2; k++)
                                        {
                                            for (int p = 0; p < n2; p++)
                                            {
                                                //writer.WriteLine(" boundaries " + boundaryPointsNumbers[k] + " " + boundaryPointsNumbers[p]);
                                                matrix[boundaryNumbers[k]][boundaryNumbers[p]] += 0.5 * deltaTau * geMatrix[k][p];
                                                tempMatrix[boundaryNumbers[k]][boundaryNumbers[p]] += geMatrix[k][p];
                                            }
                                        }
                                        double[] uEnv = new double[] { Physics.Tc[i][j], Physics.Tc[i][j] };
                                        double[] leVector = FEMSolver2D1P.multiply(geMatrix, uEnv);
                                        for (int k = 0; k < n2; k++)
                                        {
                                            vector[boundaryNumbers[k]] += leVector[k];
                                        }
                                        writer.WriteLine("\n" + "le vector ");
                                        for (int k = 0; k < n2; k++)
                                        {
                                            writer.Write(String.Format("{0:0.000}", leVector[k]) + " ");
                                        }
                                        writer.WriteLine();
                                    }
                                    break;
                                case (Condition.Neumann):
                                    {
                                        writer.WriteLine("\n" + "Neuman  " + "\n" + " condition ");
                                        double length = Math.Sqrt(Math.Pow(pEnd.X - pStart.X, 2) + Math.Pow(pEnd.Y - pStart.Y, 2));
                                        double[][] geMatrix = new double[n2][];//ліва частина 
                                        for (int k = 0; k < n2; k++)
                                        {
                                            geMatrix[k] = new double[n2];
                                            geMatrix[k][k] = (Physics.sigma[i][j] * Math.Pow(10, -16) * 2 * length) / (Physics.beta[i][j] * 6);
                                        }
                                        for (int k = 1; k < n2; k++)
                                        {
                                            for (int p = 0; p < k; p++)
                                            {
                                                geMatrix[k][p] = geMatrix[p][k] = (Physics.sigma[i][j] * Math.Pow(10, -16) * length) / (Physics.beta[i][j] * 6);
                                            }
                                        }
                                        writer.WriteLine("\n" + "Neuman  " + "\n" + "ge matrix ");
                                        for (int k = 0; k < n2; k++)
                                        {
                                            for (int p = 0; p < n2; p++)
                                            {
                                                writer.Write(String.Format("{0:0.000}", geMatrix[k][p]) + " ");
                                            }
                                            writer.WriteLine();
                                        }
                                        for (int k = 0; k < n2; k++)
                                        {
                                            for (int p = 0; p < n2; p++)
                                            {
                                                //writer.WriteLine(" boundaries " + boundaryPointsNumbers[k] + " " + boundaryPointsNumbers[p]);
                                                matrix[boundaryNumbers[k]][boundaryNumbers[p]] += 0.5 * deltaTau * geMatrix[k][p];
                                                tempMatrix[boundaryNumbers[k]][boundaryNumbers[p]] += geMatrix[k][p];
                                            }
                                        }
                                        double[] uEnv = new double[] { Physics.Tc[i][j] * Math.Pow(10, 16), Physics.Tc[i][j] * Math.Pow(10, 16) };
                                        double[] leVector = FEMSolver2D1P.multiply(geMatrix, uEnv);
                                        for (int k = 0; k < n2; k++)
                                        {
                                            vector[boundaryNumbers[k]] += leVector[k];
                                        }
                                        writer.WriteLine("\n" + "le vector ");
                                        for (int k = 0; k < n2; k++)
                                        {
                                            writer.Write(String.Format("{0:0.000}", leVector[k]) + " ");
                                        }
                                        writer.WriteLine();
                                    }
                                    break;
                                case (Condition.Robin):
                                    {
                                        double length = Math.Sqrt(Math.Pow(pEnd.X - pStart.X, 2) + Math.Pow(pEnd.Y - pStart.Y, 2));
                                        double[][] geMatrix = new double[n2][];//ліва частина                                 
                                        for (int k = 0; k < n2; k++)
                                        {
                                            geMatrix[k] = new double[n2];
                                            geMatrix[k][k] = (Physics.sigma[i][j] * 2 * length) / (Physics.beta[i][j] * 6);
                                        }
                                        for (int k = 1; k < n2; k++)
                                        {
                                            for (int p = 0; p < k; p++)
                                            {
                                                geMatrix[k][p] = geMatrix[p][k] = (Physics.sigma[i][j] * length) / (Physics.beta[i][j] * 6);
                                            }
                                        }

                                        writer.WriteLine("\n" + "Robin  " + "\n" + "ge matrix ");
                                        for (int k = 0; k < n2; k++)
                                        {
                                            for (int p = 0; p < n2; p++)
                                            {
                                                writer.Write(String.Format("{0:0.000}", geMatrix[k][p]) + " ");
                                            }
                                            writer.WriteLine();
                                        }
                                        for (int k = 0; k < n2; k++)
                                        {
                                            for (int p = 0; p < n2; p++)
                                            {
                                                //writer.WriteLine(" boundaries " + boundaryPointsNumbers[k] + " " + boundaryPointsNumbers[p]);
                                                matrix[boundaryNumbers[k]][boundaryNumbers[p]] += 0.5 * deltaTau * geMatrix[k][p];
                                                tempMatrix[boundaryNumbers[k]][boundaryNumbers[p]] += geMatrix[k][p];
                                            }
                                        }
                                        double[] uEnv = new double[] { Physics.Tc[i][j], Physics.Tc[i][j] };
                                        double[] leVector = FEMSolver2D1P.multiply(geMatrix, uEnv);
                                        for (int k = 0; k < n2; k++)
                                        {
                                            vector[boundaryNumbers[k]] += leVector[k];
                                        }
                                        writer.WriteLine("\n" + "le vector ");
                                        for (int k = 0; k < n2; k++)
                                        {
                                            writer.Write(String.Format("{0:0.000}", leVector[k]) + " ");
                                        }
                                        writer.WriteLine();

                                    }
                                    break;
                            }
                        }
                    }
                }
                writer.WriteLine("Matrix");
                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j < matrix[i].Length; j++)
                    {
                        writer.Write(String.Format("{0:0.000}", matrix[i][j]) + " ");
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("Vector ");
                for (int i = 0; i < vector.Length; i++)
                {
                    writer.Write(String.Format("{0:0.000}", vector[i]) + " ");
                }
                writer.WriteLine();
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

                resWriter.WriteLine("ktime = " + kTime);

                resWriter.WriteLine();
                for (int i = 0; i < domain.Meshes.PointsCount; i++)
                {
                    resWriter.Write(" deltaTau[" + i + "] = " + res[i]);
                }
                resWriter.WriteLine();

                resWriter.WriteLine();
                for (int i = 0; i < resMatrix[kTime].Length; i++)
                {
                    resWriter.Write(" point[" + i + "]=" + resMatrix[kTime][i] + "    ");
                }
                resWriter.WriteLine();
                //for (int i = 0; i < points.PointMas.Count; i++)
                //{
                //    resWriter.WriteLine(points.PointMas[i].X.ToString().Replace(',', '.') + " " + points.PointMas[i].Y.ToString().Replace(',', '.') + " " + res[i].ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
                //}

            }
            writer.Close();
            resWriter.Close();
            return resMatrix;
        }
    }
}
