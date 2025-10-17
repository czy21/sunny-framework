using System.ComponentModel;

namespace Sunny.Framework.Core.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null || Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is not DescriptionAttribute descriptionAttribute)
        {
            return value.ToString();
        }

        return descriptionAttribute.Description;
    }
}