namespace GetIntoTeaching.Core.Domain
{
    public interface IDomainEvent
    {
        DateTime DateTimeRaised { get; }
    }
}
