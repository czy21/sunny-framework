namespace Sunny.Framework.External.Client.DY;

public class DYLiveInfoResData
{
    public DYLiveInfoResInfo info { get; set; }
}

public class DYLiveInfoResInfo
{
    public long? room_id { get; set; }
    public string anchor_open_id { get; set; }
    public string avatar_url { get; set; }

    public string nick_name { get; set; }
}