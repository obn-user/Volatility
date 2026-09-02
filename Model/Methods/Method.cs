using CsvHelper;
using System.Globalization;
using System.IO;
using System.Windows;
using Test.Model.DataFormat;
using Test.Model.Tester.Sentiment;
using Test.Model.Tester.Spread_Volatility;

namespace Test.Model.Methods
{

    public class Method
    {
        #region Подсчёт депозита


        public static List<double> CalculateDeposit(decimal InitialDeposit, List<Trade> Trades, List<Candle> candles)
        {
            List<double> Deposit = [];
            decimal _deposit = InitialDeposit;
            decimal _contracts = new();

            for (int i = 0; i < Trades.Count; i++)
            {
                _contracts = _contracts + Trades[i].Volume;
                _deposit = _deposit - Trades[i].Volume * Trades[i].Price;
                double indicative_deposit = Convert.ToDouble((_deposit + _contracts * candles[i].Close));

                Deposit.Add(indicative_deposit);
            }
            return Deposit;
        }

        #endregion

        #region Индикаторы

        public static decimal Simple_Average_Indicator(List<decimal> data)
        {

            decimal simple_Average = 0;

            for (int i = 0; i < data.Count; i++)
            {
                simple_Average = simple_Average + data[i];
            }

            simple_Average = simple_Average / data.Count;

            return simple_Average;
        }

        public static decimal Standard_Deviation_Indicator(List<decimal> data)
        {

            decimal standardDeviation = 0;
            decimal average = Simple_Average_Indicator(data);


            for (int i = 0; i < data.Count; i++)
            {
                standardDeviation = (average - data[i]) * (average - data[i]) + standardDeviation;
            }

            standardDeviation = Convert.ToDecimal((Math.Sqrt((Convert.ToDouble(standardDeviation)) / data.Count)));

            return standardDeviation;
        }

        // 3 4 2 5 => 
        #endregion

        #region Проверки
        public int CheckInteger(string input)
        {



            if (int.TryParse(input, out int number))
            {
                return number;
            }
            else
            {
                MessageBox.Show("Необходимо ввести целое положительное  число уровней. Произведен расчёт для тестера с одним уровнем");
                return 1;
            }
        }
        #endregion

        #region Данные
        //
        public static PlotIndicator AddPlotcontrol(string indicator_name, List<double> indicator)
        {
            PlotIndicator plotIndicator = new();
            plotIndicator.indicatorName = indicator_name;
            plotIndicator.indicator = indicator;

            return plotIndicator;
        }

        public static List<ScottPlot.OHLC> Convert_to_ScottPlottCandles(List<Candle> input_candles)
        {
            List<ScottPlot.OHLC> candles = [];
            DateTime dt = new();

            for (int i = 0; i < input_candles.Count; i++)
            {
                double open = Convert.ToDouble(input_candles[i].Open);
                double close = Convert.ToDouble(input_candles[i].Close);
                double high = Convert.ToDouble(input_candles[i].High);
                double low = Convert.ToDouble(input_candles[i].Low);
                dt = input_candles[i].DateTime;
                candles.Add(new ScottPlot.OHLC(open, high, low, close, dt.AddMinutes(0), TimeSpan.FromSeconds(60)));
            }
            return candles;
        }
        //

        public List<Candle> CandleReadFromFile(string filename)
        {
            List<CSVinput> CSVCandles = new();

            string filePath = filename;

            List<Candle> candles = new();


            using (var reader = new StreamReader(filePath))


            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                CSVCandles = csv.GetRecords<CSVinput>().ToList();
            }
            //=================================================



            //===============Перевод строковых данных в формат свечей


            IFormatProvider formater = new NumberFormatInfo { NumberDecimalSeparator = "." };

            //Candle _datacandle1 = new Candle();


            for (int i = 0; i < CSVCandles.Count; i++)

            {

                Candle _datacandle1 = new();

                _datacandle1.Id = i;
                _datacandle1.Ticker = CSVCandles[i].Ticker;
                _datacandle1.Period = CSVCandles[i].Period;
                string input = CSVCandles[i].Date + CSVCandles[i].Time;
                const string format = "yyyyMMddHHmmss";
                if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    _datacandle1.DateTime = result;
                }
                else
                {
                    _datacandle1.DateTime = new();
                }





                _datacandle1.Open = decimal.Parse(CSVCandles[i].Open, formater);
                _datacandle1.High = decimal.Parse(CSVCandles[i].High, formater);
                _datacandle1.Low = decimal.Parse(CSVCandles[i].Low, formater);
                _datacandle1.Close = decimal.Parse(CSVCandles[i].Close, formater);
                _datacandle1.Volume = decimal.Parse(CSVCandles[i].Volume, formater);

