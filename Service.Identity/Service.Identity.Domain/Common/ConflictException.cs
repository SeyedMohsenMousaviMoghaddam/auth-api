using Service.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Domain.Common;
public class BaseException : Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public ApiResultStatusCode ApiStatusCode { get; set; }
    public object AdditionalData { get; set; }

    public BaseException()
        : this(ApiResultStatusCode.ServerError)
    {
    }

    public BaseException(ApiResultStatusCode statusCode)
        : this(statusCode, null)
    {
    }

    public BaseException(string message)
        : this(ApiResultStatusCode.ServerError, message)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message)
        : this(statusCode, message, HttpStatusCode.InternalServerError)
    {
    }

    public BaseException(string message, object additionalData)
        : this(ApiResultStatusCode.ServerError, message, additionalData)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, object additionalData)
        : this(statusCode, null, additionalData)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, object additionalData)
        : this(statusCode, message, HttpStatusCode.InternalServerError, additionalData)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, HttpStatusCode httpStatusCode)
        : this(statusCode, message, httpStatusCode, null)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, HttpStatusCode httpStatusCode,
        object additionalData)
        : this(statusCode, message, httpStatusCode, null, additionalData)
    {
    }

    public BaseException(string message, Exception exception)
        : this(ApiResultStatusCode.ServerError, message, exception)
    {
    }

    public BaseException(string message, Exception exception, object additionalData)
        : this(ApiResultStatusCode.ServerError, message, exception, additionalData)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, Exception exception)
        : this(statusCode, message, HttpStatusCode.InternalServerError, exception)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, Exception exception, object additionalData)
        : this(statusCode, message, HttpStatusCode.InternalServerError, exception, additionalData)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, HttpStatusCode httpStatusCode,
        Exception exception)
        : this(statusCode, message, httpStatusCode, exception, null)
    {
    }

    public BaseException(ApiResultStatusCode statusCode, string message, HttpStatusCode httpStatusCode,
        Exception exception, object additionalData)
        : base(message, exception)
    {
        ApiStatusCode = statusCode;
        HttpStatusCode = httpStatusCode;
        AdditionalData = additionalData;
    }
}
public class ConflictException : BaseException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ConflictException(Dictionary<string, string[]> errors)
        {
            Errors = errors;
        }

        public ConflictException(string message)
        {
        }
    }
