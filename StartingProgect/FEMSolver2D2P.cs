using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    class FEMSolver2D2P : FEMSolver, InnerPolygonBoundaryProblemApproximation
    {
        private Dictionary<Point, double> concRes;
        private StreamWriter streamWriter;//= new StreamWriter("Boundary Result.txt");// temporary
        public FEMSolver2D2P(Domain domain):base(domain, 6,3)
        {
        }
        public FEMSolver2D2P()
        {
            //    domain = new Domain(new TwoDimentionalMeshOfTriangularElements(), new Boundaries());
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
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n2, pVector, activeBoundaryNumber,activeSubBoundaryNumber);

            for (int i = 0; i < n2; i++)
            {
                integral.activeNumber1 = i;
                IntegralFunctionX ifXY = new IntegralFunctionX(integral.rePidIntegralFunction1D2);
                vector[i] = (Physics.sigma[activeBoundaryNumber][activeSubBoundaryNumber]) / (Physics.beta[activeBoundaryNumber][activeSubBoundaryNumber]) * Integration.GaussIntegration(0.0, 1.0, 4, ifXY);
            }
            return vector;
        }
        public double findValue(Point p)
        {
            double eps = Math.Pow(1, -10);
            for(int i=0;i<concRes.Count;i++)
            {
                if ((Math.Abs(concRes.Keys.ToList()[i].X-p.X)<eps)&&(Math.Abs(concRes.Keys.ToList()[i].Y - p.Y)<eps))
                {
                    return concRes[concRes.Keys.ToList()[i]];
                }                
            }
            return concRes.Values.First();
        }
        public double[] vectorFormation1DPressure(Point[] pVector, int activeBoundaryNumber, int activeSubBoundaryNumber)
        {
            double[] vector = new double[n2];
            for (int i = 0; i < n2; i++)
            {
                vector[i] = 0.0;
            }
            double[] concentrationInPVector = new double[n2];
            for(int i=0;i<n2;i++)
            {
                double res = findValue(pVector[i]);//concRes[pVector[i]];
                //concRes.TryGetValue(pVector[i], out res);
                concentrationInPVector[i] = res;
            }
            //18.10.2018 sending concentration vector
            IntegrationOnSquaredApproximationElement integral = new IntegrationOnSquaredApproximationElement(n2, pVector, activeBoundaryNumber, activeSubBoundaryNumber,concentrationInPVector);

            for (int i = 0; i < n2; i++)
            {
                integral.activeNumber1 = i;
                IntegralFunctionX ifXY = new IntegralFunctionX(integral.rePidIntegralFunction1D2Pressure);
                vector[i] = (Physics.sigma[activeBoundaryNumber][activeSubBoundaryNumber]) / (Physics.beta[activeBoundaryNumber][activeSubBoundaryNumber]) * Integration.GaussIntegration(0.0, 1.0, 4, ifXY);
            }

            
            streamWriter.WriteLine("x0 - " +  Math.Round(pVector[0].X,4) + "  y0 - " + Math.Round(pVector[0].Y, 4) + "xm - " + Math.Round(pVector[1].X, 4) + "  ym - " + Math.Round(pVector[1].Y, 4) + "xe - " + Math.Round(pVector[2].X, 4) + "  ye - " + Math.Round(pVector[2].Y, 4));
            streamWriter.WriteLine(" vector[0] - " + Math.Round( vector[0],4) + " vector[1] - " +Math.Round( vector[1],4) + " vector[2] - " + Math.Round(vector[2],4));
            streamWriter.WriteLine();
            
            return vector;
        }
        public InnerPolygonBoundaryProblemOutput solve()
        {
            /////solve method for solving triangulation problems
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
                Point[] trianglePoints = new Point[n];
                for (int j = 0; j < n; j++)
                {
                    trianglePoints[j] = new Point(domain.Meshes.Points[domain.Meshes.Elements[i][j]][0], domain.Meshes.Points[domain.Meshes.Elements[i][j]][1]); //2d2
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
                        Point pMiddle= new Point(domain.Meshes.Points[number2][0], domain.Meshes.Points[number2][1]);
                        Point pEnd = new Point(domain.Meshes.Points[number3][0], domain.Meshes.Points[number3][1]);

                        Point[] boundaryPoints = new Point[] { pStart, pMiddle, pEnd };
                        int[] boundaryPointsNumbers = new int[] { number1, number2, number3 };//індекси загальної матриці

                        double[][] geMatrix = matrixFormation_1D(boundaryPoints);// ліва частина інтегрування по границі
                        writeResultMatrix(geMatrix, "\n" + " Condition " + domain.Boundaries.BoundaryArray[i][j][r].condition.ToString() + "\n" + "  ge matrix ", writer);//writing matrix into file

                        double[] leVector = vectorFormation1D(boundaryPoints, i, j);
                        writeResultVector(leVector, "\n leVector ", writer);//writing leVector into file  

                        for (int k = 0; k < n2; k++)
                        {
                            vector[boundaryPointsNumbers[k]] += rightSideCoeficient * leVector[k];
                            for (int p = 0; p < n2; p++)
                            {
                                writer.WriteLine(" boundaries " + boundaryPointsNumbers[k] + " " + boundaryPointsNumbers[p]);
                                matrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += leftSideCoeficient * geMatrix[k][p];
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
            for (int i = 0; i < domain.Meshes.PointsCount; i++)
            {
                resWriter.WriteLine(domain.Meshes.Points[i][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[i][1].ToString().Replace(',', '.') + " " + res[i].ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
            }
            resWriter.Close();

            return new InnerPolygonBoundaryProblemOutput(SlaeSolver.GaussMethod(matrix, vector));//return result
        }
        public bool isPointInsideOfSomeArea(Point p)
        {
            return (p.X * p.X + p.Y * p.Y - 4.0) < 0;//x^2+y^2=2^2
        }        
        public InnerPolygonBoundaryProblemOutput solvePressure()
        {
            Physics.d = 0.0;

            streamWriter = new StreamWriter("Boundary Result.txt");
            /////solve method for solving triangulation problems
            StreamReader readerBoundaryValuesConcentration = new StreamReader("SolvedResultConcentration.txt");
            concRes = new Dictionary<Point, double>();
            while(!readerBoundaryValuesConcentration.EndOfStream)
            {
                string[] mas = readerBoundaryValuesConcentration.ReadLine().Split();
                concRes.Add(new Point(Convert.ToDouble(mas[0].Replace('.', ',')), Convert.ToDouble(mas[1].Replace('.', ','))),Convert.ToDouble(mas[2].Replace('.', ',')));
            }
            readerBoundaryValuesConcentration.Close();
            
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
                Point[] trianglePoints = new Point[n];
                for (int j = 0; j < n; j++)
                {
                    trianglePoints[j] = new Point(domain.Meshes.Points[domain.Meshes.Elements[i][j]][0], domain.Meshes.Points[domain.Meshes.Elements[i][j]][1]); //2d2
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

                        double[] leVector = vectorFormation1DPressure(boundaryPoints, i, j);
                        writeResultVector(leVector, "\n leVector ", writer);//writing leVector into file  

                        for (int k = 0; k < n2; k++)
                        {
                            vector[boundaryPointsNumbers[k]] += rightSideCoeficient * leVector[k];
                            for (int p = 0; p < n2; p++)
                            {
                                writer.WriteLine(" boundaries " + boundaryPointsNumbers[k] + " " + boundaryPointsNumbers[p]);
                                matrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += leftSideCoeficient * geMatrix[k][p];
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
            for (int i = 0; i < domain.Meshes.PointsCount; i++)
            {
                //String.Format("{0:0.00000}", points.PointMas[i].X).
                resWriter.WriteLine(String.Format("{0:0.000}",domain.Meshes.Points[i][0]).ToString().Replace(',', '.') + " " + String.Format("{0:0.000}",domain.Meshes.Points[i][1]).ToString().Replace(',', '.') + " " + String.Format("{0:0.0}",res[i]).ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
            }
            StreamWriter resWriterBounadyTumorPressure = new StreamWriter("SolvedResultPressure.txt");
            for (int k = 0; k < domain.Boundaries.BoundaryArray.Count; k++)
            {
                for (int i = 0; i < domain.Boundaries.BoundaryArray[k].Count; i++)// запис граничних точок для тиску для першого кроку !!!// створити метод
                {
                    for (int j = 0; j < domain.Boundaries.BoundaryArray[k][i].Count; j++)
                    {
                        int number1 = domain.Boundaries.BoundaryArray[k][i][j].numberP1;
                        int number2 = domain.Boundaries.BoundaryArray[k][i][j].numberP2;
                        resWriterBounadyTumorPressure.WriteLine(String.Format("{0:0.00000}", domain.Meshes.Points[number1][0]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", domain.Meshes.Points[number1][1]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[number1]).ToString().Replace(',', '.'));
                        resWriterBounadyTumorPressure.WriteLine(String.Format("{0:0.00000}", domain.Meshes.Points[number2][0]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", domain.Meshes.Points[number2][1]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[number2]).ToString().Replace(',', '.'));
                        //resWriterBounadyTumorPressure.WriteLine(domain.Meshes.Points[number1][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[number1][1].ToString().Replace(',', '.') + " " + res[number1].ToString().Replace(',', '.'));
                        //resWriterBounadyTumorPressure.WriteLine(domain.Meshes.Points[number2][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[number2][1].ToString().Replace(',', '.') + " " + res[number2].ToString().Replace(',', '.'));
                        //String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
                    }
                }
            }
            resWriterBounadyTumorPressure.Close();
            resWriter.Close();
            streamWriter.Close();
            return new InnerPolygonBoundaryProblemOutput(SlaeSolver.GaussMethod(matrix, vector));//return result
        }
        public InnerPolygonBoundaryProblemOutput solve2()// метод для розв'язування задачі із різними значеннями коефіцієнтів d ta a11,a12 // для заданих двох різних областей
        {
            StreamWriter writer = new StreamWriter("ResultConcentration.txt");

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

            double a1 = 1.0; // Physics.a11;
            double a2 = 1.0; // Physics.a22;

            double a3 = 2.0;
            double a4 = 2.0;

            double d1 = 1.0;
            double d2 = 0.0;

            for (int i = 0; i < domain.Meshes.ElementCount; i++)
            {              
                Point[] trianglePoints = new Point[n];
                for (int j = 0; j < n; j++)
                {
                    trianglePoints[j] = new Point(domain.Meshes.Points[domain.Meshes.Elements[i][j]][0], domain.Meshes.Points[domain.Meshes.Elements[i][j]][1]); //2d2
                }
                Point centroid = new Point((trianglePoints[0].X + trianglePoints[1].X + trianglePoints[2].X) / 3, (trianglePoints[0].Y + trianglePoints[1].Y + trianglePoints[2].Y) / 3);
                Physics.a11 = a1;
                Physics.a22 = a2;
                Physics.d = d1;
                if(isPointInsideOfSomeArea(centroid))
                {
                    Physics.a11 = a3;
                    Physics.a22 = a4;
                    Physics.d = d2;
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

                        double[] leVector = vectorFormation1D(boundaryPoints, i, j);
                        writeResultVector(leVector, "\n leVector ", writer);//writing leVector into file  

                        for (int k = 0; k < n2; k++)
                        {
                            vector[boundaryPointsNumbers[k]] += rightSideCoeficient * leVector[k];
                            for (int p = 0; p < n2; p++)
                            {
                                writer.WriteLine(" boundaries " + boundaryPointsNumbers[k] + " " + boundaryPointsNumbers[p]);
                                matrix[boundaryPointsNumbers[k]][boundaryPointsNumbers[p]] += leftSideCoeficient * geMatrix[k][p];
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
            for (int i = 0; i < domain.Meshes.PointsCount; i++)
            {
                resWriter.WriteLine(domain.Meshes.Points[i][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[i][1].ToString().Replace(',', '.') + " " + res[i].ToString().Replace(',', '.'));//String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
            }
            resWriter.Close();
            StreamWriter resWriterBounadyTumorConcentration = new StreamWriter("SolvedResultConcentration.txt");
            //for (int i = 0; i < domain.Boundaries.BoundaryArray[1].Count; i++)// запис граничних точок для тиску для першого кроку  !!! // потрібно змінити
            //{
            //    for (int j = 0; j < domain.Boundaries.BoundaryArray[1][i].Count; j++)
            //    {
            //        int number1 = domain.Boundaries.BoundaryArray[1][i][j].numberP1;
            //        int number2 = domain.Boundaries.BoundaryArray[1][i][j].numberP2;
            //        resWriterBounadyTumorConcentration.WriteLine(domain.Meshes.Points[number1][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[number1][1].ToString().Replace(',', '.') + " " + res[number1].ToString().Replace(',', '.'));
            //        resWriterBounadyTumorConcentration.WriteLine(domain.Meshes.Points[number2][0].ToString().Replace(',', '.') + " " + domain.Meshes.Points[number2][1].ToString().Replace(',', '.') + " " + res[number2].ToString().Replace(',', '.'));
            //        //String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
            //    }
            //}
            for (int k = 0; k < domain.Boundaries.BoundaryArray.Count; k++)
            {
                for (int i = 0; i < domain.Boundaries.BoundaryArray[k].Count; i++)// запис граничних точок для тиску для першого кроку !!!// створити метод
                {
                    for (int j = 0; j < domain.Boundaries.BoundaryArray[k][i].Count; j++)
                    {
                        int number1 = domain.Boundaries.BoundaryArray[k][i][j].numberP1;
                        int number2 = domain.Boundaries.BoundaryArray[k][i][j].numberP2;
                        resWriterBounadyTumorConcentration.WriteLine(String.Format("{0:0.00000}",domain.Meshes.Points[number1][0]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", domain.Meshes.Points[number1][1]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[number1]).ToString().Replace(',', '.'));
                        resWriterBounadyTumorConcentration.WriteLine(String.Format("{0:0.00000}",domain.Meshes.Points[number2][0]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", domain.Meshes.Points[number2][1]).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[number2]).ToString().Replace(',', '.'));
                        //String.Format("{0:0.00000}", points.PointMas[i].X).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", points.PointMas[i].Y).ToString().Replace(',', '.') + " " + String.Format("{0:0.00000}", res[i]).ToString().Replace(',', '.'));
                    }
                }
            }
            resWriterBounadyTumorConcentration.Close();            
            return new InnerPolygonBoundaryProblemOutput(SlaeSolver.GaussMethod(matrix, vector));
        }
    }
}
