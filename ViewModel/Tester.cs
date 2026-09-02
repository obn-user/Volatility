using Test.Model.DataFormat;
using Test.Model.Methods;
using Test.Model.Tester.Spread_Volatility;

namespace Test.ViewModel
{
    public class Tester : Method
    {
        public PlotData? PlotData { get; set; }
        public Tester(Spread_Volatillity_LongToAverage_Param param)
        {
            PlotData = Spread_Volatillity_LongToAverage_Algo(param);
        }
        public Tester(Spread_Volatillity_ShortToAverage_Param param)
        {
            PlotData = Spread_Volatillity_ShortToAverage_Algo(param);
        }
        //Spread_Volatillity_BiDirection_Algo
        public Tester(Spread_Volatillity_BiDirection_Param param)
        {
            PlotData = Spread_Volatillity_BiDirection_Algo(param);
        }

        public Tester(Spread_Volatillity_BiDirection_ToAverage_Param param)
        {
            PlotData = Spread_Volatillity_BiDirection_ToAverage_Algo(param);
        }

        public Tester(CalculateIdealSpread_Param param)
        {
            PlotData = CalculateIdealSpread_Algo(param);
        }

    }
}
