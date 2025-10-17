namespace Sunny.Framework.External.Client.KS;

public class KSLiveInfoRes : KSResult
{
    public KSLiveInfoResUserInfo userInfo { get; set; }
}

public class KSLiveInfoResUserInfo
{
    public string authorOpenId { get; set; }
    public string userName { get; set; }
    public string headUrl { get; set; }
    public string liveStreamUrl { get; set; }
    public bool? isPrivateLiveStream { get; set; }
}