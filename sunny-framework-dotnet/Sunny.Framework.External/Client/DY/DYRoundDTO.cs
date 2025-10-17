namespace Sunny.Framework.External.Client.DY;

public class DYRoundDTO
{
    public string app_id { get; set; }
    public string room_id { get; set; }
    public string anchor_open_id { get; set; } // 主播的open_id
    public long round_id { get; set; } // 对局Id，同一个直播间room_id下，round_id需要是递增的，建议使用开始对局时的时间戳
}