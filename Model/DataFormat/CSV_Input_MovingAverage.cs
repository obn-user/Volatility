using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.DataFormat
{
    public class CSV_Input_MovingAverage
    {
        public string Ticker { get; set; } = "";

        public string Period { get; set; } = "";

        public string Date { get; set; } = "";

        public string Time { get; set; } = "";

        public string Close { get; set; } = "";
    }
}
