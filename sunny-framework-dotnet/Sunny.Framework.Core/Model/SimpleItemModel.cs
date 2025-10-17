namespace Sunny.Framework.Core.Model;

public class SimpleItemModel<T>
{
    public string Label { get; set; }
    public T Value { get; set; }
}