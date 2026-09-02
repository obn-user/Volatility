using Test.Model.DataFormat;
using Test.Model.Indicators;
using Test.Model.Parametres;

namespace Test.Model
{

    public class EmaStandardDeviationAlgo

    {
        #region Fields
        // Результат работы класса Algo 
        public Position Position = new(); // Позиция - количество контрактов и денег
        public List<Trade> Trades = new List<Trade>(); // Список сделок
        #endregion

        public EmaStandardDeviationAlgo(EmaStandardDeviationParam param, List<Candle> data)
        {
         
           // int i = (int)direction;
            decimal Contracts = param.InitialContracts;
            decimal Depo = param.InitialDeposit;

            #region EMA and STDEV calculation
            List<decimal> _data = new();
            for (int i=0; i< data.Count; i++)
            {
                _data.Add(data[i].Close);
            }

            EmaIndicator emaIndicator = new(_data);
            List<decimal> Stdevs = new List<decimal>();
            List<decimal> Emas = new List<decimal>();
            decimal initialEma = 0; // первый член последовательности ЕМА - прстое среднее
            for (int i = 0; i < param.EmaPeriod; i++)
            {
                initialEma += data[i].Close;
            }
            initialEma = initialEma / param.EmaPeriod;
            for (int i = 0; i < data.Count; i++)
            {

                decimal ema = initialEma;
                if (i < param.EmaPeriod | i == param.EmaPeriod) // первые (период ЕМА )члены массива ЕМА и ст откл 
                                                                //невозможно рассчитать , поэтому вместо них нули
                {
                    Emas.Add(Convert.ToDecimal(0));
                    Stdevs.Add(Convert.ToDecimal(0));
                }

                if (i > param.EmaPeriod)
                {


                    decimal stdev = 0;
                    decimal a = 2;
                    decimal b = param.EmaPeriod + 1;
                    decimal N = a / b;

                    for (int k = i - param.EmaPeriod; k < i; k++)
                    {
                        ema = data[k].Close * N + (ema * (1 - N));
                    }

                    Emas.Add(ema); // локальный массив ЕМА

                    decimal simpleAverage = 0;

                    for (int k = i - param.EmaPeriod; k < i; k++)
                    {
                        simpleAverage += (data[k].Low + data[k].High) / 2;
                    }
                    simpleAverage = simpleAverage / param.EmaPeriod;

                    for (int j = i - param.EmaPeriod; j < i; j++)
                    {

                        stdev = stdev + ((simpleAverage - data[j].Low) * (simpleAverage - data[j].Low)+
                            (simpleAverage - data[j].High) * (simpleAverage - data[j].High))/2;
                    }
                    stdev = Convert.ToDecimal((Math.Sqrt((Convert.ToDouble(stdev))) / param.EmaPeriod));
                    Stdevs.Add(stdev); // локлаьный массив ст откл
                }
            }
            #endregion

            #region Tester
            for (int i = param.EmaPeriod+2; i < data.Count; i++)
            {


                if ((Emas[i - 1] - Stdevs[i - 1] > data[i].Low))
                {
                    if (Depo > 0)
                    {
                        Contracts = Depo / (Emas[i - 1] - Stdevs[i - 1]);
                        Depo = 0;
                        Trade trade = new
                             
                             (
                               i,
                               param.SecCode,
                               data[i].DateTime,
                         //      TradeDirection.Direction.Long,
                               Emas[i - 1] - Stdevs[i - 1],
                               Contracts,
                                  (   Emas[i - 1] - Stdevs[i - 1])* Contracts
                               );

                     

                        Trades.Add(trade);

                        Position.Contracts = Contracts;
                        Position.Depo = Depo;
                    }

                }

                if ((Emas[i - 1] + Stdevs[i - 1] < data[i].High))
                {
                    if (Contracts > 0)
                    {
                        Depo = (Emas[i - 1] + Stdevs[i - 1]) * Contracts;

                        Trade trade = new

                             (
                               i,
                               param.SecCode,
                               data[i].DateTime,
                           //    TradeDirection.Direction.Short,
                               Emas[i - 1] + Stdevs[i - 1],
                               Contracts,
                                         (     Emas[i - 1] + Stdevs[i - 1])* Contracts
                               );

                        Trades.Add(trade);

                        Position.Contracts = Contracts;
                        Position.Depo = Depo;

                    }

                }
                #endregion



            }
        }

    }
}
