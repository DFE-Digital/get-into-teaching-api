namespace GetIntoTeaching.Core.Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIdentifier"></typeparam>
    public abstract class AggregateRoot<TIdentifier> : Entity<TIdentifier>
        where TIdentifier : ValueObject<TIdentifier>
    {
        private readonly List<IDomainEvent> _domainEvents;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        protected AggregateRoot(TIdentifier identifier) :
            base(identifier) => _domainEvents = new List<IDomainEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventNotification"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddDomainEvent(IDomainEvent eventNotification)
        {
            if (eventNotification == null)
                throw new ArgumentNullException(nameof(eventNotification));

            _domainEvents.Add(eventNotification);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomainEvent> GetUncommittedDomainEvents() =>
            _domainEvents.AsEnumerable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventNotification"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveDomainEvent(IDomainEvent eventNotification)
        {
            if (eventNotification == null)
                throw new ArgumentNullException(nameof(eventNotification));

            _domainEvents.Remove(eventNotification);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearDomainEvents() => _domainEvents?.Clear();
    }
}
