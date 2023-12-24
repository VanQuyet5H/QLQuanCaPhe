namespace ManageCoffee.ViewModels
{
    public class StatisticsData
    {
        public int OrderCount { get; set; }
        public double Revenue { get; set; }
        public int CoffeeCount { get; set; }
        public int CustomerCount { get; set; }
        public List<StatisticsData> Data { get; set; }
    }
    public class Tg
    {
        public DateTime TuNgay { get; set; }
		public DateTime DenNgay { get; set; }
	}
}
