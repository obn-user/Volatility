using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Model.DataFormat
{
    public class Flag
    {
        public enum FlagType 
        {
            Open, 
            CloseAtProfit,
            Keep,
            StopLoss,
            LongFirstOpen,
            ShortFirstOpen,
            LongReverse,
            ShortReverse,
            ShortCloseAtAverage,
            LongCloseAtAverage,
            ShortOpen,
            LongOpen
        };
    }
}
