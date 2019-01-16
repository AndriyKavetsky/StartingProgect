using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Drawing;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StartingProgect
{
    public class Show
    {       
        private double xMin;
        private double xMax;
        private double yMin;
        private double yMax;
        private int xMinSet;    //новий діапазон
        private int xMaxSet;
        private int yMinSet;
        private int yMaxSet;
        private int xStep;
        private int yStep;
        private double hWidth;
        private double hHeigth;
        private double n;
        private double m;
        private int x0 = 20;
        private int y0 = 20;

        private Canvas canvas;
        private Brush selectedColor;
        private Brush pointColor;
        
        public Show(int[] mas, double width, double heigth,Canvas canvas)// Graphics g, PictureBox picture,
        {
            this.xMin = mas[0];
            this.xMax = mas[1];
            this.yMin = mas[2];
            this.yMax = mas[3];
            this.canvas = canvas;

            this.n = heigth;
            this.m = width;

            SetXValues();
            SetYValues();

            hWidth = (double)(m - 40) / Math.Abs(xMaxSet - xMinSet);
            hHeigth = (double)(n - 40) / Math.Abs(yMaxSet - yMinSet);
            
            selectedColor = Brushes.Blue;
            pointColor = Brushes.Green;
        }

        public void SetNewValues(double [] mas)
        {
            if(this.xMin > mas[0])
            this.xMin = mas[0];
            
            if(this.xMax < mas[1])
            this.xMax = mas[1];
            
            if(this.yMin > mas[2])
            this.yMin = mas[2];
            
            if (this.yMax < mas[3])
            this.yMax = mas[3];
            
            SetXValues();
            SetYValues();
            
            hWidth = (double)(m - 40) / Math.Abs(xMaxSet - xMinSet);
            hHeigth = (double)(n - 40) / Math.Abs(yMaxSet - yMinSet);
        }

        public void SetValues(double[] mas)//setting coordinates without checking prevoius area
        {            
                this.xMin = mas[0];            
                this.xMax = mas[1];           
                this.yMin = mas[2];
                this.yMax = mas[3];

            SetXValues();
            SetYValues();

            hWidth = (double)(m - 40) / Math.Abs(xMaxSet - xMinSet);
            hHeigth = (double)(n - 40) / Math.Abs(yMaxSet - yMinSet);
        }
        public bool isNeedToRedraw(double[] mas)
        {
            if ((mas[0] < xMinSet) || (mas[1] > xMaxSet) ||
                (mas[2] < yMinSet) || (mas[3] > yMaxSet))
                return true;
            return false;
        }
        public void SetXValues()
        {
            double xLength = Math.Abs(xMax - xMin);

            if (xLength <= 10)
            {
                xStep = 1;
            }
            else if ((xLength > 10) && (xLength <= 20))
            {
                xStep = 5;
            }
            else if ((xLength > 20) && (xLength <= 100))
            {
                xStep = 10;
            }
            else if ((xLength > 100) && (xLength <= 200))
            {
                xStep = 10;
            }
            else if ((xLength > 200) && (xLength <= 500))
            {
                xStep = 50;
            }
            else if ((xLength > 500) && (xLength <= 1000))
            {
                xStep = 100;
            }
            if (xMin < 0)
            {
                xMinSet = ((int)((double)xMin / xStep - 1)) * xStep;
                xMaxSet = (int)Math.Round(((double)xMax / xStep)) * xStep;
            }
            else
            {
                xMinSet = (int)Math.Round(((double)xMin / xStep)) * xStep;
                xMaxSet = (int)Math.Round(((double)xMax / xStep)) * xStep;
            }
        }

        public void SetYValues()
        { 
            double yLength = Math.Abs(yMax - yMin);

            if (yLength <= 10)
            {
                yStep = 1;
            }
            else if ((yLength > 10) && (yLength <= 20))
            {
                yStep = 5;
            }
            else if ((yLength > 20) && (yLength <= 100))
            {
                yStep = 10;
            }
            else if ((yLength > 100) && (yLength <= 200))
            {
                yStep = 10;
            }
            else if ((yLength > 200) && (yLength <= 500))
            {
                yStep = 50;
            }
            else if ((yLength > 500) && (yLength <= 1000))
            {
                yStep = 100;
            }
            if (yMin < 0)
            {
                yMinSet = ((int)((double)yMin / yStep - 1)) * yStep;
                yMaxSet = (int)Math.Round(((double)yMax / yStep)) * yStep;
            }
            else
            {
                yMinSet = (int)Math.Round(((double)yMin / yStep)) * yStep;
                yMaxSet = (int)Math.Round(((double)yMax / yStep)) * yStep;
            }
        }        

        public void ShowFigurePoint( Point p, RichTextBox textBox) // Вивід точки отриманої із заданої фігури
        {
            textBox.AppendText(" p.x " + Math.Round(p.X,5) +
                " p.y" +Math.Round( p.Y,5) + "\n");
            int xp = x0 + (int)Math.Round((p.X - xMinSet) * hWidth);
            int yp = y0 + (int)Math.Round((p.Y - yMinSet) * hHeigth);

            Ellipse ellipse = new Ellipse();
            ellipse.Width = 6;
            ellipse.Height = 6;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = pointColor;//Brushes.Green;//selectedColor;
            ellipse.Fill = pointColor;//Brushes.Green;// selectedColor;
            ellipse.Margin = new Thickness(xp - 2, yp - 2, 0, 0);

            canvas.Children.Add(ellipse);
        }

        public void ShowFigurePoint(Point p) // Вивід точки отриманої із заданої фігури
        {
            int xp = x0 + (int)Math.Round((p.X - xMinSet) * hWidth);
            int yp = y0 + (int)Math.Round((p.Y - yMinSet) * hHeigth);

            Ellipse ellipse = new Ellipse();
            ellipse.Width = 6;
            ellipse.Height = 6;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = pointColor;//Brushes.Green;//selectedColor;
            ellipse.Fill = pointColor;//Brushes.Green;// selectedColor;
            ellipse.Margin = new Thickness(xp - 2, yp - 2, 0, 0);

            canvas.Children.Add(ellipse);
        }

        public void ShowFigurePoint(Point p, Brush ChoosenColor) // Вивід точки отриманої із заданої фігури
        {            
            int xp = x0 + (int)Math.Round((p.X - xMinSet) * hWidth);
            int yp = y0 + (int)Math.Round((p.Y - yMinSet) * hHeigth);

            Ellipse ellipse = new Ellipse();
            ellipse.Width = 6;
            ellipse.Height = 6;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = ChoosenColor;//Brushes.Green;//selectedColor;
            ellipse.Fill = ChoosenColor;//Brushes.Green;// selectedColor;
            ellipse.Margin = new Thickness(xp - 2, yp - 2, 0, 0);

            canvas.Children.Add(ellipse);
        }

        //Побудова лінії із заданих двох точок
        public void ShowLine(Point p1, Point p2,RichTextBox textBox)
        {
            int xp1 = x0 + (int)Math.Round((p1.X - xMinSet) * hWidth);
            int yp1 = y0 + (int)Math.Round((p1.Y - yMinSet) * hHeigth);
            int xp2 = x0 + (int)Math.Round((p2.X - xMinSet) * hWidth);
            int yp2 = y0 + (int)Math.Round((p2.Y - yMinSet) * hHeigth);

            textBox.AppendText(" xp " + xp1 +
               " yp" + yp1 + "\n");
            Line line = new Line();
            line.X1 = xp1;
            line.Y1 = yp1;
            line.X2 = xp2;
            line.Y2 = yp2;
            line.StrokeThickness = 2;
            line.Stroke =selectedColor;

            canvas.Children.Add(line);
        }

        //Побудова лінії із заданих двох точок без виводу даних
        public void ShowLine(Point p1, Point p2)
        {
            int xp1 = x0 + (int)Math.Round((p1.X - xMinSet) * hWidth);
            int yp1 = y0 + (int)Math.Round((p1.Y - yMinSet) * hHeigth);
            int xp2 = x0 + (int)Math.Round((p2.X - xMinSet) * hWidth);
            int yp2 = y0 + (int)Math.Round((p2.Y - yMinSet) * hHeigth);

            Line line = new Line();
            line.X1 = xp1;
            line.Y1 = yp1;
            line.X2 = xp2;
            line.Y2 = yp2;
            line.StrokeThickness = 2;
            line.Stroke = selectedColor;

            canvas.Children.Add(line);
        }

        public Point getRealValueOfPoint(Point gp)//повертає дйісне значення точки 
        {
            double x = (gp.X - x0) / hWidth + xMinSet;
            double y = (gp.Y - y0) / hHeigth + yMinSet;
            return new Point(Math.Round(x,5), Math.Round(y,5));
        }

        public void ShowPoint(Point gp, RichTextBox text)   // Вивід точки заданої по кліку на pictureBox
        {
            double x = (gp.X - x0) / hWidth + xMinSet;
            double y = (gp.Y - y0) / hHeigth + yMinSet;
            text.AppendText(" ClickedX=" + Math.Round(x, 2) + "\n");
            text.AppendText(" ClickedY=" + Math.Round(y, 2) + "\n");

            Ellipse ellipse = new Ellipse();
            ellipse.Width = 6;
            ellipse.Height = 6;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = selectedColor;
            ellipse.Fill = selectedColor;
            ellipse.Margin = new Thickness(gp.X - 2, gp.Y - 2, 0, 0);

            canvas.Children.Add(ellipse);  
        }
        //Showing obtained results to the textbox
        public void ShowFigure(List<Point> points,RichTextBox text,RichTextBox text2)//Вивід фігури заданої списком точок
        {
            if (points.Count < 1)
            {
                return;
            }
            if (points.Count == 1)
            {
                ShowFigurePoint(points[0], text);
            }
            for (int i = 0; i < points.Count-1; i++)
            {
                ShowLine(points[i],points[i+1],text2);
                ShowFigurePoint(points[i], text);
            }
            ShowLine(points[points.Count-1], points[0],text2);
            ShowFigurePoint(points[points.Count-1], text);

        }

        public void ShowFigure(List<Point> points)//Вивід фігури заданої списком точок
        {
            if (points.Count < 1)
            {
                return;
            }
            if (points.Count == 1)
            {
                ShowFigurePoint(points[0]);
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                ShowLine(points[i], points[i + 1]);
                ShowFigurePoint(points[i]);
            }
            ShowLine(points[points.Count - 1], points[0]);
            ShowFigurePoint(points[points.Count - 1]);

        }

        //Побудова координатних осей
        public void ShowCoordinates1()
        {
            Brush p = Brushes.Black;
            ///Осі
            Line(20, 20, 20, n, p,2);
            Line(20, 20, m, 20, p,2);
            ///Риски на шкалі           

            double x = 20;
            for (int i = xMinSet; i <= xMaxSet; i += xStep) // горизонтальні риски
            {
                x = (i - xMinSet) * hWidth + 20;

                Line((int)Math.Round(x), 15, (int)Math.Round(x), 25, p,2);
                Text((int)Math.Round(x) - 8, 0, i.ToString(), Brushes.Black.Color);
               }
            double y = 20;

            for (int i = yMinSet; i <= yMaxSet; i += yStep)  //вертикальні риски
            {
                y = (i - yMinSet) * hHeigth + 20;   
                             
                Line(15, (int)Math.Round(y), 25, (int)Math.Round(y),p,2);
                Text(0, (int)Math.Round(y) - 8, i.ToString(), Brushes.Black.Color);                
            }
        }

        private void Line(double x1, double y1, double x2, double y2, Brush color,double thikness)
        {
            Line division = new Line(); 
            division.X1 = x1;
            division.Y1 = y1;
            division.X2 = x2;
            division.Y2 = y2;
            division.StrokeThickness = thikness;
            division.Stroke = color;

            canvas.Children.Add(division); 
        }

        private void Text(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);

            canvas.Children.Add(textBlock);
        }

        //Побудова сітки
        public void ShowGrid()
        {
            double x = 20;

            for (int i = xMinSet; i <= xMaxSet; i += xStep) //вертикальні лінії
            {
                x = (i - xMinSet) * hWidth + 20;
                Line((int)Math.Round(x), 20, (int)Math.Round(x), n, Brushes.Gray,2);
            }

            double y = 20;

            for (int i = yMinSet; i <= yMaxSet; i += yStep)//горизонтальні лінії
            {
                y = (i - yMinSet) * hHeigth + 20;
                Line(20, (int)Math.Round(y), m, (int)Math.Round(y), Brushes.Gray,2);
            }

            ///Осі
            Line(20, 20, 20, n, Brushes.Black,2);
            Line(20, 20, m, 20, Brushes.Black,2);
        }

        public Brush SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
            }
        }

        public double XMinSet
        {
            get
            {
                return xMinSet;
            }   
        }
        public double YMinSet
        {
            get
            {
                return yMinSet;
            }
        }
        public double XMaxSet
        {
            get
            {
                return xMaxSet;
            }
        }
        public double YMaxSet
        {
            get
            {
                return yMaxSet;
            }
        }
        public Brush PointColor
        {
            set 
            {
                pointColor = value;
            }
            get
            {
                return pointColor;
            }
        }

    }
}
