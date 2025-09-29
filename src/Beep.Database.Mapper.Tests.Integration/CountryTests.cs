using Beep.Database.Mapper.Tests.Integration.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beep.Database.Mapper.Tests.Integration
{
    public class CountryTests : SetupTests
    {
        [TestMethod]
        public void Insert()
        {
            const string sql = """
                INSERT INTO dbo.Country
                      (Currency_Id
                      ,Code
                      ,Name
                      ,PhoneCode
                      ,IsCountry
                      ,EUEntryDate
                      ,VatNumberMask
                      ,Country281
                      ,IsSepa
                      ,Synchronized)
                VALUES
                      (@CurrencyId
                      ,@Code
                      ,@Name
                      ,@PhoneCode
                      ,@IsCountry
                      ,@EUEntryDate
                      ,@VatNumberMask
                      ,@Country281
                      ,@IsSepa
                      ,@Synchronized)
                
                SELECT SCOPE_IDENTITY();
                """;

            var param = new
            {
                CurrencyId = (int?)null, // Assuming null for Currency_Id
                Code = "VN",
                Name = "Vietnam",
                PhoneCode = "+84",
                IsCountry = true,
                EUEntryDate = DateTime.Now,
                VatNumberMask = "...",
                Country281 = "217",
                IsSepa = false,
                Synchronized = false
            };

            var result = Insert<CountryDto>(sql, param, null);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            
        }
    }
}
