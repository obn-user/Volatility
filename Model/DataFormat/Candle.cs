using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.DataFormat
{
    public class Candle //Формат свечей для расчёта
    {
        public int Id = 0;
        
        public string Ticker = "";

        public string Period = "";

        public DateTime DateTime = new DateTime();

        public decimal Open = 0;

        public decimal High = 0;

        public decimal Low = 0;

        public decimal Close = 0;

        public decimal Volume = 0;
    }
}
