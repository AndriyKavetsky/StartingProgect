using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;
using Microsoft.Win32;
//using System.Windows.Forms;

namespace StartingProgect
{
    class ReadPointsandthierConnections
    {//reading points and their connection (only read points and connect them one by one)
        // pointNumber point.X point.Y point.Z
        private List<Point> points2D;
        private List<Point3D> points3D;
        private List<int[]> connectionList;

        private double[][] points;

        private string fileName;
        private int dimention;
        private bool isGoodNumeration = true;//якщо нумерація точок починається з 1 то  все ок, інакше з'єдання потрібно брати на 1 менше

        public ReadPointsandthierConnections(string fileName, int dimention)
        {
            this.dimention = dimention;
            this.fileName = fileName;
            points2D = new List<Point>();
            points3D = new List<Point3D>();
            connectionList = new List<int[]>();
            fillPointsArray();
        }

        public void fillPointsArray()
        {
            StreamReader reader = new StreamReader(fileName);

            int count = Convert.ToInt32(Regex.Replace(reader.ReadLine(), @"\s+", " ").Split()[0]);
            
            points = new double[count][];

            isGoodNumeration = true;

            for (int i = 0; i < count; i++)
            {                
                string[] mas = Regex.Replace(reader.ReadLine(), @"\s+", " ").Trim(' ').Split();
                if (i == 0)//перевіряємо нумерацію точок
                {
                    if(Convert.ToInt32(mas[0])==1)
                    {
                        isGoodNumeration = false;
                    }
                }
                if ((dimention == 2))//&&(mas[3]!="0"))
                {
                    Points2D.Add(new Point(Convert.ToDouble(mas[1].Replace('.', ',')), Convert.ToDouble(mas[2].Replace('.', ','))));
                    // масив точок та граничних значень
                    points[i] = new double[dimention + 1];
                    points[i][0] = points2D[i].X;
                    points[i][1] = points2D[i].Y;

                    if ((mas.Length > 3) && (mas[3] != null))
                    {
                        points[i][2] = Convert.ToDouble(mas[3].Replace('.', ','));
                    }
                    else
                    {
                        points[i][2] = 1;
                    }
                    //
                }
                else if ((dimention == 3))//&&(mas[4]!="0"))
                {
                    points3D.Add(new Point3D(Convert.ToDouble(mas[1].Replace('.', ',')), Convert.ToDouble(mas[2].Replace('.', ',')), Convert.ToDouble(mas[3].Replace('.', ','))));
                    // масив точок та граничних значень
                    points[i] = new double[dimention + 1];
                    points[i][0] = points3D[i].X;
                    points[i][1] = points3D[i].Y;
                    points[i][2] = points3D[i].Z;

                    if ((mas.Length > 4) && (mas[4] != null))
                    {
                        points[i][3] = Convert.ToDouble(mas[4].Replace('.', ','));
                    }
                    else
                    {
                        points[i][3] = 1;
                    }
                    //
                }
            }

            int connectionCount = Convert.ToInt32(Regex.Replace(reader.ReadLine(), @"\s+", " ").Trim(' ').Split()[0]);
            for (int i = 0; i < connectionCount; i++)
            {
                string[] mas = Regex.Replace(reader.ReadLine(), @"\s+", " ").Trim(' ').Split(); //System.Text.RegularExpressions.
                if (mas.Length >= 3) //&& (mas[3] != "0"))
                {
                    if(isGoodNumeration)
                        connectionList.Add(new int[2] { Convert.ToInt32(mas[1]), Convert.ToInt32(mas[2]) });
                    else
                        connectionList.Add(new int[2] { Convert.ToInt32(mas[1])-1, Convert.ToInt32(mas[2])-1 });
                }
            }
            reader.Close();
        }

