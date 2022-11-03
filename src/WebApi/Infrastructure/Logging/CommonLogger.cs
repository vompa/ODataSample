namespace OData.Sample.WebApi.Infrastructure.Logging;

using System;

using Microsoft.Extensions.Logging;

/// <summary>
/// Class CommonLogger.
/// </summary>
/// <remarks>
/// <see href="https://github.com/RealDotNetDave/dotNetTips.Spargine/blob/main/source/6/dotNetTips.Spargine.6.Core/Logging/EasyLogger.cs">Code from dotNetTips.Spargine.</see>
/// </remarks>
public static partial class CommonLogger
{
	/// <summary>
	/// Logs critical message.
	/// <param name="logger">The logger.</param>
	/// <param name="message">The message.</param>
	/// <param name="ex">The exception.</param>
	/// </summary>
	[LoggerMessage(EventId = 100, Level = LogLevel.Critical, EventName = "CRITICAL", Message = "{message}")]
	public static partial void LogCritical(ILogger logger, string message, Exception ex);

	/// <summary>
	/// Logs debug message.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="message">The message.</param>
	[LoggerMessage(EventId = 200, Level = LogLevel.Debug, EventName = "DEBUG", Message = "{message}")]
	public static partial void LogDebug(ILogger logger, string message);

	/// <summary>
	/// Logs error message.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="message">The message.</param>
	/// <param name="ex">The exception.</param>
	[LoggerMessage(EventId = 300, Level = LogLevel.Error, EventName = "ERROR", Message = "{message}")]
	public static partial void LogError(ILogger logger, string message, Exception ex);

	/// <summary>
	/// Logs information message.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="message">The message.</param>
	[LoggerMessage(EventId = 400, Level = LogLevel.Information, EventName = "INFORMATION", Message = "{message}")]
	public static partial void LogInformation(ILogger logger, string message);

	/// <summary>
	/// Logs trace message.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="message">The message.</param>
	[LoggerMessage(EventId = 500, Level = LogLevel.Trace, EventName = "TRACE", Message = "{message}")]
	public static partial void LogTrace(ILogger logger, string message);

	/// <summary>
	/// Logs warning message.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="message">The message.</param>
	[LoggerMessage(EventId = 600, Level = LogLevel.Warning, EventName = "WARNING", Message = "{message}")]
	public static partial void LogWarning(ILogger logger, string message);
}
