using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    public class Boundaries///2d(3d 1d) change  // потрібен для побудови границь (використовується в інших класах )
    {
        private List<List<Point>>pointsArray;//масиви граничних точок 
        private List<int> isTriangulatedBoundary;
        private List<List<int[]>> connections;//з'єднання точок задаються при потребі

        //points 
        public Boundaries()
        {
            pointsArray = new List<List<Point>>();
            isTriangulatedBoundary = new List<int>();
            connections = new List<List<int[]>>();
        }
        public Boundaries(List<List<Point>> pointsArray,
                List<int> isTriangulatedBoundary)
        {
            this.pointsArray = pointsArray;
            //this.isTriangulatedBoundary = isTriangulatedBoundary;
        }
        public Boundaries(List<List<Point>> pointsArray,
               List<int> isTriangulatedBoundary, List<List<int[]>> connections)
        {
            this.pointsArray = pointsArray;
            this.isTriangulatedBoundary = isTriangulatedBoundary;
            this.connections = connections;
        }

        //public void Sort()
        //{
        //    for (int i = 0; i < boundaryArray.Count; i++)
        //    {
        //        List<Boundary> temp = new List<Boundary>();
        //        bool isSortedBoundary = false;
        //        int k = 0;
        //        temp.Add(new Boundary(boundaryArray[i][k].NumberP1, boundaryArray[i][k].numberP2, boundaryArray[i][k].condition));

        //        while (!isSortedBoundary)
        //        {
        //            if (boundaryArray[i].Count <= 0)
        //            {
        //                isSortedBoundary = true;
        //                break;
        //            }
        //            // boundaryArray[i][k]

        //            boundaryArray[i].RemoveAt(k);

        //            temp.Add(new Boundary(boundaryArray[i][k].NumberP1, boundaryArray[i][k].numberP2, boundaryArray[i][k].condition));

        //            k = FindPosition(i, boundaryArray[i][k].numberP2);
        //        }
        //    }
        //}
        //public int FindPosition(int boundaryNumber, int pointNumber)//пошук
        //{
        //    for (int i = 0; i < boundaryArray[boundaryNumber].Count; i++)
        //    {
        //        if (boundaryArray[boundaryNumber][i].NumberP1 == pointNumber)
        //            return i;
        //    }

        //    return 0;
        //}
        public void AddNewBoundary(List<Point> points, int boundary)
        {
            if (points == null)
            {
                return;
            }
            pointsArray.Add(points);
            connections.Add(new List<int[]>());//empty when connections are ordered
            if ((boundary == 1) || (boundary == 2) || (boundary == 3))
                isTriangulatedBoundary.Add(boundary);
            else
            {
                isTriangulatedBoundary.Add(1);
            }
        }

        public void AddNewBoundary(List<Point> points, List<int[]> connection, int boundary)
        {
            if (points == null)
            {
                return;
            }
            pointsArray.Add(points);
            connections.Add(connection);
            if ((boundary == 1) || (boundary == 2) || (boundary == 3))
                isTriangulatedBoundary.Add(boundary);
            else
            {
                isTriangulatedBoundary.Add(1);
            }
        }
        public List<List<Point>> PointsArray
        {
            get
            {
                return pointsArray;
            }
        }
        public List<int> IsTriangulatedBoundary
        {
            get
            {
                return isTriangulatedBoundary;
            }
        }

        public List<List<int[]>> Connections
        {
            get
            {
                return connections;
            }
        }
    
}
}
