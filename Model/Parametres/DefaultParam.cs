using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.Parametres
{
    public class DefaultParam
    {
        public string FileName { get; set; } = "sber_d_price.txt";

        public decimal InitialDeposit { get; set; } = 1000000;

        public decimal InitialContracts { get; set; } = 0;

        public string SecurityCode { get; set; } = "Sber";

        public decimal TradeDicetion = 1;

        public decimal NumberOfStandardDeviation = 1;
    }
}

