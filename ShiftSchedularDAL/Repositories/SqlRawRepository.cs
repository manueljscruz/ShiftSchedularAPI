using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;

namespace ShiftSchedularDAL.Repositories
{
    public class SqlRawRepository<T> : ISQLRawRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbConnection _connection;

        #region Constructor

        public SqlRawRepository(DataContext context)
        {
            _context = context;
            _connection = _context.Database.GetDbConnection();
        }

        #endregion

        #region Execute Scalar

        public async Task<T> ExecuteScalar<T>(string query, Dictionary<string, object> parameters)
        {
            var connection = _connection; // Use persistent connection
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            foreach (var param in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                parameter.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }

            var result = await command.ExecuteScalarAsync();
            return result != DBNull.Value ? (T)result : default;
        }

        #endregion

        #region Get Data TableAsync

        public async Task<DataTable> GetDataTableAsync(string query, Dictionary<string, object> parameters)
        {
            DataTable dataTable = new DataTable();

            await using var connection = _connection; // Use persistent connection
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                await using var dataReader = await command.ExecuteReaderAsync();
                dataTable.Load(dataReader);

            }
            catch (TaskCanceledException ex)
            {
                string error = ex.Message;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return dataTable;
        }

        #endregion

        #region Execute Query

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string query, Dictionary<string, object> parameters)
        {
            var entities = new List<T>();

            var connection = _connection; // Use persistent connection
            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                await using var result = await command.ExecuteReaderAsync();
                while (await result.ReadAsync())
                {
                    if (typeof(T) == typeof(int))
                    {
                        entities.Add((T)(object)result.GetInt32(0));
                    }
                    else
                    {
                        var obj = Activator.CreateInstance<T>();

                        foreach (var prop in obj.GetType().GetProperties())
                        {
                            if (prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                                continue;

                            if (!result.IsDBNull(result.GetOrdinal(prop.Name)))
                            {
                                var value = result[prop.Name];

                                // Handle byte[] to string conversion
                                if (value is byte[] byteArray && prop.PropertyType == typeof(string))
                                {
                                    prop.SetValue(obj, Convert.ToBase64String(byteArray));
                                }
                                else if (value.GetType() == prop.PropertyType || prop.PropertyType.IsAssignableFrom(value.GetType()))
                                {
                                    prop.SetValue(obj, value);
                                }
                                else
                                {
                                    throw new InvalidCastException($"Cannot map column '{prop.Name}' of type '{value.GetType()}' to property '{prop.PropertyType}'");
                                }
                            }
                        }
                        entities.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }


            return entities;

        }

        #endregion
    }
}
