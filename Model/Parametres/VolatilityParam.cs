using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.Parametres
{
    public class VolatilityParam
    {

        public decimal InitialDeposit { get; set; } = 1000000;

        public decimal InitialContracts { get; set; } = 0;

        public string SecCode { get; set; } = "Sber";

        public decimal TradeDicetion = 1;

        public int EmaPeriod { get; set; }
    }
}
