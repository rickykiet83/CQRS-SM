using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindPostByIdQuery(Guid id) : BaseQuery
{
    public Guid Id { get; set; } = id;
}