using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using System.ComponentModel.DataAnnotations.Schema;

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

        #region Execute Query

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string query, Dictionary<string,object> parameters)
        {
            var connection = _context.Database.GetDbConnection();

            connection.Open();

            using( var command = connection.CreateCommand() )
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                foreach(var param in parameters )
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value;
                    command.Parameters.Add(parameter);
                }

                using(var result = await command.ExecuteReaderAsync())
                {
                    var entities = new List<T>();

                    while(await result.ReadAsync())
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

        #endregion
    }
}
