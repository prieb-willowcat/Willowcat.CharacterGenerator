using Microsoft.Extensions.Logging;
using System;

namespace Willowcat.CharacterGenerator.Core.Tests.Mock
{
    public class DebugLogger<T> : ILogger<T> where T : class
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            Console.WriteLine($"{logLevel} - {message}");
        }
    }
}
