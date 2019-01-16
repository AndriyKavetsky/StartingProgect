using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using Microsoft.Win32;

namespace StartingProgect
{
    public class OwerWritePoints //перезаписується масив точок для квадратичної апроксимації
    {
        private int approximation;
        private int dimention;
        private RichTextBox text;
        private List<double[]> points;
        private List<int[]> connections;
        private List<int[]> resConnections;
        private List<int []> triangles;
        private List<int[]> resTriangles;
        private OpenFileDialog openFileDialog;

        public OwerWritePoints(int approximation, int dimention, RichTextBox text)
        {
           // resPoints = new List<double[]>();
            resTriangles = new List<int[]>();
            //nodes = new List<string>();
            
            this.approximation = approximation;
            this.dimention = dimention;
            this.text = text;
            
            points = new List<double[]>();
            triangles = new List<int[]>();
            connections = new List<int[]>();
            
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.ele;*.poly)|*.ele;*.poly;*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = "C:\\Users\\Andriy\\Desktop\\Курсова робота\\Тріангуляція\\Triangle.NET\\Data";
            if (openFileDialog.ShowDialog() == true)
                FillPointsArrayAndTheirConnections(openFileDialog.FileName);
            if (openFileDialog.ShowDialog() == true)
                FillTriangles(openFileDialog.FileName);
            SquareApproximation();
        }

        public OwerWritePoints(int approximation, int dimention, RichTextBox text,string pointsFileName, string elementsFileName)
        {
            // resPoints = new List<double[]>();
            resTriangles = new List<int[]>();
            //nodes = new List<string>();

            this.approximation = approximation;
            this.dimention = dimention;
            this.text = text;

            points = new List<double[]>();
            triangles = new List<int[]>();
            connections = new List<int[]>();

            FillPointsArrayAndTheirConnections(pointsFileName);
            FillTriangles(elementsFileName);
            SquareApproximation();
        }

        public void FillPointsArrayAndTheirConnections(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);           

