using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model.DataFormat;

namespace Test.Model.Actions
{
    public class AddNewTrade
    {
        public Trade Trade { get; set; }
        public AddNewTrade (Trade trade)
        {
            Trade = trade;
        }
    }
}
