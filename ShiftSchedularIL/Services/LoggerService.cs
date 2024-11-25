using Microsoft.Extensions.Logging;
using ShiftSchedularIL.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularIL.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;
        private string _logDirectory;

        #region Constructor

        public LoggerService(ILogger logger, string logDirectory)
        {
            _logger = logger;
            this._logDirectory = logDirectory;
        }

        #endregion

        #region Methods

        public Task LogDebug(string message)
        {
            throw new NotImplementedException();
        }

        public Task LogError(string message)
        {
            throw new NotImplementedException();
        }

        public Task LogInfo(string message)
        {
            throw new NotImplementedException();
        }

        public Task LogWarning(string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
