namespace Beep.Database.Mapper.Tests.Integration.Dtos
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string CodeISO { get; set; }
        public int DecimalDigits { get; set; }
        public string Symbol { get; set; }
        public bool Synchronized { get; set; }
    }
}
