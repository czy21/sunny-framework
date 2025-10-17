namespace Sunny.Framework.Cache
{
    public class RedisProperties
    {
        public string Url { get; set; }
        public Dictionary<string, RedisOption> Instances { get; set; }
    }

    public class RedisOption
    {
        public string Url { get; set; }
    }
}
