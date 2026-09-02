/* using Test.DataFormat;
using Test.Model.Parametres;

namespace Test.Model
{

    public class Volatility : VM

    {
        #region Fields
        // Результат работы класса 

        private List<Trade> _trades = new();

        public List<Trade> Trades

        {

            get { return _trades; }

            set { _trades = value; OnPropertyChanged(nameof(Trades)); }
        }

        // Список сделок

        #endregion

        public Volatility(VolatilityParam param, List<Candle> data) // приеём ыходящих данных в расчёт
        {

            decimal Contracts = param.InitialContracts; // изначальный баланс контракты
            decimal Depo = param.InitialDeposit; // изначальный баланс денежные средства


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

                }

                if (i > param.EmaPeriod)
                {


                    decimal a = 2;
                    decimal b = param.EmaPeriod + 1;
                    decimal N = a / b;

                    for (int k = i - param.EmaPeriod; k < i; k++)
                    {
                        ema = data[k].Close * N + (ema * (1 - N));
                    }

                    Emas.Add(ema); // локальный массив ЕМА
                }


            }


            List<decimal> Vol = new List<decimal>(); // Массив стандартных отклонений


            for (int i = 0; i < data.Count; i++)
            {

                decimal average = (data[i].High + data[i].Low + data[i].Open + data[i].Close) / 4;
                decimal vol =
                    (data[i].High - average) * (data[i].High - average) +
                    (data[i].Low - average) * (data[i].Low - average) +
                    (data[i].Open - average) * (data[i].Open - average) +
                    (data[i].Close - average) * (data[i].Close - average);
                vol = Convert.ToDecimal((Math.Sqrt((Convert.ToDouble(vol))) / 4));

                decimal k = 1 / 2;
                vol = vol * k;

                Vol.Add(vol);


            }


            int flag = 0;


            for (int i = 1; i < data.Count; i++) // расчёт финансового результата по свечам начиная со второй
            {
                flag = 0;

                if (data[i].Low < data[i].Open - Vol[i - 1]) // если цена с открытия опустилась на одно стандартное отклонение
                {


                    if (Depo > 0) // и на депозите есть средства
                    {

                        Contracts = Depo / (data[i].Open - Vol[i - 1]); // то открывается позиция
                        Depo = 0;
                        Trade trade = new()
                        {
                            Direction = 1,
                            Ticker = param.SecCode,
                            id = i,
                            Price = data[i].Open - Vol[i - 1],
                            Volume = Contracts
                        };

                        flag = 0;

                        Trades.Add(trade);
                    }

                }

                if (data[i].High > data[i].Open + Vol[i - 1]) // если с открытия цена выросла выше стандартного отклонения
                {
                    if (Contracts > 0)// и есть на счёте контракты
                    {
                        if (flag == 0)
                        {
                            Depo = (data[i].Open + Vol[i - 1]) * Contracts - (data[i].Open + Vol[i - 1]) * Contracts / 10000; // то позиция закрывается

                            Trade trade = new();
                            trade.Volume = Contracts;
                            Contracts = 0;
                            trade.Direction = -1;
                            trade.Ticker = param.SecCode;
                            trade.id = i;
                            trade.Price = data[i].Open + Vol[i - 1];

                            Trades.Add(trade);

                        }

                    }

                }




            }
        }

        //   public event PropertyChangedEventHandler PropertyChanged;
        // public void OnPropertyChanged([CallerMemberName] string prop = "")
        //{
        //  if (PropertyChanged != null)
        //    PropertyChanged(this, new PropertyChangedEventArgs(prop));
        //}

    }
}*/