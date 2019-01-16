using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace StartingProgect
{
    public class D2TriangleMesh1:Mesh// 
    {
        //protected Approximation approximation;//get set
        //protected int domainDimention;
        //protected int pointsCount;//к-сть вузлів загальна
        //protected int elementCount;//к-сть трикутників
        //protected int formaElementa = 3;//трикутні, чотирикутні
        //protected int[][] elements;//матриця елементів
        //private double[][] points;//масив вершин
        public D2TriangleMesh1(Approximation approximation, int pointsCount, int elementCount, int [][] elements, double [][] points):base(approximation,2,pointsCount,elementCount,3,elements)
        {
            this.points = points;
        }
        public D2TriangleMesh1(Approximation approximation, int domainDimention, int formaElementa):base(approximation,2,3)// 2 - domain dimention, 3 - elements shape
        {            
        }
        public void ReadFromFile(string fileName) //reading points from file
        {
            StreamReader reader = new StreamReader(fileName);            
            try
            {
                string[] mas = Regex.Replace(reader.ReadLine(), @"\s+", " ").Split();
                pointsCount = Convert.ToInt32(mas[0]);

                points = new double[pointsCount][];
                for (int i = 0; i < pointsCount; i++)
                {
                    points[i] = new double[domainDimention + 1];
                    mas = Regex.Replace(reader.ReadLine(), @"\s+", " ").Trim(' ').Split();
                    
                    if (domainDimention == 2)
                    {
                        points[i][0] = Convert.ToDouble(mas[1].Replace('.', ','));
                        points[i][1] = Convert.ToDouble(mas[2].Replace('.', ','));
                        if ((mas.Length > 3)&&(mas[3]!=null))
                        {
                            points[i][2] = Convert.ToDouble(mas[3].Replace('.', ','));
                        }
                        else
                        {
                            points[i][2] = 1;
                        }
                    }
                    else if (domainDimention == 3)
                    {
                        points[i][0] = Convert.ToDouble(mas[1].Replace('.', ','));
                        points[i][1] = Convert.ToDouble(mas[2].Replace('.', ','));
                        points[i][2] = Convert.ToDouble(mas[3].Replace('.', ','));
                        if (mas.Length > 4)
                        {
                            points[i][3] = Convert.ToDouble(mas[4].Replace('.', ','));
                        }
                        else
                        {
                            points[i][3] = 1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Incorrect value type " + "\n" + " Please try again "+ e);
            }
            reader.Close();
        }

        public void ReadElementsFromFile(string fileName)// зчитуємо трикутні елементи з файлу
        {
            StreamReader reader = new StreamReader(fileName);           
            try
            {
                string[] mas = Regex.Replace(reader.ReadLine(), @"\s+", " ").Split();
                elementCount = Convert.ToInt32(mas[0]);

                elements = new int[elementCount][];
                for (int i = 0; i < elementCount; i++)
                {
                    elements[i] = new int[formaElementa * approximation.ApproximationDegree];
                    mas = Regex.Replace(reader.ReadLine(), @"\s+", " ").Trim(' ').Split();

                    for (int j = 0; j < formaElementa * approximation.ApproximationDegree; j++)
                    {
                        elements[i][j]= Convert.ToInt32(mas[j+1]);
                    }                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Incorrect value type " + "\n" + " Please try again " + e);
            }
            reader.Close();
        }

        public double[] GetPoint(int number)
        {
            if (number >= points.Length)
                return new double[] { 0 };
            return points[number];
        }

        public int[] GetElement(int number)
        {
            if (number >= elements.Length)
                return new int[] { 0 };
            return elements[number];
        }
    }
}
