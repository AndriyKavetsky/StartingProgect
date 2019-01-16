using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace StartingProgect
{
    class TriangulationFile//клас для запису заданого масиву граничних точок для їх подальшої тріангуляції
    {
        private Boundaries boundary;

        public TriangulationFile(Boundaries boundary)
        {
            this.boundary = boundary;
            FillTextFile();
        }

        public TriangulationFile(Boundaries boundary, string fileName)
        {
            this.boundary = boundary;
            FillTextFile(fileName);
        }

        protected void FillTextFile(string triangleFileName = "TriangleFile.poly")
        {
            int pointCount = 0;
            for (int i = 0; i < boundary.PointsArray.Count; i++)
            {
                pointCount += boundary.PointsArray[i].Count;
            }
            
            StreamWriter writer = new StreamWriter(triangleFileName);
            writer.WriteLine(pointCount +" "+ 2+" "+0+" "+1);

            int k = 0;

            for (int i = 0; i < boundary.PointsArray.Count; i++)
            {
                for (int j = 0; j < boundary.PointsArray[i].Count; j++)
                {
                    k++;
                    writer.WriteLine(k + " " + boundary.PointsArray[i][j].X.ToString().Replace(',', '.') + " " +
                           boundary.PointsArray[i][j].Y.ToString().Replace(',', '.')+" "+(i+1));
                }
            }
            writer.WriteLine(pointCount + " " + 1);
            k = 0;
            int count = 0;
            int preview = 1;

            for (int i = 0; i < boundary.PointsArray.Count; i++)
            {
                preview = count;

                while ((boundary.PointsArray[i].Count-1) != (count-preview))
                {
                    count++;
                    k++;
                    writer.WriteLine(k + " " + count + " " + (count + 1)+" "+(i+1).ToString());
                }

                count++;
                k++;
                writer.WriteLine(k + " " + count + " " + (preview+1)+" "+(i+1).ToString());                
            }
            writer.WriteLine(0);
            writer.Close();            
        }
    }
}
