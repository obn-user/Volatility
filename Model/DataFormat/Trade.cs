namespace Test.Model.DataFormat
{
    public class Trade
    {
        public int Id = 0;
        public string Ticker = "";
        public DateTime DateTime = new();
    //    public TradeDirection.Direction Direction = TradeDirection.Direction.Keep;
        public decimal Price = 0;
        public decimal Volume = 0;
        public decimal Total = 0;

        public Trade()
        {
        }

        public Trade
            (
         int id,
         string ticker,
         DateTime dateTime,
      //   TradeDirection.Direction direction,
         decimal price,
         decimal volume,
         decimal total
            )
        {
            Id = id;
          Ticker = ticker;
            DateTime = dateTime;
       //     Direction = direction;
            Price = price;
            Volume = volume;
             Total=total;
        }

    }
}
