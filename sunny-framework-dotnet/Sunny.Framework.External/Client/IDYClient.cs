using Refit;
using Sunny.Framework.External.Client.DY;

namespace Sunny.Framework.External.Client;

public interface IDYClient
{
    // 直播信息: https://developer.open-douyin.com/docs/resource/zh-CN/interaction/develop/server/live/webcastinfo
    [Post("/webcastmate/info")]
    Task<DYWebCastResult<DYLiveInfoResData>> GetLiveInfo([Body] DYLiveInfoReq param, [Header("X-Token")] string accessToken);

    // 履约上报: https://developer.open-douyin.com/docs/resource/zh-CN/interaction/develop/server/live/ack-ability
    [Post("/live_data/ack")]
    Task<DYLiveDataAckRes> Ack([Body] DYLiveDataAckReq param, [Header("access-token")] string accessToken);

    #region 消息推送任务: https: //bytedance.larkoffice.com/wiki/wikcnQe5jesCAbyUzsGx8xQeBNh
    [Post("/live_data/task/start")]
    Task<DYLiveDataTaskRes> StartTaskPush([Body] DYLiveDataTaskReq param, [Header("access-token")] string accessToken);

    [Post("/live_data/task/stop")]
    Task<DYLiveDataTaskRes> StopTaskPush([Body] DYLiveDataTaskReq param, [Header("access-token")] string accessToken);

    [Post("/live_data/task/get")]
    Task<DYLiveDataTaskRes> GetTaskStatus([Body] DYLiveDataTaskReq param, [Header("access-token")] string accessToken);

    #endregion


    #region 用户战绩与排行榜: https: //developer.open-douyin.com/docs/resource/zh-CN/interaction/develop/server/live/user-records-rankings

    // 设置当前生效的世界榜单版本
    [Post("/gaming_con/world_rank/set_valid_version")]
    Task<DYWebCastResult<object>> WorldSetValidVersion([Body] DYWorldSetValidVersionReq param, [Header("X-Token")] string accessToken);

    // 上传世界榜单列表数据
    [Post("/gaming_con/world_rank/upload_rank_list")]
    Task<DYWebCastResult<object>> WorldUploadRankList([Body] DYWorldUploadRankReq param, [Header("X-Token")] string accessToken);

    // 上报用户世界榜单的累计战绩
    [Post("/gaming_con/world_rank/upload_user_result")]
    Task<DYWebCastResult<object>> WorldUploadUserResult([Body] DYWorldUploadUserReq param, [Header("X-Token")] string accessToken);

    // 完成用户世界榜单的累计战绩上报
    [Post("/gaming_con/world_rank/complete_upload_user_result")]
    Task<DYWebCastResult<object>> WorldCompleteUploadUserResult([Body] DYWorldUploadUserCompleteReq param, [Header("X-Token")] string accessToken);

    // 同步对局状态
    [Post("/gaming_con/round/sync_status")]
    Task<DYWebCastResult<object>> RoundSync([Body] DYRoundSyncStatusReq param, [Header("X-Token")] string accessToken);

    // 上报对局榜单列表
    [Post("/gaming_con/round/upload_rank_list")]
    Task<DYWebCastResult<object>> RoundUploadRankList([Body] DYRoundUploadRankReq param, [Header("X-Token")] string accessToken);

    // 上报用户对局数据
    [Post("/gaming_con/round/upload_user_result")]
    Task<DYWebCastResult<object>> RoundUploadUserResult([Body] DYRoundUploadUserReq param, [Header("X-Token")] string accessToken);

    // 完成用户对局数据上报
    [Post("/gaming_con/round/complete_upload_user_result")]
    Task<DYWebCastResult<object>> RoundCompleteUploadUserResult([Body] DYRoundUploadUserCompleteReq param, [Header("X-Token")] string accessToken);

    #endregion
}