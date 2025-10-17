namespace Sunny.Framework.External.Client.DY;

public class DYWorldUploadUserReq : DYWorldSetValidVersionReq
{
    public List<DYRankDTO> user_list { get; set; }
}