namespace Sunny.Framework.External.Client.DY;

public class DYRankDTO
{
    public string open_id { get; set; } // 用户的open_id
    public long rank { get; set; } // 世界榜单排名，从1开始
    public long score { get; set; } // 当前用户的世界榜单积分
    public long winning_streak_count { get; set; } // 当前用户的连胜次数，如果没有连胜记录传0
    public long winning_points { get; set; } // 当前用户的胜点记录，如果没有胜点记录传0
}