                candles.Add(_datacandle1);
            }
            //=====================================================================
            return candles;
        }

        #endregion

        #region Алгоритмы

        public PlotData Sentiment_Algo(Sentiment_Param param)
        {
            #region Получение данных

            List<Candle> MCFTRs = [];
            List<Candle> RGBITRs = [];
            #endregion

            #region Проверка данных
            List<Candle> mcftrs = [];
            List<Candle> rgbitrs = [];
            mcftrs = CandleReadFromFile(param.FileName);
            rgbitrs = CandleReadFromFile(param.FileName_indicator);

            for (int i = 0; i < mcftrs.Count; i++)
            {
                int rgbitr = -1;
                for (int k = 0; k < rgbitrs.Count; k++)
                {
                    if (mcftrs[i].DateTime == rgbitrs[k].DateTime)
                    {
                        rgbitr = k;
                    }
                }

                if (rgbitr >= 0)
                {
                    MCFTRs.Add(mcftrs[i]);
                    RGBITRs.Add(rgbitrs[rgbitr]);
                }
            }
            #endregion

            #region Расчёт финансового результата для каждого уровня
            List<Trade> Trades = [];
            List<Level> Levels = [];


            for (int i = 0; i < param.NumberOfLevel; i++) //Цикл перебора уровней
            {
                #region Получение данных и объявление переменных
                decimal MedianSentiment = ((param.Max - param.Min) / 2 + param.Min);
                decimal LevelStep = (param.Max - param.Min) / (param.NumberOfLevel * 2);
                decimal SentimentIndicator = new();

                Level level = new();
                level.LevelContracts = 0;
                level.LevelDeposit = param.InitialDeposit / param.NumberOfLevel;
                level.LevelMax = MedianSentiment + LevelStep * (i + 1);
                level.LecelMin = MedianSentiment - LevelStep * (i + 1);


                #endregion

                for (int j = 0; j < MCFTRs.Count; j++) //Определение фин результата для каждого уровня
                {
                    #region Расчёт индикатора сентимента
                    SentimentIndicator = MCFTRs[j].Open / RGBITRs[j].Open;


                    #endregion

                    #region Расчёт логики открытия позиции

                    bool SignalToOpen = (level.LevelDeposit > 0) && (level.LecelMin > SentimentIndicator);
                    bool SignalToClose = (level.LevelContracts > 0) && (level.LevelMax < SentimentIndicator);

                    #endregion

                    #region Запись открытых и закрытых позиций
                    switch (SignalToOpen)
                    {
                        case true:

                            Trade trade = new
                                               (j, // Номер свечи
                                              param.SecurityCode, // Код инструмента
                                                MCFTRs[j].DateTime, // Дата и время
                                                                    //   TradeDirection.Direction.Long,// Направление сделки
                                                 MCFTRs[j].Open,  //Цена
                                                   level.LevelDeposit / MCFTRs[j].Open, //Объём
                                                   MCFTRs[j].Open * (level.LevelDeposit / MCFTRs[j].Open)
                          );

                            Trades.Add(trade);

                            level.LevelContracts = level.LevelDeposit / MCFTRs[j].Open;
                            level.LevelDeposit = 0;
                            break;
                    }


                    switch (SignalToClose)
                    {
                        case true:

                            Trade trade = new
                                                (j, // Номер свечи
                                                param.SecurityCode, // Код инструмента
                                                MCFTRs[j].DateTime, // Дата и время
                                                                    //    TradeDirection.Direction.Short,// Направление сделки
                                              MCFTRs[j].Open,  //Цена
                                               level.LevelContracts,                                               //Объём
                                               MCFTRs[j].Open * level.LevelContracts
                                                );

                            Trades.Add(trade);

                            level.LevelDeposit = MCFTRs[j].Open * level.LevelContracts;
                            level.LevelContracts = 0;
                            break;
                    }



                    #endregion
                }

                Levels.Add(level);
            }



            #endregion

            #region Подготовка свечного графика
            decimal CurrentDeposit = param.InitialDeposit;

            List<ScottPlot.OHLC> candles = Convert_to_ScottPlottCandles(MCFTRs);


            #endregion

            #region Подготовка графика депозита


            double _deposit = Convert.ToDouble(param.InitialDeposit);
            double _contracts = new();
            List<double> Deposit = [];
            List<double> Dates = [];
            List<double> SentimentIndicatorArray = [];

            for (int i = 0; i < MCFTRs.Count; i++)
            {





                double dateTime = MCFTRs[i].DateTime.ToOADate();
                Dates.Add(dateTime);
            }
            Deposit = CalculateDeposit(param.InitialDeposit, Trades, MCFTRs);
            #endregion

            #region Подготовка графика индикатора сентимента

            for (int i = 0; i < MCFTRs.Count; i++)
            {
                SentimentIndicatorArray.Add(Convert.ToDouble(MCFTRs[i].Open / RGBITRs[i].Open) * 1000);
            }

            PlotData iO = new();
            PlotIndicator plotIndicator = new();
            var PlotIndicators = iO.plotIndicators;
            iO.Candles = candles;
            iO.Dates = Dates;

            plotIndicator.indicator = SentimentIndicatorArray;
            plotIndicator.indicatorName = "Сентимент";
            PlotIndicators.Add(plotIndicator);

            PlotIndicator plotIndicator1 = new();
            plotIndicator1.indicator = Deposit;
            plotIndicator1.indicatorName = "депозит";
            PlotIndicators.Add(plotIndicator1);

            iO.plotIndicators = PlotIndicators;

            return iO;
            #endregion
        }
        public PlotData CalculateIdealSpread_Algo(CalculateIdealSpread_Param param)
        {
            List<Candle> candles = CandleReadFromFile(param.FileName);
            List<Candle> rusfar3ms = CandleReadFromFile(param.RusFarFilename);
            List<Candle> imoexs = CandleReadFromFile(param.ImoexFilename);
            List<double> IdealPrices = [];
            List<double> Dates = [];

            for (int i = 0; i < candles.Count; i++)
            {
                decimal idealprice = 0m;
                decimal rate = 0m;
                decimal spot = 0m;

                for (int j = 0; j < rusfar3ms.Count; j++)
                {

                    if (candles[i].DateTime.Date == rusfar3ms[j].DateTime.Date)
                    {
                        rate = rusfar3ms[j].Close;
                    }
                }


                for (int k = 0; k < imoexs.Count; k++)
                {

                    if (candles[i].DateTime.Date == imoexs[k].DateTime.Date)
                    {
                        spot = imoexs[k].High;
                    }
                }
                idealprice = spot * rate * 90 / 365;

                IdealPrices.Add(Convert.ToDouble(idealprice));

                Dates.Add(candles[i].DateTime.ToOADate());
            }

            PlotData plotData = new PlotData()
            {
                Dates = Dates,
                Candles = Convert_to_ScottPlottCandles(candles)
            };

            plotData.plotIndicators.Add(AddPlotcontrol(nameof(IdealPrices), IdealPrices));

            return plotData;
        }
        public PlotData Spread_Volatillity_LongToAverage_Algo(Spread_Volatillity_LongToAverage_Param param)

        {

            #region Переменные


            List<Trade> Trades = [];
            List<double> Sma = [];
            List<double> Minimums = [];
            List<double> Maximums = [];

            List<double> Axis_X_Price = [];
            #endregion


            #region Получение входных данных и объявление переменных

            decimal Contracts = param.InitialContracts; //Начальное количество контрактов
            decimal Depo = param.InitialDeposit; //Начальный депозит
            decimal InitialMargin = param.InitialMargin;
            //       Exchange_Comission exchange_Comission = param.Exchange_Comission; //Биржевая комиссия
            //      decimal Comission = exchange_Comission.PercentPart;
            List<Candle> candles = CandleReadFromFile(param.FileName); //загрузка свече
            int period = param.Period; // период осреднения
            Flag.FlagType flag; // Флаг выбора дйствий - открыть позицию, закрыть позицию или ничего не делать


            #endregion

            for (int i = 0; i < candles.Count; i++) //основной цикл перебора свечей
            {
                flag = Flag.FlagType.Keep;
                Trade trade = new();

                #region Создание массива цен закрытий для расчёта индикаторов
                if (i > period)
                {
                    List<decimal> candles_close = [];
                    for (int j = i - period; j < i - 1; j++)
                    {
                        candles_close.Add(candles[j].Close);
                    }
                    #endregion

                    #region Расчёт индикаторов 
                    // Расчёт индикаторов

                    decimal simple_Average_Indicator = Simple_Average_Indicator(candles_close); //Расчёт средней

                    decimal standard_Deviation_Indicator = Standard_Deviation_Indicator(candles_close); // Расчёт ст отклонения



                    decimal PriceLong = simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations; //средняя - ст отклонение

                    decimal PriceAverage = simple_Average_Indicator;//средняя

                    //            decimal PriceShort = simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations;//средняя + ст отклонение

                    #endregion

                    #region Перебор условий
                    //      flag = Flag.FlagType.Keep;


                    if
                        (
                        candles[i].Low < PriceLong// Открытие позиции лонг
                        &&
                        Depo > InitialMargin
                        &&
                        Contracts == 0
                        )

                    { flag = Flag.FlagType.Open; }


                    if
                            (
                            candles[i].High > PriceAverage //Закрытие позиции лонг
                            &&
                            Contracts > 0


                            )

                    { flag = Flag.FlagType.CloseAtProfit; }


                    #endregion

                    #region Учёт сделок и открытие позиций
                    switch (flag)

                    {
                        case Flag.FlagType.Open:

                            Contracts = Depo / InitialMargin; //При открытии лонга - вычисление объёма котрактов в позиции
                            Depo = Depo - Contracts * PriceLong;

                            trade = new
                               (i, // Номер свечи
                               param.SecurityCode, // Код инструмента
                               candles[i].DateTime, // Дата и время
                                                    //TradeDirection.Direction.Long,// Направление сделки
                               PriceLong,  //Цена
                            Contracts,  //Объём
                               PriceLong * Contracts
                               );
                            Trades.Add(trade);


                            //        Depo = 0;//зануление депозита
                            break;

                        case Flag.FlagType.CloseAtProfit:
                            Depo += PriceAverage * Contracts;//Вычисление прихода денег на счёт
                            trade = new
     (i, // Номер свечи
     param.SecurityCode, // Код инструмента
     candles[i].DateTime, // Дата и время
                          //   TradeDirection.Direction.Short,// Направление сделки
   PriceAverage,  //Цена
 -Contracts, //Объём
    PriceAverage * Contracts
     );
                            Trades.Add(trade);
                            Contracts = 0; //Зануление количества контрактов
                            break;




                        case Flag.FlagType.Keep:
                            trade = new
                        (i, // Номер свечи
                        param.SecurityCode, // Код инструмента
                        candles[i].DateTime, // Дата и время
                                             //   TradeDirection.Direction.Keep,// Направление сделки
                     0m,  //Цена
                        0m,// Объём
                        0m
                        );

                            Trades.Add(trade);
                            //Зануление количества контрактов

                            //Позиции неизменны
                            break;

                    }

                    #endregion

                    #region Запись индикаторов в массив
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    Sma.Add(Convert.ToDouble(simple_Average_Indicator));
                    Minimums.Add(Convert.ToDouble(simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations));
                    Maximums.Add(Convert.ToDouble(simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations));



                }
                if (i <= period)
                {
                    trade = new
                  (i, // Номер свечи
                  param.SecurityCode, // Код инструмента
                  candles[i].DateTime, // Дата и время
                                       //    TradeDirection.Direction.Keep,// Направление сделки
               Convert.ToDecimal(0),  //Цена
                  Convert.ToDecimal(0), 0m  //Объём
                  );
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    Trades.Add(trade);
                    Sma.Add(0);
                    Minimums.Add(0);
                    Maximums.Add(0);
                }
            }
            #endregion


            List<double> Deposit = CalculateDeposit(param.InitialDeposit, Trades, candles);
            PlotData plotdata = new()
            {
                Dates = Axis_X_Price,
                Candles = Convert_to_ScottPlottCandles(candles)
            };
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Maximums), Maximums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Minimums), Minimums));

            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Deposit), Deposit));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Sma), Sma));


            return plotdata;
        }

        public PlotData Spread_Volatillity_ShortToAverage_Algo(Spread_Volatillity_ShortToAverage_Param param)

        {

            #region Переменные


            List<Trade> Trades = [];
            List<double> Sma = [];
            List<double> Minimums = [];
            List<double> Maximums = [];
            List<double> Axis_X_Price = [];
            decimal InitialMargin = param.InitialMargin;
            #endregion


            #region Получение входных данных и объявление переменных

            decimal Contracts = param.InitialContracts; //Начальное количество контрактов
            decimal Depo = param.InitialDeposit; //Начальный депозит
            Exchange_Comission exchange_Comission = param.Exchange_Comission; //Биржевая комиссия
            decimal Comission = exchange_Comission.PercentPart;
            List<Candle> candles = CandleReadFromFile(param.FileName); //загрузка свече
            int period = param.Period; // период осреднения
            Flag.FlagType flag; // Флаг выбора дйствий - открыть позицию, закрыть позицию или ничего не делать


            #endregion

            for (int i = 0; i < candles.Count; i++) //основной цикл перебора свечей
            {
                #region Обнуление флагов
                flag = Flag.FlagType.Keep; // нейтральнй флаг, т.е. позиция не открывается, устанавливается каждый раз в начале
                Trade trade = new();
                #endregion

                #region Создание массива цен закрытий для расчёта индикаторов
                if (i > period)
                {
                    List<decimal> candles_close = [];
                    for (int j = i - period; j < i - 1; j++)
                    {
                        candles_close.Add(candles[j].Close);
                    }
                    #endregion

                    #region Расчёт индикаторов 
                    // Расчёт индикаторов

                    decimal simple_Average_Indicator = Simple_Average_Indicator(candles_close); //Расчёт средней

                    decimal standard_Deviation_Indicator = Standard_Deviation_Indicator(candles_close); // Расчёт ст отклонения



                    decimal PriceLong = simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations; //средняя - ст отклонение

                    decimal PriceAverage = simple_Average_Indicator;//средняя

                    decimal PriceShort = simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations;//средняя + ст отклонение

                    #endregion

                    #region Перебор условий
                    //     flag = Flag.FlagType.Keep;//занейтраливание флага

                    if
                        (
                        candles[i].High >= PriceShort// Открытие позиции шорт
                        &&
                        Contracts == 0
                        &&
                    Depo > InitialMargin

                        )

                    { flag = Flag.FlagType.Open; }


                    if
                        (
                        candles[i].Low <= PriceAverage //Закрытие позиции шорт
                        &&
                        Contracts < 0
                        )

                    { flag = Flag.FlagType.CloseAtProfit; }
                    #endregion

                    #region Учёт сделок и открытие позиций
                    switch (flag)

                    {
                        case Flag.FlagType.Open:

                            Contracts = -Depo / InitialMargin; //При открытии лонга - вычисление объёма котрактов в позиции
                            Depo -= Contracts * PriceShort;

                            trade = new
                               (i, // Номер свечи
                               param.SecurityCode, // Код инструмента
                               candles[i].DateTime, // Дата и время
                            //   TradeDirection.Direction.Short,// Направление сделки
                               PriceShort,  //Цена
                               Contracts,
                               PriceShort * Contracts//Объём
                               );
                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.CloseAtProfit:


                            Depo += PriceAverage * Contracts;//Вычисление прихода денег на счёт


                            trade = new
                                 (i, // Номер свечи
                                 param.SecurityCode, // Код инструмента
                                 candles[i].DateTime, // Дата и время
                                                      //         TradeDirection.Direction.Long,// Направление сделки
                               PriceAverage,  //Цена
                              -Contracts,
                                PriceAverage * (-1) * Contracts //Объём
                                 );

                            Trades.Add(trade);
                            Contracts = 0; //Зануление количества контрактов
                            break;

                        case Flag.FlagType.Keep:
                            trade = new
                        (i, // Номер свечи
                        param.SecurityCode, // Код инструмента
                        candles[i].DateTime, // Дата и время
                                             //              TradeDirection.Direction.Keep,// Направление сделки
                     0m,  //Цена
                        0m,
                        0m  //Объём
                        );

                            Trades.Add(trade);
                            //Зануление количества контрактов

                            //Позиции неизменны
                            break;

                    }

                    #endregion

                    #region Запись индикаторов в массив
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    Sma.Add(Convert.ToDouble(simple_Average_Indicator));
                    Minimums.Add(Convert.ToDouble(simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations));
                    Maximums.Add(Convert.ToDouble(simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations));



                }

                if (i <= period)
                {
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    trade = new
          (i, // Номер свечи
          param.SecurityCode, // Код инструмента
          candles[i].DateTime, // Дата и время
                               //   TradeDirection.Direction.Keep,// Направление сделки
       Convert.ToDecimal(0),  //Цена
          Convert.ToDecimal(0), 0m //Объём
          );

                    Trades.Add(trade);
                    Sma.Add(0);
                    Minimums.Add(0);
                    Maximums.Add(0);
                }
            }
            #endregion

            //

            List<double> Deposit = CalculateDeposit(param.InitialDeposit, Trades, candles);
            //    Deposit.Add(Deposit1[Deposit.Count]);
            //
            PlotData plotdata = new()
            {
                Dates = Axis_X_Price,
                Candles = Convert_to_ScottPlottCandles(candles)
            };
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Maximums), Maximums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Minimums), Minimums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Deposit), Deposit));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Sma), Sma));


            return plotdata;

        }

        public PlotData Spread_Volatillity_BiDirection_Algo(Spread_Volatillity_BiDirection_Param param)
        {



            #region Переменные


            List<Trade> Trades = [];
            List<double> Sma = [];
            List<double> Minimums = [];
            List<double> Maximums = [];
            List<double> Axis_X_Price = [];
            decimal InitialMargin = param.InitialMargin;
            #endregion


            #region Получение входных данных и объявление переменных

            decimal Contracts = param.InitialContracts; //Начальное количество контрактов
            decimal Depo = param.InitialDeposit; //Начальный депозит
            Exchange_Comission exchange_Comission = param.Exchange_Comission; //Биржевая комиссия
            decimal Comission = exchange_Comission.PercentPart;
            List<Candle> candles = CandleReadFromFile(param.FileName); //загрузка свече
            int period = param.Period; // период осреднения
            Flag.FlagType flag; // Флаг выбора дйствий - открыть позицию, закрыть позицию или ничего не делать


            #endregion

            for (int i = 0; i < candles.Count; i++) //основной цикл перебора свечей
            {
                #region Обнуление флагов
                flag = Flag.FlagType.Keep; // нейтральнй флаг, т.е. позиция не открывается, устанавливается каждый раз в начале
                Trade trade = new();
                #endregion

                #region Создание массива цен закрытий для расчёта индикаторов
                if (i > period)
                {
                    List<decimal> candles_close = [];
                    for (int j = i - period; j < i - 1; j++)
                    {
                        candles_close.Add(candles[j].Close);
                    }
                    #endregion

                    #region Расчёт индикаторов 
                    // Расчёт индикаторов

                    decimal simple_Average_Indicator = Simple_Average_Indicator(candles_close); //Расчёт средней

                    decimal standard_Deviation_Indicator = Standard_Deviation_Indicator(candles_close); // Расчёт ст отклонения



                    decimal PriceLong = simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations; //средняя - ст отклонение

                    decimal PriceAverage = simple_Average_Indicator;//средняя

                    decimal PriceShort = simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations;//средняя + ст отклонение

                    #endregion

                    #region Перебор условий
                    //        flag = Flag.FlagType.Keep;//занейтраливание флага

                    if
                        (
                        candles[i].High >= PriceShort// Открытие позиции шорт
                        &&
                        Contracts == 0
                        )
                    { flag = Flag.FlagType.ShortFirstOpen; }

                    if
                     (
                     candles[i].High >= PriceShort// Открытие позиции шорт
                     &&
                     Contracts > 0
                     )
                    { flag = Flag.FlagType.ShortReverse; }


                    if
                            (
                            candles[i].Low <= PriceLong //Закрытие позиции шорт
                            &&
                            Contracts == 0
                            )
                    { flag = Flag.FlagType.LongFirstOpen; }

                    if
                         (
                         candles[i].Low <= PriceLong //Закрытие позиции шорт
                         &&
                         Contracts < 0
                         )
                    { flag = Flag.FlagType.LongReverse; }
                    #endregion

                    #region Учёт сделок и открытие позиций
                    switch (flag)

                    {
                        case Flag.FlagType.ShortFirstOpen:

                            Contracts = -Depo / InitialMargin; //При открытии лонга - вычисление объёма котрактов в позиции
                            Depo = Depo - Contracts * PriceShort;

                            trade = new
                               (i, // Номер свечи
                               param.SecurityCode, // Код инструмента
                               candles[i].DateTime, // Дата и время
                                                    //   TradeDirection.Direction.Short,// Направление сделки
                               PriceShort,  //Цена
                               Contracts,
                               PriceShort * Contracts//Объём
                               );
                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.ShortReverse:
                            decimal c = -Contracts;
                            Depo = Depo + Contracts * PriceShort;
                            Contracts = -Depo / InitialMargin;
                            Depo = Depo - Contracts * PriceShort;
                            //    Contracts = -Depo / PriceShort; //При открытии лонга - вычисление объёма котрактов в позиции

                            c = c + Contracts;
                            trade = new
                               (i, // Номер свечи
                               param.SecurityCode, // Код инструмента
                               candles[i].DateTime, // Дата и время
                                                    //   TradeDirection.Direction.Short,// Направление сделки
                               PriceShort,  //Цена
                         c,
                               PriceShort * c//Объём
                               );
                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.LongFirstOpen:
                            Contracts = Depo / InitialMargin;
                            Depo = Depo - Contracts * PriceLong;

                            trade = new
                                 (i, // Номер свечи
                                 param.SecurityCode, // Код инструмента
                                 candles[i].DateTime, // Дата и время
                                                      //         TradeDirection.Direction.Long,// Направление сделки
                               PriceLong,  //Цена
                              Contracts,
                               PriceLong * Contracts //Объём
                                 );

                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.LongReverse:
                            Depo = Depo + Contracts * PriceLong;
                            decimal c1 = Contracts;
                            Contracts = Depo / InitialMargin;
                            Depo = Depo - Contracts * PriceLong;
                            c1 = Contracts - c1;


                            trade = new
                                 (i, // Номер свечи
                                 param.SecurityCode, // Код инструмента
                                 candles[i].DateTime, // Дата и время
                                                      //         TradeDirection.Direction.Long,// Направление сделки
                               PriceLong,  //Цена
                              c1,
                               PriceLong * c1 //Объём
                                 );

                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.Keep:
                            trade = new
                        (i, // Номер свечи
                        param.SecurityCode, // Код инструмента
                        candles[i].DateTime, // Дата и время
                                             //              TradeDirection.Direction.Keep,// Направление сделки
                     0m,  //Цена
                        0m,
                        0m  //Объём
                        );

                            Trades.Add(trade);
                            //Зануление количества контрактов

                            //Позиции неизменны
                            break;

                    }

                    #endregion

                    #region Запись индикаторов в массив
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    Sma.Add(Convert.ToDouble(simple_Average_Indicator));
                    Minimums.Add(Convert.ToDouble(simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations));
                    Maximums.Add(Convert.ToDouble(simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations));



                }

                if (i <= period)
                {
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    trade = new
          (i, // Номер свечи
          param.SecurityCode, // Код инструмента
          candles[i].DateTime, // Дата и время
                               //   TradeDirection.Direction.Keep,// Направление сделки
   0m,  //Цена
  0m, 0m //Объём
          );

                    Trades.Add(trade);
                    Sma.Add(0);
                    Minimums.Add(0);
                    Maximums.Add(0);
                }
            }
            #endregion

            //

            List<double> Deposit = CalculateDeposit(param.InitialDeposit, Trades, candles);
            //    Deposit.Add(Deposit1[Deposit.Count]);
            //
            PlotData plotdata = new()
            {
                Dates = Axis_X_Price,
                Candles = Convert_to_ScottPlottCandles(candles)
            };
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Maximums), Maximums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Minimums), Minimums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Deposit), Deposit));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Sma), Sma));


            return plotdata;

        }

        public PlotData Spread_Volatillity_BiDirection_ToAverage_Algo(Spread_Volatillity_BiDirection_ToAverage_Param param)
        {
            #region Переменные


            List<Trade> Trades = [];
            List<double> Sma = [];
            List<double> Minimums = [];
            List<double> Maximums = [];
            List<double> Axis_X_Price = [];
            decimal InitialMargin = param.InitialMargin;
            #endregion


            #region Получение входных данных и объявление переменных

            decimal Contracts = param.InitialContracts; //Начальное количество контрактов
            decimal Depo = param.InitialDeposit; //Начальный депозит
            Exchange_Comission exchange_Comission = param.Exchange_Comission; //Биржевая комиссия
            decimal Comission = exchange_Comission.PercentPart;
            List<Candle> candles = CandleReadFromFile(param.FileName); //загрузка свече
            int period = param.Period; // период осреднения
            Flag.FlagType flag; // Флаг выбора дйствий - открыть позицию, закрыть позицию или ничего не делать


            #endregion

            for (int i = 0; i < candles.Count; i++) //основной цикл перебора свечей
            {
                #region Обнуление флагов
                flag = Flag.FlagType.Keep; // нейтральнй флаг, т.е. позиция не открывается, устанавливается каждый раз в начале
                Trade trade = new();
                #endregion

                #region Создание массива цен закрытий для расчёта индикаторов
                if (i > period)
                {
                    List<decimal> candles_close = [];
                    for (int j = i - period; j < i - 1; j++)
                    {
                        candles_close.Add(candles[j].Close);
                    }
                    #endregion

                    #region Расчёт индикаторов 
                    // Расчёт индикаторов

                    decimal simple_Average_Indicator = Simple_Average_Indicator(candles_close); //Расчёт средней

                    decimal standard_Deviation_Indicator = Standard_Deviation_Indicator(candles_close); // Расчёт ст отклонения



                    decimal PriceLong = simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations; //средняя - ст отклонение

                    decimal PriceAverage = simple_Average_Indicator;//средняя

                    decimal PriceShort = simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations;//средняя + ст отклонение

                    #endregion

                    #region Перебор условий


                    if
                        (
                        candles[i].High >= PriceShort
                        &&
                        Contracts == 0m
                        //     &&
                        //  Depo > InitialMargin
                        )

                    { flag = Flag.FlagType.ShortOpen; }

                    if
                     (
                     candles[i].Low <= PriceLong
                     &&
                     Contracts == 0m
                     //  &&

                     //  Depo > InitialMargin
                     )

                    { flag = Flag.FlagType.LongOpen; }


                    if
                            (
                            candles[i].High >= PriceAverage
                            &&
                            Contracts > 0m
                            )

                    { flag = Flag.FlagType.LongCloseAtAverage; }

                    if
                         (
                         candles[i].Low <= PriceAverage
                         &&
                         Contracts < 0m
                         )

                    { flag = Flag.FlagType.ShortCloseAtAverage; }
                    #endregion

                    #region Учёт сделок и открытие позиций
                    switch (flag)

                    {

                        case Flag.FlagType.ShortOpen:
                            Contracts = (-1m);
                     //       Depo = Depo - Contracts * PriceShort;
                            //     decimal c = -Contracts;



                            trade = new
                               (i, // Номер свечи
                               param.SecurityCode, // Код инструмента
                               candles[i].DateTime, // Дата и время
                                                    //   TradeDirection.Direction.Short,// Направление сделки
                               PriceShort,  //Цена
                               Contracts,
                               PriceShort * (-1) * Contracts//Объём
                               );
                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.ShortCloseAtAverage:

                      //      Depo = Depo + Contracts * PriceAverage;

                            //    Contracts = -Depo / PriceShort; //При открытии лонга - вычисление объёма котрактов в позиции

                            trade = new
                               (i, // Номер свечи
                               param.SecurityCode, // Код инструмента
                               candles[i].DateTime, // Дата и время
                                                    //   TradeDirection.Direction.Short,// Направление сделки
                               PriceAverage,  //Цена
                         -Contracts,
                               PriceAverage * (-1) * Contracts//Объём
                               );

                            Trades.Add(trade);

                            Contracts = 0m;
                            break;

                        case Flag.FlagType.LongOpen:

                            //   decimal c1= -Contracts;
                            Contracts = 1m;
                 //           Depo = Depo - Contracts * PriceLong;

                            trade = new
                                 (i, // Номер свечи
                                 param.SecurityCode, // Код инструмента
                                 candles[i].DateTime, // Дата и время
                                                      //         TradeDirection.Direction.Long,// Направление сделки
                               PriceLong,  //Цена
                              Contracts,
                               PriceLong * (-1) * Contracts //Объём
                                 );

                            Trades.Add(trade);

                            break;

                        case Flag.FlagType.LongCloseAtAverage:
                       //     Depo = Depo + Contracts * PriceAverage;



                            trade = new
                                 (i, // Номер свечи
                                 param.SecurityCode, // Код инструмента
                                 candles[i].DateTime, // Дата и время
                                                      //         TradeDirection.Direction.Long,// Направление сделки
                               PriceAverage,  //Цена
                            -Contracts,
                               PriceAverage * Contracts //Объём
                                 );

                            Trades.Add(trade);
                            Contracts = 0m;
                            break;

                        case Flag.FlagType.Keep:
                            trade = new
                        (i, // Номер свечи
                        param.SecurityCode, // Код инструмента
                        candles[i].DateTime, // Дата и время
                                             //              TradeDirection.Direction.Keep,// Направление сделки
                     0m,  //Цена
                        0m,
                        0m  //Объём
                        );

                            Trades.Add(trade);
                            //Зануление количества контрактов

                            //Позиции неизменны
                            break;

                    }

                    #endregion

                    #region Запись индикаторов в массив
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    Sma.Add(Convert.ToDouble(simple_Average_Indicator));
                    Minimums.Add(Convert.ToDouble(simple_Average_Indicator - standard_Deviation_Indicator * param.NumberOfStandardDeviations));
                    Maximums.Add(Convert.ToDouble(simple_Average_Indicator + standard_Deviation_Indicator * param.NumberOfStandardDeviations));



                }

                if (i <= period)
                {
                    Axis_X_Price.Add(candles[i].DateTime.ToOADate());
                    trade = new
          (i, // Номер свечи
          param.SecurityCode, // Код инструмента
          candles[i].DateTime, // Дата и время
                               //   TradeDirection.Direction.Keep,// Направление сделки
   0m,  //Цена
  0m, 0m //Объём
          );

                    Trades.Add(trade);
                    Sma.Add(0);
                    Minimums.Add(0);
                    Maximums.Add(0);
                }
            }
            #endregion

            //

            List<double> Deposit = CalculateDeposit(param.InitialDeposit, Trades, candles);
            //    Deposit.Add(Deposit1[Deposit.Count]);
            //
            PlotData plotdata = new()
            {
                Dates = Axis_X_Price,
                Candles = Convert_to_ScottPlottCandles(candles)
            };
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Maximums), Maximums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Minimums), Minimums));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Deposit), Deposit));
            plotdata.plotIndicators.Add(AddPlotcontrol(nameof(Sma), Sma));


            return plotdata;

        }
        #endregion

        /*  
        #region - времено не испольхуются
        public static PlotData? Spread_Volatility_LongToAverage_Tester(Spread_Volatillity_LongToAverage_Param sentiment_Param)
        {
            Method method = new();
            string methodName = sentiment_Param.Algo;
            Type type = method.GetType();
            MethodInfo? method1 = type.GetMethod(methodName);
            object? result = method1?.Invoke(obj: method, parameters: [sentiment_Param]);
            PlotData? io = result as PlotData;
            return io;
        }

        public static PlotData? Spread_Volatility_ShortToAverage_Tester(Spread_Volatillity_ShortToAverage_Param sentiment_Param)
        {
            Method method = new();
            string methodName = sentiment_Param.Algo;
            Type type = method.GetType();
            MethodInfo? method1 = type.GetMethod(methodName);
            object? result = method1?.Invoke(obj: method, parameters: [sentiment_Param]);
            PlotData? io = result as PlotData;
            return io;
        }

        #endregion

        */
    }
}









