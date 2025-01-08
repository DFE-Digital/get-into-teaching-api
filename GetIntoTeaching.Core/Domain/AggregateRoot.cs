namespace GetIntoTeaching.Core.Domain
{
    public abstract class AggregateRoot<TIdentifier> : Entity<TIdentifier>
        where TIdentifier : ValueObject<TIdentifier>
    {
        private readonly List<IDomainEvent> _domainEvents;

        protected AggregateRoot(TIdentifier identifier) :
            base(identifier) => _domainEvents = new List<IDomainEvent>();

        public void AddDomainEvent(IDomainEvent eventNotification)
        {
            if (eventNotification == null)
                throw new ArgumentNullException(nameof(eventNotification));

            _domainEvents.Add(eventNotification);
        }

        public IEnumerable<IDomainEvent> GetUncommittedDomainEvents() =>
            _domainEvents.AsEnumerable();

        public void RemoveDomainEvent(IDomainEvent eventNotification)
        {
            if (eventNotification == null)
                throw new ArgumentNullException(nameof(eventNotification));

            _domainEvents.Remove(eventNotification);
        }

        public void ClearDomainEvents() => _domainEvents?.Clear();
    }
}
