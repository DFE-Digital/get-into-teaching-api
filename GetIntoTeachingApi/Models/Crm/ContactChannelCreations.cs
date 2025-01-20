using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace GetIntoTeachingApi.Models.Crm
{
    /// <summary>
    /// Provides the blueprint for an object which encapsulates a collection of
    /// <see cref="ContactChannelCreation"/> types defined by the
    /// parent <see cref="Candidate"/> instance.
    /// </summary>
    public sealed class ContactChannelCreations
    {
        private readonly IList<ContactChannelCreation> _contactChannelCreations;

        /// <summary>
        /// Initialises a new collection of <see cref="ContactChannelCreation"/> on instantiation.
        /// </summary>
        public ContactChannelCreations()
        {
            _contactChannelCreations = new List<ContactChannelCreation>();
        }

        /// <summary>
        /// Offers read-only access to the underlying
        /// <see cref="ContactChannelCreation"/> collection.
        /// </summary>
        /// <returns>
        /// The read-only collection of <see cref="ContactChannelCreation"/> configured types.
        /// </returns>
        public IReadOnlyCollection<ContactChannelCreation> GetContactChannelCreations() =>
            new ReadOnlyCollection<ContactChannelCreation>(_contactChannelCreations);

        /// <summary>
        /// Allows a safe mechanism to add (mutate) the underlying
        /// collection of <see cref="ContactChannelCreation"/> types.
        /// </summary>
        /// <param name="contactChannelCreation"
        /// The <see cref="ContactChannelCreation"/> type to add to the underlying collection.
        /// ></param>
        /// <exception cref="ArgumentNullException">
        /// Exception type thrown if the <see cref="ContactChannelCreation"/> is null.
        /// </exception>
        public void AddContactChannelCreation(ContactChannelCreation contactChannelCreation)
        {
            if (contactChannelCreation == null!)
            {
                throw new ArgumentNullException(
                    nameof(contactChannelCreation),
                    "The 'ContactChannelCreation' cannot be null.");
            }
            _contactChannelCreations.Add(contactChannelCreation);
        }

        /// <summary>
        /// Allows the current list of <see cref="ContactChannelCreation"/> types to be reset.
        /// </summary>
        public void Reset() => _contactChannelCreations.Clear();

        /// <summary>
        /// Determines whether we have any existing ContactChannelCreations.
        /// </summary>
        public bool HasExistingContactChannelCreations => _contactChannelCreations.Count > 0;
    }
}
