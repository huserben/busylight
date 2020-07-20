using System;
using Microsoft.Extensions.Logging;
using Moq;

// Copied from https://stackoverflow.com/a/59565950/909040
public static class LoggerExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> mockLogger, Func<Times> times)
    {
        mockLogger.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), times);
    }
}