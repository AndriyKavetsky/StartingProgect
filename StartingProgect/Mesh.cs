using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace StartingProgect
{
    public abstract class Mesh
    {
        protected Approximation approximation;//get set
        protected int domainDimention;
        protected int pointsCount;//к-сть вузлів загальна
        protected int elementCount;//к-сть трикутників
        protected int formaElementa=3;//трикутні, чотирикутні
        protected int[][] elements;//матриця елементів
        protected double [][] points;//масив вершин  // x // y
        public List<Dictionary<int, SortedSet<int>>> numbersOfBoundaryElements { get; set; }
        public Mesh(Approximation approximation,
           int domainDimention, int pointsCount, int elementCount,
           int formaElementa, int[][] elements) //single  
        {
            this.approximation = approximation;
            this.domainDimention = domainDimention;
            this.pointsCount = pointsCount;
            this.elementCount = elementCount;
            this.formaElementa = formaElementa;
            this.elements = elements;
            numbersOfBoundaryElements = new List<Dictionary<int, SortedSet<int>>>();
        }

        public void CreateBoundaryElements(List<List<List<Boundary>>> boundaryArray)// Пошук граничних елементів для лінійної апроксимації
        {
            numbersOfBoundaryElements = new List<Dictionary<int, SortedSet<int>>>();
            for (int i = 0; i < boundaryArray.Count; i++)
            {
                Dictionary<int, SortedSet<int>> tempDic = new Dictionary<int, SortedSet<int>>();
                for (int j = 0; j < boundaryArray[i].Count; j++)
                {
                    for (int k = 0; k < boundaryArray[i][j].Count; k++)
                    {
                        int pointNumber = boundaryArray[i][j][k].numberP1;
                        if (!tempDic.Keys.Contains(pointNumber))
                        {
                            tempDic.Add(pointNumber, FindElementsForGivenPointNumber(pointNumber));
                        }
                        pointNumber = boundaryArray[i][j][k].numberP2;
                        if(!tempDic.Keys.Contains(pointNumber))
                        {
                            tempDic.Add(pointNumber, FindElementsForGivenPointNumber(pointNumber));
                        }
                    }
                }
                numbersOfBoundaryElements.Add(tempDic);
            }
        }

        private SortedSet<int> FindElementsForGivenPointNumber(int pointNumber)// пошук трикутних елементів для заданого номеру точки
        {
            SortedSet<int> pointNumberElements = new SortedSet<int>();
            for(int i=0;i<elements.Length;i++)
            {
                for(int j=0;j<elements[i].Length;j++)
                {
                    if (elements[i][j] == pointNumber)
                    {
                        pointNumberElements.Add(i);
                        break;
                    }
                }
            }
            return pointNumberElements;
        }

        public Mesh(Approximation approximation, int domainDimention, int formaElementa)
        {
            this.approximation = approximation;
            this.domainDimention = domainDimention;
            this.formaElementa = formaElementa;
            numbersOfBoundaryElements = new List<Dictionary<int, SortedSet<int>>>();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        //public void ShowMesh(
        //public abstract void ReadFromFile(string fileName);

        //public abstract void ReadElementsFromFile(string fileName);

        //public void WriteIntoFile()//???
        //{
             
        //}

        //public abstract double[] GetPoint(int number);
        //public abstract int[] GetElement(int number);
        

        public double[][] Points
        {
            get
            {
                return points;
            }
            set
            {
                PointsCount = value.Length;
                points = value;
            }
        }

        public int[][] Elements
        {
            get
            {
                return elements;
            }
            set
            {
                elementCount = value.Length;
                elements = value;
            }
        }
        public Approximation Approximation//check?
        {
            get 
            {
                return approximation;
            }
            set
            {
                approximation = value;
            }
        }
        public int ElementCount
        {
            get
            {
                return elementCount;
            }
            set
            {
                elementCount = value;
            }
        }
        public int DomainDimention
        {
            get
            {
                return domainDimention;
            }
            set
            {
                domainDimention = value;
            }
        }
        public int FormaElementa
        {
            get 
            {
                return formaElementa;
            }
            set
            {
                formaElementa = value;
            }
        }
        public int PointsCount
        {
            get
            {
                return pointsCount;
            }
            set
            {
                pointsCount = value;
            }
        }

    }
}
