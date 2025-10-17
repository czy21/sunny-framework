namespace Sunny.Framework.External.Client.DY;

public class DYRoundUploadRankReq : DYRoundDTO
{
    public List<DYRoundUploadRankItem> rank_list { get; set; }
}

public class DYRoundUploadRankItem : DYRankDTO
{
    public int round_result { get; set; } // 对局结果（1=胜利、2=失败、3=平局）
    public string group_id { get; set; } // 阵营Id，比如 red/blue
}