using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;

namespace StartingProgect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public enum State
    {
        fillingBoundaries, triangulating, settingPhysicalParametrs, settingTimeParametrs, solvingTheProblem
    }
    public struct FilledValues
    {
        D2TriangleMesh1 d2trMesh1;
        Boundaries boundaries;
        BoundaryConnection boundaryMas;

        public FilledValues(D2TriangleMesh1 d2mesh,Boundaries bound, BoundaryConnection boundMas)
        {
            d2trMesh1 = (D2TriangleMesh1)d2mesh.Clone();
            boundaries = bound; // Add Clone
            boundaryMas = (BoundaryConnection)boundMas.Clone();
        }
        public D2TriangleMesh1 getMesh()
        {
            return d2trMesh1;
        }

        public BoundaryConnection getBoundCon()
        {
            return boundaryMas;
        }

        public Boundaries getBoundaries()
        {
            return boundaries;
        }
    }
    public partial class MainWindow : Window
    {

        public static State state = State.fillingBoundaries;
        private double n;
        private double m;

        private double width;
        private double heigth;

        private double x0;
        private double y0;

        private double radius;
        private int buttonNumber = 0;

        private Show show;
        private Circle circle;
        private Rectangle rectangle;

        private D2TriangleMesh1 d2TriangleMesh1;
        private TriangularApproximation2D1P d2Approximation;
        private Boundaries boundary;
        private List<SubCondition[]> subConditionList;
        //private bool isFindingBoundaryPoint=false; // для перевірки чи будемо ми додавати під область заданої області 
        //private int numberOfBoundary;
        //private List<List<Point>> pointsArray; // загальний масив, який містить масиви точок заданих фігур
                
        private DataTable dataInfoTable;
        private List<Point> polZapList;       

        private BoundaryConnection boundaryMas; 
        public MainWindow()
        {            
            InitializeComponent();
            boundary = new Boundaries();
            show = new Show(new int[] { 0, 10, 0, 10 }, Canvas1.Width, Canvas1.Height, Canvas1);
            show.ShowCoordinates1();
            show.ShowGrid();

            dataInfoTable = new DataTable();
            polZapList = new List<Point>();
            datagrid1.ItemsSource = dataInfoTable.DefaultView;
            subConditionList = new List<SubCondition[]>();

            boundaryMas = new BoundaryConnection();

            d2Approximation = new TriangularApproximation2D1P();  // quadratic approximation
            d2TriangleMesh1 = new D2TriangleMesh1(d2Approximation, 2, 3);//2 - dimention, 3 - formaElementa

            comboBoxCondition.Items.Clear();
            comboBoxCondition.Items.Add(Condition.Dirichlet);
            comboBoxCondition.Items.Add(Condition.Neumann);
            comboBoxCondition.Items.Add(Condition.Robin);
            comboBoxCondition.SelectedIndex = 0;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            labX0.Visibility = Visibility.Visible;
            textbox1.Visibility = Visibility.Visible;
            labY0.Visibility = Visibility.Visible;
            textbox2.Visibility = Visibility.Visible;
            labWidth.Content = "width";
            labWidth.Visibility = Visibility.Visible;
            textbox3.Visibility = Visibility.Visible;
            labN.Visibility = Visibility.Visible;
            textbox4.Visibility = Visibility.Visible;
            labHeigth.Visibility = Visibility.Visible;
            textbox5.Visibility = Visibility.Visible;
            labM.Visibility = Visibility.Visible;
            textbox6.Visibility = Visibility.Visible;

            buttonNumber = 2; 
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            datagrid1.Visibility = Visibility.Visible;
            datagrid1.IsReadOnly = false;
            if (!double.TryParse(textbox3.Text, out n))
            {
                n = 10;
            }
            datagrid1.ColumnWidth = datagrid1.Width / n;
            datagrid1.RowHeight = 40;//datagrid1.Height / 2;
                        
            //dataInfoTable.Rows.Add(new object[]{1,2,"78"});

            //datagrid1.Items.Add("1");//new object());
            //datagrid1.Items.Add("2");//new object());                       
                     
            for (int i = 0; i < n; i++)
            {
                DataGridTextColumn textcol = new DataGridTextColumn();
                
                textcol.Header = i.ToString();
                datagrid1.Columns.Add(textcol);                
            }
                        
            DataRow row = dataInfoTable.NewRow();            
            textbox1.Text = row.Table.Columns.Count.ToString();
            //row[1] = 1;
            //row[2] = "yes";
            dataInfoTable.Rows.Add(row);
                       
        }

        private void datagrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //int i=datagrid1.CurrentCell
            //datagrid1.CurrentCell=
            //datagrid1.CurrentCellChanged;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {            
            labX0.Visibility = Visibility.Visible;
            textbox1.Visibility = Visibility.Visible;
            labY0.Visibility = Visibility.Visible;
            textbox2.Visibility = Visibility.Visible;
            labWidth.Content = "radius";
            labWidth.Visibility = Visibility.Visible;
            textbox3.Visibility = Visibility.Visible;
            labN.Visibility = Visibility.Visible;
            textbox4.Visibility = Visibility.Visible;
                        
            buttonNumber = 1;            
        }

        private void Canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p=Mouse.GetPosition(Canvas1);
            double x = e.GetPosition(null).X; 
            double y = e.GetPosition(null).Y; 
           
            if ((p.X >= 20) && (p.Y >= 20))
            {
                //if (isFindingBoundaryPoint)
                //{
                //what to do when we are finding boundary point
                //}
                show.ShowPoint(p, richTextBox);
            }
        }

        private void butDraw_Click(object sender, RoutedEventArgs e)
        {
            if (buttonNumber == 1)
            {
                if (!double.TryParse(textbox1.Text, out x0))
                {
                    x0 = 0;
                }
                if (!double.TryParse(textbox2.Text, out y0))
                {
                    y0 = 0;
                }
                if (!double.TryParse(textbox3.Text, out n))
                {
                    n = 10;
                }
                if (!double.TryParse(textbox4.Text, out radius))
                {
                    radius = 4;
                }
                circle = new Circle(new Point(x0, y0), radius, n);
                boundary.AddNewBoundary(circle.Points, 1);
                subConditionList.Add(new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[boundary.PointsArray.Count - 1][0], boundary.PointsArray[boundary.PointsArray.Count - 1][0]) });          
                DrawNewFigure();                         
            }
            if (buttonNumber == 2)
            {
                if (!double.TryParse(textbox4.Text, out width))
                {
                    width = 10;
                }
                if (!double.TryParse(textbox6.Text, out heigth))
                {
                    heigth = 20;
                }
                if (!double.TryParse(textbox3.Text, out n))
                {
                    n = 10;
                }
                if (!double.TryParse(textbox5.Text, out m))
                {
                    m = 10;
                }
                if (!double.TryParse(textbox1.Text, out x0))
                {
                    x0 = 0;
                }
                if (!double.TryParse(textbox2.Text, out y0))
                {
                    y0 = 0;
                }
                
                rectangle = new Rectangle(new Point(x0, y0), width, n, heigth, m);
                boundary.AddNewBoundary(rectangle.Points, 1);//1-closed boundary 2-opened boundary
                subConditionList.Add(new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[boundary.PointsArray.Count - 1][0], boundary.PointsArray[boundary.PointsArray.Count - 1][0])  });

                DrawNewFigure();                
            }             
            HideElements();
        }

        private void butFill_Click(object sender, RoutedEventArgs e)
        {
            TriangulationFile triangFile = new TriangulationFile(boundary);
        }

        private void HideElements() //панель для зчитування даних стає невидимою після зчитування інформації
        {
            labX0.Visibility = Visibility.Hidden;
            textbox1.Visibility = Visibility.Hidden;            
            labY0.Visibility = Visibility.Hidden;
            textbox2.Visibility = Visibility.Hidden;
            labWidth.Visibility = Visibility.Hidden;
            textbox3.Visibility = Visibility.Hidden;
            labN.Visibility = Visibility.Hidden;
            textbox4.Visibility = Visibility.Hidden;
            labHeigth.Visibility = Visibility.Hidden;
            textbox5.Visibility = Visibility.Hidden;
            labM.Visibility = Visibility.Hidden;
            textbox6.Visibility = Visibility.Hidden;
            textbox1.Text = "";
            textbox2.Text = "";
            textbox3.Text = "";
            textbox4.Text = "";
            textbox5.Text = "";
            textbox6.Text = "";
        }

        private void DrawNewFigure()
        {
            MinMaxValue minMax = new MinMaxValue(boundary.PointsArray[boundary.PointsArray.Count-1]);
            if (show.isNeedToRedraw(minMax.MinMaxValues()))
            {                
                Canvas1.Children.Clear(); //drawfigures
                show.SetNewValues(minMax.MinMaxValues());
                show.ShowCoordinates1();
                show.ShowGrid();
                for (int i = 0; i < boundary.PointsArray.Count; i++)
                {
                    if (boundary.Connections[i].Count <= 0)
                    {
                        show.ShowFigure(boundary.PointsArray[i], richTextBox, richTextBox2);
                    }
                    else
                    {
                        MessageBox.Show("boundary number " + i);
                        for (int j = 0; j < boundary.PointsArray[i].Count; j++)
                        {
                            show.ShowFigurePoint(boundary.PointsArray[i][j], richTextBox);
                        }
                        for (int j=0;j<boundary.Connections[i].Count;j++)
                        {                            //draw with connections
                            show.ShowLine(boundary.PointsArray[i][boundary.Connections[i][j][0]], boundary.PointsArray[i][boundary.Connections[i][j][1]], richTextBox);
                        }
                    }
                }
            }
            else
            {
                if (boundary.Connections[boundary.PointsArray.Count - 1].Count <= 0)
                {
                    show.ShowFigure(boundary.PointsArray[boundary.PointsArray.Count - 1], richTextBox, richTextBox2);
                }
                else
                {
                    for (int j = 0; j < boundary.PointsArray[boundary.PointsArray.Count - 1].Count; j++)
                    {
                        show.ShowFigurePoint(boundary.PointsArray[boundary.PointsArray.Count - 1][j], richTextBox);
                    }
                    for (int j = 0; j < boundary.Connections[boundary.PointsArray.Count - 1].Count; j++)
                    {                           
                        show.ShowLine(boundary.PointsArray[boundary.PointsArray.Count - 1][boundary.Connections[boundary.PointsArray.Count - 1][j][0]], boundary.PointsArray[boundary.PointsArray.Count - 1][boundary.Connections[boundary.PointsArray.Count - 1][j][1]], richTextBox);
                    }
                }
                //show.ShowFigure(boundary.PointsArray[boundary.PointsArray.Count - 1], richTextBox, richTextBox2);
            } 
        }

        private void butRead_Click(object sender, RoutedEventArgs e)
        {
            var filledRes = ReadPointsandthierConnections.readPointsAndElementsFromFile(subConditionList);
            d2TriangleMesh1 = (D2TriangleMesh1)filledRes.getMesh().Clone();
            boundary = filledRes.getBoundaries();
            boundaryMas = filledRes.getBoundCon();
        }        
        private void butWrite_Click(object sender, RoutedEventArgs e)
        {
            OwerWritePoints owerWriting;
            richTextBox3.Document.Blocks.Clear();
            owerWriting = new OwerWritePoints(2, 2, richTextBox3);
            //if (openFileDialog.ShowDialog() == true)
            //owerWriting.FillPointsArray(openFileDialog.FileName);
        }

        private void butboundaries_Click(object sender, RoutedEventArgs e)//triangulation
        {
            //string location= System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString()+ "\\"+"Triangle"+"\\"+"Mesh Explorer.exe";
            string location = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + "Triangle.NET"+ "\\"+"TestApp"+"\\"+"bin"+"\\"+ "Debug" + "\\" + "Mesh Explorer.exe";
        
            location = location.Replace("\\", "\\\\");
            //var p = new Process();
            //p.StartInfo.FileName = location;  
            //p.Start();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = location;
            string locationConcPolyFile = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + "ConcentrationPoly.poly";
            //startInfo.Arguments =  locationConcPolyFile;
            Process.Start(startInfo);
        }      
        private void Button_Click_2(object sender, RoutedEventArgs e)//besier curves
        {
            int pointsCount = 4;
            
            if ((boundary != null)&&(boundary.PointsArray.Count>0))
            {
                for (int j = 0; j < boundary.PointsArray[0].Count; j+=pointsCount-1)
                {
                    if (boundary.PointsArray[0].Count < (j + pointsCount))
                    {
                        break;
                    }
                    List<Point> points = new List<Point>();
                    for (int i = 0; i < pointsCount; i++)
                    {
                        points.Add(new Point(boundary.PointsArray[0][i+j].X, boundary.PointsArray[0][i+j].Y));

                    }
                    BesierCurves bes = new BesierCurves(points);
                    //Canvas1.Children.Clear();
                    show.PointColor = Brushes.Red;
                    for (double i = 0; i <= 1; i += 0.1)
                    {
                        Point p = bes.Besier(points[0], points[1], points[2], points[3], i);
                        show.ShowFigurePoint(p, richTextBox2);
                    }
                }
            }
        }

        private void butFunc_Click(object sender, RoutedEventArgs e)
        {
            List<Point> figure = new List<Point>();
            for (double i = 0; i < 2.0 * Math.PI; i += 0.1)
            {
                //if(combWorkingMode.SelectedIndex==0)//change
                //{
                    figure.Add(BoundaryFunctions.function(i));
                //}
                //else
                //{
                    //figure.Add(BoundaryFunctions.function3(i));
                //}
            }
            boundary.AddNewBoundary(figure, 1);
            subConditionList.Add(new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[boundary.PointsArray.Count - 1][0], boundary.PointsArray[boundary.PointsArray.Count - 1][0]) });
            DrawNewFigure();

            //List<Point> figure1 = new List<Point>();
            //for (double i = 0; i < 2.0 * Math.PI; i += 0.1)
            //{
            //    if (function2(i, 3).X > 1.5)
            //    {
            //        figure1.Add(function2(i, 3));
            //    }
            //}
            //boundary.AddNewBoundary(figure1, 1);
            //DrawNewFigure();
        }       

        private void butDer_Click(object sender, RoutedEventArgs e)
        {
            if (boundary != null)
            {
                show.PointColor = Brushes.Violet;
                show.ShowFigurePoint(boundary.PointsArray[0][0],richTextBox2);

                BesierCurves besier = new BesierCurves();
                Point p0 = new Point(boundary.PointsArray[0][0].X, boundary.PointsArray[0][0].Y);
                double y1 = besier.BesierDerrivative(boundary.PointsArray[0][0], boundary.PointsArray[0][1], boundary.PointsArray[0][2], boundary.PointsArray[0][3], 0).Y * ( - 1.0 ) + p0.Y;
                double y2 = besier.BesierDerrivative(boundary.PointsArray[0][0], boundary.PointsArray[0][1], boundary.PointsArray[0][2], boundary.PointsArray[0][3], 0).Y * ( 1.0 ) + p0.Y;
                Point p1 = new Point(p0.X - 1, y1);//besier.BesierDerrivative(boundary.PointsArray[0][0],boundary.PointsArray[0][1],boundary.PointsArray[0][2],boundary.PointsArray[0][3],0).Y);
                Point p2 = new Point(p0.X + 1, y2);//besier.BesierDerrivative(boundary.PointsArray[0][0], boundary.PointsArray[0][1], boundary.PointsArray[0][2], boundary.PointsArray[0][3], 0).Y);

                show.ShowFigurePoint(p1,richTextBox2);
                show.ShowFigurePoint(p2,richTextBox2);
                show.ShowLine(p1, p2, richTextBox2);
            }
        }

        private void butPolZap_Click(object sender, RoutedEventArgs e)
        {
            Window1 w1 = new Window1();
            w1.Owner = this;
            w1.Show();
        }

        public List<Point> PolZapList
        {
            get 
            {
                return polZapList;
            }
            set
            {
                polZapList = value;
                boundary.AddNewBoundary(value, 1);//1-closed boundary 2-opened boundary
                subConditionList.Add(new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[boundary.PointsArray.Count-1][0], boundary.PointsArray[boundary.PointsArray.Count-1][0]) });

                DrawNewFigure();                
            }
        }

        private void butReadValues_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.ele;*.poly)|*.ele;*.poly;*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = "C:\\Users\\Andriy\\Desktop\\Курсова робота\\Тріангуляція\\Triangle.NET\\Data";

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    ReadPointsandthierConnections readPoints = new ReadPointsandthierConnections(openFileDialog.FileName, 2);
                    boundary.AddNewBoundary(readPoints.Points2D, readPoints.ConnectionList, 1);//need to be changed
                    DrawNewFigure();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(" Please try to read points again "+"\n"+ex.Message);
            }
            ///twoDimmentionalMesh.ReadFromFile(openFileDialog.FileName);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            subConditionList = new List<SubCondition[]>();
            butCheckBoundary.IsEnabled = false;
            boundary = new Boundaries();
            richTextBox.Document.Blocks.Clear();
            richTextBox2.Document.Blocks.Clear();
            Canvas1.Children.Clear(); //drawfigures
            show.SetValues(new double[]{0,10,0,10});
            show.ShowCoordinates1();
            show.ShowGrid();
        }                
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedIndex == 0 )
            {
                tBoxEndTime.Visibility = Visibility.Hidden;
                tBoxStartTime.Visibility = Visibility.Hidden;
                labEndTime.Visibility = Visibility.Hidden;
                labStartTime.Visibility = Visibility.Hidden;
                tBoxStep.Visibility = Visibility.Hidden;
                labStep.Visibility = Visibility.Hidden;
            }
            if(comboBox.SelectedIndex == 1 )
            {
                tBoxEndTime.Visibility = Visibility.Visible;
                tBoxStartTime.Visibility = Visibility.Visible;
                labEndTime.Visibility = Visibility.Visible;
                labStartTime.Visibility = Visibility.Visible;
                tBoxStep.Visibility = Visibility.Visible;
                labStep.Visibility = Visibility.Visible;
            }
        }

        private void butModelParams_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double a11, a22, d;

                if ((!double.TryParse(tBoxa11.Text, out a11)) || (!double.TryParse(tBoxa22.Text, out a22)) || (!double.TryParse(tBoxd.Text, out d))) 
                {
                    MessageBox.Show("Please input correct parametrs ");
                    return;
                }
                Physics.a11 = a11;
                Physics.a22 = a22;
                Physics.d = d;
                Physics.beta = new List<List<double>>(); 
                Physics.Tc = new List<List<double>>();
                Physics.sigma = new List<List<double>>();
                for (int i = 0; i < boundaryMas.BoundaryArray.Count; i++)//initialization of boundaries
                {
                    Physics.beta.Add(new List<double>());
                    Physics.sigma.Add(new List<double>());
                    Physics.Tc.Add(new List<double>());
                }
                string [] betas = tBoxbeta.Text.Split(' ');
                string[] sigmas = tBoxSigma.Text.Split(' ');
                string[] Tcs = tBoxTc.Text.Split(' ');
                for(int i=0;i<boundaryMas.BoundaryArray[0].Count;i++)//first boundary
                {                  
                    Physics.beta[0].Add(double.Parse(betas[i]));
                    Physics.sigma[0].Add(double.Parse(sigmas[i]));
                    Physics.Tc[0].Add(double.Parse(Tcs[i]));
                }
                if(subConditionList.Count>1)
                {
                    betas = tBoxbeta2.Text.Split(' ');
                    sigmas = tBoxSigma2.Text.Split(' ');
                    Tcs = tBoxTc2.Text.Split(' ');
                    for (int i = 0; i < boundaryMas.BoundaryArray[1].Count; i++)//second boundary
                    {
                        Physics.beta[1].Add(double.Parse(betas[i]));
                        Physics.sigma[1].Add(double.Parse(sigmas[i]));
                        Physics.Tc[1].Add(double.Parse(Tcs[i]));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please input correct parametrs "+"\n"+ex.Message);
            }
        }

        private void butDivision_Click(object sender, RoutedEventArgs e)  //if someone want to change condition for the hole boundary
        {
            comboBoxBoundary.Items.Clear();
           
            for (int i=0;i<boundary.PointsArray.Count; i++)
            {
                comboBoxBoundary.Items.Add(i);
            }
            comboBoxBoundary.SelectedIndex = 0;                       
            
            butCheckBoundary.IsEnabled = true;
        }

        private void butCheckBoundary_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxBoundary.Items.Count <= 0)
            {
                return;
            }
            subConditionList[comboBoxBoundary.SelectedIndex] = new SubCondition[] { new SubCondition(getSelectedCondition(), boundary.PointsArray[comboBoxBoundary.SelectedIndex][0], boundary.PointsArray[comboBoxBoundary.SelectedIndex][0]) };
          
            butCheckBoundary.IsEnabled = false;

            //Canvas1.Children.Clear(); //drawfigures
            ////show.SetNewValues(minMax.MinMaxValues());
            //show.ShowCoordinates1();
            //show.ShowGrid();
          
            //show.ShowFigure(boundary.PointsArray[comboBoxBoundary.SelectedIndex], richTextBox, richTextBox2);
            //numberOfBoundary = comboBoxBoundary.SelectedIndex;
            //isFindingBoundaryPoint = true;
        }

        public Condition getSelectedCondition()
        {
            if (comboBoxCondition.SelectedIndex == 0)
            {
                return Condition.Dirichlet;
            }
            if (comboBoxCondition.SelectedIndex == 1)
            {
                return Condition.Neumann;
            }
            if (comboBoxCondition.SelectedIndex == 2)
            {
                return Condition.Robin;
            }
            return Condition.Dirichlet;
        }

        private void butSolve_Click(object sender, RoutedEventArgs e)
        {
            ///------checking integration--------///
            //double area = Integration.areaIntegration((x, y) => x + 2 * y, 3);
            //richTBoxSolver.AppendText("Area = " + area);
            //// f= x + 3*y  measures   3 ---- 4  and 0 ------ 1+x   
            //double area2 = TestIntegration.testCase1();
            //richTBoxSolver.AppendText("Area2 = " + area2);
            //Natan, McDonald - wpf           
            ////////---normal---///////
            //
            //show = new Show(new int[] { 0, 10, 0, 10 }, cDrawBoundary.Width, cDrawBoundary.Height, cDrawBoundary, richTextBox);
            //show.ShowCoordinates1();
            //show.ShowGrid();
            //MinMaxValue minMax1 = new MinMaxValue(boundary.PointsArray[boundary.PointsArray.Count - 1]);
            //cDrawBoundary.Children.Clear(); //drawfigures
            //show.SetNewValues(minMax1.MinMaxValues());
            //show.ShowCoordinates1();
            //show.ShowGrid();
            //List<Point> normals = new List<Point>();
            //for (int i=0;i<boundary.PointsArray[0].Count;i++)
            //{
            //    D2TriangleLinearApproximationNormal normm = new D2TriangleLinearApproximationNormal(new Point(0, 0), new Point(2, 2), new Point(4, 0), true);
            //    Point ress = normm.NormalCalculating();
            //}
            //for (int i = 0; i < boundary.PointsArray.Count; i++)
            //{
            //    show.ShowFigure(boundary.PointsArray[i], richTextBox, richTextBox2);
            //}
            ////
            //show.SelectedColor = Brushes.Aqua;
            //for (int i = 0; i < boundary1.PointsArray.Count; i++)
            //{
            //    show.ShowFigure(boundary1.PointsArray[i], richTextBox, richTextBox2);
            //}

            //D2TriangleLinearApproximationNormal norm = new D2TriangleLinearApproximationNormal(new Point(0, 0), new Point(2, 2), new Point(4, 0), true);
            //Point res2 = norm.FindNormal();
            //richTBoxSolver.AppendText("n1 = " + res2.X + " n2 = " + res2.Y + "\n");

            //D2TriangleLinearApproximationNormal norm1 = new D2TriangleLinearApproximationNormal(new Point(2, 2), new Point(4, 0), new Point(2, -2), true);
            //res2 = norm1.FindNormal();
            //richTBoxSolver.AppendText("n1 = " + res2.X + " n2 = " + res2.Y + "\n");

            //norm1 = new D2TriangleLinearApproximationNormal(new Point(-2, 0), new Point(0, 2), new Point(2, 2), true);
            //res2 = norm1.FindNormal();
            //richTBoxSolver.AppendText("n1 = " + res2.X + " n2 = " + res2.Y + "\n");

            //norm1 = new D2TriangleLinearApproximationNormal(new Point(0, 4), new Point(2, 2), new Point(2, 0), true);
            //res2 = norm1.FindNormal();
            //richTBoxSolver.AppendText("n1 = " + res2.X + " n2 = " + res2.Y + "\n");

            //norm1 = new D2TriangleLinearApproximationNormal(new Point(2, 4), new Point(2, 2), new Point(0, 0), true);
            //res2 = norm1.FindNormal();
            //richTBoxSolver.AppendText("n1 = " + res2.X + " n2 = " + res2.Y + "\n");

            

            ///////-------------------------//////////////
            // try
            {
                richTBoxSolver.Document.Blocks.Clear();
                if ((d2TriangleMesh1 == null) || (boundaryMas == null) || (d2TriangleMesh1.PointsCount == 0))
                    return;

                //    OwerWriting2D1To2D2 owrWrt = new OwerWriting2D1To2D2(d2TriangleMesh1.Elements, d2TriangleMesh1.Points, boundaryMas);
                //    d2TriangleMesh1.Points = owrWrt.points;
                //    d2TriangleMesh1.Elements = owrWrt.elements;
                //    boundaryMas = owrWrt.boundaries;
                //    domain = new Domain(d2TriangleMesh1, boundaryMas);
                //    KrankNicholsonSolver2D2 ks = new KrankNicholsonSolver2D2(domain);
                //    ks.KrankNichlSolver();

                if (checkBox.IsChecked == true)//linear approximation
                {
                    Domain tempDomain = new Domain(d2TriangleMesh1, boundaryMas);

                    FEMSolver2D1P ap = new FEMSolver2D1P((Domain)tempDomain.Clone());
                    InnerPolygonBoundaryProblemApproximation solver = ap;
                    InnerPolygonBoundaryProblemOutput res = solver.solve();//solver.solve();           

                    for (int i = 0; i < res.result.Length; i++)
                    {
                        richTBoxSolver.AppendText("res[" + i + "] = " + res.result[i] + "\n");
                    }
                }
                if (checkBox1.IsChecked == true)//square approximation
                {
                    OwerWriting2D1To2D2 owrWrt = new OwerWriting2D1To2D2(d2TriangleMesh1.Elements, d2TriangleMesh1.Points, boundaryMas);
                    D2TriangleMesh1 resMesh = new D2TriangleMesh1(d2TriangleMesh1.Approximation, owrWrt.points.Length, owrWrt.elements.Length, owrWrt.elements, owrWrt.points);
                    Domain tempDomain = new Domain(resMesh, owrWrt.boundaries);

                    FEMSolver2D2P ap = new FEMSolver2D2P((Domain)tempDomain.Clone());

                    //ap.solve2();// розв'язування задачі на концентрацію
                    InnerPolygonBoundaryProblemOutput res = ap.solve2();


                    //ap.solvePressure();
                    //InnerPolygonBoundaryProblemApproximation solver = ap;
                    //InnerPolygonBoundaryProblemOutput res = solver.solve();           

                    for (int i = 0; i < res.result.Length; i++)
                    {
                        richTBoxSolver.AppendText("res[" + i + "] = " + res.result[i] + "\n");
                    }
                }
            }
            //catch (Exception ex)
            {
                //MessageBox.Show(" You made something wrong " + ex.Message);
            }
        }

        private void butSetSubBoundary_Click(object sender, RoutedEventArgs e)
        {
            WindowForDividingBoundaries w2 = new WindowForDividingBoundaries(boundary,subConditionList,boundaryMas);
            w2.Owner = this;
            w2.Show();
        }

        public BoundaryConnection BoundaryMas
        {
            get { return boundaryMas; }
            set { boundaryMas = value; }
        }
        public List<SubCondition[]> SubConditionList
        {
            get { return subConditionList; }
            set { subConditionList = value; }
        }

        private void butNextMesh_Click(object sender, RoutedEventArgs e)
        {
            if(subConditionList.Count<=0)
            {
                return;
            }
            labFirtBoundary.Content = " First boundary contains " + subConditionList[0].Length + " subBoundaries";
            if (subConditionList.Count == 2)
            {
                labSecondBoundary.Content = " Second boundary contains " + subConditionList[1].Length + " subBoundaries";
            }
        }

        private void butTimeNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butNiklson_Click(object sender, RoutedEventArgs e)
        {
            richTBoxSolver.Document.Blocks.Clear();
            if ((d2TriangleMesh1 == null) || (boundaryMas == null) || (d2TriangleMesh1.PointsCount == 0))
                return;

            OwerWriting2D1To2D2 owrWrt = new OwerWriting2D1To2D2(d2TriangleMesh1.Elements, d2TriangleMesh1.Points, boundaryMas);
            D2TriangleMesh1 resMesh = new D2TriangleMesh1(d2TriangleMesh1.Approximation, owrWrt.points.Length, owrWrt.elements.Length, owrWrt.elements, owrWrt.points);
            Domain  tempDomain = new Domain(resMesh, owrWrt.boundaries);
            
            KrankNicholsonSolver2D2 ks = new KrankNicholsonSolver2D2((Domain)tempDomain.Clone());
            ks.KrankNichlSolver();
        }

        private void butPressure_Click(object sender, RoutedEventArgs e)
        {
            //Domain d1Domain = new Domain((Mesh)d2TriangleMesh1.Clone(), (BoundaryConnection)boundaryMas.Clone());
            //FEMSolver2D1P fem1D = new FEMSolver2D1P(d1Domain);

            //fem1D.solve();

            OwerWriting2D1To2D2 owrWrt = new OwerWriting2D1To2D2(d2TriangleMesh1.Elements, d2TriangleMesh1.Points, boundaryMas);
            D2TriangleMesh1 resMesh = new D2TriangleMesh1(d2TriangleMesh1.Approximation, owrWrt.points.Length, owrWrt.elements.Length, owrWrt.elements, owrWrt.points);
            Domain tempDomain = new Domain(resMesh, owrWrt.boundaries);

            FEMSolver2D2P ap = new FEMSolver2D2P((Domain)tempDomain.Clone());
            //double[] concentrationResult = ap.solve2().result;// розв'язування задачі на концентрацію

            // InnerPolygonBoundaryProblemApproximation solver = ap;
            InnerPolygonBoundaryProblemOutput res = ap.solvePressure();

            for (int i = 0; i < res.result.Length; i++)
            {
                richTBoxSolver.AppendText("res[" + i + "] = " + res.result[i] + "\n");
            }

        }

        private void butMoveBoundary_Click(object sender, RoutedEventArgs e)
        {
            MoveBoundary.Move(cDrawBoundary);            
        }

        public void BuildMesh(string fileName)
        {
            string location = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + "Triangle.NET" + "\\" + "TestApp" + "\\" + "bin" + "\\" + "Debug" + "\\" + "Mesh Explorer.exe";

            location = location.Replace("\\", "\\\\");           
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = location;
            string locationConcPolyFile = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + "\\" + fileName;
            startInfo.Arguments = locationConcPolyFile;
            var p = new Process();
            p.StartInfo = startInfo;
            p.Start();
            while(!p.HasExited)
            {

            }
            //Process.Start(startInfo);
            //var p = new Process();
            //p.StartInfo.FileName = location;  
            //p.Start();
        }

        public void FormTrianglePolyFiles(Boundaries boundaryConc, Boundaries boundaryPress, int pressureBoundNumInConcMas, string concFileName, string pressFileName)
        {
            //Setting new values to concentration boundary array
            //boundaryConc.Connections[pressureBoundNumInConcMas] = boundaryPress.Connections[pressureBoundNumInConcMas];
            boundaryConc.PointsArray[pressureBoundNumInConcMas] = boundaryPress.PointsArray[pressureBoundNumInConcMas];
            ///boundaryConc.IsTriangulatedBoundary[pressureBoundNumInConcMas] = boundaryPress.IsTriangulatedBoundary[pressureBoundNumInConcMas];

            TriangulationFile fillConc = new TriangulationFile(boundaryConc, concFileName);
            TriangulationFile fillPressure = new TriangulationFile(boundaryPress, pressFileName);
        }

        public double[] FillConcentrationValues(Domain domainPr, Domain domainConc, double [] concSolution)
        {
            int length = domainPr.Meshes.PointsCount;
            double[] concRes = new double[length];
            for (int i=0;i<length;i++)
            {
                Point prPoint = new Point(domainPr.Meshes.Points[i][0], domainPr.Meshes.Points[i][1]);
                concRes[i] = concSolution[FindConcentrationPointNumber(domainConc, prPoint)];
            }
            return concRes;
        }

        public int FindConcentrationPointNumber(Domain domainConc, Point prPoint)
        {
            double eps = Math.Pow(1, -10);
            for (int i=0;i<domainConc.Meshes.PointsCount;i++)
            {
                Point concPoint = new Point(domainConc.Meshes.Points[i][0], domainConc.Meshes.Points[i][1]);
                if((Math.Abs(concPoint.X-prPoint.X)<eps)&&(Math.Abs(concPoint.Y-prPoint.Y)<eps))
                        return i; 
            }
            return 0;
        }

        private List<List<List<double>>> fillBoundaryValues(double [] values, Domain myDomain, BoundaryConnection myBoundaryCoonection)
        {
            List<List<List<double>>> result = new List<List<List<double>>>();
            for(int i=0;i<myBoundaryCoonection.BoundaryArray.Count;i++)
            {
                result.Add(new List<List<double>>());
                for(int j=0;j<myBoundaryCoonection.BoundaryArray[i].Count;j++)
                {
                    result[i].Add(new List<double>());
                    for(int k=0;k<myBoundaryCoonection.BoundaryArray[i][j].Count;k++)
                    {
                        result[i][j].Add(values[myBoundaryCoonection.BoundaryArray[i][j][k].numberP1]);
                    }
                }
            }
            return result;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = false;
            checkBox.IsChecked = true;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = true;
            checkBox.IsChecked = false;
        }

        private void btnSubDomainTriangles(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
