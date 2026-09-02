using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.DataFormat
{
    public class Level

    {
        public decimal LecelMin = new(); 
        public decimal LevelMax = new();

        public decimal LevelContracts = new();
        public decimal LevelDeposit = new();


        public List<Trade> LevelTrades = [];


    }
}