        public static FilledValues readPointsAndElementsFromFile(List<SubCondition[]> subConditionList, string fileDirectory="dir")
        {
            BoundaryConnection boundaryMas = new BoundaryConnection();
            TriangularApproximation2D1P d2Approximation = new TriangularApproximation2D1P();  // quadratic approximation
            D2TriangleMesh1 d2TriangleMesh1 = new D2TriangleMesh1(d2Approximation, 2, 3);//2 - dimention, 3 - formaElementa

            Boundaries boundary = new Boundaries();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.ele;*.poly)|*.ele;*.poly;*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = "C:\\Users\\Andriy\\Desktop\\Курсова робота\\Тріангуляція\\Triangle.NET\\Data";
            try
            {
                if(fileDirectory!="dir")
                {                    
                    boundaryMas.BoundaryArray = new List<List<List<Boundary>>>();
                    ReadPointsandthierConnections read = new ReadPointsandthierConnections(fileDirectory + ".poly", 2);  //отримуємо масив точок та з'єднань
                    d2TriangleMesh1.Points = read.Points;

                    //richTextBox3.Document.Blocks.Clear();
                    //TextDisplayer.DisplayDoubleMas(richTextBox3, d2TriangleMesh1.Points);// вивід зчитаних точок на екран
                    //boundary.AddNewBoundary(circle.Points, 1);
                    //subConditionList.Add(new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[boundary.PointsArray.Count - 1][0], boundary.PointsArray[boundary.PointsArray.Count - 1][0]) });

                    //
                    for (int i = 0; i < subConditionList.Count; i++)//створення масиву граничних з'єднань, к-сть границь відома із попереднього заповнення 
                    {
                        boundaryMas.BoundaryArray.Add(new List<List<Boundary>>());
                        boundaryMas.BoundaryArray[i].Add(new List<Boundary>());
                    }

                    for (int i = 0; i < read.ConnectionList.Count; i++)// заповнення масивів із граничними з'єднаннями
                    {
                        int firstPointBoundary = (int)d2TriangleMesh1.Points[read.ConnectionList[i][0]][2];//2-boundary value
                        int secondPointBoundary = (int)d2TriangleMesh1.Points[read.ConnectionList[i][1]][2];

                        if ((firstPointBoundary == secondPointBoundary) && (firstPointBoundary != 0))//((d2TriangleMesh1.Points[read.ConnectionList[i][0]][2]!=0)&&(d2TriangleMesh1.Points[read.ConnectionList[i][1]][2]!=0))
                        {
                            boundaryMas.BoundaryArray[firstPointBoundary - 1][0].Add(new Boundary(read.ConnectionList[i][0], read.ConnectionList[i][1], subConditionList[firstPointBoundary - 1][0].GetCondition));
                        }
                    }

                    for (int i = 0; i < boundaryMas.BoundaryArray.Count; i++)//сотрування граничних елементів
                    {
                        boundaryMas.SetNewValuesToBoundary(i, BoundaryConnection.Sort(0, boundaryMas.BoundaryArray[i]));
                    }

                    boundary = new Boundaries();//формування оновленого масиву граничних точок
                    for (int i = 0; i < boundaryMas.BoundaryArray.Count; i++)
                    {
                        List<Point> temp = new List<Point>();
                        for (int j = 0; j < boundaryMas.BoundaryArray[i].Count; j++)
                        {
                            for (int k = 0; k < boundaryMas.BoundaryArray[i][j].Count; k++)
                                temp.Add(new Point(d2TriangleMesh1.Points[boundaryMas.BoundaryArray[i][j][k].numberP1][0], d2TriangleMesh1.Points[boundaryMas.BoundaryArray[i][j][k].numberP1][1]));
                        }
                        boundary.AddNewBoundary(temp, (i + 1));
                    }
                    d2TriangleMesh1.ReadElementsFromFile(fileDirectory + ".ele");
                }
                else if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = System.IO.Path.GetDirectoryName(openFileDialog.FileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    boundaryMas.BoundaryArray = new List<List<List<Boundary>>>();
                    ReadPointsandthierConnections read = new ReadPointsandthierConnections(fileName + ".poly", 2);  //отримуємо масив точок та з'єднань
                    d2TriangleMesh1.Points = read.Points;

                    //richTextBox3.Document.Blocks.Clear();
                    //TextDisplayer.DisplayDoubleMas(richTextBox3, d2TriangleMesh1.Points);// вивід зчитаних точок на екран
                    //boundary.AddNewBoundary(circle.Points, 1);
                    //subConditionList.Add(new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[boundary.PointsArray.Count - 1][0], boundary.PointsArray[boundary.PointsArray.Count - 1][0]) });

                    //
                    for (int i = 0; i < subConditionList.Count; i++)//створення масиву граничних з'єднань, к-сть границь відома із попереднього заповнення 
                    {
                        boundaryMas.BoundaryArray.Add(new List<List<Boundary>>());
                        boundaryMas.BoundaryArray[i].Add(new List<Boundary>());
                    }

                    for (int i = 0; i < read.ConnectionList.Count; i++)// заповнення масивів із граничними з'єднаннями
                    {
                        int firstPointBoundary = (int)d2TriangleMesh1.Points[read.ConnectionList[i][0]][2];//2-boundary value
                        int secondPointBoundary = (int)d2TriangleMesh1.Points[read.ConnectionList[i][1]][2];

                        if ((firstPointBoundary == secondPointBoundary) && (firstPointBoundary != 0))//((d2TriangleMesh1.Points[read.ConnectionList[i][0]][2]!=0)&&(d2TriangleMesh1.Points[read.ConnectionList[i][1]][2]!=0))
                        {
                            boundaryMas.BoundaryArray[firstPointBoundary - 1][0].Add(new Boundary(read.ConnectionList[i][0], read.ConnectionList[i][1], subConditionList[firstPointBoundary - 1][0].GetCondition));
                        }
                    }

                    for (int i = 0; i < boundaryMas.BoundaryArray.Count; i++)//сотрування граничних елементів
                    {
                        boundaryMas.SetNewValuesToBoundary(i, BoundaryConnection.Sort(0, boundaryMas.BoundaryArray[i]));
                    }

                    boundary = new Boundaries();//формування оновленого масиву граничних точок
                    for (int i = 0; i < boundaryMas.BoundaryArray.Count; i++)
                    {
                        List<Point> temp = new List<Point>();
                        for (int j = 0; j < boundaryMas.BoundaryArray[i].Count; j++)
                        {
                            for (int k = 0; k < boundaryMas.BoundaryArray[i][j].Count; k++)
                                temp.Add(new Point(d2TriangleMesh1.Points[boundaryMas.BoundaryArray[i][j][k].numberP1][0], d2TriangleMesh1.Points[boundaryMas.BoundaryArray[i][j][k].numberP1][1]));
                        }
                        boundary.AddNewBoundary(temp, (i + 1));
                    }
                    d2TriangleMesh1.ReadElementsFromFile(fileName + ".ele");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show(" File has incorrect format, please try again " + ex.Message);
            }
            catch (IOException ex1)
            {
                MessageBox.Show(" File has some invalid arguments, please try again " + ex1.Message);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(" Please try again " + ex2.Message);
            }
            return new FilledValues(d2TriangleMesh1, boundary, boundaryMas);
        }

        public double [][] Points
        {
            get
            {
                return points;
            }
        }

        public List<Point> Points2D
        {
            get
            {
                return points2D;
            }
        }

        public List<Point3D> Points3D
        {
            get
            {
                return points3D;
            }
        }

        public List<int[]> ConnectionList
        {
            get
            {
                return connectionList;
            }
        }
    }
}
