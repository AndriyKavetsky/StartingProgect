using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StartingProgect
{
    /// <summary>
    /// Interaction logic for WindowForDividingBoundaries.xaml
    /// </summary>
    public partial class WindowForDividingBoundaries : Window
    {
        protected Boundaries boundary;
        protected List<SubCondition[]> subConditionList;
        private Show show;

        private bool isFindingBoundaryPoint=false;
        private int selectedBoundaryNumber=0;  // номер вибраної границі
        private List<SubCondition> subCondition;
        private List<Boundary> listOfSubBoundaryConnections;//проміжний масив з'єднань вибраної границі
       
        private BoundaryConnection boundaryCon;//масив номерів точок та їхніх умов
        private Point firstPoint;
        private bool isSettedFirstPoint = false;
        
        private double eps=0.1;

        //new version-modifying
        private Dictionary<int, Point> boundaryPointsAndTheirNumbers; //масив граничних точок та їхніх номерів(як у загальному масиві точок)
        private List<int> subBoundaryPointsNumbers;
        private HashSet<int> boundaryPointsSet;
        private int firstCheckedPointNumber = 0;
        private int firstPointNumber = 0;
        private int secondPointNumber = 0;

        private List<List<Boundary>> copyOfSelectedBoundary;//копія вибраної границі;       
        private List<List<Boundary>> resultSubBoundariesOfSelectedBoundary;// результуючий масив підграниць вибраної границі
        private List<List<Boundary>> copyOfAllBoundaries;// копія всіх границь без підграниць
        
        public WindowForDividingBoundaries()
        {
            InitializeComponent();

            show = new Show(new int[] { 0, 10, 0, 10 }, Canvas2.Width, Canvas2.Height, Canvas2);
            show.ShowCoordinates1();
            show.ShowGrid();    

            //numbersOfPoints = new List<List<int>>();
            subCondition = new List<SubCondition>();
            //selectedBoundaryList = new List<Point>();
            //new
            boundaryPointsAndTheirNumbers = new Dictionary<int, Point>();
            subBoundaryPointsNumbers = new List<int>();
            boundaryPointsSet = new HashSet<int>();

            copyOfSelectedBoundary = new List<List<Boundary>>();
            copyOfAllBoundaries = new List<List<Boundary>>();
            //selectedBoundaryWithoutSubConditions = new List<Boundary>();
            resultSubBoundariesOfSelectedBoundary = new List<List<Boundary>>();
        }

        public WindowForDividingBoundaries(Boundaries boundary,List<SubCondition[]> subConditionList,BoundaryConnection boundaryCon)
        {
            InitializeComponent();

            this.boundary = boundary;
            this.subConditionList = subConditionList;
            this.boundaryCon = boundaryCon;

            //numbersOfPoints = new List<List<int>>();
            listOfSubBoundaryConnections = new List<Boundary>();
            subCondition = new List<SubCondition>();
                        
            //new
            boundaryPointsAndTheirNumbers = new Dictionary<int, Point>();
            subBoundaryPointsNumbers = new List<int>();
            boundaryPointsSet = new HashSet<int>();

            copyOfSelectedBoundary = new List<List<Boundary>>();
            copyOfAllBoundaries = new List<List<Boundary>>();
            //selectedBoundaryWithoutSubConditions = new List<Boundary>();
            resultSubBoundariesOfSelectedBoundary = new List<List<Boundary>>();
            for(int i=0;i<boundaryCon.BoundaryArray.Count;i++)  //створення масиву границь без підграниць 
            {
                copyOfAllBoundaries.Add(createBoundaryListOfPoints(boundaryCon.BoundaryArray[i]));
            }

            show = new Show(new int[] { 0, 10, 0, 10 }, Canvas2.Width, Canvas2.Height, Canvas2);
            show.ShowCoordinates1();
            show.ShowGrid();

            comboBoxBoundary.Items.Clear();
            for (int i = 0; i < boundary.PointsArray.Count; i++)
            {
                comboBoxBoundary.Items.Add(i);
            }
            comboBoxBoundary.SelectedIndex = 0;

            comboBoxCondition.Items.Clear();
            comboBoxCondition.Items.Add(Condition.Dirichlet);
            comboBoxCondition.Items.Add(Condition.Neumann);
            comboBoxCondition.Items.Add(Condition.Robin);
            comboBoxCondition.SelectedIndex = 0;
        }

        private void butCheckBoundary_Click(object sender, RoutedEventArgs e)//вибираємо границю
        {
            if((comboBoxBoundary.SelectedIndex<0)||(boundary.PointsArray==null))
            {
                return;
            }
            firstPoint=new Point();
            isSettedFirstPoint = false;

            richTextBox.Document.Blocks.Clear();
            listOfSubBoundaryConnections = new List<Boundary>();
            subCondition = new List<SubCondition>();
            //numbersOfPoints = new List<List<int>>();
           
            isFindingBoundaryPoint = true;
            selectedBoundaryNumber = comboBoxBoundary.SelectedIndex;
            //selectedBoundaryList = copyList(boundary.PointsArray[selectedBoundaryNumber]);//boundary.PointsArray[selectedBoundaryNumber] ;

            //new
            boundaryPointsAndTheirNumbers = new Dictionary<int, Point>();
            subBoundaryPointsNumbers = new List<int>();//not useful
            boundaryPointsSet = new HashSet<int>();
            
            copyOfSelectedBoundary = boundaryCon.BoundaryArray[selectedBoundaryNumber];
            //selectedBoundaryWithoutSubConditions = new List<Boundary>();
            resultSubBoundariesOfSelectedBoundary = new List<List<Boundary>>();

            firstPointNumber = 0;
            secondPointNumber = 0;
            firstCheckedPointNumber = 0;
            for(int i=0;i<boundaryCon.BoundaryArray[selectedBoundaryNumber].Count;i++)
            {
                for (int j = 0; j < boundaryCon.BoundaryArray[selectedBoundaryNumber][i].Count; j++)
                {
                    boundaryPointsAndTheirNumbers.Add(boundaryCon.BoundaryArray[selectedBoundaryNumber][i][j].numberP1, boundary.PointsArray[selectedBoundaryNumber][j]);//check
                    subBoundaryPointsNumbers.Add(boundaryCon.BoundaryArray[selectedBoundaryNumber][i][j].numberP1);
                }
            }
            //int k = 0;
            //

            MinMaxValue minMax = new MinMaxValue(boundary.PointsArray[comboBoxBoundary.SelectedIndex]);
            Canvas2.Children.Clear(); //drawfigures
            show.SetNewValues(minMax.MinMaxValues());
            show.ShowCoordinates1();
            show.ShowGrid();
            show.ShowFigure(boundary.PointsArray[comboBoxBoundary.SelectedIndex], richTextBox, richTextBox);

            //Print points
            //for(int i=0;i<selectedBoundaryList.Count;i++)
            //{
            //    richTextBox.AppendText(" point "+i+"x= " + selectedBoundaryList[i].X + "  y= " + selectedBoundaryList[i].Y+"\n");
            //}
            richTextBox.AppendText("\n");
            //Print connections
            for(int i=0;i<boundaryCon.BoundaryArray[selectedBoundaryNumber].Count;i++)
            {
                for (int j = 0; j < boundaryCon.BoundaryArray[selectedBoundaryNumber][i].Count; j++)
                {
                    richTextBox.AppendText(" " + boundaryCon.BoundaryArray[selectedBoundaryNumber][i][j].numberP1 + "   " + boundaryCon.BoundaryArray[selectedBoundaryNumber][i][j].numberP2 + "\n");
                }
            }
        }

        public List<Boundary> createBoundaryListOfPoints(List<List<Boundary>> subBoundaries)
        {
            List<Boundary> tempBoundary = new List<Boundary>();
            List<Boundary> oneList = new List<Boundary>();
            for(int i=0;i<subBoundaries.Count;i++)
            {
                for(int j=0;j<subBoundaries[i].Count;j++)
                {
                    oneList.Add(subBoundaries[i][j]);
                }
            }
            List<List<Boundary>> temp = new List<List<Boundary>>();
            temp.Add(oneList);
            tempBoundary = BoundaryConnection.Sort(0,temp)[0];
            return tempBoundary;
        }        
        public static List<Point> copyList(List<Point> list)
        {
            List<Point> temp = new List<Point>();
            for(int i=0;i<list.Count;i++)
            {
                temp.Add(list[i]);
            }
            return temp;
        }
        private void butSubmit_Click(object sender, RoutedEventArgs e)
        {
            isFindingBoundaryPoint = false;            
            MessageBoxResult result = MessageBox.Show("SubBoundary was setted successfully");
            //
            if (subCondition.Count > 0)//додавання останньої підграниці
            {
                subCondition.Add(new SubCondition(getCondition(), subCondition[subCondition.Count - 1].EndPoint, firstPoint));
                //numbersOfPoints.Add(new List<int>());
                //numbersOfPoints[numbersOfPoints.Count - 1].Add(numbersOfPoints[numbersOfPoints.Count - 2][1]);
                //numbersOfPoints[numbersOfPoints.Count - 1].Add(numbersOfPoints[0][0]);
                addNewSubBoundaryConnection(secondPointNumber,firstCheckedPointNumber);//adding boundary elements
                boundaryCon.BoundaryArray[selectedBoundaryNumber] = resultSubBoundariesOfSelectedBoundary; //check for making copy
                //boundaryCon.BoundaryArray[selectedBoundaryNumber][0] = listOfSubBoundaryConnections;//check??

            }
            else
            {
                subCondition.Add(new SubCondition(getCondition(), firstPoint, firstPoint));
                addNewSubBoundaryConnection(firstCheckedPointNumber, firstCheckedPointNumber);
                //resultSubBoundariesOfSelectedBoundary.Add(new List<Boundary>());
                //for(int i=0;i<copyOfAllBoundaries[selectedBoundaryNumber].Count;i++)
                //{
                //    resultSubBoundariesOfSelectedBoundary[0].Add(new Boundary(copyOfAllBoundaries[selectedBoundaryNumber][i].NumberP1, copyOfAllBoundaries[0][i].numberP2, getCondition()));
                //}
                boundaryCon.BoundaryArray[selectedBoundaryNumber] = resultSubBoundariesOfSelectedBoundary;
                //boundaryCon.BoundaryArray[selectedBoundaryNumber][0] = listOfSubBoundaryConnections;
                //set new boundary conditions
            }
            //
            SubCondition[] cond = new SubCondition[subCondition.Count];//заміна масиву підграниць новим масивом
            for(int i=0;i<subCondition.Count;i++)
            {
                cond[i] = subCondition[i];
            }
            subConditionList[selectedBoundaryNumber] = cond;
            //
            richTextBox.Document.Blocks.Clear();//вивід нових підграниць
            for(int i=0;i<subCondition.Count;i++)
            {
                richTextBox.AppendText(" first point  x= "+subCondition[i].StartPoint.X+" y = "+subCondition[i].StartPoint.Y+"\n end point x = " +
                    subCondition[i].EndPoint.X+" y = "+subCondition[i].EndPoint.Y+"\n condition "+subCondition[i].GetCondition+"\n \n");
            }
            for(int i=0;i<listOfSubBoundaryConnections.Count;i++)
            {
                richTextBox.AppendText(" conection  n1 = " + listOfSubBoundaryConnections[i].numberP1 + " n2 = " + listOfSubBoundaryConnections[i].numberP2 + "  condition" + listOfSubBoundaryConnections[i].condition + "\n");
            }
        }

        private void Canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(Canvas2);
            double x = e.GetPosition(null).X;
            double y = e.GetPosition(null).Y;

            if ((p.X >= 20) && (p.Y >= 20))
            {                
                if (isFindingBoundaryPoint)//потрібно знайти вибрану точку 
                {
                    bool isFindedPoint;
                    int pointNumber;
                    Point selectedPoint = findSelectedPointAndNumber(show.getRealValueOfPoint(p),selectedBoundaryNumber,out isFindedPoint,out pointNumber);
                    richTextBox.AppendText(" p x= "+p.X+"y= "+p.Y+"\n");
                    richTextBox.AppendText(" rp x= "+show.getRealValueOfPoint(p).X+" y= "+show.getRealValueOfPoint(p).Y+"\n");
                    //
                    if (!isFindedPoint)
                    {
                        return;
                    }
                    if (boundaryPointsSet.Contains(pointNumber))//перевірка чи точка не належить одному із вже вибраних підінтервалів
                    {
                        return;
                    }               
                    MessageBoxResult result = MessageBox.Show("Do you want to add pointx= " + selectedPoint.X + " y= " + selectedPoint.Y , "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)//користувач вибирає чи хоче він додати нову точку й границю відповідно
                    {
                        return;
                    }
                    if ((isSettedFirstPoint) && (subCondition.Count <= 0))
                    {
                        if (firstPoint == selectedPoint)
                            return;
                    }
                    else if ((subCondition.Count > 0) && (subCondition[subCondition.Count - 1].EndPoint == selectedPoint) && (isSettedFirstPoint))
                    {
                        return;
                    }
                    //
                    if (!isSettedFirstPoint)
                    {
                        firstPoint = selectedPoint;
                        //numbersOfPoints.Add(new List<int>());
                        //numbersOfPoints[0].Add(pointNumber);
                        isSettedFirstPoint = true;
                        boundaryPointsSet.Add(pointNumber);
                        firstPointNumber = pointNumber;
                        firstCheckedPointNumber = pointNumber;                     
                    }
                    else
                    {
                        Condition condition = getCondition();
                        boundaryPointsSet.Add(pointNumber);

                        if (subCondition.Count<=0)//додавання нової підграниці
                        {
                            subCondition.Add(new SubCondition(condition, firstPoint, selectedPoint));
                            //numbersOfPoints[0].Add(pointNumber);//not useful
                            secondPointNumber = pointNumber;
                            addNewSubBoundaryConnection(firstPointNumber, secondPointNumber);                            
                        }
                        else
                        {
                            subCondition.Add(new SubCondition(condition,subCondition[subCondition.Count-1].EndPoint, selectedPoint));
                            //numbersOfPoints.Add(new List<int>());
                            //numbersOfPoints[numbersOfPoints.Count - 1].Add(numbersOfPoints[numbersOfPoints.Count - 2][1]);
                            //numbersOfPoints[numbersOfPoints.Count - 1].Add(pointNumber);
                            firstPointNumber = secondPointNumber;
                            secondPointNumber = pointNumber;
                            addNewSubBoundaryConnection(firstPointNumber, secondPointNumber);
                        }                        
                    }
                    //
                    show.ShowPoint(p, richTextBox);
                }                                
            }
        }

        public void addNewSubBoundaryConnection(int number1,int number2)
        {
            resultSubBoundariesOfSelectedBoundary.Add(new List<Boundary>());//new створення нового масиву номерів точок вибраної підграниці
            bool isSubConnectionFilled = false;
            while(!isSubConnectionFilled)
            {
                int position =BoundaryConnection.FindPosition(selectedBoundaryNumber, number1, 1,boundaryCon.BoundaryArray[selectedBoundaryNumber]);
                int position2 = 0;
                for(int i=0;i<boundaryCon.BoundaryArray[selectedBoundaryNumber].Count;i++)
                {
                    position = BoundaryConnection.FindPosition(i, number1, 1, boundaryCon.BoundaryArray[selectedBoundaryNumber]);
                    if(position!=-1)
                    {
                        position2 = i;
                        break;
                    }
                }

                int num1 = boundaryCon.BoundaryArray[selectedBoundaryNumber][position2][position].numberP1 ;//check for other sub conditions
                int num2 = boundaryCon.BoundaryArray[selectedBoundaryNumber][position2][position].numberP2;//not null
                listOfSubBoundaryConnections.Add(new Boundary(num1, num2, subCondition[subCondition.Count-1].GetCondition));
                resultSubBoundariesOfSelectedBoundary[resultSubBoundariesOfSelectedBoundary.Count-1].Add(new Boundary(num1,num2, subCondition[subCondition.Count - 1].GetCondition));
                boundaryPointsSet.Add(num2);//add unique element
                number1 = num2;
                show.ShowFigurePoint(boundaryPointsAndTheirNumbers[num2], Brushes.Blue);//drawing points
                if (number1 == number2)
                {
                    isSubConnectionFilled = true;
                }
            }
        }
               
        public Condition getCondition()
        {
            Condition condition = new Condition();
            if (comboBoxCondition.SelectedIndex == 0)//вибираємо умову
            {
                condition = Condition.Dirichlet;
            }
            else if (comboBoxCondition.SelectedIndex == 1)
            {
                condition = Condition.Neumann;
            }
            else if (comboBoxCondition.SelectedIndex == 2)
            {
                condition = Condition.Robin;
            }
            return condition;
        }
        public Point findSelectedPointAndNumber(Point p,int boundaryNumber,out bool isFindedPoint, out int number)
        {            
            foreach (var item in boundaryPointsAndTheirNumbers)
            {
                if ((Math.Abs(item.Value.X - p.X) < eps)&&(Math.Abs(item.Value.Y-p.Y)<eps))
                {
                    isFindedPoint = true;
                    number = item.Key;
                    return new Point(item.Value.X,item.Value.Y);
                }
            }            
            
            isFindedPoint = false;
            number = -1;
            return new Point();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (this.Owner as MainWindow).BoundaryMas = boundaryCon;
            (this.Owner as MainWindow).SubConditionList = subConditionList;
        }
    }
}
