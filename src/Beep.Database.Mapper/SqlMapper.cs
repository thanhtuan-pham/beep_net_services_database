using System.Data;
using System.Reflection;

namespace Beep.Database.Mapper
{
    public static class SqlMapper
    {
        public static Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection connection, string sql, 
            object? param = null, 
            IDbTransaction? transaction = null)
        {
            T? result = default(T);

            ConnectionState originalState = connection.State;
            if (originalState != ConnectionState.Open)
                connection.Open();

            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.AddParameters(param);
                command.CommandText = sql;

                var dataReader = command.ExecuteReader(CommandBehavior.SingleRow);
                var fields = GetDataFields(dataReader);

                while (dataReader.Read())
                {
                    result = ReadSingleRow<T>(fields, dataReader);
                }
            }
            catch
            {
                try
                {
                    transaction?.Rollback();
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                // Close the connection if that's how we got it
                if (originalState == ConnectionState.Closed)
                    connection.Close();
            }

            return Task.FromResult<T?>(result);
        }

        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string sql, 
            object? param = null, 
            IDbTransaction? transaction = null)
        {
            IList<T> result = new List<T>();

            ConnectionState originalState = connection.State;
            if (originalState != ConnectionState.Open)
                connection.Open();

            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.AddParameters(param);
                command.CommandText = sql;

                var dataReader = command.ExecuteReader();
                var fields = GetDataFields(dataReader);

                while (dataReader.Read())
                {
                    result.Add(ReadSingleRow<T>(fields, dataReader));
                }
            }
            catch
            {
                try
                {
                    transaction?.Rollback();
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                // Close the connection if that's how we got it
                if (originalState == ConnectionState.Closed)
                    connection.Close();
            }

            return Task.FromResult<IEnumerable<T>>(result);
        }

        public static Task<T?> ExecuteScalarAsync<T>(this IDbConnection connection, string sql, 
            object? param = null, 
            IDbTransaction? transaction = null)
        {
            ConnectionState originalState = connection.State;
            if (originalState != ConnectionState.Open)
                connection.Open();

            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.AddParameters(param);
                command.CommandText = sql;

                var dataReader = command.ExecuteScalar();
                if (dataReader == null)
                {
                    return Task.FromResult<T?>(default(T?));
                }

                Convert.ChangeType(dataReader, typeof(T));
                return Task.FromResult<T?>((T)dataReader!);
            }
            catch
            {
                try
                {
                    transaction?.Rollback();
                }
                catch
                {
                    throw;
                }
            }
            finally
            {
                // Close the connection if that's how we got it
                if (originalState == ConnectionState.Closed)
                    connection.Close();
            }

            return Task.FromResult<T?>(default(T));
        }

        private static string[] GetDataFields(IDataReader? dataReader)
        {
            IList<string> result = new List<string>();
            if (dataReader == null)
            {
                return result.ToArray();
            }

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                result.Add(dataReader.GetName(i));
            }

            return result.ToArray();
        }

        private static T ReadSingleRow<T>(string[]? fields, IDataRecord dataRecord)
        {
            var entity = Activator.CreateInstance(typeof(T));

            var fieldsLength = fields?.Length ?? 0;
            for (int i = 0; i < fieldsLength; i++)
            {
                var field = fields![i];

                var propertyInfo = entity!.GetType().GetProperty(field);
                if (propertyInfo == null)
                    continue;

                propertyInfo.SetValue(entity, dataRecord[field]);
            }

            return (T)entity!;
        }

        public static void AddParameters(this IDbCommand command, object? param)
        {
            if (param == null || param.GetType() == null)
                return;

            foreach (var item in param.GetType().GetProperties())
            {
                command.AddParameter(item, param);
            }
        }

        public static void AddParameter(this IDbCommand command, PropertyInfo propertyInfo, object param)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = $"@{propertyInfo.Name}";
            parameter.Value = propertyInfo.GetValue(param);
            command.Parameters.Add(parameter);
        }
    }
}
