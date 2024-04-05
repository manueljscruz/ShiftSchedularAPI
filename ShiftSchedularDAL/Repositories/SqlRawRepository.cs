using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularDAL.Repositories
{
    public class SqlRawRepository<T> : ISQLRawRepository<T> where T : class
    {
        private readonly DataContext _context;

        public SqlRawRepository(DataContext context)
        {
            _context = context;
        }

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
                    var entity = new List<T>();

                    while(await result.ReadAsync())
                    {
                        var obj = Activator.CreateInstance<T>();

                        foreach(var prop in obj.GetType().GetProperties())
                        {
                            if (prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Any())
                                continue;

                            if (!Equals(result[prop.Name], DBNull.Value))
                            {
                                prop.SetValue(obj, result[prop.Name]);
                            }
                        }
                        entity.Add(obj);
                    }

                    return entity;
                }
            }
        }
    }
}
