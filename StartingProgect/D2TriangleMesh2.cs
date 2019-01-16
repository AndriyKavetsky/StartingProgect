using System;
using System.Collections.Generic;
using System.Windows;

namespace StartingProgect
{
    public class D2TriangleMesh2:Mesh
    {
        private double eps = Math.Pow(10, -10);
        protected List<Point> tempPoints;//масив вершин для додавання ноивих точок 

        public D2TriangleMesh2(Approximation approximation, int pointsCount, int elementCount, int[][] elements, double[][] points):base(approximation,2,pointsCount,elementCount,3,elements)
        {
            this.points = points;
            tempPoints = new List<Point>();
            for (int i = 0; i < points.Length; i++)
            {
                tempPoints.Add(new Point(points[i][0], points[i][1]));
            }
            this.elements = convertElementsForSquareApproximation();            
            this.points = new double[tempPoints.Count][];
            for (int i = 0; i < tempPoints.Count; i++)
            {
                this.points[i] = new double[2];
                this.points[i][0] = tempPoints[i].X;
                this.points[i][1] = tempPoints[i].Y;
            }
            this.pointsCount = tempPoints.Count;
        }
        public int[][] convertElementsForSquareApproximation()//перезапис трикутних елементів для використання квадратичних апроксимацій
        {
            int n = 6;
            int[][] tempElements = new int[elements.Length][];

            for (int i = 0; i < elements.Length; i++)
            {
                tempElements[i] = new int[n];
                for (int j = 0; j < n; j++)
                {
                    tempElements[i][j] = 0;
                }
            }
            for (int i = 0; i < elements.Length; i++)
            {
                for (int j = 0; j < elements[i].Length; j++)
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
                    tempElements[i][k] = pointNumber;
                    k++;
                }
                newX = (points[elements[i][0]][0] + points[elements[i][2]][0]) / 2.0;
                newY = (points[elements[i][0]][1] + points[elements[i][2]][1]) / 2.0;
                newPoint = new Point(newX, newY);// координати середини відрізка між двома точками
                pointNumber = findPointNumber(newPoint);
                tempElements[i][k] = pointNumber;
            }
            return tempElements;
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
