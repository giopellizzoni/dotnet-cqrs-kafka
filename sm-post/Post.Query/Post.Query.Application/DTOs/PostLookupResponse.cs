using Post.Common.DTOs;
using Post.Query.Domain.Entities;

namespace Post.Query.Application.DTOs;

public class PostLookupResponse : BaseResponse
{
    public List<PostEntity> Posts { get; set; }
}