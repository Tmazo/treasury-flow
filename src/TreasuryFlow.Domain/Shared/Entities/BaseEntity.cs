namespace TreasuryFlow.Domain.Shared.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null;

    protected BaseEntity()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
