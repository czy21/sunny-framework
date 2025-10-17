using System.ComponentModel.DataAnnotations.Schema;

namespace Sunny.Framework.Core.Model;

public class BaseEntity<TK, TU>
{
    [Column("id")] public TK Id { get; set; }
    [Column("create_time")] public DateTime? CreateTime { get; set; }
    [Column("update_time")] public DateTime? UpdateTime { get; set; }
    [Column("create_user")] public TU CreateUser { get; set; }
    [Column("update_user")] public TU UpdateUser { get; set; }
    [Column("deleted")] public bool Deleted { get; set; }
}

public abstract class BaseEntity : BaseEntity<string, string>
{
}