namespace Sunny.Framework.External.Client.DY;

public class DYRoundUploadUserCompleteReq : DYRoundDTO
{
    public long complete_time { get; set; } // 上传完成时间，秒级时间戳，默认为当前接口请求时间
}