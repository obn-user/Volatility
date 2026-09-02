using Test.Model.DataFormat;

namespace Test.Model.Indicators
{
    public class One_Candle_Range_Indicator
    {
        public decimal One_Candle_Range { get; set; }
        public One_Candle_Range_Indicator(Candle candle)
        {              
            One_Candle_Range = candle.High - candle.Low;
        }
    }
}
