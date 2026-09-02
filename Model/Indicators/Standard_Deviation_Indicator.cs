namespace Test.Model.Indicators
{
    public class Standard_Deviation_Indicator
    {
        public decimal StandardDeviation { get; set; }
        public Standard_Deviation_Indicator(List<decimal> data)
        {
            decimal standardDeviation = 0;
            decimal average = 0;

            for (int i = 0; i < data.Count; i++)
            {
                average = average + data[i];
            }

            average = average / data.Count;

            for (int i = 0; i < data.Count; i++)
            {
                standardDeviation = (average - data[i]) * (average - data[i]) + standardDeviation;
            }

            standardDeviation = Convert.ToDecimal((Math.Sqrt((Convert.ToDouble(standardDeviation))) / data.Count));

            StandardDeviation = standardDeviation;
        }
    }
}
