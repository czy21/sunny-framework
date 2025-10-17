using System.Security.Cryptography;
using System.Text;

namespace Sunny.Framework.External.Util;

public static class KSUtil
{
    public static string SignatureReceive(string rawBody, string appSecret)
    {
        var signStr = rawBody + appSecret;
        var inputBytes = Encoding.UTF8.GetBytes(signStr);
        var hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexStringLower(hashBytes);
    }

    public static string SignatureRequest(Dictionary<string, object> param, string appSecret)
    {
        var trimmedParam = param.Where(item => !string.IsNullOrEmpty(item.Value.ToString())).ToDictionary(item => item.Key, item => item.Value);

        var sortedParam = trimmedParam.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.Value);

        var paramStr = string.Join("&", sortedParam.Select(item => $"{item.Key}={item.Value}"));
        var signStr = paramStr + appSecret;

        var inputBytes = Encoding.UTF8.GetBytes(signStr);
        var hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexStringLower(hashBytes);
    }
}