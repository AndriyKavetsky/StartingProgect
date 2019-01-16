using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    public class OwerWriting2D1To2D2
    {
        protected int n = 6;
        protected int n2 = 3;
        public int[][] elements { get; }//матриця елементів
        public double[][] points { get; }//масив вершин  // x // y
        protected List<Point> tempPoints;//масив вершин для додавання ноивих точок 
        public BoundaryConnection boundaries { get; }
        private double eps = Math.Pow(10, -10);
    
        public OwerWriting2D1To2D2(int[][] elements, double[][] points, BoundaryConnection boundaries)
        {
            this.elements = new int[elements.Length][];
            for(int i=0;i<elements.Length;i++)
            {
                this.elements[i] = new int[elements[i].Length];
                for(int j=0;j<elements[i].Length;j++)
                {
                    this.elements[i][j] = elements[i][j];
                }
            }
            this.points = new double[points.Length][];
            for (int i = 0; i < points.Length; i++)
            {
                this.points[i] = new double[points[i].Length];
                for (int j = 0; j < points[i].Length; j++)
                {
                    this.points[i][j] = points[i][j];
                }
            }
            this.boundaries = (BoundaryConnection)boundaries.Clone();//should be deep copy 
            tempPoints = new List<Point>();
            for(int i=0;i<points.Length;i++)
            {
                tempPoints.Add(new Point(points[i][0], points[i][1]));
            }
            this.elements = convertElementsForSquareApproximation();
            this.boundaries.BoundaryArray = convertBoundariesForSquareApproximation();
            this.points = new double[tempPoints.Count][];
            for (int i = 0; i < tempPoints.Count; i++)
            {
                this.points[i] = new double[2];
                this.points[i][0] = tempPoints[i].X;
                this.points[i][1] = tempPoints[i].Y;
            }
                        
        }

        public int[][] convertElementsForSquareApproximation()//перезапис трикутних елементів для використання квадратичних апроксимацій
        {
            int[][] tempElements = new int[elements.Length][];

            for(int i=0;i<elements.Length;i++)
            {
                tempElements[i] = new int[n];
                for(int j=0;j<n; j++)
                {
                    tempElements[i][j] = 0;
                }
            }
            for (int i = 0; i < elements.Length; i++)
            {
                for (int j = 0; j < elements[i].Length;j++)
                {
                    tempElements[i][j] = elements[i][j];
                }
            }
            for (int i = 0; i < elements.Length; i++)
            {
                double newX, newY;
                Point newPoint;
                int pointNumber;
                int k = 3;
                for (int j = 0; j < elements[i].Length - 1; j++)
                {
                    newX = (points[elements[i][j]][0] + points[elements[i][j + 1]][0]) / 2.0;
                    newY = (points[elements[i][j]][1] + points[elements[i][j + 1]][1]) / 2.0;
                    newPoint = new Point(newX, newY);// координати середини відрізка між двома точками
                    pointNumber = findPointNumber(newPoint);
                    tempElements[i][k]=pointNumber;
                    k++;
                }
                newX = (points[elements[i][0]][0] + points[elements[i][2]][0]) / 2.0;
                newY = (points[elements[i][0]][1] + points[elements[i][2]][1]) / 2.0;
                newPoint = new Point(newX, newY);// координати середини відрізка між двома точками
                pointNumber = findPointNumber(newPoint);
                tempElements[i][k]=pointNumber;
            }
            return tempElements;
        }
        public List<List<List<Boundary>>> convertBoundariesForSquareApproximation()//перезапис граничних елементів для використання квадратичних апроксимацій
        {
            List<List<List<Boundary>>> tempboundaryPointsNumber = new List<List<List<Boundary>>>();

            for (int i = 0; i < boundaries.BoundaryArray.Count; i++)
            {
                tempboundaryPointsNumber.Add(new List<List<Boundary>>());
                for (int r = 0; r < boundaries.BoundaryArray[i].Count; r++)
                {
                    tempboundaryPointsNumber[i].Add(new List<Boundary>());                    
                    for (int j = 0; j < boundaries.BoundaryArray[i][r].Count; j++)
                    {                        
                        double newX, newY;
                        Point newPoint;
                        int pointNumber;
                        //tempboundaryPointsNumber[i].Add(boundaryPointsNumber[i][j]);
                        newX = (points[boundaries.BoundaryArray[i][r][j].numberP1][0] + points[boundaries.BoundaryArray[i][r][j].numberP2][0]) / 2.0;
                        newY = (points[boundaries.BoundaryArray[i][r][j].numberP1][1] + points[boundaries.BoundaryArray[i][r][j].numberP2][1]) / 2.0;
                        newPoint = new Point(newX, newY);
                        pointNumber = findPointNumber(newPoint);
                        tempboundaryPointsNumber[i][r].Add(new Boundary(boundaries.BoundaryArray[i][r][j].numberP1, pointNumber, boundaries.BoundaryArray[i][r][j].numberP2, boundaries.BoundaryArray[i][r][j].condition));
                        //tempboundaryPointsNumber[i].Add(pointNumber);
                        //tempboundaryPointsNumber[i].Add(boundaryPointsNumber[i][j + 1]);
                    }
                    //tempboundaryPointsNumber[i].Add(boundaryPointsNumber[i][boundaryPointsNumber[i].Count - 1]);
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
    }
}
