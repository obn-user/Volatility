namespace Test.Model.DataFormat
{ // Класс - тип данных ==== Свечи с графика Квик
    public class CSVinput
    {

       public string Ticker { get; set; } = "";

        public string Period { get; set; } = "";

        public string Date { get; set; } = "";

        public string Time { get; set; } = "";

        public string Open { get; set; } = "";

        public string High { get; set; } = "";

        public string Low { get; set; } = "";

        public string Close { get; set; } = "";

        public string Volume { get; set; } = "";
    }
}
