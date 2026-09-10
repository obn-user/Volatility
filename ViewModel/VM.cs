using ScottPlot.WPF;
using System.Windows;
using Test.Command;
using Test.Model.DataFormat;
using Test.Model.Tester.Spread_Volatility;

namespace Test.ViewModel
{//10_09_2026_1
    public class VM : BaseVM
    {

        #region Свойства класса


        // Свойство, к которому привязан ввод и вывод параметра для пересчёта графика


        private string _numberOfStandardDeviations = "1";
        public string NumberOfStandardDeviations
        {
            get { return _numberOfStandardDeviations; }
            set
            {
                if (decimal.TryParse(value, out _))
                {
                    _numberOfStandardDeviations = value;
                }
                else
                {
                    MessageBox.Show("Ошибка ввода - необходимо ввести целое или десятичное число стандартных отклонений. Выставлено значение 1");
                    _numberOfStandardDeviations = "1";
                }
                //    _numberOfStandardDeviations = value;
                OnPropertyChanged(nameof(NumberOfStandardDeviations));
            }
        }

        private string _period = "200";
        public string Period
        {
            get { return _period; }
            set
            {
                if (int.TryParse(value, out _))
                {
                    _period = value;
                }
                else
                {
                    MessageBox.Show("Ошибка ввода - необходимо ввести целое число для периода осреднения и вычисления стандартного отклонения. Выставлено значение 200");
                    _period = "200";
                }
                //    _period = value;
                OnPropertyChanged(nameof(Period));
            }
        }


        //Определение метода,который выполняется по команде. В данном случае - перерисовка графика
        private RelayCommand? plotCommand;
        public RelayCommand? PlotCommand
        {
            get
            {
                return plotCommand ??= new RelayCommand(obj =>
                    {
                        Plot();
                        Plot1();
                        Plot2();
                        Plot3();
                        Plot4();
                    });
            }
        }

        private RelayCommand? _clearPlot;
        public RelayCommand? ClearPlot
        {
            get
            {
                return _clearPlot ??= new RelayCommand(obj =>
                    {
                        ClearALL();

                    });
            }
        }
        //Свойства, необходимые для вывода ScottPlott

        public WpfPlot? PlotControl { get; set; } = new WpfPlot();
        public WpfPlot? PlotControl1 { get; set; } = new WpfPlot();
        public WpfPlot? PlotControl2 { get; set; } = new WpfPlot();


        public WpfPlot? PlotControl3 { get; set; } = new WpfPlot();


        public WpfPlot? PlotControl4 { get; set; } = new WpfPlot();

        #endregion


        public VM()

        {

        }


        void Plot()
        {

            Spread_Volatillity_LongToAverage_Param param = new()
            {
                NumberOfStandardDeviations = Convert.ToDecimal(NumberOfStandardDeviations),
                Period = Convert.ToInt32(Period)
            };

            Tester tester = new(param);

            PlotData? plotData = tester.PlotData;


            #region Вывод графиков 

            if (plotData != null)
            {
                if (plotData.Candles != null)
                {
                    var plotCandles = PlotControl.Plot.Add.Candlestick(plotData.Candles);
                }

                for (int i = 0; i < plotData.plotIndicators.Count; i++)
                {
                    var plotIndicator = PlotControl.Plot.Add.ScatterLine(plotData.Dates, plotData.plotIndicators[i].indicator);
                    plotIndicator.LegendText = plotData.plotIndicators[i].indicatorName;
                    plotIndicator.LineWidth = 1;
                    plotIndicator.MarkerSize = 0;
                    plotIndicator.Color = PlotControl.Plot.Add.GetNextColor();
                }
                PlotControl.Plot.Axes.DateTimeTicksBottom();
                PlotControl.Plot.ShowLegend();
                PlotControl.Plot.Title("Лонг - цена ниже средней на N сигм, закрытие - цена достигла средней", 30);
                PlotControl.Plot.Axes.AutoScale();
                PlotControl.Refresh();
            }
        }

        void Plot1()
        {


            Spread_Volatillity_ShortToAverage_Param param = new()
            {
                NumberOfStandardDeviations = Convert.ToDecimal(NumberOfStandardDeviations),
                Period = Convert.ToInt32(Period)
            };

            Tester tester = new(param);

            PlotData? plotData = tester.PlotData;

            #region Вывод графиков 

            if (plotData != null)
            {
                if (plotData.Candles != null)
                {
                    var plotCandles = PlotControl1.Plot.Add.Candlestick(plotData.Candles);
                }

                for (int i = 0; i < plotData.plotIndicators.Count; i++)
                {
                    var plotIndicator = PlotControl1.Plot.Add.ScatterLine(plotData.Dates, plotData.plotIndicators[i].indicator);
                    plotIndicator.LegendText = plotData.plotIndicators[i].indicatorName;
                    plotIndicator.LineWidth = 1;
                    plotIndicator.MarkerSize = 0;
                    plotIndicator.Color = PlotControl.Plot.Add.GetNextColor();
                }
                PlotControl1.Plot.Axes.DateTimeTicksBottom();
                PlotControl1.Plot.ShowLegend();
                PlotControl1.Plot.Title("Шорт - цена выше средней на N сигм, закрытие - цена достигла средней", 30);
                PlotControl1.Plot.Axes.AutoScale();
                PlotControl1.Refresh();
            }
        }

