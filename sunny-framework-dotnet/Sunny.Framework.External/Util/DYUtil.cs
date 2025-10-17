using System.Security.Cryptography;
using System.Text;

namespace Sunny.Framework.External.Util;

public static class DYUtil
{
    public static string SignatureReceive(Dictionary<string, object> headers, string rawBody, string appSecretPush)
    {
        var sortedParam = headers.OrderBy(item => item.Key).ToDictionary(t => t.Key, item => item.Value);
        var paramStr = string.Join("&", sortedParam.Select(t => $"{t.Key}={t.Value}"));
        var signStr = paramStr + rawBody + appSecretPush;
        var inputBytes = Encoding.UTF8.GetBytes(signStr);
        var hashBytes = MD5.HashData(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }
}