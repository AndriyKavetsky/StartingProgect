using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace StartingProgect
{
    class TwoDimentionalMeshOfTriangularElements1:Mesh
    {       
        //create
        //readonly
        //фіналізовані елементи 
        //private readonly int domainDimention;
        ////RichTextBox richTextBox;
       
        public TwoDimentionalMeshOfTriangularElements1(TriangularApproximation2D1P D2Approximation, int pointsCount, int elementCount,
            int[][] Elements):base(D2Approximation,2,pointsCount,elementCount,3,Elements)
        {
            this.domainDimention = 2;
            this.formaElementa = 3;

            //this.richTextBox = text; 
        }
        public int[] GetElement(int number)
        {
            int n=formaElementa * approximation.ApproximationDegree;
            int[] vector = new int[n];
            for (int i = 0; i < n; i++)
            {
                vector[i] = elements[number][i];
            }
            return vector;//is clone??? 
        }

        public double[] GetPoint(int number)
        {
            if (number >= points.Length)
                return new double[] { 0 };
            return points[number];
        }

        public void ReadElementsFromFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            try
            {
                string[] mas = reader.ReadLine().Split();
                elementCount = Convert.ToInt32(mas[0]);

                elements = new int[elementCount][];
                for (int i = 0; i < elementCount; i++)
                {
                    elements[i] = new int[formaElementa * approximation.ApproximationDegree];
                    mas = reader.ReadLine().Split();

                    for (int j = 0; j < formaElementa * approximation.ApproximationDegree; j++)
                    {
                        elements[i][j] = Convert.ToInt32(mas[j + 1]);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Incorrect value type " + "\n" + " Please try again " + e);
            }
        }

        public void ReadFromFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string[] mas = reader.ReadLine().Split();
            try
            {
                this.ElementCount = Convert.ToInt32(mas[0]);
                while (!reader.EndOfStream)
                {
                    //richTextBox.AppendText( reader.ReadLine()+"\n"); 
                }
            }
            catch(Exception e)
            {
                throw new FormatException("Incorrect value type "+"\n"+"Must be integer", e);
            }
        }
    }
}
