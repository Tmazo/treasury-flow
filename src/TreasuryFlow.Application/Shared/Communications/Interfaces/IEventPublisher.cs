namespace TreasuryFlow.Application.Shared.Communications.Interfaces;

public interface IEventPublisher
{
    Task SendAsRawJsonAsync<TEvent>(TEvent @event, CancellationToken cancellationToken);
}
