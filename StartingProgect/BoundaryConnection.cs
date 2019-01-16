using System;
using System.Windows;
using System.Collections.Generic;


namespace StartingProgect
{
    public class BoundaryConnection
    {
        private List<List<List<Boundary>>> boundaryArray;//масиви граничних точок, що містять підграниці для можливості задати різні підграничні умови 

        private double eps = Math.Pow(10, -10);
        private List<Point> tempPoints;//масив вершин для додавання ноивих точок  

        private double epsTimeDisplacement = Math.Pow(10, -2);//Checking is displacement in time is less then eps
        private bool isSatisfiedDisplacementCondition = true;//for checking whether displacement in time is less then epsTimeDisplacement

        public BoundaryConnection()
        {
            boundaryArray = new List<List<List<Boundary>>>();
        }
        public BoundaryConnection(List<List<List<Boundary>>> boundaryArray)
        {
            this.boundaryArray = boundaryArray;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void OwerWriteToSquareApproximation(double [][] points)
        {
            tempPoints = new List<Point>();
            for (int i = 0; i < points.Length; i++)
            {
                tempPoints.Add(new Point(points[i][0], points[i][1]));
            }
            boundaryArray = convertBoundariesForSquareApproximation(points);
            points = new double[tempPoints.Count][];
            for (int i = 0; i < tempPoints.Count; i++)
            {
                points[i] = new double[2];
                points[i][0] = tempPoints[i].X;
                points[i][1] = tempPoints[i].Y;
            }
        }

        public List<List<List<Boundary>>> convertBoundariesForSquareApproximation(double[][] points)//перезапис граничних елементів для використання квадратичних апроксимацій
        {
            List<List<List<Boundary>>> tempboundaryPointsNumber = new List<List<List<Boundary>>>();

            for (int i = 0; i < boundaryArray.Count; i++)
            {
                tempboundaryPointsNumber.Add(new List<List<Boundary>>());
                for (int r = 0; r < boundaryArray[i].Count; r++)
                {
                    tempboundaryPointsNumber[i].Add(new List<Boundary>());
                    for (int j = 0; j < boundaryArray[i][r].Count; j++)
                    {
                        double newX, newY;
                        Point newPoint;
                        int pointNumber;
                        newX = (points[boundaryArray[i][r][j].numberP1][0] + points[boundaryArray[i][r][j].numberP2][0]) / 2.0;
                        newY = (points[boundaryArray[i][r][j].numberP1][1] + points[boundaryArray[i][r][j].numberP2][1]) / 2.0;
                        newPoint = new Point(newX, newY);
                        pointNumber = findPointNumber(newPoint);
                        tempboundaryPointsNumber[i][r].Add(new Boundary(boundaryArray[i][r][j].numberP1, pointNumber, boundaryArray[i][r][j].numberP2, boundaryArray[i][r][j].condition));
                    }
                }
            }

            return tempboundaryPointsNumber;
        }

        public int findPointNumber(Point point)// метод для пошуку заданої точки у масиві точок
        {
            int pointNumber = -1;
            for (int i = 0; i < tempPoints.Count; i++)
            {
                if ((Math.Abs(tempPoints[i].X - point.X) < eps) && ((Math.Abs(tempPoints[i].Y - point.Y) < eps)))
                {
                    return i;
                }
            }
            tempPoints.Add(new Point(point.X, point.Y));
            pointNumber = tempPoints.Count - 1;
            return pointNumber;
        }

        public static List<List<Boundary>> Sort(int boundaryNumber,List<List<Boundary>> tempBoundaryArray)//сортування масиву даних i-тої границі
        {
            if ((tempBoundaryArray[boundaryNumber] == null)||(tempBoundaryArray[boundaryNumber].Count<=0))
                return new List<List<Boundary>>();

            List<Boundary> temp = new List<Boundary>();
            bool isSortedBoundary = false;
            int k = 0;
            int number = tempBoundaryArray[boundaryNumber][k].numberP2;
            int pointPoisition = 1;

            temp.Add(new Boundary(tempBoundaryArray[boundaryNumber][k].numberP1, tempBoundaryArray[boundaryNumber][k].numberP2, tempBoundaryArray[boundaryNumber][k].condition));
            tempBoundaryArray[boundaryNumber].RemoveAt(k);

            while (!isSortedBoundary)
            {
                if ((tempBoundaryArray[boundaryNumber].Count <= 0)|| (tempBoundaryArray[boundaryNumber] == null))
                {
                    isSortedBoundary = true;
                    break;
                }

                k = FindPosition(boundaryNumber, number,pointPoisition,tempBoundaryArray);//boundaryArray[boundaryNumber][k].numberP2);

                if ((k < 0)&&(tempBoundaryArray[boundaryNumber]!=null))
                {
                    pointPoisition = ChangePosition(pointPoisition);
                    k = FindPosition(boundaryNumber,number, pointPoisition,tempBoundaryArray);
                }
                if(k<0)//&&(BoundaryArray[boundaryNumber]==null))
                {
                    isSortedBoundary = true;
                    break;
                }

                if (pointPoisition == 1)
                {
                    number = tempBoundaryArray[boundaryNumber][k].numberP2;
                    temp.Add(new Boundary(tempBoundaryArray[boundaryNumber][k].numberP1, tempBoundaryArray[boundaryNumber][k].numberP2, tempBoundaryArray[boundaryNumber][k].condition));
                }
                else {
                    number = tempBoundaryArray[boundaryNumber][k].numberP1;
                    temp.Add(new Boundary(tempBoundaryArray[boundaryNumber][k].numberP2, tempBoundaryArray[boundaryNumber][k].numberP1, tempBoundaryArray[boundaryNumber][k].condition));
                }
                tempBoundaryArray[boundaryNumber].RemoveAt(k);               
            }
            tempBoundaryArray[boundaryNumber] = temp;
            return tempBoundaryArray;
        }
        public static int FindPosition(int boundaryNumber, int pointNumber,int pointPosition, List<List<Boundary>> tempBoundaryArray)//пошук номеру точки для з'єднання
        {                                                                //pointPosition = 0 when left point number is finding and 1 when right point number is finding
            if(tempBoundaryArray[boundaryNumber]==null)
            {
                return -1;
            }

            for (int i = 0; i < tempBoundaryArray[boundaryNumber].Count; i++)
            {
                if (pointPosition == 1)
                {
                    if (tempBoundaryArray[boundaryNumber][i].numberP1 == pointNumber)
                        return i;
                }
                if(pointPosition==0)
                {
                    if (tempBoundaryArray[boundaryNumber][i].numberP2 == pointNumber)
                        return i;
                }
            }

            return -1;
        }

        public static int ChangePosition(int poinPoisition)
        {
            return poinPoisition == 0 ? 1 : poinPoisition == 1 ? 0 : 1;
        }
        public void AddNewBoundary(List<List<Boundary>> newBoundary, int boundary)
        {
            if (newBoundary == null)
            {
                return;
            }
            boundaryArray.Add(newBoundary);
        }       

        public void SetNewValuesToBoundary(int tempBoundaryNumber,List<List<Boundary>> tempBoundary)
        {
            if((boundaryArray==null)||(boundaryArray.Count<=0))
            {
                return;
            }
            if(boundaryArray.Count<tempBoundaryNumber)
            {
                return;
            }
            boundaryArray[tempBoundaryNumber] = new List<List<Boundary>>();
            for(int i=0;i<tempBoundary.Count;i++)
            {
                boundaryArray[tempBoundaryNumber].Add(new List<Boundary>());
                for(int j=0;j<tempBoundary[i].Count;j++)
                {
                    boundaryArray[tempBoundaryNumber][i].Add(tempBoundary[i][j]);
                }
            }
        }
        public Domain Change(double[] concentration, double [] pressure, Domain domain, bool modifyTime, ref double deltaT)//linear approximation
        {
            domain.Meshes.CreateBoundaryElements(boundaryArray);
            List<Dictionary<int, SortedSet<int>>> numbersOfBoundaryElements = domain.Meshes.numbersOfBoundaryElements;

            Dictionary<int, Point> newPoints = new Dictionary<int, Point>();
            bool isNeedToChangeTime = true;

            while (isNeedToChangeTime)
            {
                if(modifyTime==false)
                {
                    isNeedToChangeTime = false;
                }
                newPoints = new Dictionary<int, Point>();
                for (int i = 0; i < boundaryArray.Count; i++)
                {
                    for (int j = 0; j < boundaryArray[i].Count; j++)
                    {
                        for (int k = 0; k < boundaryArray[i][j].Count - 1; k++)
                        {
                            int number1 = domain.Boundaries.BoundaryArray[i][j][k].numberP1;
                            int number2 = domain.Boundaries.BoundaryArray[i][j][k].numberP3;
                            int number3 = domain.Boundaries.BoundaryArray[i][j][k + 1].numberP3;
                            newPoints.Add(number2, FindingNewPoint(number1, number2, number3, concentration, pressure, domain, numbersOfBoundaryElements, i, deltaT));
                            if((isSatisfiedDisplacementCondition==false)&&(isNeedToChangeTime))
                            {
                                goto End;
                            }
                        }
                        int length = boundaryArray[i][j].Count;
                        if (j == (boundaryArray[i].Count - 1))
                        {
                            int number1 = domain.Boundaries.BoundaryArray[i][j][length - 1].numberP1;
                            int number2 = domain.Boundaries.BoundaryArray[i][j][length - 1].numberP3;
                            int number3 = domain.Boundaries.BoundaryArray[i][0][0].numberP3;
                            newPoints.Add(number2, FindingNewPoint(number1, number2, number3, concentration, pressure, domain, numbersOfBoundaryElements, i, deltaT));
                        }
                        else
                        {
                            //1
                            int number1 = domain.Boundaries.BoundaryArray[i][j][length - 1].numberP1;
                            int number2 = domain.Boundaries.BoundaryArray[i][j][length - 1].numberP2;
                            int number3 = domain.Boundaries.BoundaryArray[i][j + 1][0].numberP1;
                            newPoints.Add(number2, FindingNewPoint(number1, number2, number3, concentration, pressure, domain, numbersOfBoundaryElements, i, deltaT));
                            //2???
                            int number11 = domain.Boundaries.BoundaryArray[i][j][length - 1].numberP2;
                            int number22 = domain.Boundaries.BoundaryArray[i][j + 1][0].numberP1;
                            int number33 = domain.Boundaries.BoundaryArray[i][j + 1][0].numberP2;
                            newPoints.Add(number22, FindingNewPoint(number11, number22, number33, concentration, pressure, domain, numbersOfBoundaryElements, i, deltaT));
                        }
                        if ((isSatisfiedDisplacementCondition == false) && (isNeedToChangeTime))
                        {
                            goto End;
                        }
                    }
                }
                if(isSatisfiedDisplacementCondition)
                {
                    isNeedToChangeTime = false;
                    break;
                }
                End:
                {                    
                    isSatisfiedDisplacementCondition = true;
                    if(isNeedToChangeTime)
                    deltaT = deltaT / 10;
                }
            }
            //Заміна точок в класі Domain

            Domain newDomain = new Domain((Mesh)domain.Meshes.Clone(),(BoundaryConnection)domain.Boundaries.Clone());
            foreach(KeyValuePair<int,Point> p in newPoints)
            {
                newDomain.Meshes.Points[p.Key][0] = p.Value.X;
                newDomain.Meshes.Points[p.Key][1] = p.Value.Y;
            }
            return newDomain;
        }

        public Point FindingNewPoint(int number1, int number2,int number3, double[] concentration, double[] pressure, Domain domain, List<Dictionary<int, 
                                    SortedSet<int>>> numbersOfBoundaryElements, int boundaryNumber, double deltaT)
        {
            int n = 3;
            Point p1 = new Point(domain.Meshes.Points[number1][0], domain.Meshes.Points[number1][1]);
            Point p2 = new Point(domain.Meshes.Points[number2][0], domain.Meshes.Points[number2][1]);
            Point p3 = new Point(domain.Meshes.Points[number3][0], domain.Meshes.Points[number3][1]);
            D2TriangleLinearApproximationNormal normalCalculating = new D2TriangleLinearApproximationNormal(p1, p2, p3, true);
            Point normal = normalCalculating.FindNormal();

            IEnumerator<int> enumerator = numbersOfBoundaryElements[boundaryNumber][number2].GetEnumerator();
            int elementNumber = enumerator.Current;//номер елементу який містить поточну граничну точку

            //масив точок поточного граничного елемента
            int currentPointNumber = domain.Meshes.Elements[elementNumber][0];
            Point[] pointsMas = new Point[n];
            pointsMas[0] = new Point(domain.Meshes.Points[currentPointNumber][0], domain.Meshes.Points[currentPointNumber][1]);
            currentPointNumber = domain.Meshes.Elements[elementNumber][1];
            pointsMas[1] = new Point(domain.Meshes.Points[currentPointNumber][0], domain.Meshes.Points[currentPointNumber][1]);
            currentPointNumber = domain.Meshes.Elements[elementNumber][2];
            pointsMas[2] = new Point(domain.Meshes.Points[currentPointNumber][0], domain.Meshes.Points[currentPointNumber][1]);
            //
            Point transformedPoint = pointTransformation(pointsMas, p2);
            
            double[] gradConcentration = new double[2];//concentration
            double[] gradPressure = new double[2];//pressure
            Integration integation = new Integration(3, pointsMas);
            double[][] jacobian = integation.jacobian_1(transformedPoint.X, transformedPoint.Y);
            //
            double[][] tempGradN = new double[2][];
            double[] tempConcentration = new double[n];
            double[] tempPressure = new double[n];

            for (int i = 0; i < tempGradN.Length; i++)
            {
                tempGradN[i] = new double[n];                
            }
            for(int j=0;j<n;j++)
            {
                tempGradN[0][j] = BaseFunctions2D1.baseFunctionDKsi(j, transformedPoint.X, transformedPoint.Y);
                tempGradN[1][j] = BaseFunctions2D1.baseFunctionDEta(j, transformedPoint.X, transformedPoint.Y);
                tempConcentration[j] = concentration[domain.Meshes.Elements[elementNumber][j]];
                tempPressure[j] = pressure[domain.Meshes.Elements[elementNumber][j]];
            }
            double[][] gradN_jac = Integration.matrixMultiplication(jacobian, tempGradN);
            for(int i=0;i<gradConcentration.Length;i++)
            {
                gradConcentration[i] = 0;
                gradPressure[i] = 0;
                for(int j=0;j<n;j++)
                {
                    gradConcentration[i] += gradN_jac[i][j] * tempConcentration[j];
                    gradPressure[i] += gradN_jac[i][j] * tempPressure[j];
                }
            }
            //            
            //for (int i = 0; i < gradConcentration.Length; i++)
            //{
            //    gradConcentration[i] = 0.0;
            //    gradPressure[i] = 0.0;
            //}
            //for(int i=0; i<n;i++)
            //{
            //    gradConcentration[0] += concentration[domain.Meshes.Elements[elementNumber][i]] * BaseFunctions2D1.baseFunctionDKsi(i, transformedPoint.X, transformedPoint.Y);
            //    gradConcentration[1] += concentration[domain.Meshes.Elements[elementNumber][i]] * BaseFunctions2D1.baseFunctionDEta(i, transformedPoint.X, transformedPoint.Y);
            //    gradPressure[0] += pressure[domain.Meshes.Elements[elementNumber][i]] * BaseFunctions2D1.baseFunctionDKsi(i, transformedPoint.X, transformedPoint.Y);
            //    gradPressure[1] += pressure[domain.Meshes.Elements[elementNumber][i]] * BaseFunctions2D1.baseFunctionDEta(i, transformedPoint.X, transformedPoint.Y);

            //}
            //double [] tempGradConcentration = new double[2];
            //double[] tempGradPressure = new double[2];
            //for(int i=0;i<jacobian.Length;i++)
            //{
            //    for(int j=0;j<jacobian[i].Length;j++)
            //    {
            //        tempGradConcentration[i] += jacobian[i][j] * gradConcentration[j];
            //        tempGradPressure[i] += jacobian[i][j] * gradPressure[j];
            //    }
            //}
            //gradConcentration = tempGradConcentration;//???
            //gradPressure = tempGradPressure;
            ////////
            //double[] gradConcentration = new double[] { ((concentration[number1] - concentration[number2])/(p1.X-p2.X)+ (concentration[number3] - concentration[number2]) / (p3.X - p2.X))/2.0,
            //                                            ((concentration[number1] - concentration[number2])/(p1.Y-p2.Y)+ (concentration[number3] - concentration[number2]) / (p3.Y - p2.Y))/2.0};
            //double[] gradPressure = new double[] { ((pressure[number1] - pressure[number2]) / (p1.X - p2.X) + (pressure[number3] - pressure[number2]) / (p3.X - p2.X)) / 2.0,
            //                                            ((pressure[number1] - pressure[number2]) / (p1.Y - p2.Y) + (pressure[number3] - pressure[number2]) / (p3.Y - p2.Y)) / 2.0};
            //-normal.X * gradPressure[0] - normal.Y * gradPressure[1]
            //- (Physics.A*Physics.G) / 2.0 * (normal.X * p2.X + normal.Y * p2.Y)
            //gradConcentration[0] = 1;
            //gradConcentration[1] = 0;
            double displacement = ( + Physics.G * normal.X * gradConcentration[0] + Physics.G * normal.Y * gradConcentration[1] )*deltaT;
            if ((double.IsNaN(displacement)) || (Math.Abs(displacement) > 1))
            {
                //speed = 0.5;
            }
            displacement = Math.Abs(displacement);
            //speed = 0.1;
            //if((double.IsNaN(normal.X))||(double.IsNaN(normal.Y)))
            //{
            //    normal = new Point(0.0,0.0);
            //}
            Point newPoint = new Point(p2.X + displacement * normal.X, p2.Y + displacement * normal.Y);

            double distance = Math.Sqrt(Math.Pow(newPoint.X - p2.X, 2) + Math.Pow(newPoint.Y - p2.Y, 2));
            if(distance>epsTimeDisplacement)
            {
                isSatisfiedDisplacementCondition = false;
            }

            return newPoint;
        }

        protected Point pointTransformation(Point[] pointsMas, Point startingPoint)
        {
            double doubleTriangleArea = findDoubleTriangleArea(pointsMas);
            double ksi = ((pointsMas[2].Y - pointsMas[0].Y) * (startingPoint.X - pointsMas[0].X) - (pointsMas[2].X - pointsMas[0].X) * (startingPoint.Y - pointsMas[0].Y)) / doubleTriangleArea;
            double eta = (-(pointsMas[1].Y - pointsMas[0].Y) * (startingPoint.X - pointsMas[0].X) + (pointsMas[1].X - pointsMas[0].X) * (startingPoint.Y - pointsMas[0].Y)) / doubleTriangleArea;
            Point transformedPoint = new Point(ksi,eta);
            return transformedPoint;
        }
        protected double findDoubleTriangleArea(Point [] pointsMas)
        {
            double doubleTriangleArea = 0.0;
            int n = pointsMas.Length;
            for (int j = 0; j < n - 1; j++)
            {
                doubleTriangleArea += pointsMas[j].X * pointsMas[j + 1].Y -
                     pointsMas[j + 1].X * pointsMas[j].Y;
            }
            doubleTriangleArea += pointsMas[n - 1].X * pointsMas[0].Y -
                pointsMas[0].X * pointsMas[n - 1].Y;            
            return doubleTriangleArea;
        }
        public List<List<List<Boundary>>> BoundaryArray
        {
            get
            {
                return boundaryArray;
            }
            set
            {
                boundaryArray = value;
            }
        }
    }
}
