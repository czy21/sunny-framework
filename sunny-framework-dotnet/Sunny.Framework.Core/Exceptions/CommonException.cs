using Sunny.Framework.Core.Model;

namespace Sunny.Framework.Core.Exceptions;

public class CommonException : Exception
{
    public int Code { get; set; }

    public CommonException(int code, string message) : base(message)
    {
        Code = code;
    }

    public CommonException(string message) : base(message)
    {
        Code = (int)CommonCodeEnum.Exception;
    }
}