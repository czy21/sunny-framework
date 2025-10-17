namespace Sunny.Framework.External.Client.DY;

public class DYRoundSyncStatusReq : DYRoundDTO
{
    public long start_time { get; set; } // 本局开始时间，秒级时间戳
    public long end_time { get; set; } // 本局结束时间，秒级时间戳。同步的对局状态为对局结束时，该字段必传。
    public int status { get; set; } // 当前房间的游戏对局状态（1=已开始、2=已结束）
    public List<DYRoundSyncStatusGroupResultItem> group_result_list { get; set; } // 对局结束时，阵型的结果数据 同步的对局状态为[已结束] 时，该字段必传。
}

public class DYRoundSyncStatusGroupResultItem
{
    public string group_id { get; set; } // 阵营id，如：red
    public int result { get; set; } // 对局结果（1=胜利、2=失败、3=平局）
}