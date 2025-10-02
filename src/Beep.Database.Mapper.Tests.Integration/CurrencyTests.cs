using Beep.Database.Mapper.Tests.Integration.Dtos;
using System.Data;

namespace Beep.Database.Mapper.Tests.Integration
{
	[TestClass]
	public class CurrencyTests : SetupTests
	{
		[TestMethod]
		public void Insert_NoTransaction()
		{
			var param = new CurrencyDto
			{
				CodeISO = "VND",
				Name = "Vietnam Dong",
				DecimalDigits = 0,
				Symbol = "VND",
				Synchronized = false
			};

			Insert(param, null);
		}

		private void Insert(CurrencyDto currency, IDbTransaction? transaction)
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

			var result = Insert<int>(sql, currency, transaction);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);

			currency.Id = result.Result;
		}

		[TestMethod]
		public void Get_HasParameter_NoTransaction()
		{
			var currency = new CurrencyDto
			{
				CodeISO = "VND",
				Name = "Vietnam Dong",
				DecimalDigits = 0,
				Symbol = "VND",
				Synchronized = false
			};

			//add default currency
			Insert(currency, null);

			const string sql = """
	                           SELECT 
	                               Id as Id,
	                               CodeISO as CodeISO,
	                               Name as Name,
	                               DecimalDigits as DecimalDigits,
	                               Symbol as Symbol,
	                               Synchronized as Synchronized
	                           FROM
	                               dbo.Currency 
	                           WHERE CodeISO = @CodeISO
	                           """;

			var parameters = new { CodeISO = "VND" };
			var result = Get<CurrencyDto>(sql, parameters, null);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, currency.Id);
			Assert.AreEqual(result.Result.CodeISO, currency.CodeISO);
			Assert.AreEqual(result.Result.Name, currency.Name);
			Assert.AreEqual(result.Result.DecimalDigits, currency.DecimalDigits);
			Assert.AreEqual(result.Result.Symbol, currency.Symbol);
			Assert.AreEqual(result.Result.Synchronized, currency.Synchronized);
		}

		[TestMethod]
		public void List_NoParameter_NoTransaction()
		{
			var currencyVND = new CurrencyDto
			{
				CodeISO = "VND",
				Name = "Vietnam Dong",
				DecimalDigits = 0,
				Symbol = "VND",
				Synchronized = false
			};

			//add default currency VND
			Insert(currencyVND, null);

			var currencyUSD = new CurrencyDto
			{
				CodeISO = "USD",
				Name = "US Dollar",
				DecimalDigits = 2,
				Symbol = "$",
				Synchronized = false
			};

			//add default currency USD
			Insert(currencyUSD, null);

			const string sql = """
	                           SELECT 
	                               Id as Id,
	                               CodeISO as CodeISO,
	                               Name as Name,
	                               DecimalDigits as DecimalDigits,
	                               Symbol as Symbol,
	                               Synchronized as Synchronized
	                           FROM
	                               dbo.Currency 
	                           """;

			var result = List<CurrencyDto>(sql, null, null);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);

			var resultList = result.Result.ToList();
			Assert.IsNotNull(resultList);
			Assert.AreEqual(resultList.Count, 2);

			Assert.AreEqual(resultList[0].Id, currencyVND.Id);
			Assert.AreEqual(resultList[0].CodeISO, currencyVND.CodeISO);
			Assert.AreEqual(resultList[0].Name, currencyVND.Name);
			Assert.AreEqual(resultList[0].DecimalDigits, currencyVND.DecimalDigits);
			Assert.AreEqual(resultList[0].Symbol, currencyVND.Symbol);
			Assert.AreEqual(resultList[0].Synchronized, currencyVND.Synchronized);

			Assert.AreEqual(resultList[1].Id, currencyUSD.Id);
			Assert.AreEqual(resultList[1].CodeISO, currencyUSD.CodeISO);
			Assert.AreEqual(resultList[1].Name, currencyUSD.Name);
			Assert.AreEqual(resultList[1].DecimalDigits, currencyUSD.DecimalDigits);
			Assert.AreEqual(resultList[1].Symbol, currencyUSD.Symbol);
			Assert.AreEqual(resultList[1].Synchronized, currencyUSD.Synchronized);
		}

		[TestMethod]
		public void List_HasParameter_NoTransaction()
		{
			var currencyVND = new CurrencyDto
			{
				CodeISO = "VND",
				Name = "Vietnam Dong",
				DecimalDigits = 0,
				Symbol = "VND",
				Synchronized = false
			};

			//add default currency VND
			Insert(currencyVND, null);

			var currencyUSD = new CurrencyDto
			{
				CodeISO = "USD",
				Name = "US Dollar",
				DecimalDigits = 2,
				Symbol = "$",
				Synchronized = false
			};

			//add default currency USD
			Insert(currencyUSD, null);

			var currencyEUR = new CurrencyDto
			{
				CodeISO = "EUR",
				Name = "Euro",
				DecimalDigits = 2,
				Symbol = "€",
				Synchronized = false
			};

			//add default currency EUR
			Insert(currencyEUR, null);

			const string sql = """
	                           SELECT 
	                               Id as Id,
	                               CodeISO as CodeISO,
	                               Name as Name,
	                               DecimalDigits as DecimalDigits,
	                               Symbol as Symbol,
	                               Synchronized as Synchronized
	                           FROM
	                               dbo.Currency 
	                           WHERE DecimalDigits = @DecimalDigits
	                           """;

			var parameters = new { DecimalDigits = 2 };
			var result = List<CurrencyDto>(sql, parameters, null);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);

			var resultList = result.Result.ToList();
			Assert.IsNotNull(resultList);
			Assert.AreEqual(resultList.Count, 2);

			Assert.AreEqual(resultList[0].Id, currencyUSD.Id);
			Assert.AreEqual(resultList[0].CodeISO, currencyUSD.CodeISO);
			Assert.AreEqual(resultList[0].Name, currencyUSD.Name);
			Assert.AreEqual(resultList[0].DecimalDigits, currencyUSD.DecimalDigits);
			Assert.AreEqual(resultList[0].Symbol, currencyUSD.Symbol);
			Assert.AreEqual(resultList[0].Synchronized, currencyUSD.Synchronized);

			Assert.AreEqual(resultList[1].Id, currencyEUR.Id);
			Assert.AreEqual(resultList[1].CodeISO, currencyEUR.CodeISO);
			Assert.AreEqual(resultList[1].Name, currencyEUR.Name);
			Assert.AreEqual(resultList[1].DecimalDigits, currencyEUR.DecimalDigits);
			Assert.AreEqual(resultList[1].Symbol, currencyEUR.Symbol);
			Assert.AreEqual(resultList[1].Synchronized, currencyEUR.Synchronized);
		}
	}
}
