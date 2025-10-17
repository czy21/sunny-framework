namespace Sunny.Framework.External.Client.DY;

public class DYWebCastResult<T>
{
    public int? errcode { get; set; }
    public string errmsg { get; set; }
    public T data { get; set; }
}