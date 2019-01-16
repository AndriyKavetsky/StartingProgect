using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect.PolZap
{
    class Operation
    {
        private SortedSet<string> name_operation = new SortedSet<string>(new String[] { "+", "-", "*", "/", "^" });

        public Operation()
        {
            name_operation = new SortedSet<string>();
            name_operation.Add("+");
            name_operation.Add("-");
            name_operation.Add("*");
            name_operation.Add("/");
            name_operation.Add("^");
        }

        public bool IsOperation(string el)
        {
            return (name_operation.Contains(el));
        }

        public int priorutet(string op1)
        {
            int p = -1;
            switch (op1)
            {
                case "+":
                case "-": { p = 0; } break;
                case "*":
                case "/": { p = 1; } break;
                case "^": { p = 2; } break;
            }
            return p;
        }

        public string NAME_ORIENTATION
        {
            set { name_operation.Add(value); }
        }

        public void DeleteOperation(string oper)
        {
            if (name_operation.Contains(oper))
            {
                name_operation.Remove(oper);
            }
        }
        public double ObchuslennyaOperatsii(double operand1, double operand2, string operation)
        {
            if (!name_operation.Contains(operation))
            {
                return 0;
            }

            double result = 0;

            switch (operation)
            {
                case "+":
                    {
                        result = operand1 + operand2;
                    } break;
                case "-":
                    {
                        result = operand1 - operand2;
                    } break;
                case "*":
                    {
                        result = operand1 * operand2;
                    } break;
                case "/":
                    {
                        if (operand2 == 0)
                        {
                            //Console.WriteLine(" Error : change parametr");                            
                            Window1.param = 1;
                            throw new Exception(" Operation incorrect");
                            //return result;
                        }
                        result = operand1 / operand2;
                    } break;
                case "^":
                    {
                        result = Math.Pow(operand1, operand2);
                    } break;
            }
            return result;
        }
    }
}
