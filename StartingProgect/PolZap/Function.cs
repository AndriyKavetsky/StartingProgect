using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect.PolZap
{
    class Function
    {
        private SortedSet<string> name_function;

        public Function()
        {
            name_function = new SortedSet<string>();

            name_function.Add("sin");
            name_function.Add("cos");
            name_function.Add("ln");
            name_function.Add("exp");
        }
        public void Show()
        {
            Console.WriteLine(name_function.ToString());
        }


        public bool IsFunction(string function)
        {
            return name_function.Contains(function);
        }

        public double ObchuslennyaFormylu(string form, double chus)
        {
            double res = 0;

            if (name_function.Contains(form))
            {
                switch (form)
                {
                    case "sin":
                        {
                            res = Math.Sin(chus);
                        } break;
                    case "cos":
                        {
                            res = Math.Cos(chus);
                        } break;
                    case "ln":
                        {
                            if (chus <= 0)
                            {
                                //Console.WriteLine(" Error : change parametr");
                                Window1.param = 1;
                                throw new Exception(" Function incorrect");
                                //return res;
                            }

                            res = Math.Log10(chus);
                        } break;
                    case "exp":
                        {
                            res = Math.Exp(chus);
                        } break;
                }
            }
            return res;
        }
    }
}
