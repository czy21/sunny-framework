using System.ComponentModel;

namespace Sunny.Framework.Core.Model;

public enum CommonCodeEnum
{
    [Description("success")] Success = 0,
    [Description("internal error")] Exception = -1,
}