namespace Test.Model.Indicators
{
    public class Simple_Average_Indicator
    {
        public decimal Simple_Average = new();

        public Simple_Average_Indicator(List<decimal> data)
        {
            decimal simple_Average = 0;

            for (int i = 0; i < data.Count; i++)
            {
                simple_Average = simple_Average + data[i];
            }

            simple_Average = simple_Average / data.Count;

            Simple_Average = simple_Average;
        }
    }
}
