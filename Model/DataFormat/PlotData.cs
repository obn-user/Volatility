using System.Collections.ObjectModel;

namespace Test.Model.DataFormat
{
    public class PlotData
    {
        public List<ScottPlot.OHLC> Candles = [];
        public List<double> Dates = [];
        public ObservableCollection<PlotIndicator> plotIndicators = [];
    }
}
