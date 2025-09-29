using Beep.Database.Mapper.Tests.Integration.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beep.Database.Mapper.Tests.Integration
{
    [TestClass]
    public class CurrencyTests : SetupTests
    {
        [TestMethod]
        public void Insert()
        {
            const string sql = """
                INSERT INTO dbo.Currency
                      (CodeISO
                      ,Name
                      ,DecimalDigits
                      ,Symbol
                      ,Synchronized)
                VALUES
                      (@CodeISO
                      ,@Name
                      ,@DecimalDigits
                      ,@Symbol
                      ,@Synchronized)
                
                SELECT 
                    Id,
                    CodeISO,
                    Name,
                    DecimalDigits,
                    Symbol,
                    Synchronized
                FROM dbo.Currency
                WHERE Id = SCOPE_IDENTITY()
                """;

            var param = new
            {
                CodeISO = "VND",
                Name = "Vietnam Dong",
                DecimalDigits = 0,
                Symbol = "VND",
                Synchronized = false
            };

            var result = Insert<CurrencyDto>(sql, param, null);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
        }
    }
}
