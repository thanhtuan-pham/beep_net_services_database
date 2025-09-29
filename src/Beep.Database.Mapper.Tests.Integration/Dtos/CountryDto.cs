namespace Beep.Database.Mapper.Tests.Integration.Dtos
{
    public class CountryDto
    {
        public int Id { get; set; }
        public int Currency_Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneCode { get; set; }
        public bool IsCountry { get; set; }
        public DateTime EUEntryDate { get; set; }
        public string VatNumberMask { get; set; }
        public string Country281 { get; set; }
        public bool IsSepa { get; set; }
        public bool Synchronized { get; set; }
    }
}
