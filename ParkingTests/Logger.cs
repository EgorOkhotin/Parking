using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingTests
{
    class Logger<T> : ILogger<T>
    {
        List<string> _storage;
        public Logger()
        {
            _storage = new List<string>();
        }

        public bool ContainsRecord(string record)
        {
            if(_storage.Count==0) return false;
            
            foreach(var r in _storage)
            {
                if(r.Contains(record)) return true;
            }
            return false;
        }

        public List<string> Logs => _storage;
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _storage.Add(formatter.Invoke(state, exception));
        }
    }
}
