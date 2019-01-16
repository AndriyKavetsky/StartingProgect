using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect.PolZap
{
    class Formula
    {
        private String formula = "3+4*(1-5)";

        private double result = 0;

        private SortedSet<Identuficator> setId;

        private string polzap;

        public Formula(String formula)
        {
            setId = new SortedSet<Identuficator>();
            this.formula = formula;
            polzap = null;
        }

        public Formula()
        {

        }

        public String PolskuyZapus()
        {
            String pz = "";

            Operation op = new Operation();

            Function func = new Function();

            String[] mas_lecksem;

            Identuficator id = new Identuficator();

            int n;

            Stack<string> stackOperation = new Stack<string>();

            Perevid(formula, out mas_lecksem, out n);

            for (int i = 0; i < n; i++)
            {
                double d;

                if (double.TryParse(mas_lecksem[i], out d))
                {
                    pz += mas_lecksem[i] + " ";
                }

                if (id.IsIdentuficator(mas_lecksem[i]))
                {
                    pz += mas_lecksem[i] + " ";
                    Identuficator ident = new Identuficator(mas_lecksem[i], 0);
                    if (!setId.Contains(ident))
                        setId.Add(ident);
                }

                if (op.IsOperation(mas_lecksem[i]))
                {
                    if (stackOperation.Count == 0)
                    {
                        stackOperation.Push(mas_lecksem[i]);
                    }
                    else
                    {
                        while ((stackOperation.Count != 0) && (op.priorutet(mas_lecksem[i]) <= op.priorutet(stackOperation.Peek())))
                            pz += stackOperation.Pop() + " ";
                        stackOperation.Push(mas_lecksem[i]);
                    }
                }


                if (mas_lecksem[i] == "(")
                    stackOperation.Push("(");

                if (mas_lecksem[i] == ")")
                {
                    while (stackOperation.Peek() != "(")
                    {
                        pz += stackOperation.Pop() + " ";
                    }

                    if (stackOperation.Count > 0)
                    {
                        string c = stackOperation.Pop();
                    }

                    if ((stackOperation.Count > 0) && (func.IsFunction(stackOperation.Peek())))
                    {
                        pz += stackOperation.Pop() + " ";
                    }
                }

                if (func.IsFunction(mas_lecksem[i]))
                {
                    stackOperation.Push(mas_lecksem[i]);
                }
            }

            while (stackOperation.Count != 0)
                pz += stackOperation.Pop() + " ";

            polzap = pz;

            return pz;
        }

        public void Perevid(String _formula, out String[] mas_lecksem, out int n)
        {
            Operation op = new Operation();
            Function func = new Function();

            String leksem = "";

            mas_lecksem = new String[100];

            n = -1;

            bool isminus = false;

            for (int i = 0; i < _formula.Length; i++)
            {
                if ((op.IsOperation(_formula[i].ToString())) || (_formula[i] == '(') || (_formula[i] == ')'))
                {
                    if ((_formula[i] == '-') && (i == 0) || (_formula[i] == '-') && (_formula[i - 1] == '('))
                    {
                        isminus = true;
                    }
                    else
                    {
                        if ((op.IsOperation(_formula[i].ToString())) && (_formula[i - 1] != ')'))
                        {
                            n++;
                            mas_lecksem[n] = leksem;
                        }
                        if ((_formula[i] == '(') && (i != 0) && (!op.IsOperation(_formula[i - 1].ToString())) && (_formula[i - 1] != '('))
                        {
                            n++;
                            mas_lecksem[n] = leksem;
                        }
                        if (_formula[i] == ')' && ((i != 0)) && (formula[i - 1] != ')'))
                        {
                            n++;
                            mas_lecksem[n] = leksem;
                        }
                        n++;
                        mas_lecksem[n] = _formula[i].ToString();
                        leksem = "";
                    }
                }
                else
                {
                    //char d;

                    if (char.IsNumber(_formula[i]) && isminus)
                    {
                        leksem = "-";
                        isminus = false;
                    }
                    leksem = leksem + _formula[i];
                }
            }
            if (_formula[_formula.Length - 1] != ')')
            {
                n++;
                mas_lecksem[n] = leksem;
            }
            n++;
        }

        public double obchuslennya()
        {
            polzap = PolskuyZapus();

            String[] mas;
            int n;

            if (polzap[polzap.Length - 1] == ' ')
            {
                polzap = polzap.Substring(0, polzap.Length - 1);
            }

            mas = polzap.Split();

            n = mas.Length;

            Stack<double> chusla = new Stack<double>();

            Function fn = new Function();

            Operation op = new Operation();

            int k = 0;

            while (true)
            {
                bool z = true;

                Window1.param = 0;

                double res = 0;

                for (int i = 0; i < n; i++)
                {
                    double d;

                    if (double.TryParse(mas[i], out d))
                    {
                        chusla.Push(d);
                    }

                    Identuficator id = new Identuficator(mas[i], 0);

                    if (setId.Contains(id))
                    {
                        foreach (Identuficator el in setId)
                        {
                            if (el.IDENT == mas[i])
                            {
                                chusla.Push(el.ZNACHENNYA);
                            }
                        }
                    }

                    if (op.IsOperation(mas[i]))
                    {
                        double operand1; double operand2;

                        operand2 = chusla.Pop();
                        operand1 = chusla.Pop();

                        res = op.ObchuslennyaOperatsii(operand1, operand2, mas[i]);

                        if (Window1.param == 1)
                        {
                            z = false;
                            break;
                        }

                        chusla.Push(res);
                        res = 0;
                    }

                    if (fn.IsFunction(mas[i]))
                    {
                        double operand;

                        operand = chusla.Pop();

                        res = fn.ObchuslennyaFormylu(mas[i], operand);

                        if (Window1.param == 1)
                        {
                            z = false;
                            break;
                        }

                        chusla.Push(res);

                        res = 0;
                    }
                }
                if (z)
                {
                    break;
                }
                k++;
                if (k == 1)
                {
                    chusla.Push(0);
                    break;
                }
            }
            result = chusla.Pop();

            return result;
        }

        public SortedSet<Identuficator> SetId
        {
            get { return setId; }
        }

        public string ZadanaFormula
        {
            get { return formula; }
            set { formula = value; }
        }

        public override string ToString()
        {
            string line = "";
            line += " formula=" + formula + "\n" + "\n";
            line += " polskuy zapus=" + polzap + "\n" + "\n";
            foreach (Identuficator el in setId)
            {
                line += el.ToString();
            }
            line += "\n" + " result = " + result + "\n" + "\n";

            return line;
        }
    }
}
