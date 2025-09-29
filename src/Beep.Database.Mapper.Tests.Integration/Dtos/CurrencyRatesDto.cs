namespace Beep.Database.Mapper.Tests.Integration.Dtos
{
    public record CurrencyRatesDto(int CurrencyId,
       DateTime Date,
       decimal BuyingRateCash,
       decimal BuyingRateTransfer,
       decimal SellingRate);
}
