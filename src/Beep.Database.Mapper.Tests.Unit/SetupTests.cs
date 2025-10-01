using Moq;
using System.Data;

namespace Beep.Database.Mapper.Tests.Unit
{
    [TestClass]
    public class SetupTests
    {
        protected Mock<IDbConnection> _dbConnection;

        [TestInitialize]
        public void SetUp()
        {//testr
            _dbConnection = new Mock<IDbConnection>();
            _dbConnection.SetupSet(c => c.ConnectionString = It.IsAny<string>()).Verifiable();
            _dbConnection.Setup(c => c.Open()).Verifiable();
            _dbConnection.Setup(c => c.State).Returns(ConnectionState.Closed).Verifiable();
        }

        public IDbCommand SetUp_QuerySingleOrDefaultAsync(string sql, IDictionary<string, object> dataRow, object? parameters)
        {
            var dataReader = new Mock<IDataReader>();

            dataReader.Setup(dataReader => dataReader.Read())
                .Callback(() => dataReader.Setup(dataReader => dataReader.Read()).Returns(false))
                .Returns(true);

            dataReader.Setup(dataReader => dataReader.FieldCount).Returns(dataRow.Keys.Count);
            foreach (var key in dataRow.Keys)
            {
                dataReader.Setup(column => column[key]).Returns(dataRow[key]);
            }

            var keyList = dataRow.Keys.ToList();
            for (int i = 0; i < dataRow.Count; i++)
            {
                dataReader.Setup(dataReader => dataReader.GetName(i)).Returns(keyList[i]);
            }

            var dbCommand = new Mock<IDbCommand>();
            dbCommand.Setup(dbCommand => dbCommand.ExecuteReader(CommandBehavior.SingleRow)).Returns(dataReader.Object);
            dbCommand.SetupGet(dbCommand => dbCommand.CommandText).Returns(sql);

            if (parameters != null)
            {
                dbCommand.Setup(dbCommand => dbCommand.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
                dbCommand.Setup(dbCommand => dbCommand.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
                dbCommand.Object.AddParameters(parameters);
            }

            _dbConnection.Setup(c => c.CreateCommand()).Returns(dbCommand.Object);

            return dbCommand.Object;
        }

        public IDbCommand SetUp_ExecuteScalarAsync<T>(string sql, T result, object? parameters)
        {
            var dataReader = new Mock<IDataReader>();

            dataReader.Setup(dataReader => dataReader.Read())
                .Callback(() => dataReader.Setup(dataReader => dataReader.Read()).Returns(false))
                .Returns(true);
           
            var dbCommand = new Mock<IDbCommand>();
            dbCommand.Setup(dbCommand => dbCommand.ExecuteScalar()).Returns(result);
            dbCommand.SetupGet(dbCommand => dbCommand.CommandText).Returns(sql);

            if (parameters != null)
            {
                dbCommand.Setup(dbCommand => dbCommand.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
                dbCommand.Setup(dbCommand => dbCommand.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
                dbCommand.Object.AddParameters(parameters);
            }

            _dbConnection.Setup(c => c.CreateCommand()).Returns(dbCommand.Object);

            return dbCommand.Object;
        }

        public IDbCommand SetUp_QueryAsync(string sql, IList<IDictionary<string, object>> dataRows, object? parameters)
        {
            var dataRow = dataRows.First();
            var dataReader = new Mock<IDataReader>();

            var dataRowIndex = -1;
            var dataRowsCount = dataRows.Count;

            dataReader.Setup(dataReader => dataReader.Read())
                .Callback(new InvocationAction(invocation =>
                          {
                              ++dataRowIndex;
                              if (dataRowIndex < dataRowsCount)
                              {
                                  var currentRow = dataRows[dataRowIndex];
                                  foreach (var key in currentRow.Keys)
                                  {
                                      dataReader.Setup(column => column[key]).Returns(currentRow[key]);
                                  }

                                  if(dataRowIndex == dataRowsCount - 1)
                                  {
                                      dataReader.Setup(dataReader => dataReader.Read()).Returns(false);
                                  }
                              }
                          }))
                .Returns(true);

            dataReader.Setup(dataReader => dataReader.FieldCount).Returns(dataRow.Keys.Count);

            var keyList = dataRow.Keys.ToList();
            for (int i = 0; i < dataRow.Count; i++)
            {
                dataReader.Setup(dataReader => dataReader.GetName(i)).Returns(keyList[i]);
            }

            var dbCommand = new Mock<IDbCommand>();
            dbCommand.Setup(dbCommand => dbCommand.ExecuteReader()).Returns(dataReader.Object);
            dbCommand.SetupGet(dbCommand => dbCommand.CommandText).Returns(sql);

            if (parameters != null)
            {
	            dbCommand.Setup(dbCommand => dbCommand.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
	            dbCommand.Setup(dbCommand => dbCommand.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
	            dbCommand.Object.AddParameters(parameters);
            }

			_dbConnection.Setup(c => c.CreateCommand()).Returns(dbCommand.Object);

            return dbCommand.Object;
        }

        public Task<T?> QuerySingleOrDefaultAsync<T>(string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            return _dbConnection.Object.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            return _dbConnection.Object.QueryAsync<T>(sql, param, transaction);
        }

        public Task<T?> ExecuteScalarAsync<T>(string sql,
            object? param = null,
            IDbTransaction? transaction = null)
        {
            return _dbConnection.Object.ExecuteScalarAsync<T>(sql, param, transaction);
        }
    }
}
