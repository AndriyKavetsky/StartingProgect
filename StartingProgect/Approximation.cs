using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartingProgect
{
    public delegate double BaseFunctions(double [] point);//point - contains 1,2,3 d - dimentions 
    
    public abstract class Approximation //linear approximation
    {
        protected int approximationDegree;
        protected int domainDimention;//is it correct //readonly
        protected int elementForm;//triangle or rectangle
        protected List<BaseFunctions> baseFunctions;//multi dimentional base functions 

        public Approximation(int approximationDegree, int domainDimention, int elementForm)
        {            
            this.approximationDegree = approximationDegree;
            this.domainDimention = domainDimention;
            this.elementForm=elementForm;
            baseFunctions = new List<BaseFunctions>();
        }
        public List<BaseFunctions> BaseFunctions
        {
            get
            {
                return baseFunctions;
            }
            set
            {
                baseFunctions = value;
            }
        }
        public int DomainDimention
        {
            get
            {
                return domainDimention;
            }
            set
            {
                domainDimention = value;
            }
        }
        public int ApproximationDegree
        {
            get
            {
                return approximationDegree;
            }
            set
            {
                approximationDegree = value;
            }
        }

    }
}
