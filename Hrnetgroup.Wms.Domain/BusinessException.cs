using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Hrnetgroup.Wms.Domain;

[Serializable]
public class UserFriendlyException : BusinessException
{
    public UserFriendlyException(
        string message,
        string code = null,
        string details = null,
        Exception innerException = null,
        LogLevel logLevel = LogLevel.Warning)
        : base(
            code,
            message,
            details,
            innerException,
            logLevel)
    {
        Details = details;
    }

    /// <summary>
    /// Constructor for serializing.
    /// </summary>
    public UserFriendlyException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }
}

[Serializable]
public class BusinessException : Exception
{
    public string Code { get; set; }

    public string Details { get; set; }

    public LogLevel LogLevel { get; set; }

    public BusinessException(
        string code = null,
        string message = null,
        string details = null,
        Exception innerException = null,
        LogLevel logLevel = LogLevel.Warning)
        : base(message, innerException)
    {
        Code = code;
        Details = details;
        LogLevel = logLevel;
    }

    /// <summary>
    /// Constructor for serializing.
    /// </summary>
    public BusinessException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {

    }

    public BusinessException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}