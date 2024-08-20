using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ShiftSchedularDAL.Repositories
{
    public class SqlRawRepository<T> : ISQLRawRepository<T> where T : class
    {
        private readonly DataContext _context;

        #region Constructor

        public SqlRawRepository(DataContext context)
        {
            _context = context;
        }

        #endregion

        #region Execute Scalar

        public async Task<T> ExecuteScalar<T>(string query, Dictionary<string, object> parameters)
        {
            var connection = _context.Database.GetDbConnection();

            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value;
                    command.Parameters.Add(parameter);
                }

                return (T)await command.ExecuteScalarAsync();
            }
        }

        #endregion

        #region Get Data TableAsync

        public async Task<DataTable> GetDataTableAsync(string query, Dictionary<string, object> parameters)
        {
            DataTable dataTable = new DataTable();

            var connection = _context.Database.GetDbConnection();

            connection.Open();

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;

                    foreach (var param in parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = param.Key;
                        parameter.Value = param.Value ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }

                    using (var dataReader = command.ExecuteReader())
                    {
                        dataTable.Load(dataReader);
                    }

                }
            }
            catch (TaskCanceledException ex)
            {
                string error = ex.Message;
                // Check ex.CancellationToken.IsCancellationRequested here.
                // If false, it's pretty safe to assume it was a timeout.
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                string error = ex.Message;
            }

            connection.Close();

            return dataTable;
        }

        #endregion

        #region Execute Query

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string query, Dictionary<string,object> parameters)
        {
            var connection = _context.Database.GetDbConnection();

            connection.Open();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;

                    foreach (var param in parameters)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = param.Key;
                        parameter.Value = param.Value;
                        command.Parameters.Add(parameter);
                    }

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        var entities = new List<T>();

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

                                    if (!Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name]);
                                    }
                                }
                                entities.Add(obj);
                            }
                        }

                        return entities;
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            connection.Close();

            return null;
            
        }

        #endregion
    }
}
