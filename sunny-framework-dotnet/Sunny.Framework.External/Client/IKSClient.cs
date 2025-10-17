using Refit;
using Sunny.Framework.External.Client.KS;

namespace Sunny.Framework.External.Client;

public interface IKSClient
{
    [Get("/oauth2/access_token")]
    Task<KSAccessTokenRes> GetAccessToken([Query] KSAccessTokenReq param);

    // 直播信息: https://docs.qingque.cn/d/home/eZQDI-eILgkZm7C_1srSIhO8Y?identityId=20QDYkVmxXQ#section=h.hkhux256an8q
    [Post("/openapi/developer/live/data/interactive/live/stream/info")]
    Task<KSLiveInfoRes> GetLiveInfo([Query("app_id")] string appId, [Query("access_token")] string accessToken, [Body] Dictionary<string, object> param);

    // 推送任务: https://docs.qingque.cn/d/home/eZQD5kc0s4EA3C-XlYKi-9_-M?identityId=20QDYkVmxXQ#section=h.m0gicdoofhrp
    [Post("/openapi/developer/live/smallPlay/bind")]
    Task<KSResult> Bind([Query("app_id")] string appId, [Query("access_token")] string accessToken, [Body] Dictionary<string, object> param);

    // 消息确认: https://docs.qingque.cn/d/home/eZQBAXkHcoW9Zs2Xu9JaNDTTw?identityId=20QDYkVmxXQ#section=h.wybd5zfvl9nt
    [Post("/openapi/developer/live/data/interactive/ack/receive")]
    Task<KSResult> Ack([Query("app_id")] string appId, [Query("access_token")] string accessToken, [Body] Dictionary<string, object> param);
}