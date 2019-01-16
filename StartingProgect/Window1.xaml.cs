using StartingProgect.PolZap;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class Window1 : Window
    {
        public static int param { get; set; }

        private double a;
        private double b;
        private double count; //amount of points
        private double h;

        private Formula formula1;
        private Formula formula2;

        //private int n = 0;
        private List<Point> points;
        //Identuficator el;               

        public Window1()
        {            
            InitializeComponent();
            points = new List<Point>();
        }

        private void butInput_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(textbox3.Text, out a))
            {
                a = 0;
            }
            if (!double.TryParse(textbox4.Text, out b))
            {
                b = 1.0;
            }
            if (!double.TryParse(textbox5.Text, out count))
            {
                count = 100;
            }            

            formula1 = new Formula(ReadText(textBox1));
            formula2 = new Formula(ReadText(textbox2));
            if ((formula1.ZadanaFormula.Length <= 0)||(formula2.ZadanaFormula.Length<=0))
            {
                MessageBox.Show("Please input formula");
                return;
            }
            
            try
            {

            formula1.PolskuyZapus();
            formula2.PolskuyZapus();

            if ((formula1.SetId.Count > 1) || (formula2.SetId.Count > 1))
            {  //if amount of unknown values is bigger than 1
                MessageBox.Show("Formula should contain only one unknown value");
                return;
            }

                h=(b-a)/count;
                //richtextbox1.AppendText("a= " + a + "  b= " + b +"  count= "+count+"  h= "+h+ "\n");
                double step = a;
                for (int i = 0; i <= count; i++)
                {
                    if(formula1.SetId.Count>0)
                    formula1.SetId.ElementAt(0).ZNACHENNYA = step;
                    if(formula2.SetId.Count>0)
                    formula2.SetId.ElementAt(0).ZNACHENNYA = step;
                    //calculation

                    double x=formula1.obchuslennya();
                    double y = formula2.obchuslennya();
                    //richtextbox1.AppendText(" i= "+i+ "  x= " + x + "  y= " + y + "\n");
                    points.Add(new Point(x, y));
                    step += h;
                }
                (this.Owner as MainWindow).PolZapList = points;
                this.Close();
            }
            catch(ArgumentException ex)
            {
                MessageBox.Show("There is an error in one of formulas"+"\n", ex.Message);
                return;
                throw new Exception(ex.Message, ex); 
            }

        }

        public string ReadText(TextBox text)
        {
            string s = text.Text;
            
            while (s.IndexOf(' ') > -1)
            {
                int k = s.IndexOf(' ');
                if (k < 0)
                    break;
                s = s.Remove(k, 1);
            }
            return s; 
        }
    }
}
