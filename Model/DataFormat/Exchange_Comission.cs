using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.DataFormat
{
    public class Exchange_Comission
    {
        public decimal FixedPart = new();

        public decimal PercentPart = new();

        public Exchange_Comission()
        {
        }

        public Exchange_Comission(decimal fixedPart, decimal percentPart)
        {
            FixedPart = fixedPart; 
            PercentPart = percentPart;
        }
    }
}
