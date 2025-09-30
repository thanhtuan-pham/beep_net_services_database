using Microsoft.Data.SqlClient;
using System.Data;
using Testcontainers.MsSql;

namespace Beep.Database.Mapper.Tests.Integration
{
    [TestClass]
    public class SetupTests
    {
        private MsSqlContainer _sqlContainer;
        private string _connectionString;
        private IDbConnection _dbConnection;

        [TestInitialize]
        public void SetUp()
        {
            SetupDatabase();
            CreateTables();
        }

        private void SetupDatabase()
        {
            _sqlContainer = new MsSqlBuilder().Build();
            _sqlContainer.StartAsync().Wait(CancellationToken.None);
            //
            _connectionString = _sqlContainer.GetConnectionString();
        }

        private void CreateTables()
        {
            _dbConnection = new SqlConnection(_connectionString);

            var originalStatus = _dbConnection.State;
            if(originalStatus != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            
            CreateCurrencyTable(transaction);
            CreateCountryTable(transaction);

            transaction.Commit();
        }

        private void CreateCurrencyTable(IDbTransaction dbTransaction)
        {
            const string createTableCountry = """
                CREATE TABLE [dbo].[Currency](
                	[Id] [int] IDENTITY(1,1) NOT NULL,
                	[CodeISO] [char](3) NOT NULL,
                	[Name] [nvarchar](254) NOT NULL,
                	[DecimalDigits] [tinyint] NOT NULL,
                	[Symbol] [nvarchar](10) NULL,
                	[Synchronized] [bit] NOT NULL,
                 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
                (
                	[Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
                 CONSTRAINT [AK_Currency_Code] UNIQUE NONCLUSTERED 
                (
                	[CodeISO] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY]
                ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_DecimalCount]  DEFAULT ((2)) FOR [DecimalDigits]
                ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_Synchronized]  DEFAULT ((0)) FOR [Synchronized]
                """;

            using var command = _dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            command.CommandText = createTableCountry;
            command.ExecuteNonQuery();
        }

        private void CreateCountryTable(IDbTransaction dbTransaction)
        {
            const string createTableCountry = """
                CREATE TABLE [dbo].[Country](
                	[Id] [int] IDENTITY(1,1) NOT NULL,
                	[Currency_Id] [int] NULL,
                	[Code] [nvarchar](3) NOT NULL,
                	[Name] [nvarchar](256) NULL,
                	[PhoneCode] [char](10) NULL,
                	[IsCountry] [bit] NOT NULL,
                	[EUEntryDate] [date] NULL,
                	[VatNumberMask] [nvarchar](510) NULL,
                	[Country281] [nvarchar](10) NULL,
                	[IsSepa] [bit] NULL,
                	[Synchronized] [bit] NOT NULL,
                 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
                (
                	[Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
                 CONSTRAINT [AK_Country_Code] UNIQUE NONCLUSTERED 
                (
                	[Code] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY]
                ALTER TABLE [dbo].[Country] ADD  CONSTRAINT [DF_Country_IsCountry]  DEFAULT ((0)) FOR [IsCountry]
                ALTER TABLE [dbo].[Country] ADD  CONSTRAINT [DF_Country_Synchronized]  DEFAULT ((0)) FOR [Synchronized]
                ALTER TABLE [dbo].[Country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Currency] FOREIGN KEY([Currency_Id])
                REFERENCES [dbo].[Currency] ([Id])
                ALTER TABLE [dbo].[Country] CHECK CONSTRAINT [FK_Country_Currency]
                """;

            using var command = _dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            command.CommandText = createTableCountry;
            command.ExecuteNonQuery();
        }

        [TestCleanup]
        public void Clean()
        {
            _sqlContainer.DisposeAsync();
        }

        public Task<T?> Insert<T>(string sql, object? param, IDbTransaction? transaction)
            where T : class
        {
            return _dbConnection.ExecuteScalarAsync<T>(sql, param, transaction);
        }

        public Task<T?> Update<T>(string sql, object? param, IDbTransaction? transaction)
            where T : class
        {
            return _dbConnection.ExecuteScalarAsync<T>(sql, param, transaction);
        }

        public Task<T?> Delete<T>(string sql, object? param, IDbTransaction? transaction)
           where T : class
        {
            return _dbConnection.ExecuteScalarAsync<T>(sql, param, transaction);
        }

        public Task<T?> Get<T>(string sql, object? param, IDbTransaction? transaction)
            where T : class
        {
            return _dbConnection.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
        }

        public Task<IEnumerable<T>> List<T>(string sql, object? param, IDbTransaction? transaction)
            where T : class
        {
            return _dbConnection.QueryAsync<T>(sql, param, transaction);
        }
    }
}
