using GetIntoTeachingApi.Models.Crm;
using System.Collections.Generic;

namespace GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles
{
    /// <summary>
    /// Provides utility methods to build stubbed contact channel creation records for testing.
    /// </summary>
    internal static class ContactChannelCreationTestDouble
    {
        /// <summary>
        /// Builds a default set of contact channel creation records representing common onboarding sources.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ContactChannelCreation"/> objects including 'Apply' and 'SchoolExperience' sources.
        /// </returns>
        public static List<ContactChannelCreation> BuildDefaultContactCreationChannelsStub() =>
            Build(
                (ContactChannelCreation.CreationChannelSource.Apply,
                    ContactChannelCreation.CreationChannelService.CreatedOnApply),
                (ContactChannelCreation.CreationChannelSource.SchoolExperience,
                    ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience)
            );

        /// <summary>
        /// Builds one or more contact channel creation stubs using the specified source/service pairs.
        /// </summary>
        /// <param name="channelPairs">
        /// A collection of tuples where each tuple contains a source and service enum value.
        /// </param>
        /// <returns>
        /// A list of <see cref="ContactChannelCreation"/> instances representing stubbed records.
        /// </returns>
        public static List<ContactChannelCreation> Build(
            params (ContactChannelCreation.CreationChannelSource source,
            ContactChannelCreation.CreationChannelService service)[] channelPairs)
        {
            var stubs = new List<ContactChannelCreation>();

            foreach (var (source, service) in channelPairs)
            {
                stubs.Add(new ContactChannelCreation
                {
                    CreationChannelSourceId = (int)source,
                    CreationChannelServiceId = (int)service
                });
            }

            return stubs;
        }
    }
}
