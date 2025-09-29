using Beep.Database.Mapper.Tests.Unit.Dtos;
using Moq;
using System.Data;

namespace Beep.Database.Mapper.Tests.Unit
{
    [TestClass]
    public sealed class SqlMapperTests : SetupTests
    {
		#region ** QueryAsync **

		[TestMethod]
        public void QueryAsync_NoParameter_NoTransaction()
        {
            const string sql = """
                SELECT 
                    Id as Id,
                    Code as Code
                FROM
                    SqlMapperTable
                """;

            var list = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "Id", 130 },
                    { "Code", "VN" }
                },
                new Dictionary<string, object>
                {
                    { "Id", 131 },
                    { "Code", "BE" }
                },
                new Dictionary<string, object>
                {
                    { "Id", 132 },
                    { "Code", "US" }
                }
            };

            var result = QueryAsync<SqlMapperDto>(sql, list, null, null) as Task<IEnumerable<SqlMapperDto>>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
           
            var resultList = result.Result.ToList();
            Assert.IsNotNull(resultList);
            Assert.AreEqual(resultList.Count, 3);
            Assert.AreEqual(resultList[0].Id, 130);
            Assert.AreEqual(resultList[0].Code, "VN");
            Assert.AreEqual(resultList[1].Id, 131);
            Assert.AreEqual(resultList[1].Code, "BE");
            Assert.AreEqual(resultList[2].Id, 132);
            Assert.AreEqual(resultList[2].Code, "US");
        }

        [TestMethod]
		public void QueryAsync_HasParameter_NoTransaction() 
        {
	        const string sql = """
	                           SELECT 
	                               Id as Id,
	                               Code as Code
	                           FROM
	                               SqlMapperTable
	                           WHERE Code = @Code
	                           """;

	        var list = new List<IDictionary<string, object>>
	        {
		        new Dictionary<string, object>
		        {
			        { "Id", 130 },
			        { "Code", "VN" }
		        },
		        new Dictionary<string, object>
		        {
			        { "Id", 131 },
			        { "Code", "BE" }
		        },
		        new Dictionary<string, object>
		        {
			        { "Id", 132 },
			        { "Code", "US" }
		        }
	        };

	        var parameters = new { Code = "VN" };

			var result = QueryAsync<SqlMapperDto>(sql, list, parameters, null) as Task<IEnumerable<SqlMapperDto>>;

	        Assert.IsNotNull(result);
	        Assert.IsNotNull(result.Result);

	        var resultList = result.Result.ToList();
	        Assert.IsNotNull(resultList);
	        Assert.AreEqual(resultList.Count, 3);
	        Assert.AreEqual(resultList[0].Id, 130);
	        Assert.AreEqual(resultList[0].Code, "VN");
	        Assert.AreEqual(resultList[1].Id, 131);
	        Assert.AreEqual(resultList[1].Code, "BE");
	        Assert.AreEqual(resultList[2].Id, 132);
	        Assert.AreEqual(resultList[2].Code, "US");
		}

		[TestMethod]
		public void QueryAsync_NoParameter_HasTransaction()
        {
	        const string sql = """
	                           SELECT 
	                               Id as Id,
	                               Code as Code
	                           FROM
	                               SqlMapperTable
	                           """;

	        var list = new List<IDictionary<string, object>>
	        {
		        new Dictionary<string, object>
		        {
			        { "Id", 130 },
			        { "Code", "VN" }
		        },
		        new Dictionary<string, object>
		        {
			        { "Id", 131 },
			        { "Code", "BE" }
		        },
		        new Dictionary<string, object>
		        {
			        { "Id", 132 },
			        { "Code", "US" }
		        }
	        };

	        var transaction = new Mock<IDbTransaction>();
			var result = QueryAsync<SqlMapperDto>(sql, list, null, transaction.Object) as Task<IEnumerable<SqlMapperDto>>;

	        Assert.IsNotNull(result);
	        Assert.IsNotNull(result.Result);

	        var resultList = result.Result.ToList();
	        Assert.IsNotNull(resultList);
	        Assert.AreEqual(resultList.Count, 3);
	        Assert.AreEqual(resultList[0].Id, 130);
	        Assert.AreEqual(resultList[0].Code, "VN");
	        Assert.AreEqual(resultList[1].Id, 131);
	        Assert.AreEqual(resultList[1].Code, "BE");
	        Assert.AreEqual(resultList[2].Id, 132);
	        Assert.AreEqual(resultList[2].Code, "US");

	        transaction.Object.Commit();
		}

		[TestMethod]
		public void QueryAsync_HasParameter_HasTransaction()
        {
	        const string sql = """
	                           SELECT 
	                               Id as Id,
	                               Code as Code
	                           FROM
	                               SqlMapperTable
	                           WHERE Code = @Code
	                           """;

	        var list = new List<IDictionary<string, object>>
	        {
		        new Dictionary<string, object>
		        {
			        { "Id", 130 },
			        { "Code", "VN" }
		        },
		        new Dictionary<string, object>
		        {
			        { "Id", 131 },
			        { "Code", "BE" }
		        },
		        new Dictionary<string, object>
		        {
			        { "Id", 132 },
			        { "Code", "US" }
		        }
	        };

	        var parameters = new { Code = "VN" };
	        var transaction = new Mock<IDbTransaction>();
			var result = QueryAsync<SqlMapperDto>(sql, list, parameters, transaction.Object) as Task<IEnumerable<SqlMapperDto>>;

	        Assert.IsNotNull(result);
	        Assert.IsNotNull(result.Result);

	        var resultList = result.Result.ToList();
	        Assert.IsNotNull(resultList);
	        Assert.AreEqual(resultList.Count, 3);
	        Assert.AreEqual(resultList[0].Id, 130);
	        Assert.AreEqual(resultList[0].Code, "VN");
	        Assert.AreEqual(resultList[1].Id, 131);
	        Assert.AreEqual(resultList[1].Code, "BE");
	        Assert.AreEqual(resultList[2].Id, 132);
	        Assert.AreEqual(resultList[2].Code, "US");

	        transaction.Object.Commit();
		}

		private Task QueryAsync<T>(string sql, IList<IDictionary<string, object>> dataRows, object? parameters, IDbTransaction? transaction)
        {
	        var dbCommand = SetUp_QueryAsync(sql, dataRows, parameters);
	        var result = QueryAsync<T>(sql, parameters, transaction);

	        Assert.IsNotNull(dbCommand);
	        Assert.AreEqual(dbCommand.CommandText, sql);

	        return result;
        }

		#endregion ** QueryAsync **

		#region ** QuerySingleOrDefaultAsync **

		[TestMethod]
        public void QuerySingleOrDefaultAsync_NoParameter_NoTransaction()
        {
            const string sql = """
                SELECT 
                    Id as Id,
                    Code as Code
                FROM
                    SqlMapperTable 
                """;

            var dataRow = new Dictionary<string, object>
            {
                { "Id", 130 },
                { "Code", "VN" }
            };

            var result = QuerySingleOrDefaultAsync<SqlMapperDto>(sql, dataRow, null, null) as Task<SqlMapperDto>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Id, 130);
            Assert.AreEqual(result.Result.Code, "VN");
        }

        [TestMethod]
		public void QuerySingleOrDefaultAsync_HasParameter_NoTransaction()
        {
	        const string sql = """
	                           SELECT 
	                               Id as Id,
	                               Code as Code
	                           FROM
	                               SqlMapperTable 
	                           WHERE Code = @Code
	                           """;

	        var dataRow = new Dictionary<string, object>
	        {
		        { "Id", 130 },
		        { "Code", "VN" }
	        };
	        var parameters = new { Code = "VN" };

			var result = QuerySingleOrDefaultAsync<SqlMapperDto>(sql, dataRow, parameters, null) as Task<SqlMapperDto>;

	        Assert.IsNotNull(result);
	        Assert.IsNotNull(result.Result);
	        Assert.AreEqual(result.Result.Id, 130);
	        Assert.AreEqual(result.Result.Code, "VN");
		}

        [TestMethod]
		public void QuerySingleOrDefaultAsync_NoParameter_HasTransaction()
		{
			const string sql = """
			                   SELECT 
			                       Id as Id,
			                       Code as Code
			                   FROM
			                       SqlMapperTable 
			                   """;

			var dataRow = new Dictionary<string, object>
			{
				{ "Id", 130 },
				{ "Code", "VN" }
			};
			var transaction = new Mock<IDbTransaction>();
			var result = QuerySingleOrDefaultAsync<SqlMapperDto>(sql, dataRow, null, transaction.Object) as Task<SqlMapperDto>;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, 130);
			Assert.AreEqual(result.Result.Code, "VN");

			transaction.Object.Commit();
		}

		[TestMethod]
		public void QuerySingleOrDefaultAsync_HasParameter_HasTransaction()
		{
			const string sql = """
			                   SELECT 
			                       Id as Id,
			                       Code as Code
			                   FROM
			                       SqlMapperTable 
			                   WHERE Code = @Code
			                   """;

			var dataRow = new Dictionary<string, object>
			{
				{ "Id", 130 },
				{ "Code", "VN" }
			};
			var parameters = new { Code = "VN" };
			var transaction = new Mock<IDbTransaction>();
			var result = QuerySingleOrDefaultAsync<SqlMapperDto>(sql, dataRow, parameters, transaction.Object) as Task<SqlMapperDto>;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, 130);
			Assert.AreEqual(result.Result.Code, "VN");

			transaction.Object.Commit();
		}

		private Task QuerySingleOrDefaultAsync<T>(string sql, IDictionary<string, object> dataRow, object? parameters, IDbTransaction? transaction)
        {
	        var dbCommand = SetUp_QuerySingleOrDefaultAsync(sql, dataRow, parameters);
	        var result = QuerySingleOrDefaultAsync<T>(sql, parameters, transaction);

	        Assert.IsNotNull(dbCommand);
	        Assert.AreEqual(dbCommand.CommandText, sql);

	        return result;
        }

		#endregion ** QuerySingleOrDefaultAsync **

		#region  ** ExecuteScalarAsync **

		[TestMethod]
		public void ExecuteScalarAsync_NoParameter_NoTransaction()
		{
			const string sql = """
			                   SELECT 
			                       Id as Id
			                   FROM
			                       SqlMapperTable 
			                   """;

			var dataRow = new SqlMapperDto()
			{
				Id = 130,
			};

			var result = ExecuteScalarAsync<SqlMapperDto>(sql, dataRow, null, null) as Task<SqlMapperDto>;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, 130);
		}

		[TestMethod]
		public void ExecuteScalarAsync_HasParameter_NoTransaction()
		{
			const string sql = """
	                           SELECT 
	                               Id as Id
	                           FROM
	                               SqlMapperTable 
	                           WHERE Code = @Code
	                           """;

			var dataRow = new SqlMapperDto()
			{
				Id = 130,
			};

			var parameters = new { Code = "VN" };

			var result = ExecuteScalarAsync<SqlMapperDto>(sql, dataRow, parameters, null) as Task<SqlMapperDto>;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, 130);
		}

		[TestMethod]
		public void ExecuteScalarAsync_NoParameter_HasTransaction()
		{
			const string sql = """
	                           SELECT 
	                               Id as Id
	                           FROM
	                               SqlMapperTable
	                           """;

			var dataRow = new SqlMapperDto()
			{
				Id = 130,
			};

			var transaction = new Mock<IDbTransaction>();
			var result = ExecuteScalarAsync<SqlMapperDto>(sql, dataRow, null, transaction.Object) as Task<SqlMapperDto>;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, 130);

			transaction.Object.Commit();
		}

		[TestMethod]
		public void ExecuteScalarAsync_HasTransaction_HasParameter()
		{
			const string sql = """
	                           SELECT 
	                               Id as Id
	                           FROM
	                               SqlMapperTable
	                           WHERE Code = @Code
	                           """;

			var dataRow = new SqlMapperDto()
			{
				Id = 130,
			};
			var parameters = new { Code = "VN" };

			var transaction = new Mock<IDbTransaction>();
			var result = ExecuteScalarAsync<SqlMapperDto>(sql, dataRow, parameters, transaction.Object) as Task<SqlMapperDto>;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Result);
			Assert.AreEqual(result.Result.Id, 130);

			transaction.Object.Commit();
		}

		private Task ExecuteScalarAsync<T>(string sql, T dataRow, object? parameters, IDbTransaction? transaction)
		{
			var dbCommand = SetUp_ExecuteScalarAsync(sql, dataRow, parameters);
			var result = ExecuteScalarAsync<T>(sql, parameters, transaction);

			Assert.IsNotNull(dbCommand);
			Assert.AreEqual(dbCommand.CommandText, sql);

			return result;
		}

		#endregion ** ExecuteScalarAsync **
	}
}
