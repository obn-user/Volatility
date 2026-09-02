using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model.DataFormat;

namespace Test.Model.Tester.Spread_Volatility
{
    public class Spread_Volatillity_BiDirection_ToAverage_Param
    {

        public string FileName = "SIU6_history.csv"; //имя файла
        public decimal InitialDeposit { get; set; } = 11000; //начальный депозит в деньгах

        public decimal InitialMargin { get; set; } = 11000; //начальный депозит в деньгах
        public int Period { get; set; } = 300; //период осреднения средней линии
        public decimal InitialContracts { get; set; } = 0; //начальный объйм контрактов, логично что он нулевой, но мало ли
        public string SecurityCode { get; set; } = "Si-6.26-9.26"; //Код инструмента

        public decimal TradeDicetion = -1; //Направление торговли, лонг 1

        public decimal NumberOfStandardDeviations = 1;//оличество стандартных отклонений в канале
        public string Algo = "Spread_Volatillity_BiDirection_ToAverage_Algo";
        static decimal fixedPartl = 0; //фиксированная часть комиссии
        static decimal percentPart = 1 / 10000; //процентная часть комиссии
        public Exchange_Comission Exchange_Comission = new(fixedPartl, percentPart);

    }
}