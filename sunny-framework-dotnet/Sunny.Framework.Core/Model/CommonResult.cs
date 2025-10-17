using Sunny.Framework.Core.Extensions;

namespace Sunny.Framework.Core.Model;

public class CommonResult<T>(int code, string message, T data)
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;
    public T Data { get; set; } = data;

    public static CommonResult<T> Ok(T data = default)
    {
        return new CommonResult<T>((int)CommonCodeEnum.Success, CommonCodeEnum.Success.GetDescription(), data);
    }

    public static CommonResult<T> Error()
    {
        return Error(CommonCodeEnum.Success.GetDescription());
    }

    public static CommonResult<T> Error(string msg)
    {
        return Error((int)CommonCodeEnum.Exception, msg);
    }

    public static CommonResult<T> Error(int code, string msg)
    {
        return new CommonResult<T>(code, msg, default);
    }

    public static CommonResult<T> Create(int code, string message, T data)
    {
        return new CommonResult<T>(code, message, data);
    }
}