            try
            {
                string[] mas = reader.ReadLine().Split();

                int pointsCount = Convert.ToInt32(mas[0]);
                text.AppendText("Points count = " + mas[0] + "\n" + "\n");

                for (int i=0;i<pointsCount;i++)
                {
                    mas = reader.ReadLine().Split();
                    double [] point = new double[dimention+1];
                    for (int j = 0; j < dimention; j++)
                    {
                        point[j] = Convert.ToDouble(mas[j + 1].Replace('.',','));
                        text.AppendText("point"+ i+"  =" + point[j] + "  ");
                    }
                    if (mas.Length > 2)
                    {
                        point[dimention] = Convert.ToInt32(mas[3]);
                        text.AppendText(" dimention =" + point[dimention] + "  ");
                    }
                    else
                    {
                        point[dimention] = 0;
                    }
                    text.AppendText("\n");
                    points.Add(point);
                }

                mas = reader.ReadLine().Split();
                int connectionsCount = Convert.ToInt32(mas[0]);
                for(int i=0;i<connectionsCount;i++)
                {
                    mas = reader.ReadLine().Split();
                    int[] pointNumbers = new int[3];// два номери точок та номер границі що їх з'єднує
                    for (int j = 0; j < 3; j++)
                    {
                        pointNumbers[j] = Convert.ToInt32(mas[j + 1].Replace('.', ','));
                    }
                    connections.Add(pointNumbers);
                }
                //while (!reader.EndOfStream)
                //{
                //    nodes.Add(reader.ReadLine()); 
                //}
            }
            catch(Exception e)
            {
                throw new FormatException("Inncorrect value type must be double " + "\n", e);
            }
            reader.Close();
        }

        public void FillTriangles(string fileName)
        {            
            StreamReader reader = new StreamReader(fileName);
                        
            string[] mas = reader.ReadLine().Split();

            int trianglesCount = Convert.ToInt32(mas[0]);
            text.AppendText("Triangles count = " + mas[0] + "\n" + "\n");

            try
            {
                for (int i = 0; i < trianglesCount; i++)
                {
                    mas = reader.ReadLine().Split();
                    int[] triangle = new int[dimention+1];//triangle - three points, line - two points
                    for (int j = 0; j < triangle.Length; j++)
                    {
                        triangle[j] = Convert.ToInt32(mas[j + 1]);
                        text.AppendText("triangle" + i + "  =" + triangle[j] + "  ");
                    }
                    text.AppendText("\n");
                    triangles.Add(triangle);
                }
            }
            catch(Exception e)
            {
                throw new FormatException("Inncorrect value type must be double " + "\n", e); 
            }
            reader.Close();
        }

        public void SquareApproximation()
        {
            //StreamWriter writer = new StreamWriter("NewTriangles.txt");
            for (int i = 0; i<triangles.Count; i++)
            {
                int[] mas = new int[2 * (dimention + 1)];
                int p = 0;

                for (int j = 0; j < triangles[i].Length; j++)
                {
                    mas[p] = triangles[i][j];
                    p++; 
                }
                
                for (int j = 0; j < triangles[i].Length-1; j++)
                {
                    double[] point = new double[dimention + 1];
                    //text.AppendText("Begin"+"\n");
                    //PrintPoints();
                    //text.AppendText("End" + "\n");

                   //// mas[p] = triangles[i][j];
                   /// p++;

                    for (int k = 0; k < dimention; k++)// new point
                    {
                        point[k] = (points[triangles[i][j]][k] + points[triangles[i][j + 1]][k])/2.0;
                        //text.AppendText("point" + triangles[i][j] + " point" + triangles[i][j + 1]+"\n");
                       // text.AppendText(" p1 " + points[triangles[i][j]][k] + " p2 " + points[triangles[i][j + 1]][k] + "\n");
                    }

                    if (points[triangles[i][j]][dimention] == points[triangles[i][j + 1]][dimention])
                    {
                        point[dimention] = points[triangles[i][j]][dimention];
                    }
                    else
                    {
                        point[dimention] = 0;
                    }

                    //text.AppendText(" pX " + point[0] + " pY " + point[1] +" dimention="+point[2]+ "\n");

                    if (IsNeedToAddPoint(point))//points.Contains(point))//overload contain
                    {
                        points.Add(point);
                        //text.AppendText(" added x " + points[points.Count-1][0] + " added y " + points[points.Count-1][1] + "\n");
                        //PrintPoints();
                        mas[p] = points.Count-1;//
                        p++;
                    }
                    else
                    {
                       // text.AppendText(" index " +FindPointsIndex(point)+ "\n");
                        //text.AppendText(" finded point x" + points[FindPointsIndex(point)][0] + " finded point y" + points[FindPointsIndex(point)][1] + "\n");
                        mas[p] = FindPointsIndex(point);//points.FindIndex(x=>x==point);//
                        p++;
                    }
                    //PrintPoints();
                }

                double[] point1 = new double[dimention + 1];
                ////mas[p] = triangles[i][triangles[i].Length - 1];
                ////p++;
                for (int k = 0; k < dimention; k++)
                {
                    point1[k] = (points[triangles[i][0]][k] + points[triangles[i][triangles[i].Length - 1]][k]) / 2.0;
                   // text.AppendText("point" + triangles[i][0] + " point" + triangles[i][triangles[i].Length - 1] + "\n");
                   // text.AppendText(" x1 " + points[triangles[i][0]][k] + " x2" + points[triangles[i][triangles[i].Length - 1]][k] + "\n");
                }
                if (points[triangles[i][0]][dimention] == points[triangles[i][triangles[i].Length - 1]][dimention])
                {
                    point1[dimention] = points[triangles[i][0]][dimention];
                }
                else
                {
                    point1[dimention] = 0;
                }

                //text.AppendText(" pX " + point1[0] + " pY " + point1[1] + " dimention=" + point1[2] + "\n");

                if (IsNeedToAddPoint(point1))//!points.Contains(point))
                {
                    points.Add(point1);
                    //text.AppendText(" added x " + points[points.Count - 1][0] + " added y " + points[points.Count - 1][1] + "\n");
                    //PrintPoints();
                    mas[p] = points.Count-1;//
                    p++;
                }
                else
                {
                    //text.AppendText(" index " + FindPointsIndex(point) + "\n");
                   // text.AppendText( " finded point x"+points[FindPointsIndex(point)][0]+" finded point y"+points[FindPointsIndex(point)][1]+"\n");
                    mas[p] = FindPointsIndex(point1);//points.FindIndex(x => x == point);//
                    p++; 
                }
                //PrintPoints();
                resTriangles.Add(mas);
                //for(int p=0;

            }

            resConnections = new List<int[]>();
            for(int i=0;i<connections.Count;i++)//перезапис з'єднань
            {
                double[] point = new double[dimention + 1];
            
                for (int k = 0; k < dimention; k++)// new point
                {
                    point[k] = (points[connections[i][0]][k] + points[connections[i][1]][k]) / 2.0;                    
                }
                if (points[connections[i][0]][dimention] == points[connections[i][1]][dimention])
                {
                    point[dimention] = points[connections[i][0]][dimention];
                }
                else
                {
                    point[dimention] = 0;
                }
                if (IsNeedToAddPoint(point))//
                {
                    points.Add(point);
                    resConnections.Add(new int[] { connections[i][0], points.Count - 1, connections[i][2] });
                    resConnections.Add(new int[] { points.Count - 1, connections[i][1], connections[i][2] });
                }
                else
                {
                    int pointNumber = FindPointsIndex(point);
                    resConnections.Add(new int[] { connections[i][0], pointNumber, connections[i][2] });
                    resConnections.Add(new int[] { pointNumber, connections[i][1], connections[i][2] });                    
                }
            }

            //PrintResult();
            WriteIntoFile();
        }

        public void WriteIntoFile()
        {
            StreamWriter writer = new StreamWriter("Result.ele");
            StreamWriter writer2 = new StreamWriter("Result.poly");
            writer.WriteLine((resTriangles.Count) + " " +6 + " " + 0);
            for (int i = 0; i < resTriangles.Count; i++)
            {
                writer.Write(i);                
                for (int j = 0; j < resTriangles[i].Length; j++)
                {
                    writer.Write(" " + resTriangles[i][j]);                    
                }
                writer.WriteLine();
            }
            writer.Close();
            writer2.WriteLine(points.Count +" "+2+ " " + 0 + " " + 1);
            for (int i = 0; i < points.Count; i++)
            {
                writer2.Write(i);                
                for (int j = 0; j < points[i].Length; j++)
                {
                    writer2.Write(" " + points[i][j].ToString().Replace(',','.'));                    
                }
                writer2.WriteLine();
            }
            writer2.WriteLine(resConnections.Count + " " + 1);
            for(int i=0;i<resConnections.Count;i++)
            {
                writer2.Write(i);
                for(int j=0;j<resConnections[i].Length;j++)
                {
                    writer2.Write(" " + resConnections[i][j].ToString().Replace(',', '.'));
                }
                writer2.WriteLine();
            }
            writer2.WriteLine(0);        
            writer2.Close(); 
        }

        public void PrintResult()
        {
            StreamWriter writer = new StreamWriter("Points.ele");
            StreamWriter writer2 = new StreamWriter("Points.poly");
           
            for (int i = 0; i < resTriangles.Count; i++)
            {
                writer.Write(i);
                text.AppendText("  triangle "+i+"   ");
                for (int j = 0; j < resTriangles[i].Length; j++)
                {
                    writer.Write(' ' + resTriangles[i][j]);
                    text.AppendText("  "+resTriangles[i][j]);
                }
                writer.WriteLine();
                //writer.Write("\n");
                text.AppendText("\n");
            }
            writer2.WriteLine(points.Count+' '+0+' '+1);
            for (int i = 0; i < points.Count; i++)
            {
                writer2.Write(i);
                text.AppendText("  point " + i + "   ");
                for (int j = 0; j < points[i].Length; j++)
                {
                    writer2.Write(' ' + points[i][j]);
                    text.AppendText("  " + points[i][j]);
                }
                writer2.WriteLine();
                //writer2.Write("\n");
                text.AppendText("\n");
            }
            writer2.WriteLine(resConnections.Count + " " + 1);
            for (int i = 0; i < resConnections.Count; i++)
            {
                writer2.Write(i);
                for (int j = 0; j < resConnections[i].Length; j++)
                {
                    writer2.Write(" " + resConnections[i][j].ToString().Replace(',', '.'));
                }
                writer2.WriteLine();
            }
            writer2.WriteLine(0);
            writer.Close();
            writer2.Close();
        }

        public void PrintPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points[i].Length; j++)
                {
                    text.AppendText("  points[" + i + " , " + j + "]=" + points[i][j]);
                }
                text.AppendText("\n");
            }
        }

        public int FindPointsIndex(double[] point)
        {            
            for (int i = 0; i < points.Count; i++)
            {
                int k = 0;
                for (int j = 0; j < dimention; j++)
                {
                    if (Math.Abs(points[i][j] - point[j]) < Math.Pow(10, -10))
                    {
                        k++;
                    }
                }
                if (k == dimention)
                {
                    return i;
                }
            }
            return 0;
        }
        //are we adding new point?
        public bool IsNeedToAddPoint(double [] point)
        {
            //text.AppendText("++++++ points count = "+points.Count+"\n");
            for (int i = 0; i < points.Count; i++)
            {
                int k=0;
                for (int j = 0; j < dimention; j++)
                {
                    //text.AppendText(" cheking" + i + " on points " + points[i][j] + " point" + point[j] + "\n");
                    if (Math.Abs(points[i][j] - point[j]) < Math.Pow(10, -10))
                    {
                        k++;
                    }
                }
                if (k == 2)// dimention)
                {
                    //text.AppendText("---------"+"\n");
                    return false;
                }
            }
            return true;
        }
    }
}