        void Plot2()
        {


            Spread_Volatillity_BiDirection_Param param = new()
            {
                NumberOfStandardDeviations = Convert.ToDecimal(NumberOfStandardDeviations),
                Period = Convert.ToInt32(Period)
            };

            Tester tester = new(param);

            PlotData? plotData = tester.PlotData;

            #region Вывод графиков 

            if (plotData != null)
            {
                if (plotData.Candles != null)
                {
                    var plotCandles = PlotControl2.Plot.Add.Candlestick(plotData.Candles);
                }

                for (int i = 0; i < plotData.plotIndicators.Count; i++)
                {
                    var plotIndicator = PlotControl2.Plot.Add.ScatterLine(plotData.Dates, plotData.plotIndicators[i].indicator);
                    plotIndicator.LegendText = plotData.plotIndicators[i].indicatorName;
                    plotIndicator.LineWidth = 1;
                    plotIndicator.MarkerSize = 0;
                    plotIndicator.Color = PlotControl2.Plot.Add.GetNextColor();
                }
                PlotControl2.Plot.Axes.DateTimeTicksBottom();
                PlotControl2.Plot.ShowLegend();
                PlotControl2.Plot.Title("Двустороннее открытие позици лонг и шорт", 30);
                PlotControl2.Plot.Axes.AutoScale();
                PlotControl2.Refresh();
            }
        }

        void Plot3()
        {


            Spread_Volatillity_BiDirection_ToAverage_Param param = new()
            {
                NumberOfStandardDeviations = Convert.ToDecimal(NumberOfStandardDeviations),
                Period = Convert.ToInt32(Period)
            };

            Tester tester = new(param);

            PlotData? plotData = tester.PlotData;

            #region Вывод графиков 

            if (plotData != null)
            {
                if (plotData.Candles != null)
                {
                    var plotCandles = PlotControl3.Plot.Add.Candlestick(plotData.Candles);
                }

                for (int i = 0; i < plotData.plotIndicators.Count; i++)
                {
                    var plotIndicator = PlotControl3.Plot.Add.ScatterLine(plotData.Dates, plotData.plotIndicators[i].indicator);
                    plotIndicator.LegendText = plotData.plotIndicators[i].indicatorName;
                    plotIndicator.LineWidth = 1;
                    plotIndicator.MarkerSize = 0;
                    plotIndicator.Color = PlotControl3.Plot.Add.GetNextColor();
                }
                PlotControl3.Plot.Axes.DateTimeTicksBottom();
                PlotControl3.Plot.ShowLegend();
                PlotControl3.Plot.Title("Двустороннее открытие позици лонг и шорт, закрытие на средней", 30);
                PlotControl3.Plot.Axes.AutoScale();
                PlotControl3.Refresh();
            }
        }
        void Plot4()
        {


            CalculateIdealSpread_Param param = new()
            {
                NumberOfStandardDeviations = Convert.ToDecimal(NumberOfStandardDeviations),
                Period = Convert.ToInt32(Period)
            };

            Tester tester = new(param);

            PlotData? plotData = tester.PlotData;

            #region Вывод графиков 

            if (plotData != null)
            {
                if (plotData.Candles != null)
                {
                    var plotCandles = PlotControl4.Plot.Add.Candlestick(plotData.Candles);
                }

                for (int i = 0; i < plotData.plotIndicators.Count; i++)
                {
                    var plotIndicator = PlotControl4.Plot.Add.ScatterLine(plotData.Dates, plotData.plotIndicators[i].indicator);
                    plotIndicator.LegendText = plotData.plotIndicators[i].indicatorName;
                    plotIndicator.LineWidth = 1;
                    plotIndicator.MarkerSize = 2;
                    plotIndicator.Color = PlotControl4.Plot.Add.GetNextColor();
                }
                PlotControl4.Plot.Axes.DateTimeTicksBottom();
                PlotControl4.Plot.ShowLegend();
                PlotControl4.Plot.Title("Двустороннее открытие позици лонг и шорт, закрытие на средней", 30);
                PlotControl4.Plot.Axes.AutoScale();
                PlotControl4.Refresh();
            }
        }
        #endregion

        #region Обнуление графика
        void ClearALL()
        {
            PlotControl.Plot.Clear();
            PlotControl1.Plot.Clear();
            PlotControl2.Plot.Clear();
            PlotControl3.Plot.Clear();
            PlotControl4.Plot.Clear();

            PlotControl.Refresh();
            PlotControl1.Refresh();
            PlotControl2.Refresh();
            PlotControl3.Refresh();
            PlotControl4.Refresh();
        }
        #endregion
    }
}

#endregion
#endregion

#endregion
#endregion
