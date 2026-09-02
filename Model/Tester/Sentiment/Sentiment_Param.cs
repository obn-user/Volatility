using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model.DataFormat;

namespace Test.Model.Tester.Sentiment
{
    public class Sentiment_Param
    {
        public string FileName = "MCFTR_20042026.txt"; //имя файла
        public string FileName_indicator = "RGBITR_20042026.txt";
        public decimal InitialDeposit { get; set; } = 1000; //начальный депозит в деньгах

        public int NumberOfLevel { get; set; } = 2;
        public decimal InitialContracts { get; set;} = 0; //начальный объйм контрактов, логично что он нулевой, но мало ли
        public string SecurityCode { get; set; } = "MCFTR"; //Код инструмента

        public decimal TradeDicetion = 1; //Направление торговли, лонг 1

        public decimal CentralPoint = 10;
        public decimal Max = 13;
        public decimal Min = 7; 


        static decimal fixedPartl = 0; //фиксированная часть комиссии
        static decimal percentPart = 1 / 10000; //процентная часть комиссии
        public Exchange_Comission Exchange_Comission = new(fixedPartl, percentPart);

        public string Algo = "Sentiment_Algo";
    }
}
