using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.Indicators
{
    public class Indicator
    {
        public Indicator()
        { }
        public decimal Simple_Average(List<decimal> data)
        {
            decimal simple_Average = 0;

            for (int i = 0; i < data.Count; i++)
            {
                simple_Average = simple_Average + data[i];
            }

            return simple_Average = simple_Average / data.Count;

        }

        public decimal Standard_Deviation(List<decimal> data)
        {
            decimal standardDeviation = 0;
            decimal average = 0;

            for (int i = 0; i < data.Count; i++)
            {
                average = average + data[i];
            }

            average = average / data.Count;

            for (int i = 0; i < data.Count; i++)
            {
                standardDeviation = (average - data[i]) * (average - data[i]) + standardDeviation;
            }

            return _ = Convert.ToDecimal((Math.Sqrt((Convert.ToDouble(standardDeviation))) / data.Count));

        }
    }
    }
    
    

