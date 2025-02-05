using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Query.Domain.Entities;


[Table("Posts")]
public class PostEntity : IEntity
{
    [Key]
    public Guid PostId { get; set; }
    public string Author { get; set; }
    public DateTime DatePosted { get; set; }
    public string Message { get; set; }
    public int Likes { get; set; }
    public virtual ICollection<CommentEntity> Comments { get; set; }

}