
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model.DataFormat;

namespace Test.Model.Tester.Spread_Volatility
    {
        public class CalculateIdealSpread_Param
        {
            public string FileName = "MIX-9.26-12.26 [Price]D.txt"; //имя файла
        public string RusFarFilename = "RUSFAR3M [Price]1D.txt"; //имя файла
        public string ImoexFilename = "IMOEX [Price]1D.txt"; //имя файла

        public decimal InitialDeposit { get; set; } = 15000; //начальный депозит в деньгах

            public decimal InitialMargin { get; set; } = 11000; //начальный депозит в деньгах
            public int Period { get; set; } = 200; //период осреднения средней линии
            public decimal InitialContracts { get; set; } = 0; //начальный объйм контрактов, логично что он нулевой, но мало ли
            public string SecurityCode { get; set; } = "Si-6.26-9.26"; //Код инструмента

            public decimal TradeDicetion = 1; //Направление торговли, лонг 1

            public decimal NumberOfStandardDeviations = 1;//оличество стандартных отклонений в канале
            public string Algo = "Spread_Volatillity_LongToAverage_Algo";
            static decimal fixedPartl = 0; //фиксированная часть комиссии
            static decimal percentPart = 1 / 10000; //процентная часть комиссии
            public Exchange_Comission Exchange_Comission = new(fixedPartl, percentPart);
        }
    }

