namespace Test.Model.Param
{
    public class Param
    {
        public Dictionary<string, string> Params = [];
        public Param()
        {
            var _params = new Dictionary<string, string>()
{
    { "3", "Tom"},
    { "3", "Sam"},
    { "3", "Bob"}
};
            Params = _params;
        }



        /*


        public string FileName = "Si626926_Price.txt";

        public decimal InitialDeposit { get; set; } = 1000000;

        public decimal InitialContracts { get; set; } = 0;

        public string SecurityCode { get; set; } = "Si-6.26-9.26";

        public decimal TradeDiretion = 1;

        public decimal NumberOfStandardDeviation = 1;

        static decimal fixedPartl = 0;
        static decimal percentPart = 1 / 10000;
        public Exchange_Comission Exchange_Comission = new(fixedPartl, percentPart);
        */
    }

}


